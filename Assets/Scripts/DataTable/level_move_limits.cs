using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml.Serialization;
using UnityEngine;

namespace Table
{
    [Serializable]
    public class level_move_limits
    {
        /// <summary>
        /// Mode
        /// </summary>
        [XmlAttribute("mode")]
        public string mode;

        /// <summary>
        /// World index
        /// </summary>
        [XmlAttribute("world")]
        public int world;

        /// <summary>
        /// Level index
        /// </summary>
        [XmlAttribute("level")]
        public int level;

        /// <summary>
        /// Allowed player moves; 0 means unlimited
        /// </summary>
        [XmlAttribute("moveLimit")]
        public int moveLimit;

        public static List<level_move_limits> LoadBytes()
        {
            TextAsset asset = Resources.Load<TextAsset>("DataTable/level_move_limits");
            if (asset == null || asset.bytes == null || asset.bytes.Length == 0)
                return null;
            using (MemoryStream stream = new MemoryStream(asset.bytes))
            {
                BinaryFormatter binaryFormatter = new BinaryFormatter();
                alllevel_move_limits table = binaryFormatter.Deserialize(stream) as alllevel_move_limits;
                return table == null ? null : table.level_move_limitss;
            }
        }
    }

    [Serializable]
    public class alllevel_move_limits
    {
        public List<level_move_limits> level_move_limitss;
    }
}
