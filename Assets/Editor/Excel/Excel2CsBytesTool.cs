using System.IO;
using Excel;
using System.Data;
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using Table;
using System;
using System.Xml;
using System.Xml.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

/// <summary>
/// Excel生成bytes和cs工具
/// </summary>
public class Excel2CsBytesTool
{
    static string ExcelDataPath = Application.dataPath + "/../ExcelData";//源Excel文件夹,xlsx格式
    static string BytesDataPath = Application.dataPath + "/Resources/DataTable";//生成的bytes文件夹
    static string CsClassPath = Application.dataPath + "/Scripts/DataTable";//生成的c#脚本文件夹
    static string XmlDataPath = ExcelDataPath + "/tempXmlData";//生成的xml(临时)文件夹..
    static string AllCsHead = "all";//序列化结构体的数组类.类名前缀

    static char ArrayTypeSplitChar = '#';//数组类型值拆分符: int[] 1#2#34 string[] 你好#再见 bool[] true#false ...
    static bool IsDeleteXmlInFinish = true;//生成bytes后是否删除中间文件xml

    [MenuItem("SDGSupporter/Excel/Excel2Cs")]
    static void Excel2Cs()
    {
        Init();
        Excel2CsOrXml(true);
    }

    [MenuItem("SDGSupporter/Excel/Excel2Bytes")]
    static void Excel2Xml2Bytes()
    {
        Init();
        //生成中间文件xml
        Excel2CsOrXml(false);
        //生成bytes
        WriteBytes();
    }

    static void Init()
    {
        if (!Directory.Exists(CsClassPath))
        {
            Directory.CreateDirectory(CsClassPath);
        }
        if (!Directory.Exists(XmlDataPath))
        {
            Directory.CreateDirectory(XmlDataPath);
        }
        if (!Directory.Exists(BytesDataPath))
        {
            Directory.CreateDirectory(BytesDataPath);
        }
    }

    static void WriteCs(string className, string[] names, string[] types, string[] descs)
    {
        try
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("using System;");
            stringBuilder.AppendLine("using System.Collections.Generic;");
            stringBuilder.AppendLine("using System.IO;");
            stringBuilder.AppendLine("using System.Runtime.Serialization.Formatters.Binary;");
            stringBuilder.AppendLine("using System.Xml.Serialization;");
            stringBuilder.AppendLine("using UnityEngine;");
            stringBuilder.Append("\n");
            stringBuilder.AppendLine("namespace Table");
            stringBuilder.AppendLine("{");
            stringBuilder.AppendLine("    [Serializable]");
            stringBuilder.AppendLine("    public class " + className);
            stringBuilder.AppendLine("    {");
            for (int i = 0; i < names.Length; i++)
            {
                stringBuilder.AppendLine("        /// <summary>");
                stringBuilder.AppendLine("        /// " + descs[i]);
                stringBuilder.AppendLine("        /// </summary>");
                stringBuilder.AppendLine("        [XmlAttribute(\"" + names[i] + "\")]");

                string type = types[i];
                if (type.Contains("[]"))
                {
                    type = type.Replace("[]", "");
                    stringBuilder.AppendLine("        public List<" + type + "> " + names[i] + "");
                    stringBuilder.AppendLine("        {");
                    stringBuilder.AppendLine("            get");
                    stringBuilder.AppendLine("            {");
                    stringBuilder.AppendLine("                if (_" + names[i] + " != null)");
                    stringBuilder.AppendLine("                {");
                    stringBuilder.AppendLine("                    return _" + names[i] + ".item;");
                    stringBuilder.AppendLine("                }");
                    stringBuilder.AppendLine("                return null;");
                    stringBuilder.AppendLine("            }");
                    stringBuilder.AppendLine("        }");
                    stringBuilder.AppendLine("        [XmlElementAttribute(\"" + names[i] + "\")]");
                    stringBuilder.AppendLine("        public " + type + "Array _" + names[i] + ";");
                }
                else
                {
                    stringBuilder.AppendLine("        public " + type + " " + names[i] + ";");
                }

                stringBuilder.Append("\n");
            }
            stringBuilder.AppendLine("        public static List<" + className + "> LoadBytes()");
            stringBuilder.AppendLine("        {");
            stringBuilder.AppendLine("            TextAsset asset = Resources.Load<TextAsset>(\"DataTable/" + className + "\");");
            stringBuilder.AppendLine("            if (asset == null || asset.bytes == null || asset.bytes.Length == 0)");
            stringBuilder.AppendLine("                return null;");
            stringBuilder.AppendLine("            using (MemoryStream stream = new MemoryStream(asset.bytes))");
            stringBuilder.AppendLine("            {");
            stringBuilder.AppendLine("                BinaryFormatter binaryFormatter = new BinaryFormatter();");
            stringBuilder.AppendLine("                all" + className + " table = binaryFormatter.Deserialize(stream) as all" + className + ";");
            stringBuilder.AppendLine("                return table == null ? null : table." + className + "s;");
            stringBuilder.AppendLine("            }");
            stringBuilder.AppendLine("        }");
            stringBuilder.AppendLine("    }");
            stringBuilder.Append("\n");
            stringBuilder.AppendLine("    [Serializable]");
            stringBuilder.AppendLine("    public class " + AllCsHead + className);
            stringBuilder.AppendLine("    {");
            stringBuilder.AppendLine("        public List<" + className + "> " + className + "s;");
            stringBuilder.AppendLine("    }");
            stringBuilder.AppendLine("}");

            string csPath = CsClassPath + "/" + className + ".cs";
            if (File.Exists(csPath))
            {
                File.Delete(csPath);
            }
            using (StreamWriter sw = new StreamWriter(csPath))
            {
                sw.Write(stringBuilder);
                Debug.Log("生成:" + csPath);
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError("写入CS失败:" + e.Message);
            throw;
        }
    }

    static void WriteXml(string className, string[] names, string[] types, List<string[]> datasList)
    {
        try
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
            stringBuilder.AppendLine("<" + AllCsHead + className + ">");
            stringBuilder.AppendLine("<" + className + "s>");
            for (int d = 0; d < datasList.Count; d++)
            {
                stringBuilder.Append("\t<" + className + " ");
                //单行数据
                string[] datas = datasList[d];
                //填充属性节点
                for (int c = 0; c < datas.Length; c++)
                {
                    string type = types[c];
                    if (!type.Contains("[]"))
                    {
                        string name = names[c];
                        string value = datas[c];
                        stringBuilder.Append(name + "=\"" + value + "\"" + (c == datas.Length - 1 ? "" : " "));
                    }
                }
                stringBuilder.Append(">\n");
                //填充子元素节点(数组类型字段)
                for (int c = 0; c < datas.Length; c++)
                {
                    string type = types[c];
                    if (type.Contains("[]"))
                    {
                        string name = names[c];
                        string value = datas[c];
                        string[] values = value.Split(ArrayTypeSplitChar);
                        stringBuilder.AppendLine("\t\t<" + name + ">");
                        for (int v = 0; v < values.Length; v++)
                        {
                            stringBuilder.AppendLine("\t\t\t<item>" + values[v] + "</item>");
                        }
                        stringBuilder.AppendLine("\t\t</" + name + ">");
                    }
                }
                stringBuilder.AppendLine("\t</" + className + ">");
            }
            stringBuilder.AppendLine("</" + className + "s>");
            stringBuilder.AppendLine("</" + AllCsHead + className + ">");

            string xmlPath = XmlDataPath + "/" + className + ".xml";
            if (File.Exists(xmlPath))
            {
                File.Delete(xmlPath);
            }
            using (StreamWriter sw = new StreamWriter(xmlPath))
            {
                sw.Write(stringBuilder);
                Debug.Log("生成文件:" + xmlPath);
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError("写入Xml失败:" + e.Message);
        }
    }

    static void Excel2CsOrXml(bool isCs)
    {
        List<string> paths = new List<string>();
        paths.AddRange(Directory.GetFiles(ExcelDataPath, "*.xlsx"));
        paths.AddRange(Directory.GetFiles(ExcelDataPath, "*.csv"));
        for (int e = 0; e < paths.Count; e++)
        {
            string filePath = paths[e];
            string ext = Path.GetExtension(filePath).ToLower();
            string className = Path.GetFileNameWithoutExtension(filePath).ToLower();
            string[] names = null;
            string[] types = null;
            string[] descs = null;
            List<string[]> datasList = null;

            try
            {
                List<string[]> allRows = null;
                if (ext == ".csv")
                {
                    allRows = ReadCsv(filePath);
                    if (allRows != null && allRows.Count >= 3)
                        Debug.Log("从 CSV 读取表 " + className + "，共 " + allRows.Count + " 行");
                }
                else
                {
                    allRows = ReadXlsx(filePath);
                }


                names = allRows[0];
                types = allRows[1];
                descs = allRows[2];
                datasList = new List<string[]>();
                for (int r = 3; r < allRows.Count; r++)
                    datasList.Add(allRows[r]);
                if (datasList.Count == 0)
                    Debug.LogWarning("表 " + className + " 无数据行，将生成空数据。");

                if (names == null || names.Length == 0 || types == null || types.Length == 0)
                {
                    Debug.LogError("前两行（字段名、类型）无效: " + filePath);
                    continue;
                }
            }
            catch (System.Exception exc)
            {
                Debug.LogError("读取失败 [" + Path.GetFileName(filePath) + "]: " + exc.Message);
                continue;
            }

            if (isCs)
                WriteCs(className, names, types, descs);
            else
                WriteXml(className, names, types, datasList);
        }

        AssetDatabase.Refresh();
    }

    static List<string[]> ReadCsv(string csvPath)
    {
        List<string[]> rows = new List<string[]>();
        try
        {
            using (StreamReader sr = new StreamReader(csvPath, System.Text.Encoding.UTF8))
            {
                string line;
                int columns = 0;
                while ((line = sr.ReadLine()) != null)
                {
                    string[] cells = line.Split(',');
                    for (int i = 0; i < cells.Length; i++)
                        cells[i] = (cells[i] ?? "").Trim();
                    if (columns == 0 && cells.Length > 0) columns = cells.Length;
                    if (cells.Length < columns)
                    {
                        string[] full = new string[columns];
                        for (int i = 0; i < columns; i++)
                            full[i] = i < cells.Length ? cells[i] : "";
                        cells = full;
                    }
                    rows.Add(cells);
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError("ReadCsv 失败: " + e.Message);
            return null;
        }
        return rows;
    }

    static List<string[]> ReadXlsx(string excelPath)
    {
        List<string[]> allRows = new List<string[]>();
        int columns = 0;
        try
        {
            using (FileStream fileStream = File.Open(excelPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            using (IExcelDataReader reader = ExcelReaderFactory.CreateOpenXmlReader(fileStream))
            {
                do
                {
                    while (reader.Read())
                    {
                        int fieldCount = reader.FieldCount;
                        if (fieldCount <= 0) continue;
                        if (columns == 0) columns = fieldCount;
                        string[] row = new string[columns];
                        for (int c = 0; c < columns; c++)
                        {
                            object cellVal = c < fieldCount ? reader.GetValue(c) : null;
                            string value = (cellVal == null || cellVal == DBNull.Value) ? "" : cellVal.ToString();
                            if (value != null) value = value.Trim();
                            row[c] = value ?? "";
                        }
                        allRows.Add(row);
                    }
                } while (reader.NextResult());
            }
        }
        catch (Exception e)
        {
            Debug.LogError("ReadXlsx 失败: " + e.Message);
            return null;
        }
        return allRows;
    }

    static void WriteBytes()
    {
        string csAssemblyPath = Application.dataPath + "/../Library/ScriptAssemblies/Assembly-CSharp.dll";
        Assembly assembly = Assembly.LoadFile(csAssemblyPath);
        if (assembly != null)
        {
            Type[] types = assembly.GetTypes();
            for (int i = 0; i < types.Length; i++)
            {
                Type type = types[i];
                if (type.Namespace == "Table" && type.Name.Contains(AllCsHead))
                {
                    string className = type.Name.Replace(AllCsHead, "");

                    // 只处理本次导表生成了 Xml 的表；若某表在程序集中存在但 ExcelData 中无对应 xlsx，则跳过
                    string xmlPath = XmlDataPath + "/" + className + ".xml";
                    if (!File.Exists(xmlPath))
                    {
                        Debug.LogWarning("跳过表 " + className + "：未找到对应 Xml（若需导出该表，请在 ExcelData 中放入 " + className + ".xlsx 后重新执行 Excel2Bytes）");
                        continue;
                    }
                    object table;
                    using (Stream reader = new FileStream(xmlPath, FileMode.Open))
                    {
                        //读取xml实例化table: all+classname
                        //object table = assembly.CreateInstance("Table." + type.Name);
                        XmlSerializer xmlSerializer = new XmlSerializer(type);
                        table = xmlSerializer.Deserialize(reader);
                    }
                    //obj序列化二进制
                    string bytesPath = BytesDataPath + "/" + className + ".bytes";
                    if (File.Exists(bytesPath))
                    {
                        File.Delete(bytesPath);
                    }
                    using (FileStream fileStream = new FileStream(bytesPath, FileMode.Create))
                    {
                        BinaryFormatter binaryFormatter = new BinaryFormatter();
                        binaryFormatter.Serialize(fileStream, table);
                        Debug.Log("生成:" + bytesPath);
                    }

                    if (IsDeleteXmlInFinish)
                    {
                        File.Delete(xmlPath);
                        Debug.Log("删除临时Xml:" + xmlPath);
                    }
                }
            }
        }

    }
}
