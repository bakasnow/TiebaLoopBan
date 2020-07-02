using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace TiebaLoopBan
{
    static class Lib
    {
        /// <summary>
        /// 获取封禁时长
        /// </summary>
        /// <returns></returns>
        public static int HuoquFengjinShichang(DateTimePicker kaishisj, DateTimePicker jieshusj)
        {
            return (jieshusj.Value - kaishisj.Value).Days;
        }

        /// <summary>
        /// List扩展方法 是否存在
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="func"></param>
        public static bool ListIsRepeat<T>(this List<T> list, Func<T, bool> func)
        {
            for (int i = 0; i < list.Count; i++)
            {
                if (func(list[i]))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
