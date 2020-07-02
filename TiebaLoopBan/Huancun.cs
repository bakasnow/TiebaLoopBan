using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using TiebaLib;

namespace TiebaLoopBan
{
    class Huancun
    {
        /// <summary>
        /// Fid缓存列表
        /// </summary>
        private static readonly List<FidHuancunJiegou> FidHuancunLiebiao = new List<FidHuancunJiegou>();

        /// <summary>
        /// 获取Fid
        /// </summary>
        /// <param name="tiebaname"></param>
        /// <returns></returns>
        public static string GetFid(string tiebaname)
        {
            //先判断缓存里有没有
            foreach (FidHuancunJiegou fidCanshu in FidHuancunLiebiao)
            {
                if (fidCanshu.Tiebaname == tiebaname)
                {
                    return fidCanshu.Fid;
                }
            }

            //获取
            string fid = "";
            for (int i = 0; i < 3; i++)
            {
                fid = Tieba.GetTiebaFid(tiebaname);
                if (fid != "")
                {
                    FidHuancunJiegou fidCanshu = new FidHuancunJiegou
                    {
                        Tiebaname = tiebaname,
                        Fid = fid
                    };
                    FidHuancunLiebiao.Add(fidCanshu);
                    break;
                }
            }

            return fid;
        }

        /// <summary>
        /// 批量添加列表
        /// </summary>
        public static List<string> PiliangTianjiaLiebiao = new List<string>();
    }

    /// <summary>
    /// Fid缓存结构
    /// </summary>
    class FidHuancunJiegou
    {
        public string Tiebaname;
        public string Fid;
    }
}
