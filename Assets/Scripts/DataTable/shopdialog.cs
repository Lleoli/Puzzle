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
        /// ����
        /// </summary>
        [XmlAttribute("type")]
        public string type;

        /// <summary>
        /// ��
        /// </summary>
        [XmlAttribute("key")]
        public string key;

        /// <summary>
        /// ��ʾ�ı�
        /// </summary>
        [XmlAttribute("text")]
        public string text;

        /// <summary>
        /// �۸��ı�
        /// </summary>
        [XmlAttribute("price")]
        public string price;

        /// <summary>
        /// ��������
        /// </summary>
        [XmlAttribute("cost")]
        public int cost;

        /// <summary>
        /// �Ƿ�����
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
