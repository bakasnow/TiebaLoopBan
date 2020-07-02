using System.Windows.Forms;

namespace TiebaLoopBan
{
    static class Quanju
    {
        //版本号
        public const string Vname = "tlb";
        public const string Version = "1.3.2.190517";
        public const string QunLianjie = "https://jq.qq.com/?_wv=1027&k=5BIkMNS";
        public static readonly string RizhiLujing = Application.StartupPath + "\\Log";

        //基本参数
        public static bool Stop = true;//主线程控制
        public const string PeizhiMing = "'配置1'";
        public static int SaomiaoJiange = 0;
        public static int FengjinJiange = 0;
        public static int ChongshiCishu = 0;
        public static int ChongshiJiange = 0;
        public static bool KehuduanFengjinJiekou = false;

        //全局参数
        public static string Cookie = "";
        public static string YonghuMing = "";
    }
}
