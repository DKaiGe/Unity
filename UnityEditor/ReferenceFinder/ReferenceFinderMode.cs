using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace ReferenceFinder
{
    static class ReferenceFinderMode
    {
         #region 初始化基本信息常量
        //插件名称
        internal const string PluginsName = "Reference Finder v0.1";
        //插件入口路径
        internal const string PluginsPath = "UGameTools/Utils/Reference Finder";
        //DIY化标签与按钮名
        internal const string Label_InputPath = "Input The Target Directory";
        internal const string Label_SelecteEnumType = "Select Reference File Type";
        internal const string Label_Result = "The following files are not referenced";
        internal const string ButtonName_1 = "Find UnReference File";
        internal const string ButtonName_2 = "Export TXT";
        internal const string ButtonName_3 = "Read me"; 
        internal const string ButtonName_4 = "Delete These File";
        #endregion

        #region 初始化Finder变量
        internal static bool selectedStateSign = false;
        internal static bool selectedRealState = false;
        internal static FileType selected_type;
        internal static Vector2 scollPos = Vector2.zero;

        internal static StringBuilder stringContent = new StringBuilder();
        internal static string targetPath = AssetCheckConstant.SEARCH_PATH_PREFAB;
        internal static Dictionary<string, bool> toggleInfo = new Dictionary<string, bool>();
        #endregion

        //可扩展枚举查找文件类型
        internal enum FileType
        {
            Shader,
        }

        internal static string GetUseType()
        {
            switch (selected_type)
            {
                case FileType.Shader:
                    return "*.mat";                    
                default:
                    return "*";
            }
        }
    }
}
