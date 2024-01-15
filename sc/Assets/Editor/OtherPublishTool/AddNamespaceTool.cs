using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Text;

namespace OtherPublishTools
{
    /// <summary>
    /// 给脚本添加和修改命名空间
    /// </summary>
    public class AddNamespaceTool : ScriptableWizard
    {

        public string folder = "Assets/";
        public string namespaceName;
        public SearchOption searchOption = SearchOption.TopDirectoryOnly;

        void OnEnable()
        {
            if (Selection.activeObject != null)
            {
                string dirPath = AssetDatabase.GetAssetOrScenePath(Selection.activeObject);
                if (File.Exists(dirPath))
                {
                    dirPath = dirPath.Substring(0, dirPath.LastIndexOf("/"));
                }

                folder = dirPath;
            }

            namespaceName = EditorPrefs.GetString("NameSpaceTool_Key", "");
        }

        [MenuItem("PublishTools/添加命名空间", false, 10)]
        static void CreateWizard()
        {
            AddNamespaceTool editor = ScriptableWizard.DisplayWizard<AddNamespaceTool>("Add Namespace", "Add");
            editor.minSize = new Vector2(300, 200);
        }

        public void OnWizardCreate()
        {
            //save settting

            if (!string.IsNullOrEmpty(folder) && !string.IsNullOrEmpty(namespaceName))
            {
                EditorPrefs.SetString("NameSpaceTool_Key", namespaceName);
                List<string> filesPaths = new List<string>();
                filesPaths.AddRange(
                    Directory.GetFiles(Path.GetFullPath(".") + Path.DirectorySeparatorChar + folder, "*.cs", searchOption)
                );
                Dictionary<string, bool> scripts = new Dictionary<string, bool>();

                int counter = -1;
                foreach (string filePath in filesPaths)
                {

                    scripts[filePath] = true;

                    EditorUtility.DisplayProgressBar("Add Namespace", filePath, counter / (float)filesPaths.Count);
                    counter++;

                    Encoding curEncoding = GetTextFileEncodingType(filePath);
                    string contents = File.ReadAllText(filePath, curEncoding);


                    string result = "";
                    bool havsNS = contents.Contains("namespace ");
                    Debug.Log("0000000   curEncoding=" + curEncoding.EncodingName + "   fileName=" + Path.GetFileName(filePath));
                    string t = havsNS ? "" : "\t";

                    using (TextReader reader = new StringReader(contents))
                    {
                        int index = 0;
                        bool addedNS = false;
                        while (reader.Peek() != -1)
                        {
                            string line = reader.ReadLine();

                            if (line.IndexOf("using") > -1 || line.Contains("#") || line.Contains("//<Tools"))
                            {
                                result += line + "\n";
                            }
                            else if (!addedNS && !havsNS)
                            {
                                result += "\nnamespace " + namespaceName + " {\r\n";
                                addedNS = true;
                                result += t + line + "\n";
                            }
                            else
                            {
                                if (havsNS && line.Contains("namespace "))
                                {
                                    if (line.Contains("{"))
                                    {
                                        result += "namespace " + namespaceName + " {\r\n";
                                    }
                                    else
                                    {
                                        result += "namespace " + namespaceName + "\r\n";
                                    }
                                }
                                else
                                {
                                    result += t + line + "\n";
                                }
                            }

                            ++index;
                        }

                        reader.Close();
                    }

                    if (!havsNS)
                    {
                        result += "}";
                    }

                    File.WriteAllText(filePath, result, curEncoding);
                }



                //处理加了命名空间后出现方法miss
                filesPaths.AddRange(
                    Directory.GetFiles(Path.GetFullPath(".") + Path.DirectorySeparatorChar + folder, "*.unnity",
                        SearchOption.AllDirectories)
                );
                filesPaths.AddRange(
                    Directory.GetFiles(Path.GetFullPath(".") + Path.DirectorySeparatorChar + folder, "*.prefab",
                        SearchOption.AllDirectories)
                );


                counter = -1;
                foreach (string filePath in filesPaths)
                {
                    EditorUtility.DisplayProgressBar("Modify Script Ref", filePath, counter / (float)filesPaths.Count);
                    counter++;
                    Encoding curEncoding = GetTextFileEncodingType(filePath);
                    Debug.Log("111111111   curEncoding=" + curEncoding.EncodingName + "   fileName=" + Path.GetFileName(filePath));
                    string contents = File.ReadAllText(filePath, curEncoding);

                    string result = "";
                    using (TextReader reader = new StringReader(contents))
                    {
                        int index = 0;
                        bool addedNS = false;
                        while (reader.Peek() != -1)
                        {
                            string line = reader.ReadLine();

                            if (line.IndexOf("m_ObjectArgumentAssemblyTypeName:") > -1 && !line.Contains(namespaceName))
                            {

                                string scriptName = line.Split(':')[1].Split(',')[0].Trim();
                                if (scripts.ContainsKey(scriptName))
                                {
                                    line = line.Replace(scriptName, "namespaceName." + scriptName);
                                }

                                result += line + "\n";
                            }
                            else
                            {
                                result += line + "\n";
                            }

                            ++index;
                        }

                        reader.Close();
                    }

                    File.WriteAllText(filePath, result, curEncoding);
                }


                EditorUtility.ClearProgressBar();
                AssetDatabase.Refresh();
            }
        }


        /// <summary>
        /// 获取文本文件的字符编码类型
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static Encoding GetTextFileEncodingType(string fileName)
        {
            Encoding encoding = Encoding.GetEncoding("GB2312");
            FileStream fileStream = new FileStream(fileName, FileMode.Open, FileAccess.Read);
            BinaryReader binaryReader = new BinaryReader(fileStream, encoding);
            byte[] buffer = binaryReader.ReadBytes((int)fileStream.Length);
            binaryReader.Close();
            fileStream.Close();
            if (buffer.Length >= 3 && buffer[0] == 239 && buffer[1] == 187 && buffer[2] == 191)
            {
                encoding = Encoding.UTF8;
            }
            else if (buffer.Length >= 3 && buffer[0] == 254 && buffer[1] == 255 && buffer[2] == 0)
            {
                encoding = Encoding.BigEndianUnicode;
            }
            else if (buffer.Length >= 3 && buffer[0] == 255 && buffer[1] == 254 && buffer[2] == 65)
            {
                encoding = Encoding.Unicode;
            }
            else if (IsUTF8Bytes(buffer))
            {
                encoding = Encoding.UTF8;
            }

            return encoding;
        }

        /// <summary>
        /// 判断是否是不带 BOM 的 UTF8 格式
        /// BOM（Byte Order Mark），字节顺序标记，出现在文本文件头部，Unicode编码标准中用于标识文件是采用哪种格式的编码。
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private static bool IsUTF8Bytes(byte[] data)
        {
            int charByteCounter = 1; //计算当前正分析的字符应还有的字节数 
            byte curByte; //当前分析的字节. 
            for (int i = 0; i < data.Length; i++)
            {
                curByte = data[i];
                if (charByteCounter == 1)
                {
                    if (curByte >= 0x80)
                    {
                        //判断当前 
                        while (((curByte <<= 1) & 0x80) != 0)
                        {
                            charByteCounter++;
                        }

                        //标记位首位若为非0 则至少以2个1开始 如:110XXXXX...........1111110X 
                        if (charByteCounter == 1 || charByteCounter > 6)
                        {
                            return false;
                        }
                    }
                }
                else
                {
                    //若是UTF-8 此时第一位必须为1 
                    if ((curByte & 0xC0) != 0x80)
                    {
                        return false;
                    }

                    charByteCounter--;
                }
            }

            if (charByteCounter > 1)
            {
                throw new Exception("非预期的byte格式");
            }

            return true;
        }
    }
}

