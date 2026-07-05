using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml.Serialization;
using UnityEngine;

namespace Table
{
    [Serializable]
    public class shopdialog
    {
        /// <summary>
        /// 类型
        /// </summary>
        [XmlAttribute("type")]
        public string type;

        /// <summary>
        /// 键
        /// </summary>
        [XmlAttribute("key")]
        public string key;

        /// <summary>
        /// 显示文本
        /// </summary>
        [XmlAttribute("text")]
        public string text;

        /// <summary>
        /// 价格文本
        /// </summary>
        [XmlAttribute("price")]
        public string price;

        /// <summary>
        /// 消耗金币
        /// </summary>
        [XmlAttribute("cost")]
        public int cost;

        /// <summary>
        /// 是否启用
        /// </summary>
        [XmlAttribute("active")]
        public bool active;

        public static List<shopdialog> LoadBytes()
        {
            TextAsset asset = Resources.Load<TextAsset>("DataTable/shopdialog");
            if (asset == null || asset.bytes == null || asset.bytes.Length == 0)
                return null;
            using (MemoryStream stream = new MemoryStream(asset.bytes))
            {
                BinaryFormatter binaryFormatter = new BinaryFormatter();
                allshopdialog table = binaryFormatter.Deserialize(stream) as allshopdialog;
                return table == null ? null : table.shopdialogs;
            }
        }
    }

    [Serializable]
    public class allshopdialog
    {
        public List<shopdialog> shopdialogs;
    }
}
