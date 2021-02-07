using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using TiebaLib;

namespace TiebaLoopBan
{
    public partial class PiLiangTianJia : Form
    {
        public PiLiangTianJia()
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
            label4.Text = $"封禁时长：{Tools.HuoQuFengJinShiChang(dateTimePicker1, dateTimePicker2)}天";
        }

        /// <summary>
        /// 结束时间被修改
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dateTimePicker2_ValueChanged(object sender, EventArgs e)
        {
            label4.Text = $"封禁时长：{Tools.HuoQuFengJinShiChang(dateTimePicker1, dateTimePicker2)}天";
        }

        /// <summary>
        /// 批量添加
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            if (Tools.HuoQuFengJinShiChang(dateTimePicker1, dateTimePicker2) <= 0)
            {
                MessageBox.Show("封禁时长必须大于0", "笨蛋雪说：", buttons: MessageBoxButtons.OK, icon: MessageBoxIcon.Exclamation);
                return;
            }

            string kaiShiShiJian = dateTimePicker1.Text;
            string jieShuShiJian = dateTimePicker2.Text;
            string[] yongHuMingDan = textBox1.Text.Split(Environment.NewLine.ToCharArray());

            //过滤
            PiLiangTianJiaHuanCun.LieBiao = new List<PiLiangTianJiaHuanCun.JieGou>();
            foreach (string mingDan in yongHuMingDan)
            {
                if (string.IsNullOrEmpty(mingDan))
                {
                    continue;
                }

                string[] fenGe = mingDan.Split('|');
                if (fenGe.Length != 2)
                {
                    continue;
                }

                if (PiLiangTianJiaHuanCun.LieBiao.Count(md => md.TiebaName == fenGe[0] && md.Id == fenGe[1]) > 0)
                {
                    continue;
                }

                PiLiangTianJiaHuanCun.LieBiao.Add(new PiLiangTianJiaHuanCun.JieGou
                {
                    TiebaName = fenGe[0],
                    Id = fenGe[1]
                });
            }

            //添加到数据库
            int chengGongShu = 0;
            int shiBaiShu = 0;
            for (int i = 0; i < PiLiangTianJiaHuanCun.LieBiao.Count; i++)
            {
                string tiebaName = PiLiangTianJiaHuanCun.LieBiao[i].TiebaName;
                string id = PiLiangTianJiaHuanCun.LieBiao[i].Id;

                Tieba.MingPianJieGou mingPianJieGou = Tieba.GetTiebaMingPian(id);
                if (!mingPianJieGou.HuoQuChengGong)
                {
                    shiBaiShu += 1;
                    continue;
                }

                //数据库检查重复
                if (Form1.access.GetDataTable($"select * from 封禁列表 where 头像='{id}' and 贴吧名='{tiebaName}'").Rows.Count > 0)
                {
                    shiBaiShu += 1;
                    continue;
                }

                int jieGuo = Form1.access.DoCommand($"insert into 封禁列表 (用户名,头像,贴吧名,最后封禁时间,循环开始时间,循环结束时间)" +
                    $" values('{mingPianJieGou.YongHuMing}','{mingPianJieGou.TouXiang}','{tiebaName}','1970-01-01','{kaiShiShiJian}','{jieShuShiJian}')");
                if (jieGuo > 0)
                {
                    chengGongShu += 1;
                    PiLiangTianJiaHuanCun.LieBiao[i].TianJiaChengGong = true;
                }
                else
                {
                    shiBaiShu += 1;
                }
            }

            string msg = "批量添加结果如下：\n" +
                $"总计={PiLiangTianJiaHuanCun.LieBiao.Count}个\n" +
                $"成功={chengGongShu}个\n" +
                $"失败={shiBaiShu}个";

            if (shiBaiShu > 0)
            {
                msg += "\n失败名单已退回列表";
            }

            string shiBaiMingDan = string.Empty;
            foreach (var mingDan in PiLiangTianJiaHuanCun.LieBiao.Where(md => md.TianJiaChengGong == false))
            {
                shiBaiMingDan += $"{mingDan.TiebaName}|{mingDan.Id}\r\n";
            }

            textBox1.Text = shiBaiMingDan;

            MessageBox.Show(msg, "笨蛋雪说：", buttons: MessageBoxButtons.OK, icon: MessageBoxIcon.Information);
        }
    }
}
