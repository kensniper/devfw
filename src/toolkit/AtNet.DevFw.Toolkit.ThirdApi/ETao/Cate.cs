﻿namespace AtNet.DevFw.Toolkit.ThirdApi.ETao
{
    public class Cate
    {
        /// <summary>
        /// 商家子定义的类目ID
        /// </summary>
        public int scid { get; set; }

        /// <summary>
        /// 类目名称
        /// </summary>
        public string name { get; set; }

        /// <summary>
        /// 子分类
        /// </summary>
        public Cate[] child { get; set; }

    }
}
