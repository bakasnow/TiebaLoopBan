using System;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using TiebaApi.TiebaAppApi;
using TiebaApi.TiebaJieGou;
using TiebaApi.TiebaWebApi;

namespace TiebaLoopBan
{
    public partial class BianJi : Form
    {
        /// <summary>
        /// 状态类型枚举
        /// </summary>
        public enum ZhuangTaiLeiXing
        {
            /// <summary>
            /// 新建
            /// </summary>
            XinJian = 0,

            /// <summary>
            /// 编辑
            /// </summary>
            BianJi = 1
        }

        /// <summary>
        /// 结构
        /// </summary>
        public FengJinXinXi.JieGou JieGou { get; private set; }

        /// <summary>
        /// 状态类型
        /// </summary>
        private readonly ZhuangTaiLeiXing ZhuangTai;

        /// <summary>
        /// 数据库ID
        /// </summary>
        private readonly string ID;

        public BianJi(ZhuangTaiLeiXing zhuangTaiLeiXing, string id)
        {
            InitializeComponent();

            ZhuangTai = zhuangTaiLeiXing;
            ID = id;
        }

        /// <summary>
        /// 窗口创建
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Bianji_Load(object sender, EventArgs e)
        {
            //将类型索引设为0
            comboBox_ziDong_leiXing.SelectedIndex = 0;

            dateTimePicker1.Enabled = false;

            if (ZhuangTai == ZhuangTaiLeiXing.BianJi)
            {
                Text = "编辑";

                comboBox_ziDong_leiXing.Enabled = false;
                textBox_ziDong_neiRong.Enabled = false;
                button_ziDong_huoQu.Enabled = false;

                textBox_tiebaName.Enabled = false;
                textBox_touXiang.Enabled = false;

                label_yongHuMing.Visible = false;
                textBox_yongHuMing.Visible = false;

                FengJinXinXi.JieGou jieGou = FengJinXinXi.Get(ID);

                textBox_tiebaName.Text = jieGou.TiebaName;
                textBox_touXiang.Text = jieGou.TouXiangID;
                textBox_zhuXianZhangHao.Text = jieGou.ZhuXianZhangHao;
                textBox_yongHuMing.Text = jieGou.YongHuMing;
                dateTimePicker1.Value = Convert.ToDateTime(jieGou.XunHuanKaiShiShiJian);
                dateTimePicker2.Value = Convert.ToDateTime(jieGou.XunHuanJieShuShiJian);

                pictureBox_touXiang.ImageLocation = $"http://tb.himg.baidu.com/sys/portrait/item/{jieGou.TouXiangID}";
            }
            else
            {
                Text = "新建";

                comboBox_ziDong_leiXing.Enabled = true;
                textBox_ziDong_neiRong.Enabled = true;
                button_ziDong_huoQu.Enabled = true;

                textBox_tiebaName.Enabled = true;
                textBox_touXiang.Enabled = true;

                label_yongHuMing.Visible = true;
                textBox_yongHuMing.Visible = true;

                dateTimePicker1.Value = DateTime.Now;
                dateTimePicker2.Value = DateTime.Now.AddMonths(1);
            }

            //限制格式
            dateTimePicker1.Format = DateTimePickerFormat.Custom;
            dateTimePicker1.CustomFormat = "yyyy-MM-dd";

            dateTimePicker2.Format = DateTimePickerFormat.Custom;
            dateTimePicker2.CustomFormat = "yyyy-MM-dd";

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

            if (JieGou == null)
            {
                JieGou = new FengJinXinXi.JieGou();
            }
        }

        /// <summary>
        /// 保存
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            JieGou = new FengJinXinXi.JieGou
            {
                TouXiangID = textBox_touXiang.Text,
                TiebaName = textBox_tiebaName.Text,
                ZhuXianZhangHao = textBox_zhuXianZhangHao.Text,
                YongHuMing = textBox_yongHuMing.Text,
                XunHuanKaiShiShiJian = dateTimePicker1.Text,
                XunHuanJieShuShiJian = dateTimePicker2.Text
            };

            //新建
            if (ZhuangTai == ZhuangTaiLeiXing.XinJian)
            {
                if (string.IsNullOrEmpty(textBox_touXiang.Text))
                {
                    MessageBox.Show("头像ID不得为空", "笨蛋雪说：", buttons: MessageBoxButtons.OK, icon: MessageBoxIcon.Exclamation);
                    JieGou = null;
                    return;
                }

                if (string.IsNullOrEmpty(Tieba.GuoLvTouXiangID(textBox_touXiang.Text)))
                {
                    MessageBox.Show("头像ID格式错误", "笨蛋雪说：", buttons: MessageBoxButtons.OK, icon: MessageBoxIcon.Exclamation);
                    JieGou = null;
                    return;
                }

                if (string.IsNullOrEmpty(JieGou.TiebaName))
                {
                    MessageBox.Show("请填写贴吧名", "笨蛋雪说：", buttons: MessageBoxButtons.OK, icon: MessageBoxIcon.Exclamation);
                    JieGou = null;
                    return;
                }
            }

            //获取封禁时长
            if (Tools.HuoQuFengJinShiChang(dateTimePicker1, dateTimePicker2) <= 0)
            {
                MessageBox.Show("封禁时长必须大于0", "笨蛋雪说：", buttons: MessageBoxButtons.OK, icon: MessageBoxIcon.Exclamation);
                JieGou = null;
                return;
            }

            //新建
            if (ZhuangTai == ZhuangTaiLeiXing.XinJian)
            {
                //TiebaMingPianJieGou mingPianJieGou = TiebaWeb.GetTiebaMingPian(JieGou.TouXiangID);
                //if (!mingPianJieGou.HuoQuChengGong)
                //{
                //    MessageBox.Show("用户信息获取失败", "笨蛋雪说：", buttons: MessageBoxButtons.OK, icon: MessageBoxIcon.Error);
                //    JieGou = null;
                //    return;
                //}

                ////更新任务参数
                //JieGou.ZhuXianZhangHao = mingPianJieGou.YongHuMing;
                //JieGou.TouXiangID = mingPianJieGou.TouXiangID;

                //检查重复
                if (DB.access.GetDataTable($"select * from 封禁列表 where 头像='{JieGou.TouXiangID}' and 贴吧名='{JieGou.TiebaName}'").Rows.Count > 0)
                {
                    MessageBox.Show($"{JieGou.ZhuXianZhangHao}已经在{JieGou.TiebaName}吧添加过了", "笨蛋雪说：", buttons: MessageBoxButtons.OK, icon: MessageBoxIcon.Asterisk);
                    JieGou = null;
                    return;
                }

                int jieGuo = DB.access.DoCommand($"insert into 封禁列表 (用户名,头像,贴吧名,最后封禁时间,循环开始时间,循环结束时间)" +
                    $" values('{JieGou.ZhuXianZhangHao}','{JieGou.TouXiangID}','{JieGou.TiebaName}','1970-01-01','{JieGou.XunHuanKaiShiShiJian}','{JieGou.XunHuanJieShuShiJian}')");
                if (jieGuo > 0)
                {
                    Dispose();
                }
                else
                {
                    MessageBox.Show("添加失败", "笨蛋雪说：", buttons: MessageBoxButtons.OK, icon: MessageBoxIcon.Error);
                    JieGou = null;
                    return;
                }
            }
            //编辑
            else if (ZhuangTai == ZhuangTaiLeiXing.BianJi)
            {
                int jieGuo = DB.access.DoCommand($"update 封禁列表 set 用户名='{JieGou.ZhuXianZhangHao}',循环开始时间='{JieGou.XunHuanKaiShiShiJian}',循环结束时间='{JieGou.XunHuanJieShuShiJian}' where 头像='{JieGou.TouXiangID}' and 贴吧名='{JieGou.TiebaName}'");
                if (jieGuo > 0)
                {
                    Dispose();
                }
                else
                {
                    MessageBox.Show("更新失败", "笨蛋雪说：", buttons: MessageBoxButtons.OK, icon: MessageBoxIcon.Error);
                    JieGou = null;
                    return;
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

        private void label8_Click(object sender, EventArgs e)
        {
            MessageBox.Show("头像ID可以封禁没有用户名的账号\n\n请使用上方“用户信息获取”功能获取头像ID", "笨蛋雪说：", buttons: MessageBoxButtons.OK, icon: MessageBoxIcon.Information);
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

        /// <summary>
        ///  编辑框 头像 文本有变动
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void textBox_touXiang_TextChanged(object sender, EventArgs e)
        {
            string touXiang = Tieba.GuoLvTouXiangID(textBox_touXiang.Text);
            if (string.IsNullOrEmpty(touXiang))
            {
                return;
            }

            TiebaMingPianJieGou mingPianJieGou = TiebaWeb.GetTiebaMingPian(touXiang);
            if (!mingPianJieGou.HuoQuChengGong)
            {
                return;
            }

            pictureBox_touXiang.ImageLocation = $"http://tb.himg.baidu.com/sys/portrait/item/{touXiang}";

            textBox_touXiang.Text = mingPianJieGou.TouXiangID;
            textBox_zhuXianZhangHao.Text = Tools.HuoQuZhuXianZhangHao(mingPianJieGou);
            textBox_yongHuMing.Text = mingPianJieGou.YongHuMing;
        }

        /// <summary>
        /// 按钮 自动获取 被单击
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button_ziDong_huoQu_Click(object sender, EventArgs e)
        {
            string neiRong = textBox_ziDong_neiRong.Text;
            if (string.IsNullOrEmpty(neiRong))
            {
                return;
            }

            TiebaMingPianJieGou mingPianJieGou;
            string touXiangID;

            switch (comboBox_ziDong_leiXing.SelectedIndex)
            {
                case 0:
                    mingPianJieGou = TiebaWeb.GetTiebaMingPian(neiRong);
                    if (!mingPianJieGou.HuoQuChengGong)
                    {
                        MessageBox.Show("头像ID获取失败，请稍后重试", "笨蛋雪说：", buttons: MessageBoxButtons.OK, icon: MessageBoxIcon.Exclamation);
                        return;
                    }

                    pictureBox_touXiang.ImageLocation = $"http://tb.himg.baidu.com/sys/portrait/item/{mingPianJieGou.TouXiangID}";

                    textBox_touXiang.Text = mingPianJieGou.TouXiangID;
                    textBox_zhuXianZhangHao.Text = Tools.HuoQuZhuXianZhangHao(mingPianJieGou);
                    textBox_yongHuMing.Text = mingPianJieGou.YongHuMing;

                    break;

                case 1:
                    //先从链接本身提取
                    touXiangID = Tieba.GuoLvTouXiangID(neiRong);

                    //链接本身提取成功
                    if (!string.IsNullOrEmpty(touXiangID))
                    {
                        //直接去获取名片
                    }
                    ////从链接内获取
                    //else if (neiRong.Contains("tieba.baidu.com/home/main"))
                    //{
                    //    //如果是个人主页
                    //    string html = TiebaHttpHelper.Get(neiRong, Config.Cookie);

                    //    Debug.WriteLine(html);

                    //    //检查抓到的数据
                    //    touXiangID = Tieba.GuoLvTouXiangID(BST.JieQuWenBen(html, "<a class=\"nav_icon nav_main\"", ">他的主页</a>"));

                    //    Debug.WriteLine("----------------------------------");

                    //    Debug.WriteLine(touXiangID);

                    //    if (string.IsNullOrEmpty(touXiangID))
                    //    {
                    //        //如果头像ID不正确
                    //        MessageBox.Show("头像ID获取失败，请稍后重试", "笨蛋雪说：", buttons: MessageBoxButtons.OK, icon: MessageBoxIcon.Exclamation);
                    //        return;
                    //    }
                    //}
                    else
                    {
                        MessageBox.Show("无效或不受支持的链接类型，如需帮助请联系作者", "笨蛋雪说：", buttons: MessageBoxButtons.OK, icon: MessageBoxIcon.Exclamation);
                        return;
                    }

                    //获取名片
                    mingPianJieGou = TiebaWeb.GetTiebaMingPian(touXiangID);
                    if (!mingPianJieGou.HuoQuChengGong)
                    {
                        MessageBox.Show("头像ID获取失败，请稍后重试", "笨蛋雪说：", buttons: MessageBoxButtons.OK, icon: MessageBoxIcon.Exclamation);
                        return;
                    }

                    pictureBox_touXiang.ImageLocation = $"http://tb.himg.baidu.com/sys/portrait/item/{mingPianJieGou.TouXiangID}";

                    textBox_touXiang.Text = mingPianJieGou.TouXiangID;
                    textBox_zhuXianZhangHao.Text = Tools.HuoQuZhuXianZhangHao(mingPianJieGou);
                    textBox_yongHuMing.Text = mingPianJieGou.YongHuMing;

                    break;

                case 2:
                    //先从链接本身提取
                    touXiangID = Tieba.GuoLvTouXiangID(neiRong);

                    //链接本身提取成功
                    if (!string.IsNullOrEmpty(touXiangID))
                    {
                        //直接去获取名片
                    }
                    else
                    {
                        MessageBox.Show("无效或不受支持的链接类型，如需帮助请联系作者", "笨蛋雪说：", buttons: MessageBoxButtons.OK, icon: MessageBoxIcon.Exclamation);
                        return;
                    }

                    //获取名片
                    mingPianJieGou = TiebaWeb.GetTiebaMingPian(touXiangID);
                    if (!mingPianJieGou.HuoQuChengGong)
                    {
                        MessageBox.Show("头像ID获取失败，请稍后重试", "笨蛋雪说：", buttons: MessageBoxButtons.OK, icon: MessageBoxIcon.Exclamation);
                        return;
                    }

                    pictureBox_touXiang.ImageLocation = $"http://tb.himg.baidu.com/sys/portrait/item/{mingPianJieGou.TouXiangID}";

                    textBox_touXiang.Text = mingPianJieGou.TouXiangID;
                    textBox_zhuXianZhangHao.Text = Tools.HuoQuZhuXianZhangHao(mingPianJieGou);
                    textBox_yongHuMing.Text = mingPianJieGou.YongHuMing;

                    break;

                case 3:
                    if (!long.TryParse(new Regex("[0-9]{8,}").Match(neiRong).Value, out long tiebaHao))
                    {
                        MessageBox.Show("贴吧号是纯数字，请使用最新版贴吧客户端复制", "笨蛋雪说：", buttons: MessageBoxButtons.OK, icon: MessageBoxIcon.Exclamation);
                        return;
                    }

                    TiebaYongHuSouSuoJieGou tiebaYongHuSouSuoJieGou = TiebaApp.TiebaYongHuSouSuo(tiebaHao);
                    if (!tiebaYongHuSouSuoJieGou.HuoQuChengGong)
                    {
                        MessageBox.Show("贴吧号获取失败，请10秒后重试", "笨蛋雪说：", buttons: MessageBoxButtons.OK, icon: MessageBoxIcon.Exclamation);
                        return;
                    }

                    pictureBox_touXiang.ImageLocation = $"http://tb.himg.baidu.com/sys/portrait/item/{tiebaYongHuSouSuoJieGou.TouXiangID}";

                    textBox_touXiang.Text = tiebaYongHuSouSuoJieGou.TouXiangID;
                    textBox_zhuXianZhangHao.Text = Tools.HuoQuZhuXianZhangHao(tiebaYongHuSouSuoJieGou);
                    textBox_yongHuMing.Text = tiebaYongHuSouSuoJieGou.YongHuMing;

                    break;
            }
        }

        private void comboBox_ziDong_leiXing_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}
