using System;
using System.Data;

namespace TiebaLoopBan
{
    public class FengJinXinXi
    {
        public static JieGou Get(string id)
        {
            JieGou jieGou = new JieGou();

            DataTable dt = Form1.access.GetDataTable($"select top 1 * from 封禁列表 where ID={id}");
            if (dt.Rows.Count == 0)
            {
                return jieGou;
            }

            jieGou.ID = Convert.ToString(dt.Rows[0]["ID"]);
            jieGou.ZhuXianZhangHao = Convert.ToString(dt.Rows[0]["用户名"]);
            jieGou.TouXiang = Convert.ToString(dt.Rows[0]["头像"]);
            jieGou.YongHuMing = Convert.ToString(dt.Rows[0]["用户名"]);
            jieGou.TiebaName = Convert.ToString(dt.Rows[0]["贴吧名"]);
            jieGou.ZuiHouFengJinShiJian = Convert.ToString(dt.Rows[0]["最后封禁时间"]);
            jieGou.XunHuanKaiShiShiJian = Convert.ToString(dt.Rows[0]["循环开始时间"]);
            jieGou.XunHuanJieShuShiJian = Convert.ToString(dt.Rows[0]["循环结束时间"]);

            return jieGou;
        }

        public class JieGou
        {
            public string ID;
            public string ZhuXianZhangHao;
            public string TouXiang;
            public string YongHuMing;
            public string TiebaName;
            public string ZuiHouFengJinShiJian;
            public string XunHuanKaiShiShiJian;
            public string XunHuanJieShuShiJian;
        }
    }
}
