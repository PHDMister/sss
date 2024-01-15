using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Data;
using Excel;
using System;
using System.Text;

public class ToolExcel
{
    ///<summary
    ///excel文件存放路径
    ///</summary>
    public static string EXCEL_PATH = Application.dataPath + "/Config/Excel/";

    ///<summary>
    ///数据结构类脚本存储位置
    ///</summary>
    public static string DATA_CLASS_PATH = Application.dataPath + "/Scripts/ExcelData/DataClass/";

    ///<summary>
    ///容器类脚本存储位置
    ///</summary>
    public static string DATA_CONTAINER_PATH = Application.dataPath + "/Scripts/ExcelData/Container/";

    [MenuItem("GameTool/GenerateExcel")]
    private static void GenerateExcelInfo()
    {
        //在指定路径中的所有Excel文件用于生成对应的3个文件
        DirectoryInfo dInfo = Directory.CreateDirectory(EXCEL_PATH);
        //得到指定目录的所有文件信息
        FileInfo[] files = dInfo.GetFiles();
        //数据表容器
        DataTableCollection tableCollection;
        for (int i = 0; i < files.Length; i++)
        {
            if (files[i].Extension != ".xlsx" && files[i].Extension != ".xls")
                continue;
            using (FileStream fs = files[i].Open(FileMode.Open, FileAccess.Read))
            {
                IExcelDataReader excelDataReader = ExcelReaderFactory.CreateOpenXmlReader(fs);
                tableCollection = excelDataReader.AsDataSet().Tables;
                fs.Close();
            }
            //遍历文件中所有表信息
            foreach (DataTable table in tableCollection)
            {
                //生成数据结构类
                GenerateExcelDataClass(table);
                //生成容器类
                GenerateExcelContainer(table);
                //生成二进制数据
                GenerateExcelBinary(table);
            }
        }
    }
    /// <summary>
    /// 获取变量名所在行
    /// </summary>
    /// <param name="table"></param>
    /// <returns></returns>
    private static DataRow GetVariableNameRow(DataTable table)
    {
        return table.Rows[1];
    }

    /// <summary>
    /// 获取变量类型所在行
    /// </summary>
    /// <param name="table"></param>
    /// <returns></returns>
    private static DataRow GetVariableTypeRow(DataTable table)
    {
        return table.Rows[2];
    }

    /// <summary>
    /// 生成Excel对应的数据结构类
    /// </summary>
    /// <param name="table"></param>
    private static void GenerateExcelDataClass(DataTable table)
    {
        DataRow rowName = GetVariableNameRow(table);

        DataRow rowType = GetVariableTypeRow(table);

        if ((!Directory.Exists(DATA_CLASS_PATH)))
            Directory.CreateDirectory(DATA_CLASS_PATH);
        string str = "public class " + table.TableName + "\n{\n";
        //变量进行字符串拼接
        for (int i = 0; i < table.Columns.Count; i++)
        {
            str += " public " + rowType[i].ToString() + " " + rowName[i].ToString() + ";\n";
        }
        str += "}";
        //拼接好的字符串存到指定文件中去
        File.WriteAllText(DATA_CLASS_PATH + table.TableName + ".cs", str);

        AssetDatabase.Refresh();
    }

    private static int GetKeyIndex(DataTable table)
    {
        DataRow row = table.Rows[3];
        for (int i = 0; i < table.Columns.Count; i++)
        {
            if (row[i].ToString() == "key")
                return i;
        }
        return 0;
    }
    /// <summary>
    /// 生成Excel表对应的数据容器类
    /// </summary>
    /// <param name="table"></param>
    private static void GenerateExcelContainer(DataTable table)
    {
        int keyIndex = GetKeyIndex(table);
        DataRow rowType = GetVariableTypeRow(table);
        if (!Directory.Exists(DATA_CONTAINER_PATH))
            Directory.CreateDirectory(DATA_CONTAINER_PATH);
        string str = "using System.Collections.Generic;\n";
        str += "public class " + table.TableName + "Container" + "\n{\n";
        str += "    ";
        str += "public Dictionary<" + rowType[keyIndex].ToString() + "," + table.TableName + ">";
        str += " dataDic = new " + "Dictionary<" + rowType[keyIndex].ToString() + "," + table.TableName + ">();\n";
        str += "}";
        File.WriteAllText(DATA_CONTAINER_PATH + "/" + table.TableName + "Container.cs", str);
        AssetDatabase.Refresh();
    }
    /// <summary>
    /// 生成Excel二进制数据
    /// </summary>
    /// <param name="table"></param>
    private static void GenerateExcelBinary(DataTable table)
    {
        if (!Directory.Exists(BinaryDataMgr.DATA_BINARY_PATH))
            Directory.CreateDirectory(BinaryDataMgr.DATA_BINARY_PATH);
        string tabName = table.TableName;
        using (FileStream fs = new FileStream(BinaryDataMgr.DATA_BINARY_PATH + table.TableName + ".txt", FileMode.OpenOrCreate, FileAccess.Write))
        {
            //fs.Write(BitConverter.GetBytes(table.Rows.Count - 4), 0, 4);
            int rowCount = GetRealRowCount(table);
            //Debug.Log(table.TableName + "实际行数：" + rowCount);
            fs.Write(BitConverter.GetBytes(rowCount - 4), 0, 4);
            //存储主键变量名
            string keyName = GetVariableNameRow(table)[GetKeyIndex(table)].ToString();
            byte[] bytes = Encoding.UTF8.GetBytes(keyName);
            //存储字符串字节组长度
            fs.Write(BitConverter.GetBytes(bytes.Length), 0, 4);
            fs.Write(bytes, 0, bytes.Length);
            DataRow row;
            DataRow rowType = GetVariableTypeRow(table);
            int BINGEN_INDEX = 4;
            for (int i = BINGEN_INDEX; i < table.Rows.Count; i++)
            {
                row = table.Rows[i];
                int columnNullNum = 0;
                for (int j = 0; j < table.Columns.Count; j++)
                {
                    if (row.IsNull(j))
                    {
                        columnNullNum++;
                    }
                }
                if (columnNullNum == table.Columns.Count)//大于实际行不做处理
                {
                    break;
                }
                for (int j = 0; j < table.Columns.Count; j++)
                {
                    Debug.Log("table.TableName:     " + table.TableName + " row[" + i + "]" + " column[" + j + "] = " + row[j].ToString());
                    switch (rowType[j].ToString())
                    {
                        case "int":
                            fs.Write(BitConverter.GetBytes(int.Parse(row[j].ToString())), 0, 4);
                            break;
                        case "float":
                            fs.Write(BitConverter.GetBytes(float.Parse(row[j].ToString())), 0, 4);
                            break;
                        case "bool":
                            fs.Write(BitConverter.GetBytes(int.Parse(row[j].ToString())), 0, 1);
                            break;
                        case "string":
                            bytes = Encoding.UTF8.GetBytes(row[j].ToString());
                            fs.Write(BitConverter.GetBytes(bytes.Length), 0, 4);
                            fs.Write(bytes, 0, bytes.Length);
                            break;
                    }
                }
            }
            fs.Close();
        }
        AssetDatabase.Refresh();
    }

    public static int GetRealRowCount(DataTable table)
    {
        int rowNum = table.Rows.Count;
        for (int i = 0; i < table.Rows.Count; i++)
        {
            DataRow row = table.Rows[i];
            int columnNullNum = 0;
            for (int j = 0; j < table.Columns.Count; j++)
            {
                if (row.IsNull(j))
                {
                    columnNullNum++;
                }
            }
            if (columnNullNum == table.Columns.Count)//只要一行不为空就计数
            {
                rowNum--;
            }
        }
        return rowNum;
    }
}
