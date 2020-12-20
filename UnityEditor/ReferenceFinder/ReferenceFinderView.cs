using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.UI;
using UnityEngine;

namespace ReferenceFinder
{
    public  class ReferenceFinderView : EditorWindow
    {                
        //构造
        ReferenceFinderView()
        {
            this.titleContent = new UnityEngine.GUIContent(ReferenceFinderMode.PluginsName);
            ReferenceFinderController.Init();
        }

        private void OnDestroy()
        {
            ReferenceFinderController.Destroy();
            GC.Collect();
        }  

        //public delegate string Func<FileType,string>()
        //public delegate string RefreshInfoHandler(FileType _fileType, string _destinationDirectory);
        //public static event RefreshInfoHandler RefreshInfoEvent;

        //设置入口
        [MenuItem(ReferenceFinderMode.PluginsPath)]
        public static void ShowWindow()
        {
            var window = EditorWindow.GetWindow(typeof(ReferenceFinderView));
            window.position = new Rect(650, 150, 600, 600);            
        }

        private void OnGUI()
        {            
            ShowTitle();
            ShowInputPathTextArea();
            ShowEnumLabel();            
            ShowButtonGroup();
            ShowResultLabel();
            ShowResult();
            ShowDeleteButton();
        }
        #region 刷新相应部分功能
        private static void ShowTitle()
        {
            GUILayout.Label(ReferenceFinderMode.PluginsName, EditorStyles.whiteLargeLabel);
            GUILayout.Space(10);
        }

        private void ShowInputPathTextArea()
        {
            GUILayout.Label(ReferenceFinderMode.Label_InputPath, EditorStyles.boldLabel);
            ReferenceFinderMode.targetPath = GUILayout.TextField(ReferenceFinderMode.targetPath);
            GUILayout.Space(10);
        }

        private void ShowEnumLabel()
        {
            GUILayout.Label(ReferenceFinderMode.Label_SelecteEnumType, EditorStyles.boldLabel);
            ReferenceFinderMode.selected_type = (ReferenceFinderMode.FileType)EditorGUILayout.EnumPopup(ReferenceFinderMode.selected_type);
            GUILayout.Space(10);
        }

        private void ShowButtonGroup()
        {
            GUILayout.BeginHorizontal("HelpBox");
            if (GUILayout.Button(ReferenceFinderMode.ButtonName_1))
            {                
                ReferenceFinderController.FindUnReferenceFile();
                ReferenceFinderController.BuildUnReferenceFileDict();   
                //RefreshInfoEvent?.Invoke(selected_type, targetPath);
            }
            //Export TXT
            else if (GUILayout.Button(ReferenceFinderMode.ButtonName_2))
            {
                if (ReferenceFinderMode.stringContent == null)
                {
                    EditorUtility.DisplayDialog("提示","请先执行查找再执行导出TXT!","确认");                    
                    return;
                }
                ReferenceFinderController.ExportTXT();
            }
            //Read me
            else if (GUILayout.Button(ReferenceFinderMode.ButtonName_3))
            {
                ReferenceFinderController.GetReadmeInfo();
            }
            GUILayout.EndHorizontal();
        }        

        private void ShowResultLabel()
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(ReferenceFinderMode.Label_Result, EditorStyles.boldLabel);
            ReferenceFinderMode.selectedStateSign = GUILayout.Toggle(ReferenceFinderMode.selectedStateSign, "全选");
            if (ReferenceFinderMode.selectedStateSign == true && ReferenceFinderMode.selectedRealState == false)
            {
                foreach (var pathName in ReferenceFinderMode.toggleInfo.Keys.ToArray())
                {
                    if (pathName != "")
                    {
                        ReferenceFinderMode.toggleInfo[pathName] = true;
                    }
                }
                ReferenceFinderMode.selectedRealState = true;
            }
            else if (ReferenceFinderMode.selectedStateSign == false && ReferenceFinderMode.selectedRealState == true)
            {
                foreach (var pathName in ReferenceFinderMode.toggleInfo.Keys.ToArray())
                {
                    if (pathName != "")
                    {
                        ReferenceFinderMode.toggleInfo[pathName] = false;
                    }
                }
            }
            GUILayout.EndHorizontal();
        }

        private void ShowResult()
        {            
            ReferenceFinderMode.scollPos = EditorGUILayout.BeginScrollView(ReferenceFinderMode.scollPos);
            //显示传回字符串                  
            foreach (var pathName in ReferenceFinderMode.toggleInfo.Keys.ToArray())
            {
                if (pathName != "")
                {
                    bool lastState = ReferenceFinderMode.toggleInfo[pathName];
                    ReferenceFinderMode.toggleInfo[pathName] = EditorGUILayout.ToggleLeft(pathName, ReferenceFinderMode.toggleInfo[pathName]);
                    //控制勾选标记逻辑
                    if (ReferenceFinderMode.toggleInfo[pathName] == false )
                    {                       
                        if (ReferenceFinderMode.selectedRealState == true)
                        {
                            ReferenceFinderMode.selectedRealState = false;                            
                        }
                        ReferenceFinderMode.selectedStateSign = false;
                    }
                    if (lastState == false && ReferenceFinderMode.toggleInfo[pathName] == true)
                    {
                        Selection.activeObject = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(pathName);
                    }
                    //Debug.LogError("Selection:" ,Selection.activeContext);
                    //TODO 挨个勾选全部时全选自动被勾上
                }
            }
            EditorGUILayout.EndScrollView();
        }

        private void ShowDeleteButton()
        {
            if (GUILayout.Button(ReferenceFinderMode.ButtonName_4))
            {
                ReferenceFinderController.DeleteSelectedFiles();
            }
        }
        #endregion
    }
}
