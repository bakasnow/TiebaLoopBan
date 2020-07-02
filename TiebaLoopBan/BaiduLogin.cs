using System;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

using TiebaLib;

namespace TiebaLoopBan
{
    public partial class BaiduLogin : Form
    {
        public BaiduLogin()
        {
            InitializeComponent();
        }

        private void baiduLogin_Load(object sender, EventArgs e)
        {
            Text = "请登录百度账号";
            webBrowser1.Url = new Uri("https://passport.baidu.com/v2/?login");
        }

        private const int INTERNET_COOKIE_HTTPONLY = 0x00002000;

        [DllImport("wininet.dll", SetLastError = true)]
        private static extern bool InternetGetCookieEx(
            string url,
            string cookieName,
            StringBuilder cookieData,
            ref int size,
            int flags,
            IntPtr pReserved);

        public static string GetCookie(string url)
        {
            int size = 512;
            StringBuilder sb = new StringBuilder(size);
            if (!InternetGetCookieEx(url, null, sb, ref size, INTERNET_COOKIE_HTTPONLY, IntPtr.Zero))
            {
                if (size < 0)
                {
                    return "";
                }
                sb = new StringBuilder(size);
                if (!InternetGetCookieEx(url, null, sb, ref size, INTERNET_COOKIE_HTTPONLY, IntPtr.Zero))
                {
                    return "";
                }
            }

            string sbstr = sb.ToString();
            return sbstr;
        }

        private void webBrowser1_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            if (e.Url.ToString().IndexOf("https://passport.baidu.com/center") != -1)
            {
                webBrowser1.Url = new Uri("https://tieba.baidu.com/");
                return;
            }

            if (e.Url.ToString().IndexOf("https://tieba.baidu.com/") != -1)
            {
                string cookie = GetCookie("https://tieba.baidu.com/");
                string yhm = Tieba.GetBaiduYongHuMing(cookie);
                if (yhm != "")
                {
                    Quanju.Cookie = cookie;
                }
                else
                {
                    MessageBox.Show(text: " 登录失败，请重新登录", caption: "笨蛋雪说：", buttons: MessageBoxButtons.OK, icon: MessageBoxIcon.Error);
                }

                Dispose();
            }
        }

        //窗口关闭前
        private void baiduLogin_FormClosing(object sender, FormClosingEventArgs e)
        {
            webBrowser1.Dispose();
        }
    }
}
