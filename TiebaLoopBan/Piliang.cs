using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace TiebaLoopBan
{
    public partial class Piliang : Form
    {
        public Piliang()
        {
            InitializeComponent();
        }

        private void Piliang_Load(object sender, EventArgs e)
        {
            Text = "批量添加";

            //限制格式
            dateTimePicker1.Format = DateTimePickerFormat.Custom;
            dateTimePicker1.CustomFormat = "yyyy-MM-dd";

            dateTimePicker2.Format = DateTimePickerFormat.Custom;
            dateTimePicker2.CustomFormat = "yyyy-MM-dd";

            dateTimePicker1.Value = DateTime.Now;
            dateTimePicker2.Value = DateTime.Now.AddMonths(1);
        }

        /// <summary>
        /// 开始时间被修改
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {
            label4.Text = $"封禁时长：{Lib.HuoquFengjinShichang(dateTimePicker1, dateTimePicker2)}天";
        }

        /// <summary>
        /// 结束时间被修改
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dateTimePicker2_ValueChanged(object sender, EventArgs e)
        {
            label4.Text = $"封禁时长：{Lib.HuoquFengjinShichang(dateTimePicker1, dateTimePicker2)}天";
        }

        /// <summary>
        /// 批量添加
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            if (Lib.HuoquFengjinShichang(dateTimePicker1, dateTimePicker2) <= 0)
            {
                MessageBox.Show("封禁时长必须大于0", "笨蛋雪说：", buttons: MessageBoxButtons.OK, icon: MessageBoxIcon.Exclamation);
                return;
            }

            if (textBox2.Text == "" || textBox2.Text == null)
            {
                MessageBox.Show("请填写贴吧名", "笨蛋雪说：", buttons: MessageBoxButtons.OK, icon: MessageBoxIcon.Exclamation);
                return;
            }

            string tiebaName = textBox2.Text;
            string kaishiSj = dateTimePicker1.Text;
            string jieshuSj = dateTimePicker2.Text;
            string[] tempStr = textBox1.Text.Split(Environment.NewLine.ToCharArray());

            //过滤
            Huancun.PiliangTianjiaLiebiao = new List<string>();
            foreach (string str in tempStr)
            {
                if (str == "" || str == null)
                {
                    continue;
                }

                //清理重复
                if (Huancun.PiliangTianjiaLiebiao.ListIsRepeat(canshu => canshu == str))
                {
                    continue;
                }

                Huancun.PiliangTianjiaLiebiao.Add(str);
            }

            //添加到数据库
            int chenggong = 0;
            int shibai = 0;
            string shibaiMingdan = "";
            for (int i = 0; i < Huancun.PiliangTianjiaLiebiao.Count; i++)
            {
                string yonghuMing = Huancun.PiliangTianjiaLiebiao[i];

                //数据库检查重复
                if (Form1.db_tlb.GetDataTable($"select * from 封禁列表 where 用户名='{yonghuMing}' and 贴吧名='{tiebaName}'").Rows.Count > 0)
                {
                    shibai += 1;
                    shibaiMingdan += yonghuMing + "\r\n";
                    continue;
                }

                int jieguo = Form1.db_tlb.DoCommand($"insert into 封禁列表 (用户名,贴吧名,最后封禁时间,循环开始时间,循环结束时间)" +
                    $" values('{yonghuMing}','{tiebaName}','1970-01-01','{kaishiSj}','{jieshuSj}')");
                if (jieguo > 0)
                {
                    chenggong += 1;
                }
            }

            string msg = "批量添加结果如下：\r\n";
            msg += "总计 " + Huancun.PiliangTianjiaLiebiao.Count.ToString() + " 个\r\n";
            msg += "成功 " + chenggong.ToString() + " 个\r\n";
            msg += "失败 " + shibai.ToString() + " 个";
            MessageBox.Show(msg, "笨蛋雪说：", buttons: MessageBoxButtons.OK, icon: MessageBoxIcon.Asterisk);
            textBox1.Text = shibaiMingdan;
        }
    }
}
