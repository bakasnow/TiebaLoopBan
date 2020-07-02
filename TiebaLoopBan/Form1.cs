using System;
using System.Data;
using System.IO;
using System.Windows.Forms;
using System.Threading;
using System.Text;

using BakaSnowTool;
using BakaSnowTool.Http;
using TiebaLib;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Diagnostics;
using CsharpHttpHelper;
using CsharpHttpHelper.Enum;

namespace TiebaLoopBan
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 数据库连接
        /// </summary>
        public static Access db_tlb = new Access(Application.StartupPath + @"\db_tlb.mdb");

        /// <summary>
        /// 版本验证
        /// </summary>
        /// <returns></returns>
        private bool Version()
        {
            string html, v;
            while (true)
            {
                HttpHelper http = new HttpHelper();
                HttpItem item = new HttpItem()
                {
                    URL = "http://www.bakasnow.com/version.php?n=" + Quanju.Vname,//URL     必需项
                    Method = "GET",//URL     可选项 默认为Get
                    Timeout = 100000,//连接超时时间     可选项默认为100000
                    ReadWriteTimeout = 30000,//写入Post数据超时时间     可选项默认为30000
                    IsToLower = false,//得到的HTML代码是否转成小写     可选项默认转小写
                    Cookie = "",//字符串Cookie     可选项
                    UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64; rv:18.0) Gecko/20100101 Firefox/18.0",//用户的浏览器类型，版本，操作系统     可选项有默认值
                    Accept = "text/html, application/xhtml+xml, */*",//    可选项有默认值
                    ContentType = "text/html",//返回类型    可选项有默认值
                    Referer = "http://www.bakasnow.com/",//来源URL     可选项
                    Allowautoredirect = false,//是否根据３０１跳转     可选项
                    AutoRedirectCookie = false,//是否自动处理Cookie     可选项
                    Postdata = "",//Post数据     可选项GET时不需要写
                    ResultType = ResultType.String,//返回数据类型，是Byte还是String
                };
                HttpResult result = http.GetHtml(item);
                html = result.Html;
                v = BST.JieQuWenBen(html, "<version>", "</version>");

                if (string.IsNullOrEmpty(v))
                {
                    if (MessageBox.Show(text: "版本获取失败，可能是网络异常，点击\"取消\"跳过验证。", caption: "笨蛋雪说：", buttons: MessageBoxButtons.RetryCancel, icon: MessageBoxIcon.Asterisk) == DialogResult.Cancel)
                    {
                        return true;
                    }
                }
                else
                {
                    //获取版本号成功
                    break;
                }
            }

            if (v != Quanju.Version)
            {
                string msg =
                    "发现新版本，请至群共享下载最新版\r\n" +
                    "当前版本：" + Quanju.Version + "\r\n" +
                    "最新版本：" + v + "\r\n\r\n" +
                    "是否立即加群？";
                if (MessageBox.Show(text: msg, caption: "笨蛋雪说：", buttons: MessageBoxButtons.YesNo, icon: MessageBoxIcon.Exclamation) == DialogResult.Yes)
                {
                    string target = BST.JieQuWenBen(html, "<link>", "</link>");
                    if (!string.IsNullOrEmpty(target))
                    {
                        Process.Start("iexplore.exe", target);
                    }

                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// 窗口创建
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form1_Load(object sender, EventArgs e)
        {
            //版本验证
            if (!Version())
            {
                Dispose();
            }

            //判断数据库是否存在
            if (!File.Exists(Application.StartupPath + @"\db_tlb.mdb"))
            {
                MessageBox.Show(text: " 数据库丢失，请重新下载", caption: "笨蛋雪说：", buttons: MessageBoxButtons.OK, icon: MessageBoxIcon.Error);
                Dispose();
            }

            //打开数据库
            db_tlb.Open();

            //为第一次访问初始化
            if (db_tlb.GetDataTable("select * from 基本设置 where 配置名=" + Quanju.PeizhiMing).Rows.Count == 0)
            {
                db_tlb.DoCommand("insert into 基本设置 (配置名,Cookie,扫描间隔,封禁间隔,重试次数,重试间隔,反脱缰模式,客户端封禁接口) values(" + Quanju.PeizhiMing + ",'',60,10,1,10,false,false)");
            }

            Text = "贴吧循环封禁 v" + Quanju.Version;

            CheckForIllegalCrossThreadCalls = false;
            button4.Enabled = false;
            listView1.MultiSelect = true;
            listView1.FullRowSelect = true;

            Read();//读取基本配置
            ZhanghaoYanzheng(true);

            Say("软件永久停更，请使用更高级的酷Q插件版，感谢您的支持！");
            Say("吧务辅助工具催更群：984150818，提供免费数据迁移服务。");
        }

        /// <summary>
        /// 读取数据
        /// </summary>
        private void Read()
        {
            Quanju.Cookie = Convert.ToString(db_tlb.GetDataResult("select Cookie from 基本设置 where 配置名=" + Quanju.PeizhiMing));
            textBox1.Text = Convert.ToString(db_tlb.GetDataResult("select 封禁间隔 from 基本设置 where 配置名=" + Quanju.PeizhiMing));
            textBox2.Text = Convert.ToString(db_tlb.GetDataResult("select 重试次数 from 基本设置 where 配置名=" + Quanju.PeizhiMing));

            //更新封禁列表
            GengxinFengjinLiebiao();
        }

        /// <summary>
        /// 更新封禁列表
        /// </summary>
        public void GengxinFengjinLiebiao()
        {
            DataTable dt = db_tlb.GetDataTable("select 用户名,贴吧名,最后封禁时间,循环开始时间,循环结束时间 from 封禁列表");

            listView1.Items.Clear();
            listView1.BeginUpdate();
            foreach (DataRow dr in dt.Rows)
            {
                ListViewItem lvi = new ListViewItem()
                {
                    Text = Convert.ToString(dr["用户名"])
                };

                lvi.SubItems.Add(Convert.ToString(dr["贴吧名"]));
                lvi.SubItems.Add(Convert.ToString(dr["最后封禁时间"]));
                lvi.SubItems.Add(Convert.ToString(dr["循环开始时间"]));
                lvi.SubItems.Add(Convert.ToString(dr["循环结束时间"]));
                listView1.Items.Add(lvi);
            }
            listView1.EndUpdate();

            dt.Clone();
        }

        /// <summary>
        /// 保存数据
        /// </summary>
        private void Save()
        {
            db_tlb.DoCommand($"update 基本设置 set Cookie='{Quanju.Cookie}' where 配置名=" + Quanju.PeizhiMing);
            db_tlb.DoCommand($"update 基本设置 set 封禁间隔={textBox1.Text},重试次数={textBox2.Text} where 配置名=" + Quanju.PeizhiMing);
        }

        /// <summary>
        /// 窗口关闭
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (MessageBox.Show("是否退出？", "笨蛋雪说：", buttons: MessageBoxButtons.YesNo, icon: MessageBoxIcon.Asterisk) == DialogResult.No)
            {
                e.Cancel = true;
                return;
            }

            Save();
            db_tlb.Close();
        }

        /// <summary>
        /// 基本参数
        /// </summary>
        private void JibenCanshu()
        {
            Quanju.Cookie = Convert.ToString(db_tlb.GetDataResult("select Cookie from 基本设置 where 配置名=" + Quanju.PeizhiMing));
            Quanju.SaomiaoJiange = Convert.ToInt32(db_tlb.GetDataResult("select 扫描间隔 from 基本设置 where 配置名=" + Quanju.PeizhiMing));
            Quanju.FengjinJiange = Convert.ToInt32(db_tlb.GetDataResult("select 封禁间隔 from 基本设置 where 配置名=" + Quanju.PeizhiMing));
            Quanju.ChongshiCishu = Convert.ToInt32(db_tlb.GetDataResult("select 重试次数 from 基本设置 where 配置名=" + Quanju.PeizhiMing));
            Quanju.ChongshiJiange = Convert.ToInt32(db_tlb.GetDataResult("select 重试间隔 from 基本设置 where 配置名=" + Quanju.PeizhiMing));
            Quanju.KehuduanFengjinJiekou = Convert.ToBoolean(Form1.db_tlb.GetDataResult("select 客户端封禁接口 from 基本设置 where 配置名=" + Quanju.PeizhiMing));
        }

        /// <summary>
        /// 账号验证
        /// </summary>
        /// <param name="isLoad">窗口是否加载中</param>
        /// <returns></returns>
        public bool ZhanghaoYanzheng(bool isLoad = false)
        {
            Quanju.YonghuMing = Tieba.GetBaiduYongHuMing(Quanju.Cookie);
            if (Quanju.YonghuMing == "" || Quanju.YonghuMing == null)
            {
                if (!isLoad)
                {
                    Say("账号验证失败");
                }

                label3.Text = "未登录";
                button1.Enabled = true;
                button2.Enabled = false;
                return false;
            }
            else
            {
                label3.Text = Quanju.YonghuMing;
                button1.Enabled = false;
                button2.Enabled = true;
                return true;
            }
        }

        /// <summary>
        /// 批量禁用控件
        /// </summary>
        /// <param name="b"></param>
        private void PiliangJinyongKongjian(bool b)
        {
            textBox1.Enabled = b;
            textBox2.Enabled = b;
        }

        /// <summary>
        /// 开始
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button3_Click(object sender, EventArgs e)
        {
            if (BST.JianYiZhengZe(textBox1.Text, "([0-9]{1,})") == "")
            {
                MessageBox.Show("封禁间隔格式错误", "笨蛋雪说：", buttons: MessageBoxButtons.OK, icon: MessageBoxIcon.Exclamation);
                return;
            }

            if (Convert.ToInt32(textBox1.Text) < 5)
            {
                MessageBox.Show("为了操作安全，封禁间隔不得低于5秒", "笨蛋雪说：", buttons: MessageBoxButtons.OK, icon: MessageBoxIcon.Exclamation);
                textBox1.Text = "5";
                return;
            }

            if (BST.JianYiZhengZe(textBox2.Text, "([0-9]{1,})") == "")
            {
                MessageBox.Show("失败重试格式错误", "笨蛋雪说：", buttons: MessageBoxButtons.OK, icon: MessageBoxIcon.Exclamation);
                return;
            }

            button3.Enabled = false;

            Save();//先保存
            JibenCanshu();//初始化参数

            Quanju.Stop = false;
            Thread mainThr = new Thread(new ThreadStart(Main))
            {
                IsBackground = true
            };
            mainThr.Start();

            button4.Enabled = true;
            PiliangJinyongKongjian(false);
        }

        /// <summary>
        /// 停止
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button4_Click(object sender, EventArgs e)
        {
            button4.Enabled = false;
            Quanju.Stop = true;
        }

        /// <summary>
        /// 主线程
        /// </summary>
        private void Main()
        {
            bool GongzuoShiduan = true;

            Say("循环封禁任务开始");
            while (true)
            {
                if (Quanju.Stop)
                {
                    break;
                }

                //判断是否在工作时间
                if (0 <= DateTime.Now.Hour && DateTime.Now.Hour <= 4)
                {
                    if (GongzuoShiduan)//防止重复输出
                    {
                        Say("当前非工作时段，任务将在凌晨5点开始，请保持挂机");
                        GongzuoShiduan = false;
                    }

                    //扫描间隔
                    Dengdai(Quanju.SaomiaoJiange);
                    continue;
                }
                else
                {
                    GongzuoShiduan = true;
                }

                //删除过期
                int jieguo = db_tlb.DoCommand($"delete from 封禁列表 where 循环结束时间<'{DateTime.Now.ToString("yyyy-MM-dd")}'");
                if (jieguo > 0)
                {
                    Say($"删除{jieguo}条过期任务");
                }

                //载入最新列表
                GengxinFengjinLiebiao();

                //开始遍历
                for (int i = 0; i < listView1.Items.Count; i++)
                {
                    if (Quanju.Stop)
                    {
                        break;
                    }

                    string dangqianSj = DateTime.Now.ToString("yyyy-MM-dd");

                    string yonghuMing = listView1.Items[i].SubItems[0].Text;
                    string tiebaName = listView1.Items[i].SubItems[1].Text;
                    string zuihouFengjinSj = listView1.Items[i].SubItems[2].Text;
                    //string xunhuanKaishiSj = listView1.Items[i].SubItems[3].Text;
                    //string xunhuanJieshuSj = listView1.Items[i].SubItems[4].Text;

                    if (Convert.ToDateTime(zuihouFengjinSj) >= Convert.ToDateTime(dangqianSj))
                    {
                        continue;
                    }

                    TiebaBaWu bawu = new TiebaBaWu
                    {
                        Cookie = Quanju.Cookie,
                        YongHuMing = yonghuMing,
                        TiebaName = tiebaName,
                        Fid = Huancun.GetFid(tiebaName)
                    };

                    for (int chongshiCishu = 0; chongshiCishu < Quanju.ChongshiCishu + 1; chongshiCishu++)
                    {
                        if (Quanju.Stop)
                        {
                            break;
                        }

                        int day = 1;
                        string liyou = "由于您违反" + tiebaName + "吧规定，故封禁1天，如有疑问请联系吧务团队。";
                        string msg;
                        bool fengjinJieguo;

                        //是否使用客户端封禁接口
                        if (Quanju.KehuduanFengjinJiekou)
                        {
                            fengjinJieguo = bawu.FengJin(day, liyou, out msg);
                        }
                        else
                        {
                            fengjinJieguo = bawu.FengJin(day, liyou, out msg);
                        }

                        //封禁结果
                        if (fengjinJieguo)
                        {
                            Say(yonghuMing + "在" + tiebaName + "吧，封禁1天成功。");
                            jieguo = db_tlb.DoCommand($"update 封禁列表 set 最后封禁时间='{dangqianSj}' where 用户名='{yonghuMing}' and 贴吧名='{tiebaName}'");
                            if (jieguo > 0)
                            {
                                listView1.Items[i].SubItems[2].Text = dangqianSj;
                            }

                            //封禁间隔
                            Dengdai(Quanju.FengjinJiange);
                            break;
                        }
                        else
                        {
                            Say(yonghuMing + "在" + tiebaName + "吧，封禁1天失败：" + msg + " " + Quanju.ChongshiJiange.ToString() + "秒后重试。");

                            //重试间隔
                            if (msg == "need vcode")
                            {
                                Say("错误信息：need vcode，账号操作频繁 暂停5分钟后重试。");
                                Dengdai(300);
                            }
                            else
                            {
                                Dengdai(Quanju.ChongshiJiange);
                            }
                        }
                    }
                }

                //扫描间隔
                Dengdai(Quanju.SaomiaoJiange);
            }

            Say("任务结束");

            button3.Enabled = true;
            PiliangJinyongKongjian(true);
        }

        /// <summary>
        /// 等待
        /// </summary>
        /// <param name="s"></param>
        private void Dengdai(int s)
        {
            for (int i = 0; i < s; i++)
            {
                if (Quanju.Stop)
                {
                    break;
                }

                Thread.Sleep(1000);
            }
        }

        /// <summary>
        /// 登录账号
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            if (!Quanju.Stop)
            {
                return;
            }

            BaiduLogin blform = new BaiduLogin();
            blform.ShowDialog();
            blform.Dispose();

            ZhanghaoYanzheng();
        }

        /// <summary>
        /// 删除账号
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button2_Click(object sender, EventArgs e)
        {
            if (!Quanju.Stop)
            {
                return;
            }

            if (MessageBox.Show(text: " 确定要删除账号吗？", caption: "笨蛋雪说：操作不可逆", buttons: MessageBoxButtons.OKCancel, icon: MessageBoxIcon.Exclamation) == DialogResult.Cancel)
            {
                return;
            }

            if (db_tlb.DoCommand("update 基本设置 set Cookie='' where 配置名=" + Quanju.PeizhiMing) > 0)
            {
                Quanju.Cookie = "";
                Say("账号已成功删除");
                label3.Text = "未登录";
                button1.Enabled = true;
                button2.Enabled = false;
            }
            else
            {
                MessageBox.Show(text: " 操作失败", caption: "笨蛋雪说：", buttons: MessageBoxButtons.OK, icon: MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// 输出条数
        /// </summary>
        private int ShuchuTiaoshu = 0;

        /// <summary>
        /// 信息输出
        /// </summary>
        /// <param name="msg"></param>
        public void Say(string msg)
        {
            WriteLog(msg);

            if (ShuchuTiaoshu >= 500)
            {
                listBox1.Items.Clear();
                ShuchuTiaoshu = 0;
            }

            ShuchuTiaoshu++;

            listBox1.Items.Add(BST.ShiJianGeShiHua(0, ziDingYi: "yy/MM/dd HH:mm:ss") + " " + msg);
            listBox1.SelectedIndex = listBox1.Items.Count - 1;
        }

        /// <summary>
        /// 写日志
        /// </summary>
        /// <param name="text"></param>
        public void WriteLog(string text)
        {
            if (!Directory.Exists(Quanju.RizhiLujing))//若文件夹不存在则新建文件夹   
            {
                Directory.CreateDirectory(Quanju.RizhiLujing); //新建文件夹   
            }

            FileStream fs = new FileStream(Quanju.RizhiLujing + "\\" + DateTime.Now.ToString("yyyy-MM-dd") + ".txt", FileMode.Append);
            //获得字节数组
            byte[] data = System.Text.Encoding.Default.GetBytes(BST.ShiJianGeShiHua(0) + " " + text + "\n");
            //开始写入
            fs.Write(data, 0, data.Length);
            //清空缓冲区、关闭流
            fs.Flush();
            fs.Close();
        }

        private void 添加ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            BianjiShuju.BianjiZhuangtai = ChuandiZhuangtai.Xinjian;
            Bianji bianjiForm = new Bianji();

            bianjiForm.ShowDialog();
            GengxinFengjinLiebiao();
        }

        private void 批量添加ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Piliang piliangForm = new Piliang();
            piliangForm.ShowDialog();
            GengxinFengjinLiebiao();
        }

        private void 编辑ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count == 1)
            {
                BianjiShuju.BianjiZhuangtai = ChuandiZhuangtai.Bianji;
                BianjiShuju.ShujuJiegou shuju = new BianjiShuju.ShujuJiegou
                {
                    Yonghuming = listView1.SelectedItems[0].SubItems[0].Text,
                    Tiebaname = listView1.SelectedItems[0].SubItems[1].Text,
                    ZuihouFengjinSj = listView1.SelectedItems[0].SubItems[2].Text,
                    XunhuanKaishiSj = Convert.ToDateTime(listView1.SelectedItems[0].SubItems[3].Text),
                    XunhuanJieshuSj = Convert.ToDateTime(listView1.SelectedItems[0].SubItems[4].Text),
                };
                BianjiShuju.SetShuju(shuju);

                Bianji bianjiForm = new Bianji();
                bianjiForm.ShowDialog();

                string yonghuMing = listView1.SelectedItems[0].SubItems[0].Text;
                string tiebaName = listView1.SelectedItems[0].SubItems[1].Text;
                listView1.SelectedItems[0].SubItems[3].Text = Convert.ToString(db_tlb.GetDataResult($"select 循环开始时间 from 封禁列表 where 用户名='{yonghuMing}' and 贴吧名='{tiebaName}'"));
                listView1.SelectedItems[0].SubItems[4].Text = Convert.ToString(db_tlb.GetDataResult($"select 循环结束时间 from 封禁列表 where 用户名='{yonghuMing}' and 贴吧名='{tiebaName}'"));
            }
        }

        private void 删除ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int shuliang = listView1.SelectedItems.Count;
            List<ShanchuLiebiaoJiegou> ShanchuLiebiao = new List<ShanchuLiebiaoJiegou>();

            if (shuliang == 1)//判断选中了几条
            {
                string yonghuMing = listView1.SelectedItems[0].SubItems[0].Text;
                string tiebaName = listView1.SelectedItems[0].SubItems[1].Text;

                if (MessageBox.Show($"确定要赦免{tiebaName}吧的{tiebaName}吗？", "笨蛋雪说：", buttons: MessageBoxButtons.OKCancel, icon: MessageBoxIcon.Exclamation) == DialogResult.Cancel)
                {
                    return;
                };

                //加入队列
                ShanchuLiebiao.Add(new ShanchuLiebiaoJiegou
                {
                    YonghuMing = yonghuMing,
                    TiebaName = tiebaName
                });
            }
            else
            {
                if (MessageBox.Show("确定要赦免选中的" + shuliang.ToString() + "人吗？", "笨蛋雪说：", buttons: MessageBoxButtons.OKCancel, icon: MessageBoxIcon.Exclamation) == DialogResult.Cancel)
                {
                    return;
                };

                //加入队列
                for (int i = 0; i < shuliang; i++)
                {
                    ShanchuLiebiao.Add(new ShanchuLiebiaoJiegou
                    {
                        YonghuMing = listView1.SelectedItems[i].SubItems[0].Text,
                        TiebaName = listView1.SelectedItems[i].SubItems[1].Text
                    });
                }
            }

            //从数据库中删除
            foreach (ShanchuLiebiaoJiegou shanchuCanshu in ShanchuLiebiao)
            {
                int jieguo = db_tlb.DoCommand($"delete from 封禁列表 where 用户名='{shanchuCanshu.YonghuMing}' and 贴吧名='{shanchuCanshu.TiebaName}'");
                if (jieguo > 0)
                {
                    listView1.SelectedItems[0].Remove();
                }
                else
                {
                    Say("删除" + shanchuCanshu.TiebaName + "吧的" + shanchuCanshu.YonghuMing + "，失败");
                }
            }

            //更新列表
            GengxinFengjinLiebiao();
        }

        /// <summary>
        /// 右键菜单打开前
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void contextMenuStrip1_Opening(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (!Quanju.Stop)
            {
                添加ToolStripMenuItem.Enabled = false;
                批量添加ToolStripMenuItem.Enabled = false;
                编辑ToolStripMenuItem.Enabled = false;
                删除ToolStripMenuItem.Enabled = false;
                导入ToolStripMenuItem.Enabled = false;
                导出ToolStripMenuItem.Enabled = false;
                return;
            }
            else
            {
                添加ToolStripMenuItem.Enabled = true;
                批量添加ToolStripMenuItem.Enabled = true;
                导入ToolStripMenuItem.Enabled = true;
                导出ToolStripMenuItem.Enabled = true;
            }

            if (listView1.SelectedItems.Count == 1)
            {
                编辑ToolStripMenuItem.Enabled = true;
                删除ToolStripMenuItem.Enabled = true;
            }
            else if (listView1.SelectedItems.Count > 1)
            {
                编辑ToolStripMenuItem.Enabled = false;
                删除ToolStripMenuItem.Enabled = true;
            }
            else
            {
                编辑ToolStripMenuItem.Enabled = false;
                删除ToolStripMenuItem.Enabled = false;
            }
        }

        /// <summary>
        /// 窗体状态
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form1_Resize(object sender, EventArgs e)
        {
            if (WindowState == FormWindowState.Minimized)
            {
                ShowInTaskbar = false;
            }
            else
            {
                ShowInTaskbar = true;
            }
        }

        /// <summary>
        /// 双击托盘图标
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            notifyIcon1.Visible = true;
            WindowState = FormWindowState.Normal;
        }

        /// <summary>
        /// 清空信息输出
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void linkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            listBox1.Items.Clear();
        }

        /// <summary>
        /// 关于
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            const string msg =
                "作者：笨蛋雪\n" +
                "贴吧：祭雪夏炎吧\n\n" +
                "吧务辅助工具催更群：984150818\n" +
                "是否立即加群？";
            if (MessageBox.Show(msg, "笨蛋雪说：关于", buttons: MessageBoxButtons.YesNo, icon: MessageBoxIcon.Asterisk) == DialogResult.Yes)
            {
                string target = Quanju.QunLianjie;
                try
                {
                    System.Diagnostics.Process.Start(target);
                }
                catch (System.ComponentModel.Win32Exception noBrowser)
                {
                    if (noBrowser.ErrorCode == -2147467259)
                        MessageBox.Show(noBrowser.Message);
                }
                catch (Exception other)
                {
                    MessageBox.Show(other.Message);
                }
            }
        }

        /// <summary>
        /// 高级功能
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void linkLabel3_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (!Quanju.Stop)
            {
                MessageBox.Show("任务过程中无法使用高级功能，请先停止任务。", "笨蛋雪说：", buttons: MessageBoxButtons.OK, icon: MessageBoxIcon.Exclamation);
                return;
            }

            Gaoji gaojiForm = new Gaoji();
            gaojiForm.ShowDialog();
        }

        /// <summary>
        /// 导入
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void 导入ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog
            {
                Multiselect = false,
                Title = "笨蛋雪说：请选择需要导入的数据",
                Filter = "文本文件|*.txt"
            };
            if (ofd.ShowDialog() != DialogResult.OK) return;

            StreamReader sr = new StreamReader(ofd.FileName, Encoding.Default);

            string line;
            int chenggong = 0;
            int shibai = 0;
            string shibaiMingdan = "";
            while ((line = sr.ReadLine()) != null)
            {
                string[] canshu = Regex.Split(line, "\t", RegexOptions.IgnoreCase);
                if (canshu.Length == 5)
                {
                    //数据库检查重复
                    if (db_tlb.GetDataTable($"select * from 封禁列表 where 用户名='{canshu[0]}' and 贴吧名='{canshu[1]}'").Rows.Count > 0)
                    {
                        shibai += 1;
                        shibaiMingdan += canshu[0] + "\n";
                        continue;
                    }

                    int jieguo = db_tlb.DoCommand($"insert into 封禁列表 (用户名,贴吧名,最后封禁时间,循环开始时间,循环结束时间)" +
                        $" values('{canshu[0]}','{canshu[1]}','{canshu[2]}','{canshu[3]}','{canshu[4]}')");
                    if (jieguo > 0)
                    {
                        chenggong += 1;
                    }
                }
            }

            string msg = "导入结果如下：\n";
            msg += "成功 " + chenggong.ToString() + " 个\n";
            msg += "失败 " + shibai.ToString() + " 个";

            GengxinFengjinLiebiao();
            MessageBox.Show(msg, "笨蛋雪说：", buttons: MessageBoxButtons.OK, icon: MessageBoxIcon.Asterisk);
        }

        /// <summary>
        /// 导出
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void 导出ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog
            {
                Title = "笨蛋雪说：请选择将数据导出的路径",
                FileName = DateTime.Now.ToString("yy-MM-dd") + " 循环封禁名单",
                Filter = "文本文件|*.txt"
            };
            if (sfd.ShowDialog() != DialogResult.OK) return;

            StringBuilder sb = new StringBuilder();

            DataTable dt = db_tlb.GetDataTable("select 用户名,贴吧名,最后封禁时间,循环开始时间,循环结束时间 from 封禁列表");
            foreach (DataRow dr in dt.Rows)
            {
                sb.Append($"{Convert.ToString(dr["用户名"])}\t");
                sb.Append($"{Convert.ToString(dr["贴吧名"])}\t");
                sb.Append($"{Convert.ToString(dr["最后封禁时间"])}\t");
                sb.Append($"{Convert.ToString(dr["循环开始时间"])}\t");
                sb.Append($"{Convert.ToString(dr["循环结束时间"])}\n");
            }
            dt.Clone();

            FileStream fs = new FileStream(sfd.FileName, FileMode.Create);
            //获得字节数组
            byte[] data = Encoding.Default.GetBytes(sb.ToString());
            //开始写入
            fs.Write(data, 0, data.Length);
            //清空缓冲区、关闭流
            fs.Flush();
            fs.Close();

            MessageBox.Show("导出成功\n" + sfd.FileName, "笨蛋雪说：", buttons: MessageBoxButtons.OK, icon: MessageBoxIcon.Asterisk);
        }
    }

    /// <summary>
    /// 删除列表结构
    /// </summary>
    class ShanchuLiebiaoJiegou
    {
        public string YonghuMing;
        public string TiebaName;
    }
}
