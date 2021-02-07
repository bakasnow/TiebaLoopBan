using System.Collections.Generic;

using TiebaLib;

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
        public static string GetFid(string tiebaName)
        {
            //先判断缓存里有没有
            foreach (JieGou fidCanShu in LieBiao)
            {
                if (fidCanShu.TiebaName == tiebaName)
                {
                    return fidCanShu.Fid;
                }
            }

            //获取
            string fid = string.Empty;
            for (int i = 0; i < 3; i++)
            {
                fid = Tieba.GetTiebaFid(tiebaName);
                if (!string.IsNullOrEmpty(fid))
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
            public string Fid;
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
            /// ID
            /// </summary>
            public string Id;

            /// <summary>
            /// 添加成功
            /// </summary>
            public bool TianJiaChengGong;
        }
    }
}
