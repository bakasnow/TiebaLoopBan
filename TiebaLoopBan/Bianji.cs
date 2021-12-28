using System;
using System.Windows.Forms;
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
            dateTimePicker1.Enabled = false;

            if (ZhuangTai == ZhuangTaiLeiXing.BianJi)
            {
                Text = "编辑";

                textBox_tiebaName.Enabled = false;
                textBox_touXiang.Enabled = false;

                FengJinXinXi.JieGou jieGou = FengJinXinXi.Get(ID);

                textBox_tiebaName.Text = jieGou.TiebaName;
                textBox_touXiang.Text = jieGou.TouXiang;
                textBox_zhuXianZhangHao.Text = jieGou.ZhuXianZhangHao;
                dateTimePicker1.Value = Convert.ToDateTime(jieGou.XunHuanKaiShiShiJian);
                dateTimePicker2.Value = Convert.ToDateTime(jieGou.XunHuanJieShuShiJian);
            }
            else
            {
                Text = "新建";

                textBox_tiebaName.Enabled = true;
                textBox_touXiang.Enabled = true;

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
                TouXiang = textBox_touXiang.Text,
                TiebaName = textBox_tiebaName.Text,
                ZhuXianZhangHao = textBox_zhuXianZhangHao.Text,
                XunHuanKaiShiShiJian = dateTimePicker1.Text,
                XunHuanJieShuShiJian = dateTimePicker2.Text
            };

            //自动获取头像ID
            if (JieGou.ZhuXianZhangHao.Length > 0 && Tieba.GuoLvTouXiangID(JieGou.TouXiang).Length <= 0)
            {
                TiebaMingPianJieGou mingPianJieGou = TiebaWeb.GetTiebaMingPian(JieGou.ZhuXianZhangHao);
                if (!mingPianJieGou.HuoQuChengGong)
                {
                    MessageBox.Show("头像ID获取失败，请人工填写或重试", "笨蛋雪说：", buttons: MessageBoxButtons.OK, icon: MessageBoxIcon.Exclamation);
                    return;
                }

                pictureBox1.ImageLocation = $"http://tb.himg.baidu.com/sys/portrait/item/{mingPianJieGou.TouXiang}";

                textBox_touXiang.Text = mingPianJieGou.TouXiang;
                JieGou.TouXiang = mingPianJieGou.TouXiang;

                if (MessageBox.Show("请查看头像，确定用户信息是否正确", "笨蛋雪说：", buttons: MessageBoxButtons.YesNo, icon: MessageBoxIcon.Information) == DialogResult.No)
                {
                    return;
                }
            }

            //新建
            if (ZhuangTai == ZhuangTaiLeiXing.XinJian)
            {
                if (string.IsNullOrEmpty(JieGou.TouXiang))
                {
                    MessageBox.Show("请填写头像ID", "笨蛋雪说：", buttons: MessageBoxButtons.OK, icon: MessageBoxIcon.Exclamation);
                    return;
                }

                if (!JieGou.TouXiang.Contains("tb."))
                {
                    MessageBox.Show("头像ID格式错误", "笨蛋雪说：", buttons: MessageBoxButtons.OK, icon: MessageBoxIcon.Exclamation);
                    return;
                }

                if (string.IsNullOrEmpty(JieGou.TiebaName))
                {
                    MessageBox.Show("请填写贴吧名", "笨蛋雪说：", buttons: MessageBoxButtons.OK, icon: MessageBoxIcon.Exclamation);
                    return;
                }
            }

            //获取封禁时长
            if (Tools.HuoQuFengJinShiChang(dateTimePicker1, dateTimePicker2) <= 0)
            {
                MessageBox.Show("封禁时长必须大于0", "笨蛋雪说：", buttons: MessageBoxButtons.OK, icon: MessageBoxIcon.Exclamation);
                return;
            }

            //新建
            if (ZhuangTai == ZhuangTaiLeiXing.XinJian)
            {
                TiebaMingPianJieGou mingPianJieGou = TiebaWeb.GetTiebaMingPian(JieGou.TouXiang);
                if (!mingPianJieGou.HuoQuChengGong)
                {
                    MessageBox.Show("用户信息获取失败", "笨蛋雪说：", buttons: MessageBoxButtons.OK, icon: MessageBoxIcon.Error);
                    return;
                }

                //更新任务参数
                JieGou.ZhuXianZhangHao = mingPianJieGou.YongHuMing;
                JieGou.TouXiang = mingPianJieGou.TouXiang;

                //检查重复
                if (Form1.access.GetDataTable($"select * from 封禁列表 where 头像='{JieGou.TouXiang}' and 贴吧名='{JieGou.TiebaName}'").Rows.Count > 0)
                {
                    MessageBox.Show($"{JieGou.ZhuXianZhangHao}已经在{JieGou.TiebaName}吧添加过了", "笨蛋雪说：", buttons: MessageBoxButtons.OK, icon: MessageBoxIcon.Asterisk);
                    return;
                }

                int jieGuo = Form1.access.DoCommand($"insert into 封禁列表 (用户名,头像,贴吧名,最后封禁时间,循环开始时间,循环结束时间)" +
                    $" values('{JieGou.ZhuXianZhangHao}','{JieGou.TouXiang}','{JieGou.TiebaName}','1970-01-01','{JieGou.XunHuanKaiShiShiJian}','{JieGou.XunHuanJieShuShiJian}')");
                if (jieGuo > 0)
                {
                    Dispose();
                }
                else
                {
                    MessageBox.Show("添加失败", "笨蛋雪说：", buttons: MessageBoxButtons.OK, icon: MessageBoxIcon.Error);
                }
            }
            //编辑
            else if (ZhuangTai == ZhuangTaiLeiXing.BianJi)
            {
                int jieGuo = Form1.access.DoCommand($"update 封禁列表 set 用户名='{JieGou.ZhuXianZhangHao}',循环开始时间='{JieGou.XunHuanKaiShiShiJian}',循环结束时间='{JieGou.XunHuanJieShuShiJian}' where 头像='{JieGou.TouXiang}' and 贴吧名='{JieGou.TiebaName}'");
                if (jieGuo > 0)
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
            label5.Text = $"封禁时长：{Tools.HuoQuFengJinShiChang(dateTimePicker1, dateTimePicker2)}天";
        }

        /// <summary>
        /// 结束时间被修改
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dateTimePicker2_ValueChanged(object sender, EventArgs e)
        {
            label5.Text = $"封禁时长：{Tools.HuoQuFengJinShiChang(dateTimePicker1, dateTimePicker2)}天";
        }

        private void label8_Click(object sender, EventArgs e)
        {
            MessageBox.Show("头像ID可以封禁没有用户名的账号\n了解详情请加贴吧管理器交流群：984150818", "笨蛋雪说：", buttons: MessageBoxButtons.OK, icon: MessageBoxIcon.Information);
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

            pictureBox1.ImageLocation = $"http://tb.himg.baidu.com/sys/portrait/item/{touXiang}";

            textBox_zhuXianZhangHao.Text = Tools.HuoQuZhuXianZhangHao(mingPianJieGou);
        }
    }
}
