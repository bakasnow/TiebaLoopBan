using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using TiebaApi.TiebaAppApi;
using TiebaApi.TiebaJieGou;
using TiebaApi.TiebaWebApi;

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

            dateTimePicker1.Enabled = false;

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
            label_fengJinShiChang.Text = $"封禁时长：{Tools.HuoQuFengJinShiChang(dateTimePicker1, dateTimePicker2)}天";
        }

        /// <summary>
        /// 结束时间被修改
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dateTimePicker2_ValueChanged(object sender, EventArgs e)
        {
            label_fengJinShiChang.Text = $"封禁时长：{Tools.HuoQuFengJinShiChang(dateTimePicker1, dateTimePicker2)}天";
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

            button1.Enabled = false;
            textBox1.Enabled = false;
            dateTimePicker1.Enabled = false;
            dateTimePicker2.Enabled = false;
            button_day_type1.Enabled = false;
            button_day_type2.Enabled = false;
            button_day_type3.Enabled = false;
            radioButton1.Enabled = false;
            radioButton2.Enabled = false;

            Task.Run(() =>
            {
                progressBar1.Minimum = 0;
                progressBar1.Maximum = 200;
                progressBar1.Value = 0;

                int chengGongShu = 0, shiBaiShu = 0, chongFuShu = 0;

                string kaiShiShiJian = dateTimePicker1.Text;
                string jieShuShiJian = dateTimePicker2.Text;
                string[] yongHuMingDan = textBox1.Text.Split(Environment.NewLine.ToCharArray());

                string shiBaiMingDan = string.Empty;

                //用户名为纯数字的识别方式
                bool isTiebaHao = radioButton2.Checked;

                //初始化时间戳，减11秒保证第一次运行
                DateTime tiebaHaoShiJianChuo = DateTime.Now.AddSeconds(-11);

                //过滤+去重复
                PiLiangTianJiaHuanCun.LieBiao = new List<PiLiangTianJiaHuanCun.JieGou>();
                for (int i = 0; i < yongHuMingDan.Length; i++)
                {
                    progressBar1.Value = (i + 1) * 100 / yongHuMingDan.Length;

                    if (string.IsNullOrEmpty(yongHuMingDan[i]))
                    {
                        continue;
                    }

                    string[] fenGe = yongHuMingDan[i].Split('|');
                    if (fenGe.Length != 2)
                    {
                        continue;
                    }

                    string tiebaName = fenGe[0];

                    //分类
                    string touXiangID = "", zhuXianZhangHao = "";

                    //用户名是纯数字，并且是贴吧号
                    if (Regex.IsMatch(fenGe[1], "^[0-9]{1,}$") && isTiebaHao)
                    {
                        //贴吧号添加间隔大于10秒
                        while ((DateTime.Now - tiebaHaoShiJianChuo).TotalSeconds < 10)
                        {
                            Thread.Sleep(500);
                        }

                        //更新时间戳
                        tiebaHaoShiJianChuo = DateTime.Now;

                        TiebaYongHuSouSuoJieGou tiebaYongHuSouSuoJieGou = TiebaApp.TiebaYongHuSouSuo(Convert.ToInt64(fenGe[1]));
                        if (tiebaYongHuSouSuoJieGou.HuoQuChengGong)
                        {
                            touXiangID = tiebaYongHuSouSuoJieGou.TouXiangID;
                            zhuXianZhangHao = Tools.HuoQuZhuXianZhangHao(tiebaYongHuSouSuoJieGou);
                        }
                    }
                    //用户名或头像
                    else
                    {
                        TiebaMingPianJieGou mingPianJieGou = TiebaWeb.GetTiebaMingPian(fenGe[1]);
                        if (mingPianJieGou.HuoQuChengGong)
                        {
                            touXiangID = mingPianJieGou.TouXiangID;
                            zhuXianZhangHao = Tools.HuoQuZhuXianZhangHao(mingPianJieGou);
                        }
                    }

                    //跳过获取失败的
                    if (string.IsNullOrEmpty(touXiangID))
                    {
                        shiBaiMingDan += $"{yongHuMingDan[i]}\r\n";
                        shiBaiShu++;
                        continue;
                    }

                    //去重复
                    if (PiLiangTianJiaHuanCun.LieBiao.Count(md => md.TiebaName == fenGe[0] && md.TouXiangID == fenGe[1]) > 0)
                    {
                        chongFuShu++;
                        continue;
                    }

                    PiLiangTianJiaHuanCun.LieBiao.Add(new PiLiangTianJiaHuanCun.JieGou
                    {
                        TiebaName = tiebaName,
                        TouXiangID = touXiangID,
                        ZhuXianZhangHao = zhuXianZhangHao
                    });
                }

                progressBar1.Value = 100;

                //添加到数据库
                for (int i = 0; i < PiLiangTianJiaHuanCun.LieBiao.Count; i++)
                {
                    progressBar1.Value = ((i + 1) * 100 / PiLiangTianJiaHuanCun.LieBiao.Count) + 100;

                    string tiebaName = PiLiangTianJiaHuanCun.LieBiao[i].TiebaName;
                    string touXiang = PiLiangTianJiaHuanCun.LieBiao[i].TouXiangID;
                    string yongHuMing = PiLiangTianJiaHuanCun.LieBiao[i].ZhuXianZhangHao;

                    TiebaMingPianJieGou mingPianJieGou = TiebaWeb.GetTiebaMingPian(touXiang);
                    if (!mingPianJieGou.HuoQuChengGong)
                    {
                        shiBaiMingDan += $"{tiebaName}|{(string.IsNullOrEmpty(yongHuMing) ? touXiang : yongHuMing)}\r\n";
                        shiBaiShu++;
                        continue;
                    }

                    //数据库检查重复
                    if (DB.access.GetDataTable($"select * from 封禁列表 where 头像='{mingPianJieGou.TouXiangID}' and 贴吧名='{tiebaName}'").Rows.Count > 0)
                    {
                        chongFuShu++;
                        continue;
                    }

                    int jieGuo = DB.access.DoCommand($"insert into 封禁列表 (用户名,头像,贴吧名,最后封禁时间,循环开始时间,循环结束时间)" +
                        $" values('{Tools.HuoQuZhuXianZhangHao(mingPianJieGou)}','{mingPianJieGou.TouXiangID}','{tiebaName}','1970-01-01','{kaiShiShiJian}','{jieShuShiJian}')");
                    if (jieGuo > 0)
                    {
                        chengGongShu++;
                    }
                    else
                    {
                        shiBaiMingDan += $"{tiebaName}|{(string.IsNullOrEmpty(yongHuMing) ? touXiang : yongHuMing)}\r\n";
                        shiBaiShu++;
                    }
                }

                progressBar1.Value = 200;

                string msg = "批量添加结果如下：\n" +
                    $"总计={(chengGongShu + shiBaiShu + chongFuShu)}个\n" +
                    $"成功={chengGongShu}个\n" +
                    $"失败={shiBaiShu}个\n" +
                    $"重复={chongFuShu}个";

                if (shiBaiShu > 0)
                {
                    msg += "\n失败名单将退回列表，重复用户已被剔除。";
                }

                textBox1.Text = shiBaiMingDan;

                MessageBox.Show(msg, "笨蛋雪说：", buttons: MessageBoxButtons.OK, icon: MessageBoxIcon.Information);

                textBox1.Enabled = true;
                dateTimePicker2.Enabled = true;
                button_day_type1.Enabled = true;
                button_day_type2.Enabled = true;
                button_day_type3.Enabled = true;
                radioButton1.Enabled = true;
                radioButton2.Enabled = true;
                button1.Enabled = true;
            });
        }

        /// <summary>
        /// 按钮 天数 类型1 被单击
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button_day_type1_Click(object sender, EventArgs e)
        {
            dateTimePicker2.Value = dateTimePicker1.Value.AddDays(30);
        }

        /// <summary>
        /// 按钮 天数 类型2 被单击
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button_day_type2_Click(object sender, EventArgs e)
        {
            dateTimePicker2.Value = dateTimePicker1.Value.AddYears(1);
        }

        /// <summary>
        /// 按钮 天数 类型3 被单击
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button_day_type3_Click(object sender, EventArgs e)
        {
            dateTimePicker2.Value = new DateTime(2999, 12, 31, 23, 59, 59);
        }
    }
}
