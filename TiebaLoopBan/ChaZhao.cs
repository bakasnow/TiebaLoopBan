using System;
using System.Data;
using System.Windows.Forms;

namespace TiebaLoopBan
{
    public partial class ChaZhao : Form
    {
        public ChaZhao()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 窗口1列表1
        /// </summary>
        public ListView Form1ListView1;

        /// <summary>
        /// 查询结果列表
        /// </summary>
        private static DataTable ChaXunJieGuoLieBiao = new DataTable();

        /// <summary>
        /// 索引
        /// </summary>
        private static int SuoYin = 0;

        /// <summary>
        /// 窗口 创建
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ChaZhao_Load(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// 编辑框 贴吧名 内容被改变
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void textBox_tiebaName_TextChanged(object sender, EventArgs e)
        {
            //数据库查询
            ShuJuKuChaXun();
        }

        /// <summary>
        /// 编辑框 头像ID 内容被改变
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void textBox_touXiangID_TextChanged(object sender, EventArgs e)
        {
            //数据库查询
            ShuJuKuChaXun();
        }

        /// <summary>
        /// 编辑框 主显账号 内容被改变
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void textBox_zhuXianZhangHao_TextChanged(object sender, EventArgs e)
        {
            //数据库查询
            ShuJuKuChaXun();
        }

        /// <summary>
        /// 数据库查询
        /// </summary>
        private void ShuJuKuChaXun()
        {
            //初始化
            ChaXunJieGuoLieBiao = new DataTable();
            SuoYin = 0;
            label_chaXunJieGuo.Text = $"查找到{ChaXunJieGuoLieBiao.Rows.Count}个结果丨当前是第{SuoYin}个";

            string tiebaName = textBox_tiebaName.Text;
            string touXiangID = textBox_touXiangID.Text;
            string zhuXianZhangHao = textBox_zhuXianZhangHao.Text;

            //如果3个都没填
            if (string.IsNullOrEmpty(tiebaName + touXiangID + zhuXianZhangHao))
            {
                return;
            }

            string sqlStr = $"select ID from 封禁列表 where";

            //拼接贴吧名
            if (!string.IsNullOrEmpty(tiebaName))
            {
                sqlStr += $" 贴吧名='{tiebaName}'";
            }

            //拼接贴吧名
            if (!string.IsNullOrEmpty(touXiangID))
            {
                if (sqlStr.Contains(" 贴吧名="))
                {
                    sqlStr += " and";
                }

                sqlStr += $" 头像='{touXiangID}'";
            }

            //拼接主显账号
            if (!string.IsNullOrEmpty(zhuXianZhangHao))
            {
                if (sqlStr.Contains(" 贴吧名=") || sqlStr.Contains(" 头像="))
                {
                    sqlStr += " and";
                }

                sqlStr += $" 用户名 like '%{zhuXianZhangHao}%'";
            }

            //查询
            ChaXunJieGuoLieBiao = DB.access.GetDataTable(sqlStr);
            if (ChaXunJieGuoLieBiao.Rows.Count <= 0)
            {
                button_shangYiGe.Enabled = false;
                button_xiaYiGe.Enabled = false;
                return;
            }

            button_shangYiGe.Enabled = true;
            button_xiaYiGe.Enabled = true;
            label_chaXunJieGuo.Text = $"查找到{ChaXunJieGuoLieBiao.Rows.Count}个结果丨当前是第{SuoYin}个";
        }

        /// <summary>
        /// 按钮 上一个 被单击
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button_shangYiGe_Click(object sender, EventArgs e)
        {
            if (SuoYin <= 1)
            {
                SuoYin = ChaXunJieGuoLieBiao.Rows.Count;
            }
            else
            {
                SuoYin--;
            }

            label_chaXunJieGuo.Text = $"查找到{ChaXunJieGuoLieBiao.Rows.Count}个结果丨当前是第{SuoYin}个";

            for (int i = 0; i < Form1ListView1.Items.Count; i++)
            {
                if (Form1ListView1.Items[i].Text == Convert.ToString(ChaXunJieGuoLieBiao.Rows[SuoYin - 1]["ID"]))
                {
                    //使该列可见
                    Form1ListView1.Items[i].EnsureVisible();
                    //选中该列
                    Form1ListView1.Items[i].Selected = true;
                    break;
                }
            }
        }

        /// <summary>
        /// 按钮 下一个 被单击
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button_xiaYiGe_Click(object sender, EventArgs e)
        {
            if (SuoYin >= ChaXunJieGuoLieBiao.Rows.Count)
            {
                SuoYin = 1;
            }
            else
            {
                SuoYin++;
            }

            label_chaXunJieGuo.Text = $"查找到{ChaXunJieGuoLieBiao.Rows.Count}个结果丨当前是第{SuoYin}个";

            for (int i = 0; i < Form1ListView1.Items.Count; i++)
            {
                if (Form1ListView1.Items[i].Text == Convert.ToString(ChaXunJieGuoLieBiao.Rows[SuoYin - 1]["ID"]))
                {
                    //使该列可见
                    Form1ListView1.Items[i].EnsureVisible();
                    //选中该列
                    Form1ListView1.Items[i].Selected = true;
                    break;
                }
            }
        }

        private void label8_Click(object sender, EventArgs e)
        {
            MessageBox.Show("主显账号支持关键词查询。", "笨蛋雪说：", buttons: MessageBoxButtons.OK, icon: MessageBoxIcon.Information);
        }
    }
}
