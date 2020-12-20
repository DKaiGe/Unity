using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace ReferenceFinder
{
    internal static class ReferenceFinderController
    {               
        internal static void  Init()
        {
            //ReferenceFinderView.RefreshInfoEvent += FindUnReferenceFile;
        }

        internal static void Destroy()
        {
            //ReferenceFinderView.RefreshInfoEvent -= FindUnReferenceFile;
        }
        #region 逻辑功能函数
        internal static void FindUnReferenceFile()
        {
            ReferenceFinderMode.stringContent.Clear();
            DateTime time = DateTime.Now;
            HashSet<string> hash_TypeFile = new HashSet<string>();
            HashSet<string> hash_TargetPathFile = new HashSet<string>();            
            string[] typeFile_Guids = AssetDatabase.FindAssets(string.Format("t:{0}", ReferenceFinderMode.selected_type));
            Debug.LogWarning(string.Format("统计{0}类型文件在工程中总数量为{1}", ReferenceFinderMode.selected_type, typeFile_Guids.Length));

            //获取目标路径下所有依赖文件
            DirectoryInfo directoryInfo = new DirectoryInfo(Directory.GetParent(Application.dataPath).FullName + '/' + ReferenceFinderMode.targetPath);                                                            
            var targetPathFile_Paths = directoryInfo.GetFiles(ReferenceFinderMode.GetUseType(), SearchOption.AllDirectories);
            //进度条相关进度变量
            float nub = 0;
            float nubsum = typeFile_Guids.Length + targetPathFile_Paths.Length;
            //构建所有该类型的path集合
            foreach (var Guid in typeFile_Guids)
            {
                nub++;
                EditorUtility.DisplayProgressBar(string.Format("Build All {0} File paths index", ReferenceFinderMode.selected_type), Guid, nub/nubsum);
                hash_TypeFile.Add(AssetDatabase.GUIDToAssetPath(Guid));
            }
            if (targetPathFile_Paths != null)
            {
                foreach (var path in targetPathFile_Paths)
                {
                    nub++;
                    EditorUtility.DisplayProgressBar(string.Format("Get All {0} File Dependencies", ReferenceFinderMode.selected_type), path.ToString(), nub / nubsum);
                    foreach (var dp_path in AssetDatabase.GetDependencies(path.ToString().Substring(Directory.GetCurrentDirectory().Length + 1)))
                    {                       
                        if (hash_TypeFile.Contains(dp_path))
                        {
                            hash_TypeFile.Remove(dp_path);
                            break;
                        }
                    }                    
                }
            }
            EditorUtility.ClearProgressBar();
            Debug.LogWarning(string.Format("统计在路径{0}中{1}类型文件未被引用数量为{2}", ReferenceFinderMode.targetPath, ReferenceFinderMode.selected_type, hash_TypeFile.Count));
            foreach (var path in hash_TypeFile)
            {
                ReferenceFinderMode.stringContent.Append(path + "\n");
            }
            Debug.LogWarning(string.Format("TimeCost:{0}", (DateTime.Now - time)));
        }                

        internal static void ExportTXT()
        {                        
            string targetPath = Directory.GetCurrentDirectory() + "\\ReferenceLog";
            string TXTPath = targetPath + string.Format("\\UnReferenceFileLog_{0}.txt", DateTime.Today.ToString("yyyy-MM-dd"));
            FileStream fileStream;                     
            if (!Directory.Exists(targetPath))
            {
                Directory.CreateDirectory("ReferenceLog");
            }
            else
            {
                //判断日志文件是否存在
                if (!File.Exists(TXTPath))
                {
                    //创建
                    fileStream = new FileStream(TXTPath, FileMode.Create, FileAccess.Write);                    
                }
                else
                {
                    //重写
                    fileStream = new FileStream(TXTPath, FileMode.Open, FileAccess.Write);                    
                }
                StreamWriter streamWriter = new StreamWriter(fileStream);
                //日志格式化
                streamWriter.WriteLine(ReferenceFinderMode.selected_type.ToString() + " UnReference File:");
                streamWriter.Write(ReferenceFinderMode.stringContent);
                streamWriter.WriteLine();
                streamWriter.Close();
                fileStream.Close();
                System.Diagnostics.Process.Start("explorer.exe", TXTPath);
            }
        }

        internal static void DeleteSelectedFiles()
        {
            foreach (var path in ReferenceFinderMode.toggleInfo.Keys.ToArray())
            {
                if (ReferenceFinderMode.toggleInfo[path] == true)
                {
                    AssetDatabase.DeleteAsset(path);
                    Debug.LogWarning("DeletedAsset:" + path);
                }
            }
        }

        internal static void BuildUnReferenceFileDict()
        {
            ReferenceFinderMode.toggleInfo.Clear();
            foreach (string path in ReferenceFinderMode.stringContent.ToString().Split('\n'))
            {
                ReferenceFinderMode.toggleInfo.Add(path, false);
            }            
        }

        internal static void GetReadmeInfo()
        {
            #region readmeinfo
            const string readmeinfo = @"Reference Finder(v0.1)

工具用途:
    该工具设计目的是为了根据资源文件类型查找项目特定目录中未被引用的文件并根据选择是否进行清理.

工具使用说明:
    1.选中需要查找的目录，工具会对所设置的目录下所选择的资源文件类型的一级应用项文件索引其依赖项并在所有该资源类型文件目录中去重，比如若选择Shader，那么会记录所选择目录下的所有材质文件所依赖的Shader来与所有Shader文件目录进行去重操作，最后输出所有Shader集合中剩余的部分即是未被引用到的Shader文件。目前是设置的默认的资源路径""Assets/ArtEdit""，也可手动修改.
    2.选中欲过滤的资源文件类型，目前仅支持Shader.
    3.根据需求选择是否导出为TXT文件查看.
    4.根据需求选择是否对这些文件进行删除.(删除会同时删掉.meta文件)

工具作者:
    启明(如需扩展该工具请钉钉联系我).";
            #endregion
            EditorUtility.DisplayDialog("Read me", readmeinfo, "确定");
            //if (!File.Exists(Directory.GetCurrentDirectory() + @"\Assets\DevelopTools\Editor\ReferenceFinder\Readme.txt"))
            //{
            #region readmeinfo
//            const string readmeinfo = @"Reference Finder(v0.1)

//工具用途:
//                该工具设计目的是为了根据资源文件类型查找项目特定目录中未被引用的文件并根据选择是否进行清理。

//工具使用说明:
//                1.选中需要查找的目录，工具会对所设置的目录下所选择的资源文件类型的一级应用项文件索引其依赖项并在所有该资源类型文件目录中去重，比如若选择Shader，那么会记录所选择目录下的所有材质文件所依赖的Shader来与所有Shader文件目录进行去重操作，最后输出所有Shader集合中剩余的部分即是未被引用到的Shader文件。目前是设置的默认的资源路径""Assets/ArtEdit""，也可手动修改。
//                2.选中欲过滤的资源文件类型，目前仅支持Shader。	            
//	            3.根据需求选择是否导出为TXT文件查看。
//	            4.根据需求选择是否对这些文件进行删除。(删除会同时删掉.meta文件)

//工具作者:
//	启明(如需扩展该工具请钉钉联系我)。";
            #endregion
            //    using (StreamWriter sw = File.CreateText(Directory.GetCurrentDirectory() + @"\Assets\DevelopTools\Editor\ReferenceFinder\Readme.txt"))
            //    {                    
            //        sw.Write(readmeinfo);                    
            //    }                                              
            //}            
            //System.Diagnostics.Process.Start("explorer.exe", Directory.GetCurrentDirectory() + @"\Assets\DevelopTools\Editor\ReferenceFinder\Readme.txt");
        }
        #endregion
    }
}

