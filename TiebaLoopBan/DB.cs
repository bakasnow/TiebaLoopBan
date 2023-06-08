using System.Windows.Forms;

namespace TiebaLoopBan
{
    public class DB
    {
        /// <summary>
        /// 数据库连接
        /// </summary>
        public static Access access = new Access(Application.StartupPath + @"\TiebaLoopBan.mdb");
    }
}
