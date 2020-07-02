using System;

namespace TiebaLoopBan
{
    //状态
    enum ChuandiZhuangtai
    {
        Xinjian,
        Bianji
    }

    //数据传递
    class BianjiShuju
    {
        //编辑状态
        public static ChuandiZhuangtai BianjiZhuangtai;

        //全局数据
        private static ShujuJiegou QuanjuShuju;

        //设置数据
        public static void SetShuju(ShujuJiegou sjjg)
        {
            QuanjuShuju = sjjg;
        }

        //获取数据
        public static ShujuJiegou GetShuju()
        {
            if (BianjiZhuangtai == ChuandiZhuangtai.Xinjian)
            {
                ShujuJiegou shujuJiegou = new ShujuJiegou();
                shujuJiegou.Zhuangtai = "新建";
                return shujuJiegou;
            }
            else if (BianjiZhuangtai == ChuandiZhuangtai.Bianji)
            {
                ShujuJiegou shujuJiegou = QuanjuShuju;
                shujuJiegou.Zhuangtai = "编辑";
                return shujuJiegou;
            }
            else
            {
                return new ShujuJiegou();
            }
        }

        //数据结构
        public class ShujuJiegou
        {
            public string Zhuangtai = "";
            public int Id = 0;
            public string Yonghuming = "";
            public string Tiebaname = "";
            public string ZuihouFengjinSj = "";
            public DateTime XunhuanKaishiSj = DateTime.Now;
            public DateTime XunhuanJieshuSj = DateTime.Now.AddMonths(1);
        }
    }
}
