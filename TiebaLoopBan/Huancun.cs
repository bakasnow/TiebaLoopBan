using System.Collections.Generic;
using TiebaApi.TiebaWebApi;

namespace TiebaLoopBan
{
    /// <summary>
    /// Fid缓存
    /// </summary>
    public class FidHuanCun
    {
        /// <summary>
        /// Fid缓存列表
        /// </summary>
        private static readonly List<JieGou> LieBiao = new List<JieGou>();

        /// <summary>
        /// 获取Fid
        /// </summary>
        /// <param name="tiebaName"></param>
        /// <returns></returns>
        public static long GetFid(string tiebaName)
        {
            //先判断缓存里有没有
            foreach (JieGou fidCanShu in LieBiao)
            {
               // Form1.Say($"{fidCanShu.TiebaName} {fidCanShu.Fid}");

                if (fidCanShu.TiebaName == tiebaName)
                {
                    return fidCanShu.Fid;
                }
            }

            //获取
            long fid = 0;
            for (int i = 0; i < 3; i++)
            {
                fid = TiebaWeb.GetTiebaFid(tiebaName);
                if (fid >= 0)
                {
                    JieGou fidCanShu = new JieGou
                    {
                        TiebaName = tiebaName,
                        Fid = fid
                    };
                    LieBiao.Add(fidCanShu);
                    break;
                }
            }

            return fid;
        }

        /// <summary>
        /// Fid缓存结构
        /// </summary>
        public class JieGou
        {
            /// <summary>
            /// 贴吧名
            /// </summary>
            public string TiebaName;

            /// <summary>
            /// Fid
            /// </summary>
            public long Fid;
        }
    }

    /// <summary>
    /// 批量添加缓存
    /// </summary>
    public class PiLiangTianJiaHuanCun
    {
        /// <summary>
        /// 批量添加列表
        /// </summary>
        public static List<JieGou> LieBiao = new List<JieGou>();

        /// <summary>
        /// 批量添加结构
        /// </summary>
        public class JieGou
        {
            /// <summary>
            /// 贴吧名
            /// </summary>
            public string TiebaName;

            /// <summary>
            /// 头像ID
            /// </summary>
            public string TouXiangID;

            /// <summary>
            /// 主显账号
            /// </summary>
            public string ZhuXianZhangHao;
        }
    }
}
