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
    public partial class Gaoji : Form
    {
        public Gaoji()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 窗口创建
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Gaoji_Load(object sender, EventArgs e)
        {
            Text = "高级设置";

            checkBox2.Checked = Convert.ToBoolean(Form1.db_tlb.GetDataResult("select 客户端封禁接口 from 基本设置 where 配置名=" + Quanju.PeizhiMing));
        }

        /// <summary>
        /// 功能介绍
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            string msg = "" +
                "【反脱缰模式】该功能未开放\r\n" +
                "简单的说就是从每天封禁1次（24小时/次）改为每天封禁2次（12小时/次）\r\n" +
                "软件每天早上5点开始循环任务，假设队列中有30个ID，每个封禁间隔是10秒。" +
                "如果顺利的话全部封禁完成已经是5点05分，也就是说排在后面的ID有几分钟可以发言的时间，这几分钟我称之为“脱缰”。" +
                "为了避免这种情况发生，每12小时封禁1次即可刷新上次封禁时间，避免“脱缰”带来的尴尬。";
            MessageBox.Show(msg, "笨蛋雪说：");
        }

        /// <summary>
        /// 反脱缰被选中
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// 使用客户端接口封禁
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox2.Checked)
            {
                Form1.db_tlb.DoCommand("update 基本设置 set 客户端封禁接口=true where 配置名=" + Quanju.PeizhiMing);
            }
            else
            {
                Form1.db_tlb.DoCommand("update 基本设置 set 客户端封禁接口=false where 配置名=" + Quanju.PeizhiMing);
            }
        }
    }
}
