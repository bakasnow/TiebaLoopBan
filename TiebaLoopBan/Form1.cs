using System;
using System.Data;
using System.IO;
using System.Windows.Forms;
using System.Threading;
using System.Text;

using BakaSnowTool;
using BakaSnowTool.Http;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Diagnostics;
using CsharpHttpHelper;
using CsharpHttpHelper.Enum;
using BaiduLogin;
using TiebaApi.TiebaWebApi;
using TiebaApi.TiebaJieGou;
using TiebaApi.TiebaBaWuApi;

namespace TiebaLoopBan
{
    public partial class Form1 : Form
    {
        private static bool IsZiQiDong = false;

        public Form1(string[] args)
        {
            InitializeComponent();

            foreach (var p in args)
            {
                if (p.ToLower() == "-start")
                {
                    IsZiQiDong = true;
                    break;
                }
            }
        }

        /// <summary>
        /// 版本验证
        /// </summary>
        /// <returns></returns>
        private bool Version()
        {
            //如果是自启动跳过版本验证
            if (IsZiQiDong)
            {
                return true;
            }

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
                        Process.Start(target);
                    }

                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// 窗口 创建
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
            DB.access.Open();

            //校验数据库版本
            string dbv = Convert.ToString(DB.access.GetDataResult("select top 1 版本号 from 数据库版本"));
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
            if (DB.access.GetDataTable($"select * from 基本设置 where 配置名='{Config.PeiZhiMing}'").Rows.Count == 0)
            {
                DB.access.DoCommand($"insert into 基本设置 (配置名,Cookie,扫描间隔,封禁间隔,重试次数,重试间隔) values('{Config.PeiZhiMing}','',60,10,1,10)");
            }

            Text = "贴吧循环封禁 v" + Config.Version;

            CheckForIllegalCrossThreadCalls = false;
            listView1.MultiSelect = true;
            listView1.FullRowSelect = true;
            statusStrip1.LayoutStyle = ToolStripLayoutStyle.HorizontalStackWithOverflow;
            toolStripStatusLabel1.Alignment = ToolStripItemAlignment.Left;
            toolStripProgressBar1.Alignment = ToolStripItemAlignment.Right;

            //读取基本配置
            Config.Cookie = Convert.ToString(DB.access.GetDataResult($"select Cookie from 基本设置 where 配置名='{Config.PeiZhiMing}'"));
            textBox1.Text = Convert.ToString(DB.access.GetDataResult($"select 封禁间隔 from 基本设置 where 配置名='{Config.PeiZhiMing}'"));

            //更新封禁列表
            GengXinFengJinLieBiao();

            //账号验证
            ZhangHaoYanZheng(true);

            Say("贴吧管理器交流群：984150818（免费下载吧务工具）");

            //是否自启动
            if (IsZiQiDong && !string.IsNullOrEmpty(Config.ZhuXianZhangHao))
            {
                button3_Click(null, new EventArgs());
            }
        }

        /// <summary>
        /// 窗口 第一次显示
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form1_Shown(object sender, EventArgs e)
        {
            if (listView1.Items.Count <= 0)
            {
                MessageBox.Show(text: "操作指南：右键单击列表或日志有菜单。", caption: "笨蛋雪说：", buttons: MessageBoxButtons.OK, icon: MessageBoxIcon.Information);
            }
        }

        /// <summary>
        /// 更新封禁列表
        /// </summary>
        public void GengXinFengJinLieBiao()
        {
            DataTable dt = DB.access.GetDataTable("select * from 封禁列表");

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
            DB.access.DoCommand($"update 基本设置 set Cookie='{Config.Cookie}' where 配置名='{Config.PeiZhiMing}'");
            DB.access.DoCommand($"update 基本设置 set 封禁间隔={textBox1.Text} where 配置名='{Config.PeiZhiMing}'");
        }

        /// <summary>
        /// 窗口关闭
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (MessageBox.Show("是否关闭软件？", "笨蛋雪说：", buttons: MessageBoxButtons.YesNo, icon: MessageBoxIcon.Asterisk) == DialogResult.No)
            {
                e.Cancel = true;
                return;
            }

            Save();
            DB.access.Close();
        }

        /// <summary>
        /// 基本参数
        /// </summary>
        private void JiBenCanShu()
        {
            Config.Cookie = Convert.ToString(DB.access.GetDataResult($"select Cookie from 基本设置 where 配置名='{Config.PeiZhiMing}'"));
            Config.SaoMiaoJianGe = Convert.ToInt32(DB.access.GetDataResult($"select 扫描间隔 from 基本设置 where 配置名='{Config.PeiZhiMing}'"));
            //Config.ChongShiCiShu = Convert.ToInt32(DB.access.GetDataResult($"select 重试次数 from 基本设置 where 配置名='{Config.PeiZhiMing}'"));
            Config.FengJinJianGe = Convert.ToInt32(DB.access.GetDataResult($"select 封禁间隔 from 基本设置 where 配置名='{Config.PeiZhiMing}'"));
            Config.ChongShiJianGe = Convert.ToInt32(DB.access.GetDataResult($"select 重试间隔 from 基本设置 where 配置名='{Config.PeiZhiMing}'"));
        }

        /// <summary>
        /// 账号验证
        /// </summary>
        /// <param name="isLoad">窗口是否加载中</param>
        /// <returns></returns>
        public bool ZhangHaoYanZheng(bool isLoad = false)
        {
            Config.ZhuXianZhangHao = Tools.HuoQuZhuXianZhangHao(TiebaWeb.GetTiebaZhangHaoXinXi(Config.Cookie));

            if (string.IsNullOrEmpty(Config.ZhuXianZhangHao))
            {
                if (!isLoad)
                {
                    Say("账号验证失败");
                }

                label3.Text = "未登录";
                button1.Text = "登录账号";
                return false;
            }
            else
            {
                label3.Text = Config.ZhuXianZhangHao;
                button1.Text = "删除账号";
                return true;
            }
        }

        /// <summary>
        /// 批量禁用控件
        /// </summary>
        /// <param name="b"></param>
        private void PiLiangJinYongKongJian(bool b)
        {
            button1.Enabled = b;
            textBox1.Enabled = b;
        }

        /// <summary>
        /// 开始
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button3_Click(object sender, EventArgs e)
        {
            button3.Enabled = false;

            if (button3.Text == "开始")
            {
                if (!int.TryParse(BST.JianYiZhengZe(textBox1.Text, "([0-9]{1,})"), out int fengJinJianGe))
                {
                    MessageBox.Show("封禁间隔格式错误", "笨蛋雪说：", buttons: MessageBoxButtons.OK, icon: MessageBoxIcon.Exclamation);
                    button3.Enabled = true;
                    return;
                }

                if (fengJinJianGe < 3)
                {
                    MessageBox.Show("为了操作安全，封禁间隔不得低于3秒", "笨蛋雪说：", buttons: MessageBoxButtons.OK, icon: MessageBoxIcon.Exclamation);
                    textBox1.Text = "3";
                    button3.Enabled = true;
                    return;
                }

                textBox1.Text = Convert.ToString(fengJinJianGe);

                Save();//先保存
                JiBenCanShu();//初始化参数

                Config.Stop = false;
                Thread mainThr = new Thread(new ThreadStart(MainThread))
                {
                    IsBackground = true
                };
                mainThr.Start();

                button3.Text = "停止";
                button3.Enabled = true;
                PiLiangJinYongKongJian(false);
            }
            else
            {
                button3.Enabled = false;
                Config.Stop = true;
            }
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
                if (0 <= DateTime.Now.Hour && DateTime.Now.Hour < 1)
                {
                    if (gongZuoShiDuan)//防止重复输出
                    {
                        Say("为避开网络高峰，任务将在凌晨1点开始，请保持挂机");
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
                int shanChuShu = DB.access.DoCommand($"delete from 封禁列表 where 循环结束时间<'{DateTime.Now:yyyy-MM-dd}'");
                if (shanChuShu > 0)
                {
                    Say($"删除{shanChuShu}条过期任务");
                }


                //循环前初始化进度条
                int weiFengJinShu = (int)DB.access.GetDataResult("select count(*) from 封禁列表 where DateDiff(\"d\", now(), 最后封禁时间)<0");
                int lieBiaoZongShu = (int)DB.access.GetDataResult("select count(*) from 封禁列表");

                toolStripStatusLabel1.Text = $"未封禁数：{weiFengJinShu}   列表总数：{lieBiaoZongShu}";

                toolStripProgressBar1.Minimum = 0;
                toolStripProgressBar1.Maximum = lieBiaoZongShu;
                toolStripProgressBar1.Value = lieBiaoZongShu - weiFengJinShu;

                //开始遍历
                for (int i = 0; i < listView1.Items.Count; i++)
                {
                    if (Config.Stop) break;

                    string dangQianShiJian = DateTime.Now.ToString("yyyy-MM-dd");

                    string id = listView1.Items[i].SubItems[0].Text;
                    DataTable dt = DB.access.GetDataTable($"select top 1 * from 封禁列表 where ID={id}");
                    if (dt.Rows.Count == 0)
                    {
                        continue;
                    }

                    string zhuXianZhangHao = Convert.ToString(dt.Rows[0]["用户名"]);
                    string touXiang = Convert.ToString(dt.Rows[0]["头像"]);
                    string tiebaName = Convert.ToString(dt.Rows[0]["贴吧名"]);
                    string zuiHouFengJinShiJian = Convert.ToString(dt.Rows[0]["最后封禁时间"]);

                    if (Convert.ToDateTime(zuiHouFengJinShiJian) >= Convert.ToDateTime(dangQianShiJian))
                    {
                        continue;
                    }

                    //获取用户信息
                    if (string.IsNullOrEmpty(zhuXianZhangHao))
                    {
                        TiebaMingPianJieGou mingPianJieGou = TiebaWeb.GetTiebaMingPian(touXiang);
                        if (mingPianJieGou.HuoQuChengGong)
                        {
                            //更新任务参数
                            zhuXianZhangHao = Tools.HuoQuZhuXianZhangHao(mingPianJieGou);

                            //写进数据库
                            string sqlStr = $"update 封禁列表 set 用户名='{zhuXianZhangHao}',头像='{touXiang}' where ID={id}";
                            if (DB.access.DoCommand(sqlStr) == 0)
                            {
                                Say($"SQL执行失败：{sqlStr}");
                            }
                        }
                        else
                        {
                            Say($"用户名={zhuXianZhangHao}，用户信息获取失败：{mingPianJieGou.Msg}", zhuXianZhangHao, touXiang);
                            DengDai(Config.FengJinJianGe);
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
                        YongHuMing = zhuXianZhangHao,
                        NiCheng = string.Empty,
                        TouXiang = touXiang
                    };

                    int day = 1;
                    string liYou = $"由于您违反{tiebaName}吧规定，故封禁1天，如有疑问请联系吧务团队";

                    //封禁结果
                    if (baWu.FengJin(day, liYou, out string msg))
                    {
                        Say($"{zhuXianZhangHao}在{tiebaName}吧，封禁1天成功", zhuXianZhangHao, touXiang);
                        int jieGuo = DB.access.DoCommand($"update 封禁列表 set 最后封禁时间='{dangQianShiJian}' where ID={id}");
                        if (jieGuo > 0)
                        {
                            listView1.Items[i].SubItems[3].Text = dangQianShiJian;
                        }

                        weiFengJinShu = (int)DB.access.GetDataResult("select count(*) from 封禁列表 where DateDiff(\"d\", now(), 最后封禁时间)<0");
                        toolStripStatusLabel1.Text = $"未封禁数：{weiFengJinShu}   列表总数：{lieBiaoZongShu}";
                        toolStripProgressBar1.Value = lieBiaoZongShu - weiFengJinShu;

                        //封禁间隔
                        DengDai(Config.FengJinJianGe);
                    }
                    else
                    {
                        Say($"{zhuXianZhangHao}在{tiebaName}吧，封禁1天失败：{msg} {Config.ChongShiJianGe}秒后重试", zhuXianZhangHao, touXiang);

                        //重试间隔
                        if (msg == "need vcode")
                        {
                            Say("错误信息：need vcode，账号操作频繁，暂停5分钟后重试");
                            DengDai(300);
                        }
                        else
                        {
                            DengDai(Config.ChongShiJianGe);
                        }
                    }
                }

                //扫描间隔
                DengDai(Config.SaoMiaoJianGe);
            }

            Say("任务结束");

            button3.Text = "开始";
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
            if (!Config.Stop) return;

            if (button1.Text.Contains("登录"))
            {
                //登录账号
                BaiduLoginForm baiduLoginForm = new BaiduLoginForm();
                baiduLoginForm.ShowDialog();
                Config.Cookie = baiduLoginForm.Cookie;

                ZhangHaoYanZheng();
            }
            else
            {
                //删除账号
                if (MessageBox.Show(text: "确定要删除账号吗？", caption: "笨蛋雪说：操作不可逆", buttons: MessageBoxButtons.OKCancel, icon: MessageBoxIcon.Exclamation) == DialogResult.Cancel)
                {
                    return;
                }

                if (DB.access.DoCommand($"update 基本设置 set Cookie='' where 配置名='{Config.PeiZhiMing}'") > 0)
                {
                    Config.Cookie = string.Empty;
                    Say("账号已成功删除");
                    label3.Text = "未登录";
                    button1.Text = "登录账号";
                }
                else
                {
                    MessageBox.Show(text: "操作失败", caption: "笨蛋雪说：", buttons: MessageBoxButtons.OK, icon: MessageBoxIcon.Error);
                }
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
        public void Say(string msg, string zhuXianZhangHao = "", string touXiangID = "")
        {
            WriteLog(msg);

            if (ShuChuTiaoShu >= 500)
            {
                listView2.Items.Clear();
                ShuChuTiaoShu = 0;
            }

            ShuChuTiaoShu++;

            ListViewItem listViewItem = new ListViewItem
            {
                Text = zhuXianZhangHao
            };
            listViewItem.SubItems.Add(touXiangID);
            listViewItem.SubItems.Add($"{DateTime.Now:yy/MM/dd HH:mm:ss} {msg}");

            listView2.Items.Add(listViewItem);
            listView2.Items[listView2.Items.Count - 1].EnsureVisible();
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

            string id = Convert.ToString(DB.access.GetDataResult($"select ID from 封禁列表 where 头像='{bianJiForm.JieGou.TouXiangID}' and 贴吧名='{bianJiForm.JieGou.TiebaName}'"));
            if (string.IsNullOrEmpty(id))
            {
                return;
            }

            FengJinXinXi.JieGou jieGou = FengJinXinXi.Get(id);

            ListViewItem lvi = new ListViewItem()
            {
                Text = Convert.ToString(jieGou.ID)
            };

            lvi.SubItems.Add(jieGou.ZhuXianZhangHao);
            lvi.SubItems.Add(jieGou.TiebaName);
            lvi.SubItems.Add(jieGou.ZuiHouFengJinShiJian);
            lvi.SubItems.Add(jieGou.XunHuanKaiShiShiJian);
            lvi.SubItems.Add(jieGou.XunHuanJieShuShiJian);
            listView1.Items.Add(lvi);

            listView1.Items[listView1.Items.Count - 1].EnsureVisible();
        }

        private void 批量添加ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PiLiangTianJia piLiangTianJiaForm = new PiLiangTianJia();

            piLiangTianJiaForm.ShowDialog();
            GengXinFengJinLieBiao();
        }

        private void 编辑ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count != 1) return;

            //取数据库主键
            string id = listView1.SelectedItems[0].SubItems[0].Text;

            //打开编辑窗口
            BianJi bianJiForm = new BianJi(BianJi.ZhuangTaiLeiXing.BianJi, id);
            bianJiForm.ShowDialog();

            //更新选中列的信息
            FengJinXinXi.JieGou jieGou = FengJinXinXi.Get(id);
            listView1.SelectedItems[0].SubItems[0].Text = jieGou.ID;
            listView1.SelectedItems[0].SubItems[1].Text = jieGou.ZhuXianZhangHao;
            listView1.SelectedItems[0].SubItems[2].Text = jieGou.TiebaName;
            listView1.SelectedItems[0].SubItems[3].Text = jieGou.ZuiHouFengJinShiJian;
            listView1.SelectedItems[0].SubItems[4].Text = jieGou.XunHuanKaiShiShiJian;
            listView1.SelectedItems[0].SubItems[5].Text = jieGou.XunHuanJieShuShiJian;
        }

        private void 删除ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int shuLiang = listView1.SelectedItems.Count;
            List<string> ShanChuLieBiao = new List<string>();

            if (shuLiang == 1)//判断选中了几条
            {
                string id = listView1.SelectedItems[0].SubItems[0].Text;
                string zhuXianZhangHao = listView1.SelectedItems[0].SubItems[1].Text;
                string tiebaName = listView1.SelectedItems[0].SubItems[2].Text;

                if (MessageBox.Show($"确定要赦免{tiebaName}吧的{zhuXianZhangHao}吗？", "笨蛋雪说：", buttons: MessageBoxButtons.OKCancel, icon: MessageBoxIcon.Exclamation) == DialogResult.Cancel)
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
                int jieGuo = DB.access.DoCommand($"delete from 封禁列表 where ID={id}");
                if (jieGuo > 0)
                {
                    //从列表中删除
                    for (int i = listView1.Items.Count - 1; i >= 0; i--)
                    {
                        if (listView1.Items[i].Text == id)
                        {
                            listView1.Items[i].Remove();
                            break;
                        }
                    }
                }
                else
                {
                    Say($"ID={id}删除失败");
                }
            }
        }

        /// <summary>
        /// 封禁名单 右键菜单打开前
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void contextMenuStrip1_Opening(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (Config.Stop)
            {
                //已停机
                添加ToolStripMenuItem.Enabled = true;
                批量添加ToolStripMenuItem.Enabled = true;
                导入ToolStripMenuItem.Enabled = true;
                导出ToolStripMenuItem.Enabled = true;

                //选中1个
                if (listView1.SelectedItems.Count == 1)
                {
                    编辑ToolStripMenuItem.Enabled = true;
                    删除ToolStripMenuItem.Enabled = true;
                    复制选中toolStripMenuItem.Enabled = true;
                }
                //选中多个
                else if (listView1.SelectedItems.Count > 1)
                {
                    编辑ToolStripMenuItem.Enabled = false;
                    删除ToolStripMenuItem.Enabled = true;
                    复制选中toolStripMenuItem.Enabled = false;
                }
                //没有选中
                else
                {
                    编辑ToolStripMenuItem.Enabled = false;
                    删除ToolStripMenuItem.Enabled = false;
                    复制选中toolStripMenuItem.Enabled = false;
                }
            }
            else
            {
                //任务进行中
                添加ToolStripMenuItem.Enabled = false;
                批量添加ToolStripMenuItem.Enabled = false;
                编辑ToolStripMenuItem.Enabled = false;
                删除ToolStripMenuItem.Enabled = false;
                导入ToolStripMenuItem.Enabled = false;
                导出ToolStripMenuItem.Enabled = false;

                //选中1个
                if (listView1.SelectedItems.Count == 1)
                {
                    复制选中toolStripMenuItem.Enabled = true;
                }
                //选中多个
                else if (listView1.SelectedItems.Count > 1)
                {
                    复制选中toolStripMenuItem.Enabled = false;
                }
                //没有选中
                else
                {
                    复制选中toolStripMenuItem.Enabled = false;
                }
            }
        }

        /// <summary>
        /// 日志 右键菜单打开前
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void contextMenuStrip2_Opening(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (listView2.SelectedItems.Count > 0)
            {
                复制日志toolStripMenuItem.Enabled = true;

                if (string.IsNullOrEmpty(listView2.SelectedItems[0].Text))
                {
                    日志_复制用户名toolStripMenuItem.Enabled = false;
                    日志_复制用户名toolStripMenuItem.Text = "复制用户名（用户名为空）";
                }
                else
                {
                    日志_复制用户名toolStripMenuItem.Enabled = true;
                    日志_复制用户名toolStripMenuItem.Text = $"复制用户名：{listView2.SelectedItems[0].Text}";
                }

                if (string.IsNullOrEmpty(listView2.SelectedItems[0].SubItems[1].Text))
                {
                    只复制头像IDtoolStripMenuItem.Enabled = false;
                    只复制头像IDtoolStripMenuItem.Text = "复制头像ID（头像ID为空）";
                }
                else
                {
                    只复制头像IDtoolStripMenuItem.Enabled = true;
                    只复制头像IDtoolStripMenuItem.Text = $"复制头像ID：{listView2.SelectedItems[0].SubItems[1].Text}";
                }

                if (!string.IsNullOrEmpty(listView2.SelectedItems[0].SubItems[1].Text))
                {
                    日志_访问TA的主页toolStripMenuItem.Enabled = true;
                }
                else if (!string.IsNullOrEmpty(listView2.SelectedItems[0].Text))
                {
                    日志_访问TA的主页toolStripMenuItem.Enabled = true;
                }
                else
                {
                    日志_访问TA的主页toolStripMenuItem.Enabled = false;
                }

                清屏toolStripMenuItem.Enabled = true;
            }
            else
            {
                复制日志toolStripMenuItem.Enabled = false;
                日志_复制用户名toolStripMenuItem.Enabled = false;
                只复制头像IDtoolStripMenuItem.Enabled = false;

                if (listView2.Items.Count > 0)
                {
                    清屏toolStripMenuItem.Enabled = true;
                }
                else
                {
                    清屏toolStripMenuItem.Enabled = false;
                }
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
            listView2.Items.Clear();
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
                "贴吧管理器交流群：984150818\n" +
                "是否立即加群？";
            if (MessageBox.Show(msg, "笨蛋雪说：", buttons: MessageBoxButtons.YesNo, icon: MessageBoxIcon.Asterisk) == DialogResult.Yes)
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
                Title = "笨蛋雪说：请选择需要导入的循封名单",
                Filter = "文本文件|*.txt"
            };
            if (ofd.ShowDialog() != DialogResult.OK) return;

            StreamReader sr = new StreamReader(ofd.FileName, Encoding.UTF8);

            string line;
            int chengGong = 0;
            int shiBai = 0;
            string shiBaiMingDan = string.Empty;
            while ((line = sr.ReadLine()) != null)
            {
                string[] canshu = Regex.Split(line, "\t", RegexOptions.IgnoreCase);
                if (canshu.Length == 6)
                {
                    //数据库检查重复
                    if (DB.access.GetDataTable($"select * from 封禁列表 where 头像='{canshu[1]}' and 贴吧名='{canshu[2]}'").Rows.Count > 0)
                    {
                        shiBai += 1;
                        shiBaiMingDan += $"{canshu[0]}\t{canshu[1]}\t{canshu[2]}\t{canshu[3]}\t{canshu[4]}\t{canshu[5]}\n";
                        continue;
                    }

                    int jieGuo = DB.access.DoCommand($"insert into 封禁列表 (用户名,头像,贴吧名,最后封禁时间,循环开始时间,循环结束时间)" +
                        $" values('{canshu[0]}','{canshu[1]}','{canshu[2]}','{canshu[3]}','{canshu[4]}','{canshu[5]}')");
                    if (jieGuo > 0)
                    {
                        chengGong += 1;
                    }
                }
            }

            string msg = "导入结果如下：\n";
            msg += $"成功={chengGong}个\n";
            msg += $"失败={shiBai}个\n";

            GengXinFengJinLieBiao();
            MessageBox.Show(msg, "笨蛋雪说：", buttons: MessageBoxButtons.OK, icon: MessageBoxIcon.Asterisk);

            if (shiBai > 0)
            {
                string path = $@"{Application.StartupPath}\{DateTime.Now:yy-MM-dd} 导入失败名单.txt";

                try
                {
                    File.WriteAllText(path, shiBaiMingDan, Encoding.UTF8);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "笨蛋雪说：", buttons: MessageBoxButtons.OK, icon: MessageBoxIcon.Error);
                    return;
                }

                MessageBox.Show($"导入失败名单\n{path}", "笨蛋雪说：", buttons: MessageBoxButtons.OK, icon: MessageBoxIcon.Asterisk);
            }
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

            DataTable dt = DB.access.GetDataTable("select 用户名,头像,贴吧名,最后封禁时间,循环开始时间,循环结束时间 from 封禁列表");
            foreach (DataRow dr in dt.Rows)
            {
                sb.Append($"{Convert.ToString(dr["用户名"])}\t");
                sb.Append($"{Convert.ToString(dr["头像"])}\t");
                sb.Append($"{Convert.ToString(dr["贴吧名"])}\t");
                sb.Append($"{Convert.ToString(dr["最后封禁时间"])}\t");
                sb.Append($"{Convert.ToString(dr["循环开始时间"])}\t");
                sb.Append($"{Convert.ToString(dr["循环结束时间"])}\n");
            }
            dt.Clone();

            try
            {
                File.WriteAllText(sfd.FileName, sb.ToString(), Encoding.UTF8);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "笨蛋雪说：", buttons: MessageBoxButtons.OK, icon: MessageBoxIcon.Error);
                return;
            }

            MessageBox.Show($"导出成功\n{sfd.FileName}", "笨蛋雪说：", buttons: MessageBoxButtons.OK, icon: MessageBoxIcon.Asterisk);
        }

        private void 复制日志toolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (listView2.SelectedItems.Count <= 0) return;

            Tools.FuZhiDaoJianQieBan(listView2.SelectedItems[0].SubItems[2].Text);
        }

        private void 日志_复制用户名toolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (listView2.SelectedItems.Count <= 0) return;

            Tools.FuZhiDaoJianQieBan(listView2.SelectedItems[0].Text);
        }

        private void 日志_复制头像IDtoolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (listView2.SelectedItems.Count <= 0) return;

            Tools.FuZhiDaoJianQieBan(listView2.SelectedItems[0].SubItems[1].Text);
        }

        private void 日志_访问TA的主页toolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (listView2.SelectedItems.Count <= 0) return;

            string url;

            if (listView2.SelectedItems[0].SubItems[1].Text.Length > 0)
            {
                url = $"https://tieba.baidu.com/home/main?id={listView2.SelectedItems[0].SubItems[1].Text}";
            }
            else if (listView2.SelectedItems[0].Text.Length > 0)
            {
                url = $"https://tieba.baidu.com/home/main?un={Http.UrlEncode(listView2.SelectedItems[0].Text)}";
            }
            else
            {
                return;
            }

            Process.Start(url);
        }

        private void 清屏toolStripMenuItem_Click(object sender, EventArgs e)
        {
            listView2.Items.Clear();
        }

        private void 列表_复制用户名ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count != 1) return;

            string id = listView1.SelectedItems[0].Text;

            Tools.FuZhiDaoJianQieBan(FengJinXinXi.Get(id).YongHuMing);
        }

        private void 列表_复制头像IDToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count != 1) return;

            string id = listView1.SelectedItems[0].Text;

            Tools.FuZhiDaoJianQieBan(FengJinXinXi.Get(id).TouXiangID);
        }

        private void 列表_访问TA的主页ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count != 1) return;

            string id = listView1.SelectedItems[0].Text, url;

            FengJinXinXi.JieGou jieGou = FengJinXinXi.Get(id);

            if (!string.IsNullOrEmpty(jieGou.TouXiangID))
            {
                url = $"https://tieba.baidu.com/home/main?id={jieGou.TouXiangID}";
            }
            else if (!string.IsNullOrEmpty(jieGou.YongHuMing))
            {
                url = $"https://tieba.baidu.com/home/main?un={Http.UrlEncode(jieGou.YongHuMing)}";
            }
            else
            {
                return;
            }

            Process.Start(url);
        }

        private void 复制选中toolStripMenuItem_DropDownOpening(object sender, EventArgs e)
        {
            string id = listView1.SelectedItems[0].Text;

            FengJinXinXi.JieGou jieGou = FengJinXinXi.Get(id);
            if (string.IsNullOrEmpty(jieGou.YongHuMing))
            {
                列表_复制用户名ToolStripMenuItem.Enabled = false;
                列表_复制用户名ToolStripMenuItem.Text = "复制用户名（用户名为空）";
            }
            else
            {
                列表_复制用户名ToolStripMenuItem.Enabled = true;
                列表_复制用户名ToolStripMenuItem.Text = $"复制用户名：{jieGou.YongHuMing}";
            }

            if (string.IsNullOrEmpty(jieGou.TouXiangID))
            {
                列表_复制头像IDToolStripMenuItem.Enabled = false;
                列表_复制头像IDToolStripMenuItem.Text = "复制头像ID（头像ID为空）";
            }
            else
            {
                列表_复制头像IDToolStripMenuItem.Enabled = true;
                列表_复制头像IDToolStripMenuItem.Text = $"复制头像ID：{jieGou.TouXiangID}";
            }
        }

        /// <summary>
        /// 查找窗口
        /// 防止重复开启
        /// </summary>
        private ChaZhao ChaZhaoFrom = new ChaZhao();

        private void 查找toolStripMenuItem_Click(object sender, EventArgs e)
        {
            //创建窗口
            ChaZhaoFrom = new ChaZhao
            {
                Form1ListView1 = listView1
            };

            ChaZhaoFrom.Show();
        }
    }
}
