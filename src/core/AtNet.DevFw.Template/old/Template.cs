﻿//
// Copyright 2011 @ S1N1.COM,All right reseved.
// Name:Template.cs
// Author:newmin
// Create:2011/06/13
//

using System;
using System.IO;
using System.Text.RegularExpressions;

namespace AtNet.DevFw.Template
{
    /// <summary>
    /// 模板实体
    /// </summary>
    public sealed class Template
    {
        /// <summary>
        /// 模板ID
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// 模板路径
        /// </summary>
        public string FilePath { get; set; }

        /// <summary>
        /// 模板注释(第一个html注释)
        /// </summary>
        public string Comment
        {
            get
            {
                //读取文件开头部分注释
                Regex reg = new Regex("<!--(.+?)-->", RegexOptions.Multiline);

                //保存内容到变量，避免重复读取
                string content = null;

                using (StreamReader sr = new StreamReader(FilePath))
                {
                    content = sr.ReadToEnd();
                }
                Match match = reg.Match(content);


                return match == null ? String.Empty : match.Groups[1].Value;
            }
        }


        /// <summary>
        /// 模板内容
        /// </summary>
        public string Content
        {
            get
            {
                string content;
                IDataContrainer dc = new HttpDataContrainer();

                //从缓存中读取模板内容
                if (Config.EnabledCache)
                {
                    content = dc.GetTemplatePageCacheContent(this.ID);
                    if (content != null) return content;
                }

                if (String.IsNullOrEmpty(this.FilePath)) throw new ArgumentNullException("模板文件不存在!" + this.FilePath);

                //读取内容并缓存
                StreamReader sr = new StreamReader(this.FilePath);
                content = sr.ReadToEnd();
                sr.Dispose();


                string partialFilePath = "";
                //读取模板里的部分视图
                content = TemplateRegexUtility.partialRegex.Replace(content, m =>
                {
                    string _path = m.Groups[1].Value;
                    string tplID = TemplateUtility.GetPartialTemplateID(_path, this.FilePath, out partialFilePath);
                    return Regex.Replace(m.Value, _path, tplID);
                });


                //替换系统标签
                content = TemplateRegexUtility.Replace(content, m => { return TemplateCache.Tags[m.Groups[1].Value]; });


                //压缩模板代码
                if (false || Config.EnabledCompress)
                {
                    content = TemplateUtility.CompressHtml(content);
                }

                //缓存模板
                if (Config.EnabledCache) dc.SetTemplatePageCacheContent(this.ID, content, this.FilePath);

                return content;
            }
        }
    }
}