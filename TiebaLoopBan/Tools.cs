
using System;
using System.Data;
using System.Windows.Forms;
using TiebaApi.TiebaJieGou;

namespace TiebaLoopBan
{
    static class Tools
    {
        /// <summary>
        /// 获取封禁时长
        /// </summary>
        /// <returns></returns>
        public static int HuoQuFengJinShiChang(DateTimePicker kaiShiShiJian, DateTimePicker jieShuShiJian)
        {
            return (jieShuShiJian.Value - kaiShiShiJian.Value).Days;
        }

        /// <summary>
        /// 获取主显账号
        /// </summary>
        /// <param name="tiebaZhangHaoXinXiJieGou">贴吧账号信息结构</param>
        /// <returns></returns>
        public static string HuoQuZhuXianZhangHao(TiebaZhangHaoXinXiJieGou tiebaZhangHaoXinXiJieGou)
        {
            return tiebaZhangHaoXinXiJieGou.YongHuMing;

            //if (!string.IsNullOrEmpty(tiebaZhangHaoXinXiJieGou.FuGaiMing))
            //{
            //    return tiebaZhangHaoXinXiJieGou.FuGaiMing;
            //}
            //else if (!string.IsNullOrEmpty(tiebaZhangHaoXinXiJieGou.NiCheng))
            //{
            //    return tiebaZhangHaoXinXiJieGou.NiCheng;
            //}
            //else if (!string.IsNullOrEmpty(tiebaZhangHaoXinXiJieGou.YongHuMing))
            //{
            //    return tiebaZhangHaoXinXiJieGou.YongHuMing;
            //}

            //return "";
        }

        /// <summary>
        /// 获取主显账号
        /// </summary>
        /// <param name="tiebaMingPianJieGou">贴吧名片结构</param>
        /// <returns></returns>
        public static string HuoQuZhuXianZhangHao(TiebaMingPianJieGou tiebaMingPianJieGou)
        {
            return tiebaMingPianJieGou.YongHuMing;

            //if (!string.IsNullOrEmpty(tiebaMingPianJieGou.FuGaiMing))
            //{
            //    return tiebaMingPianJieGou.FuGaiMing;
            //}
            //else if (!string.IsNullOrEmpty(tiebaMingPianJieGou.NiCheng))
            //{
            //    return tiebaMingPianJieGou.NiCheng;
            //}
            //else if (!string.IsNullOrEmpty(tiebaMingPianJieGou.YongHuMing))
            //{
            //    return tiebaMingPianJieGou.YongHuMing;
            //}

            //return "";
        }

        /// <summary>
        /// 获取主显账号
        /// </summary>
        /// <param name="tiebaYongHuSouSuoJieGou">贴吧用户搜索结构</param>
        /// <returns></returns>
        public static string HuoQuZhuXianZhangHao(TiebaYongHuSouSuoJieGou tiebaYongHuSouSuoJieGou)
        {
            return tiebaYongHuSouSuoJieGou.YongHuMing;

            //if (!string.IsNullOrEmpty(tiebaYongHuSouSuoJieGou.FuGaiMing))
            //{
            //    return tiebaYongHuSouSuoJieGou.FuGaiMing;
            //}
            //else if (!string.IsNullOrEmpty(tiebaYongHuSouSuoJieGou.NiCheng))
            //{
            //    return tiebaYongHuSouSuoJieGou.NiCheng;
            //}
            //else if (!string.IsNullOrEmpty(tiebaYongHuSouSuoJieGou.YongHuMing))
            //{
            //    return tiebaYongHuSouSuoJieGou.YongHuMing;
            //}

            //return "";
        }
    }
}
