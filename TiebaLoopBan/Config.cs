using System.Windows.Forms;

namespace TiebaLoopBan
{
    static class Config
    {
        //版本号
        public const string Vname = "tlb";
        public static string Version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
        public const string DataBaseVersion = "1.4.0.119";//这个不跟软件版本
        public const string QunLianJie = "https://jq.qq.com/?_wv=1027&k=AFbQe7pl";

        //基本参数
        public static bool Stop = true;//主线程控制
        public const string PeiZhiMing = "配置1";
        public static int SaoMiaoJianGe = 0;
        public static int FengJinJianGe = 0;
        public static int ChongShiCiShu = 0;
        public static int ChongShiJianGe = 0;

        //全局参数
        public static string Cookie = string.Empty;
        public static string YongHuMing = string.Empty;
    }
}
