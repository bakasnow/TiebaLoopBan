using System;
using System.Windows.Forms;
using TiebaLib;

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
            if (ZhuangTai == ZhuangTaiLeiXing.BianJi)
            {
                Text = "编辑";

                textBox1.Enabled = false;
                textBox3.Enabled = false;
                textBox2.Enabled = false;

                FengJinXinXi.JieGou jieGou = FengJinXinXi.Get(ID);

                textBox1.Text = jieGou.YongHuMing;
                textBox3.Text = jieGou.TouXiang;
                textBox2.Text = jieGou.TiebaName;
                dateTimePicker1.Value = Convert.ToDateTime(jieGou.XunHuanKaiShiShiJian);
                dateTimePicker2.Value = Convert.ToDateTime(jieGou.XunHuanJieShuShiJian);
            }
            else
            {
                Text = "新建";

                textBox1.Enabled = true;
                textBox3.Enabled = true;
                textBox2.Enabled = true;

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
        }

        /// <summary>
        /// 保存
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            //新建
            if (ZhuangTai == ZhuangTaiLeiXing.XinJian)
            {
                if (string.IsNullOrEmpty(textBox1.Text) && string.IsNullOrEmpty(textBox3.Text))
                {
                    MessageBox.Show("请填写用户名或头像", "笨蛋雪说：", buttons: MessageBoxButtons.OK, icon: MessageBoxIcon.Exclamation);
                    return;
                }

                //检查头像前缀
                if (textBox3.Text.StartsWith("tb.") && textBox3.Text.Length < 36)
                {
                    MessageBox.Show("头像格式错误", "笨蛋雪说：", buttons: MessageBoxButtons.OK, icon: MessageBoxIcon.Exclamation);
                    return;
                }

                if (string.IsNullOrEmpty(textBox2.Text))
                {
                    MessageBox.Show("请填写贴吧名", "笨蛋雪说：", buttons: MessageBoxButtons.OK, icon: MessageBoxIcon.Exclamation);
                    return;
                }
            }

            if (Tools.HuoQuFengJinShiChang(dateTimePicker1, dateTimePicker2) <= 0)
            {
                MessageBox.Show("封禁时长必须大于0", "笨蛋雪说：", buttons: MessageBoxButtons.OK, icon: MessageBoxIcon.Exclamation);
                return;
            }

            string yongHuMing = textBox1.Text;
            string touXiang = textBox3.Text;
            string tiebaName = textBox2.Text;
            string kaiShiShiJian = dateTimePicker1.Text;
            string jieShuShiJian = dateTimePicker2.Text;

            //新建
            if (ZhuangTai == ZhuangTaiLeiXing.XinJian)
            {
                Tieba.MingPianJieGou mingPianJieGou = Tieba.GetTiebaMingPian(string.IsNullOrEmpty(yongHuMing) ? touXiang : yongHuMing);
                if (!mingPianJieGou.HuoQuChengGong)
                {
                    MessageBox.Show("用户信息获取失败", "笨蛋雪说：", buttons: MessageBoxButtons.OK, icon: MessageBoxIcon.Error);
                    return;
                }

                //更新任务参数
                yongHuMing = mingPianJieGou.YongHuMing;
                touXiang = mingPianJieGou.TouXiang;

                //检查重复
                if (Form1.access.GetDataTable($"select * from 封禁列表 where (用户名='{yongHuMing}' or TouXiang='{touXiang}') and 贴吧名='{tiebaName}'").Rows.Count > 0)
                {
                    MessageBox.Show(string.IsNullOrEmpty(yongHuMing) ? mingPianJieGou.NiCheng : yongHuMing + " 在 " + tiebaName + "吧已被添加", "笨蛋雪说：", buttons: MessageBoxButtons.OK, icon: MessageBoxIcon.Asterisk);
                    return;
                }

                int jieGuo = Form1.access.DoCommand($"insert into 封禁列表 (用户名,头像,贴吧名,最后封禁时间,循环开始时间,循环结束时间)" +
                    $" values('{yongHuMing}','{touXiang}','{tiebaName}','1970-01-01','{kaiShiShiJian}','{jieShuShiJian}')");
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
                int jieGuo = Form1.access.DoCommand($"update 封禁列表 set 循环开始时间='{kaiShiShiJian}',循环结束时间='{jieShuShiJian}' where 用户名='{yongHuMing}' and 贴吧名='{tiebaName}'");
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
            MessageBox.Show("填写头像Id可以封禁没有用户名的账号", "笨蛋雪说：", buttons: MessageBoxButtons.OK, icon: MessageBoxIcon.Information);
        }
    }
}
