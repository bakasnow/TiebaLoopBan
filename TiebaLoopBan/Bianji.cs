using System;
using System.Windows.Forms;

namespace TiebaLoopBan
{
    public partial class Bianji : Form
    {
        public Bianji()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 窗口创建
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Bianji_Load(object sender, EventArgs e)
        {
            if (BianjiShuju.BianjiZhuangtai == ChuandiZhuangtai.Bianji)
            {
                textBox1.Enabled = false;
                textBox2.Enabled = false;
            }
            else
            {
                textBox1.Enabled = true;
                textBox2.Enabled = true;
            }

            //限制格式
            dateTimePicker1.Format = DateTimePickerFormat.Custom;
            dateTimePicker1.CustomFormat = "yyyy-MM-dd";

            dateTimePicker2.Format = DateTimePickerFormat.Custom;
            dateTimePicker2.CustomFormat = "yyyy-MM-dd";

            //读取数据
            BianjiShuju.ShujuJiegou shuju = BianjiShuju.GetShuju();
            Text = shuju.Zhuangtai;
            textBox1.Text = shuju.Yonghuming;
            textBox2.Text = shuju.Tiebaname;
            dateTimePicker1.Value = shuju.XunhuanKaishiSj;
            dateTimePicker2.Value = shuju.XunhuanJieshuSj;
            shuju = null;
        }

        /// <summary>
        /// 窗口关闭前
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Bianji_FormClosing(object sender, FormClosingEventArgs e)
        {
            //e.Cancel = false;
            //if (MessageBox.Show("是否关闭？", "笨蛋雪说：", buttons: MessageBoxButtons.YesNo, icon: MessageBoxIcon.Asterisk) == DialogResult.No)
            //{
            //    e.Cancel = true;
            //}
        }

        /// <summary>
        /// 保存
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            //新建
            if (BianjiShuju.BianjiZhuangtai == ChuandiZhuangtai.Xinjian)
            {
                if (textBox1.Text == "" || textBox1.Text == null)
                {
                    MessageBox.Show("请填写用户名", "笨蛋雪说：", buttons: MessageBoxButtons.OK, icon: MessageBoxIcon.Exclamation);
                    return;
                }

                if (textBox2.Text == "" || textBox2.Text == null)
                {
                    MessageBox.Show("请填写贴吧名", "笨蛋雪说：", buttons: MessageBoxButtons.OK, icon: MessageBoxIcon.Exclamation);
                    return;
                }
            }
                
            if (Lib.HuoquFengjinShichang(dateTimePicker1, dateTimePicker2) <= 0)
            {
                MessageBox.Show("封禁时长必须大于0", "笨蛋雪说：", buttons: MessageBoxButtons.OK, icon: MessageBoxIcon.Exclamation);
                return;
            }

            string yonghuming = textBox1.Text;
            string tiebaname = textBox2.Text;
            string kaishisj = dateTimePicker1.Text;
            string jieshusj = dateTimePicker2.Text;

            //新建
            if (BianjiShuju.BianjiZhuangtai == ChuandiZhuangtai.Xinjian)
            {
                //检查重复
                if (Form1.db_tlb.GetDataTable($"select * from 封禁列表 where 用户名='{yonghuming}' and 贴吧名='{tiebaname}'").Rows.Count > 0)
                {
                    MessageBox.Show(yonghuming + " 在 " + tiebaname + "吧已被添加", "笨蛋雪说：", buttons: MessageBoxButtons.OK, icon: MessageBoxIcon.Asterisk);
                    return;
                }

                int jieguo = Form1.db_tlb.DoCommand($"insert into 封禁列表 (用户名,贴吧名,最后封禁时间,循环开始时间,循环结束时间) values('{yonghuming}','{tiebaname}','1970-01-01','{kaishisj}','{jieshusj}')");
                if (jieguo > 0)
                {
                    Dispose();
                }
                else
                {
                    MessageBox.Show("添加失败", "笨蛋雪说：", buttons: MessageBoxButtons.OK, icon: MessageBoxIcon.Error);
                }
            }
            //编辑
            else if (BianjiShuju.BianjiZhuangtai == ChuandiZhuangtai.Bianji)
            {
                int jieguo = Form1.db_tlb.DoCommand($"update 封禁列表 set 循环开始时间='{kaishisj}',循环结束时间='{jieshusj}' where 用户名='{yonghuming}' and 贴吧名='{tiebaname}'");
                if (jieguo > 0)
                {
                    Dispose();
                }
                else
                {
                    MessageBox.Show("更新失败", "笨蛋雪说：", buttons: MessageBoxButtons.OK, icon: MessageBoxIcon.Error);
                }
            }
        }

        /// <summary>
        /// 开始时间被修改
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {
            label5.Text = $"封禁时长：{Lib.HuoquFengjinShichang(dateTimePicker1, dateTimePicker2)}天";
        }

        /// <summary>
        /// 结束时间被修改
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dateTimePicker2_ValueChanged(object sender, EventArgs e)
        {
            label5.Text = $"封禁时长：{Lib.HuoquFengjinShichang(dateTimePicker1, dateTimePicker2)}天";
        }
    }
}
