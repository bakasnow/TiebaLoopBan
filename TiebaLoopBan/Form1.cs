using System;
using System.Data;
using System.IO;
using System.Windows.Forms;
using System.Threading;
using System.Text;

using BakaSnowTool;
using TiebaLib;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Diagnostics;
using CsharpHttpHelper;
using CsharpHttpHelper.Enum;
using BaiduLogin;

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
        public static Access access = new Access(Application.StartupPath + @"\TiebaLoopBan.mdb");

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
                    URL = "http://www.bakasnow.com/version.php?n=" + Config.Vname,//URL     必需项
                    Method = "GET",//URL     可选项 默认为Get
                    Timeout = 100000,//连接超时时间     可选项默认为100000
                    ReadWriteTimeout = 30000,//写入Post数据超时时间     可选项默认为30000
                    IsToLower = false,//得到的HTML代码是否转成小写     可选项默认转小写
                    Cookie = string.Empty,//字符串Cookie     可选项
                    UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64; rv:18.0) Gecko/20100101 Firefox/18.0",//用户的浏览器类型，版本，操作系统     可选项有默认值
                    Accept = "text/html, application/xhtml+xml, */*",//    可选项有默认值
                    ContentType = "text/html",//返回类型    可选项有默认值
                    Referer = "http://www.bakasnow.com/",//来源URL     可选项
                    Allowautoredirect = false,//是否根据３０１跳转     可选项
                    AutoRedirectCookie = false,//是否自动处理Cookie     可选项
                    Postdata = string.Empty,//Post数据     可选项GET时不需要写
                    ResultType = ResultType.String,//返回数据类型，是Byte还是String
                };
                HttpResult result = http.GetHtml(item);
                html = result.Html;
                v = BST.JieQuWenBen(html, "<version>", "</version>");

                if (string.IsNullOrEmpty(v))
                {
                    if (MessageBox.Show(text: "版本获取失败，可能是网络异常，点击\"取消\"跳过验证", caption: "笨蛋雪说：", buttons: MessageBoxButtons.RetryCancel, icon: MessageBoxIcon.Asterisk) == DialogResult.Cancel)
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

            if (v != Config.Version)
            {
                string msg =
                    "发现新版本，请至群共享下载最新版\n" +
                    "当前版本：" + Config.Version + "\n" +
                    "最新版本：" + v + "\n\n" +
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
            if (!File.Exists(Application.StartupPath + @"\TiebaLoopBan.mdb"))
            {
                MessageBox.Show(text: "数据库丢失，请重新下载", caption: "笨蛋雪说：", buttons: MessageBoxButtons.OK, icon: MessageBoxIcon.Error);
                Dispose();
            }

            //打开数据库
            access.Open();

            //校验数据库版本
            string dbv = Convert.ToString(access.GetDataResult("select top 1 版本号 from 数据库版本"));
            if (dbv != Config.DataBaseVersion)
            {
                string msg = "数据库版本不符，无法正常使用\n" +
                    $"需求版本：{Config.DataBaseVersion}\n" +
                    $"当前版本：{dbv}\n\n" +
                    "请联系作者更新";

                MessageBox.Show(text: msg, caption: "笨蛋雪说：", buttons: MessageBoxButtons.OK, icon: MessageBoxIcon.Error);
                Dispose();
            }

            //为第一次访问初始化
            if (access.GetDataTable($"select * from 基本设置 where 配置名='{Config.PeiZhiMing}'").Rows.Count == 0)
            {
                access.DoCommand($"insert into 基本设置 (配置名,Cookie,扫描间隔,封禁间隔,重试次数,重试间隔) values('{Config.PeiZhiMing}','',60,10,1,10)");
            }

            Text = "贴吧循环封禁 v" + Config.Version;

            CheckForIllegalCrossThreadCalls = false;
            button4.Enabled = false;
            listView1.MultiSelect = true;
            listView1.FullRowSelect = true;

            //读取基本配置
            Config.Cookie = Convert.ToString(access.GetDataResult($"select Cookie from 基本设置 where 配置名='{Config.PeiZhiMing}'"));
            textBox1.Text = Convert.ToString(access.GetDataResult($"select 封禁间隔 from 基本设置 where 配置名='{Config.PeiZhiMing}'"));
            textBox2.Text = Convert.ToString(access.GetDataResult($"select 重试次数 from 基本设置 where 配置名='{Config.PeiZhiMing}'"));

            //更新封禁列表
            GengXinFengJinLieBiao();

            //账号验证
            ZhangHaoYanZheng(true);

            Say("贴吧管理器交流群：984150818");
        }

        /// <summary>
        /// 更新封禁列表
        /// </summary>
        public void GengXinFengJinLieBiao()
        {
            DataTable dt = access.GetDataTable("select * from 封禁列表");

            listView1.Items.Clear();
            listView1.BeginUpdate();
            foreach (DataRow dr in dt.Rows)
            {
                ListViewItem lvi = new ListViewItem()
                {
                    Text = Convert.ToString(dr["ID"])
                };

                lvi.SubItems.Add(Convert.ToString(dr["用户名"]));
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
            access.DoCommand($"update 基本设置 set Cookie='{Config.Cookie}' where 配置名='{Config.PeiZhiMing}'");
            access.DoCommand($"update 基本设置 set 封禁间隔={textBox1.Text},重试次数={textBox2.Text} where 配置名='{Config.PeiZhiMing}'");
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
            access.Close();
        }

        /// <summary>
        /// 基本参数
        /// </summary>
        private void JiBenCanShu()
        {
            Config.Cookie = Convert.ToString(access.GetDataResult($"select Cookie from 基本设置 where 配置名='{Config.PeiZhiMing}'"));
            Config.SaoMiaoJianGe = Convert.ToInt32(access.GetDataResult($"select 扫描间隔 from 基本设置 where 配置名='{Config.PeiZhiMing}'"));
            Config.FengJinJianGe = Convert.ToInt32(access.GetDataResult($"select 封禁间隔 from 基本设置 where 配置名='{Config.PeiZhiMing}'"));
            Config.ChongShiCiShu = Convert.ToInt32(access.GetDataResult($"select 重试次数 from 基本设置 where 配置名='{Config.PeiZhiMing}'"));
            Config.ChongShiJianGe = Convert.ToInt32(access.GetDataResult($"select 重试间隔 from 基本设置 where 配置名='{Config.PeiZhiMing}'"));
        }

        /// <summary>
        /// 账号验证
        /// </summary>
        /// <param name="isLoad">窗口是否加载中</param>
        /// <returns></returns>
        public bool ZhangHaoYanZheng(bool isLoad = false)
        {
            Config.YongHuMing = Tieba.GetBaiduYongHuMing(Config.Cookie);
            if (string.IsNullOrEmpty(Config.YongHuMing))
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
                label3.Text = Config.YongHuMing;
                button1.Enabled = false;
                button2.Enabled = true;
                return true;
            }
        }

        /// <summary>
        /// 批量禁用控件
        /// </summary>
        /// <param name="b"></param>
        private void PiLiangJinYongKongJian(bool b)
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
            if (string.IsNullOrEmpty(BST.JianYiZhengZe(textBox1.Text, "([0-9]{1,})")))
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

            if (string.IsNullOrEmpty(BST.JianYiZhengZe(textBox2.Text, "([0-9]{1,})")))
            {
                MessageBox.Show("失败重试格式错误", "笨蛋雪说：", buttons: MessageBoxButtons.OK, icon: MessageBoxIcon.Exclamation);
                return;
            }

            button3.Enabled = false;

            Save();//先保存
            JiBenCanShu();//初始化参数

            Config.Stop = false;
            Thread mainThr = new Thread(new ThreadStart(MainThread))
            {
                IsBackground = true
            };
            mainThr.Start();

            button4.Enabled = true;
            PiLiangJinYongKongJian(false);
        }

        /// <summary>
        /// 停止
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button4_Click(object sender, EventArgs e)
        {
            button4.Enabled = false;
            Config.Stop = true;
        }

        /// <summary>
        /// 主线程
        /// </summary>
        private void MainThread()
        {
            //工作时段
            bool gongZuoShiDuan = true;

            Say("循环封禁任务开始");
            while (true)
            {
                if (Config.Stop) break;

                //判断是否在工作时间
                if (0 <= DateTime.Now.Hour && DateTime.Now.Hour <= 4)
                {
                    if (gongZuoShiDuan)//防止重复输出
                    {
                        Say("当前非工作时段，任务将在凌晨5点开始，请保持挂机");
                        gongZuoShiDuan = false;
                    }

                    //扫描间隔
                    DengDai(Config.SaoMiaoJianGe);
                    continue;
                }
                else
                {
                    gongZuoShiDuan = true;
                }

                //删除过期
                int jieGuo = access.DoCommand($"delete from 封禁列表 where 循环结束时间<'{DateTime.Now:yyyy-MM-dd}'");
                if (jieGuo > 0)
                {
                    Say($"删除{jieGuo}条过期任务");
                }

                //载入最新列表
                GengXinFengJinLieBiao();

                //开始遍历
                for (int i = 0; i < listView1.Items.Count; i++)
                {
                    if (Config.Stop) break;

                    string dangQianShiJian = DateTime.Now.ToString("yyyy-MM-dd");

                    string id = listView1.Items[i].SubItems[0].Text;
                    DataTable dt = access.GetDataTable($"select top 1 * from 封禁列表 where ID={id}");
                    if (dt.Rows.Count == 0)
                    {
                        continue;
                    }

                    string yongHuMing = Convert.ToString(dt.Rows[0]["用户名"]);
                    string touXiang = Convert.ToString(dt.Rows[0]["头像"]);
                    string tiebaName = Convert.ToString(dt.Rows[0]["贴吧名"]);
                    string zuiHouFengJinShiJian = Convert.ToString(dt.Rows[0]["最后封禁时间"]);

                    if (Convert.ToDateTime(zuiHouFengJinShiJian) >= Convert.ToDateTime(dangQianShiJian))
                    {
                        continue;
                    }

                    //获取用户信息
                    if (string.IsNullOrEmpty(yongHuMing) || string.IsNullOrEmpty(touXiang))
                    {
                        for (int mingPianChongShiCiShu = 0; mingPianChongShiCiShu < 2; mingPianChongShiCiShu++)
                        {
                            Tieba.MingPianJieGou mingPianJieGou = Tieba.GetTiebaMingPian(yongHuMing);
                            if (mingPianJieGou.HuoQuChengGong)
                            {
                                //更新任务参数
                                yongHuMing = mingPianJieGou.YongHuMing;
                                touXiang = mingPianJieGou.TouXiang;

                                //写进数据库
                                string sqlStr = $"update 封禁列表 set 用户名='{yongHuMing}',头像='{touXiang}' where ID={id}";
                                if (access.DoCommand(sqlStr) == 0)
                                {
                                    Say($"SQL执行失败：{sqlStr}");
                                }

                                break;
                            }
                            else
                            {
                                Say($"{(string.IsNullOrEmpty(yongHuMing) ? $"头像={touXiang}" : $"用户名={yongHuMing}")}，用户信息获取失败：{mingPianJieGou.Msg}");
                                Thread.Sleep(Config.FengJinJianGe);
                            }
                        }
                    }

                    //跳过没有头像的
                    if (string.IsNullOrEmpty(touXiang))
                    {
                        continue;
                    }

                    TiebaBaWu baWu = new TiebaBaWu
                    {
                        Cookie = Config.Cookie,
                        TiebaName = tiebaName,
                        Fid = FidHuanCun.GetFid(tiebaName),
                        YongHuMing = yongHuMing,
                        NiCheng = string.Empty,
                        TouXiang = touXiang
                    };

                    for (int chongShiCiShu = 0; chongShiCiShu < Config.ChongShiCiShu + 1; chongShiCiShu++)
                    {
                        if (Config.Stop) break;

                        int day = 1;
                        string liYou = $"由于您违反{tiebaName}吧规定，故封禁1天，如有疑问请联系吧务团队";

                        //封禁结果
                        if (baWu.FengJin(day, liYou, out string msg))
                        {
                            Say($"{(string.IsNullOrEmpty(yongHuMing) ? $"{touXiang}" : $"{yongHuMing}")}在{tiebaName}吧，封禁1天成功");
                            jieGuo = access.DoCommand($"update 封禁列表 set 最后封禁时间='{dangQianShiJian}' where ID={id}");
                            if (jieGuo > 0)
                            {
                                listView1.Items[i].SubItems[3].Text = dangQianShiJian;
                            }

                            //封禁间隔
                            DengDai(Config.FengJinJianGe);
                            break;
                        }
                        else
                        {
                            Say($"{yongHuMing}在{tiebaName}吧，封禁1天失败：{msg} {Config.ChongShiJianGe}秒后重试");

                            //重试间隔
                            if (msg == "need vcode")
                            {
                                Say("错误信息：need vcode，账号操作频繁 暂停5分钟后重试");
                                DengDai(300);
                            }
                            else
                            {
                                DengDai(Config.ChongShiJianGe);
                            }
                        }
                    }
                }

                //扫描间隔
                DengDai(Config.SaoMiaoJianGe);
            }

            Say("任务结束");

            button3.Enabled = true;
            PiLiangJinYongKongJian(true);
        }

        /// <summary>
        /// 等待
        /// </summary>
        /// <param name="s"></param>
        private void DengDai(int s)
        {
            for (int i = 0; i < s; i++)
            {
                if (Config.Stop) break;

                Thread.Sleep(500);
            }
        }

        /// <summary>
        /// 登录账号
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            if (!Config.Stop) return;

            BaiduLoginForm baiduLoginForm = new BaiduLoginForm();
            baiduLoginForm.ShowDialog();
            Config.Cookie = baiduLoginForm.Cookie;

            ZhangHaoYanZheng();
        }

        /// <summary>
        /// 删除账号
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button2_Click(object sender, EventArgs e)
        {
            if (!Config.Stop) return;

            if (MessageBox.Show(text: "确定要删除账号吗？", caption: "笨蛋雪说：操作不可逆", buttons: MessageBoxButtons.OKCancel, icon: MessageBoxIcon.Exclamation) == DialogResult.Cancel)
            {
                return;
            }

            if (access.DoCommand($"update 基本设置 set Cookie='' where 配置名='{Config.PeiZhiMing}'") > 0)
            {
                Config.Cookie = string.Empty;
                Say("账号已成功删除");
                label3.Text = "未登录";
                button1.Enabled = true;
                button2.Enabled = false;
            }
            else
            {
                MessageBox.Show(text: "操作失败", caption: "笨蛋雪说：", buttons: MessageBoxButtons.OK, icon: MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// 输出条数
        /// </summary>
        private int ShuChuTiaoShu = 0;

        /// <summary>
        /// 信息输出
        /// </summary>
        /// <param name="msg"></param>
        public void Say(string msg)
        {
            WriteLog(msg);

            if (ShuChuTiaoShu >= 500)
            {
                listBox1.Items.Clear();
                ShuChuTiaoShu = 0;
            }

            ShuChuTiaoShu++;

            listBox1.Items.Add($"{DateTime.Now:yy/MM/dd HH:mm:ss} {msg}");
            listBox1.SelectedIndex = listBox1.Items.Count - 1;
        }

        /// <summary>
        /// 写日志
        /// </summary>
        /// <param name="text"></param>
        public void WriteLog(string text)
        {
            if (!Directory.Exists(Application.StartupPath + "\\Log"))//若文件夹不存在则新建文件夹   
            {
                Directory.CreateDirectory(Application.StartupPath + "\\Log"); //新建文件夹   
            }

            try
            {
                File.AppendAllText($"{Application.StartupPath}\\Log\\{DateTime.Now:yyyy-MM-dd}.txt", $"{DateTime.Now:yy/MM/dd HH:mm:ss} {text}\n");
            }
            catch
            {

            }
        }

        private void 添加ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            BianJi bianJiForm = new BianJi(BianJi.ZhuangTaiLeiXing.XinJian, string.Empty);

            bianJiForm.ShowDialog();
            GengXinFengJinLieBiao();
        }

        private void 批量添加ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PiLiangTianJia piLiangTianJiaForm = new PiLiangTianJia();

            piLiangTianJiaForm.ShowDialog();
            GengXinFengJinLieBiao();
        }

        private void 编辑ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count == 1)
            {
                string id = listView1.SelectedItems[0].SubItems[0].Text;

                BianJi bianJiForm = new BianJi(BianJi.ZhuangTaiLeiXing.BianJi, id);
                bianJiForm.ShowDialog();

                FengJinXinXi.JieGou jieGou = FengJinXinXi.Get(id);
                listView1.SelectedItems[0].SubItems[0].Text = jieGou.ID;
                listView1.SelectedItems[0].SubItems[1].Text = jieGou.YongHuMing;
                listView1.SelectedItems[0].SubItems[2].Text = jieGou.TiebaName;
                listView1.SelectedItems[0].SubItems[3].Text = jieGou.ZuiHouFengJinShiJian;
                listView1.SelectedItems[0].SubItems[4].Text = jieGou.XunHuanKaiShiShiJian;
                listView1.SelectedItems[0].SubItems[5].Text = jieGou.XunHuanJieShuShiJian;
            }
        }

        private void 删除ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int shuLiang = listView1.SelectedItems.Count;
            List<string> ShanChuLieBiao = new List<string>();

            if (shuLiang == 1)//判断选中了几条
            {
                string id = listView1.SelectedItems[0].SubItems[0].Text;
                string yongHuMing = listView1.SelectedItems[0].SubItems[1].Text;
                string tiebaName = listView1.SelectedItems[0].SubItems[2].Text;

                if (MessageBox.Show($"确定要赦免{tiebaName}吧的{yongHuMing}吗？", "笨蛋雪说：", buttons: MessageBoxButtons.OKCancel, icon: MessageBoxIcon.Exclamation) == DialogResult.Cancel)
                {
                    return;
                };

                //加入队列
                ShanChuLieBiao.Add(id);
            }
            else
            {
                if (MessageBox.Show($"确定要赦免选中的{shuLiang}人吗？", "笨蛋雪说：", buttons: MessageBoxButtons.OKCancel, icon: MessageBoxIcon.Exclamation) == DialogResult.Cancel)
                {
                    return;
                };

                //加入队列
                for (int i = 0; i < shuLiang; i++)
                {
                    ShanChuLieBiao.Add(listView1.SelectedItems[i].SubItems[0].Text);
                }
            }

            //从数据库中删除
            foreach (string id in ShanChuLieBiao)
            {
                int jieGuo = access.DoCommand($"delete from 封禁列表 where ID={id}");
                if (jieGuo > 0)
                {
                    listView1.SelectedItems[0].Remove();
                }
                else
                {
                    Say($"ID={id}删除失败");
                }
            }

            //更新列表
            GengXinFengJinLieBiao();
        }

        /// <summary>
        /// 右键菜单打开前
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void contextMenuStrip1_Opening(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (!Config.Stop)
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
                "作者：雪\n" +
                "贴吧：祭雪夏炎吧\n\n" +
                "贴吧管理器交流群：984150818\n" +
                "是否立即加群？";
            if (MessageBox.Show(msg, "笨蛋雪说：关于", buttons: MessageBoxButtons.YesNo, icon: MessageBoxIcon.Asterisk) == DialogResult.Yes)
            {
                string target = Config.QunLianJie;
                try
                {
                    Process.Start(target);
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
            int chengGong = 0;
            int shiBai = 0;
            string shiBaiMingDan = string.Empty;
            while ((line = sr.ReadLine()) != null)
            {
                string[] canshu = Regex.Split(line, "\t", RegexOptions.IgnoreCase);
                if (canshu.Length == 5)
                {
                    //数据库检查重复
                    if (access.GetDataTable($"select * from 封禁列表 where 用户名='{canshu[0]}' and 贴吧名='{canshu[1]}'").Rows.Count > 0)
                    {
                        shiBai += 1;
                        shiBaiMingDan += canshu[0] + "\n";
                        continue;
                    }

                    int jieGuo = access.DoCommand($"insert into 封禁列表 (用户名,贴吧名,最后封禁时间,循环开始时间,循环结束时间)" +
                        $" values('{canshu[0]}','{canshu[1]}','{canshu[2]}','{canshu[3]}','{canshu[4]}')");
                    if (jieGuo > 0)
                    {
                        chengGong += 1;
                    }
                }
            }

            string msg = "导入结果如下：\n";
            msg += $"成功={chengGong}个\n";
            msg += $"失败={shiBai}个";

            GengXinFengJinLieBiao();
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
                Filter = "文本文档|*.txt"
            };
            if (sfd.ShowDialog() != DialogResult.OK) return;

            StringBuilder sb = new StringBuilder();

            DataTable dt = access.GetDataTable("select 用户名,贴吧名,最后封禁时间,循环开始时间,循环结束时间 from 封禁列表");
            foreach (DataRow dr in dt.Rows)
            {
                sb.Append($"{Convert.ToString(dr["用户名"])}\t");
                sb.Append($"{Convert.ToString(dr["贴吧名"])}\t");
                sb.Append($"{Convert.ToString(dr["最后封禁时间"])}\t");
                sb.Append($"{Convert.ToString(dr["循环开始时间"])}\t");
                sb.Append($"{Convert.ToString(dr["循环结束时间"])}\n");
            }
            dt.Clone();

            try
            {
                File.WriteAllText(sfd.FileName, sb.ToString());
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "笨蛋雪说：", buttons: MessageBoxButtons.OK, icon: MessageBoxIcon.Error);
                return;
            }

            MessageBox.Show($"导出成功\n{sfd.FileName}", "笨蛋雪说：", buttons: MessageBoxButtons.OK, icon: MessageBoxIcon.Asterisk);
        }
    }
}
