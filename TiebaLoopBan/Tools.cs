
using System;
using System.Data;
using System.Windows.Forms;

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
    }
}
