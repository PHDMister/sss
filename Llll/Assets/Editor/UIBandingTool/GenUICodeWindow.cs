using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
//using UIFW;
using UnityEngine;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEditorInternal;
using UnityEngine.Experimental.GlobalIllumination;

//生成代码的模版
public class GenUICodeTemp
{
    /// <summary>
    /// 需要注册事件的UI控件类型
    /// </summary>
    public enum EventWidgetType
    {
        Button,
        Toggle,
        Slider,
        InputField,
        ScrollRect,
        Scrollbar,
        Dropdown,
    }

    public static Dictionary<string, string> eventCBParamDic = new Dictionary<string, string> {
            { "Toggle", "bool" },
            { "Slider", "float" },
            { "InputField", "string" },
            { "ScrollRect", "Vector2" },
            { "Scrollbar", "float" },
            { "Dropdown", "int" },
        };

    #region cs代码格式
    public const string regionStartFmt = "\n\t// {0} START\n";
    public const string regionEnd = "\t// {0} END\n";

    public static string statementRegion = string.Format(regionStartFmt, "UI VARIABLE STATEMENT");
    public static string statementRegionEnd = string.Format(regionEnd, "UI VARIABLE STATEMENT");
    public static string eventRegion = string.Format(regionStartFmt, "UI EVENT REGISTER");
    public static string eventRegionEnd = string.Format(regionEnd, "UI EVENT REGISTER");
    public static string eventFuncRegion = string.Format(regionStartFmt, "UI EVENT FUNC");
    public static string eventFuncRegionEnd = string.Format(regionEnd, "UI EVENT FUNC");
    public static string assignRegion = string.Format(regionStartFmt, "UI VARIABLE ASSIGNMENT");
    public static string assignRegionEnd = string.Format(regionEnd, "UI VARIABLE ASSIGNMENT");

    public const string methodStartFmt = "\tprivate void {0}() \n\t{{\n";   //'{'要转义
    public const string methodOverroidStartFmt = "\tpublic override void {0}() \n\t{{\n";
    public const string methodCallbackFmt = "\t\tbase.{0}();";
    public const string methodEnd = "\n\t}\n";

    public const string codeAnnotation = @"//<Tools\GenUICode>工具生成, UI变化重新生成";
    public const string usingNamespace = "\nusing UnityEngine;\nusing System.Collections;\nusing UnityEngine.UI;\nusing UIFW;\n";
    public const string classMonoStart = "\npublic class {0} : BaseUIForm\n{{\n";
    public const string classStart = "\npublic class {0}\n{{\n";
    public const string classEnd = "\n}\n";
    public const string methodAnnotation = "\n\t/// <summary>\n\t/// {0}\n\t/// </summary>\n";

    #region 序列化初始化代码格式
    //控件遍历声明,0:类型 1:名称
    public const string serilStateCodeFmt = "\tprivate {0} {1}; \n";

    //public const string onClickSerilCode = "\t\t{0}.onClick.AddListener(On{1}Clicked); \n";
    public const string onClickSerilCode = "\t\tRigisterCompEvent({0}, On{1}Clicked);\n";
    public const string onValueChangeSerilCode = "\n\t\t{0}.onValueChanged.AddListener(On{1}ValueChanged);\n";

    public const string btnCallbackSerilCode = "\tprivate void On{0}Clicked(GameObject go)\n\t{{\n\n\t}}\n";
    public const string eventCallbackSerilCode = "\tprivate void On{0}ValueChanged({1} arg)\n\t{{\n\n\t}}\n";
    #endregion

    #region 控件查找赋值格式
    //public const string assignCodeFmt = "\t\t{0} = transform.Find(\"{1}\").GetComponent<{2}>(); \n";
    public const string assignCodeFmt = "\t\t{0} = FindComp<{2}>(\"{1}\"); \n";
    public const string assignGameObjectCodeFmt = "\t\t{0} = transform.Find(\"{1}\").gameObject; \n";
    public const string addEventCodeFmt = "\t\tAddEvent(); \n";
    //根物体上挂载的控件
    public const string assignRootCodeFmt = "\t\t{0} = transform.GetComponent<{1}>(); \n";
    #endregion

    #region 查找初始化代码格式
    public const string stateTransform = "\tprivate Transform transform; \n";
    public const string stateCodeFmt = "\tprivate {0} {1}; \n";
    public const string assignTransform = "\t\t//assign transform by your ui framework\n\t\t//transform = ; \n";
    #endregion

    #endregion
}

public class UIGameObjectRef : ScriptableObject
{
    //ui游戏对象列表
    public List<GameObject> uiObjects = new List<GameObject>();
}

//生成UI代码的窗口
public class GenUICodeWindow : EditorWindow
{
    [MenuItem("Tools/GenUICode")]
    public static void OpenWindow()
    {
        if (codeWindow == null)
            codeWindow = EditorWindow.GetWindow(typeof(GenUICodeWindow)) as GenUICodeWindow;

        codeWindow.titleContent = new GUIContent("GenUICode");
        codeWindow.minSize = new Vector2(500, 300);
        codeWindow.Show();

    }

    private static GenUICodeWindow codeWindow = null;
    private SerializedObject serializedObj;

    //选择的根游戏体
    private GameObject root;
    //ui控件列表
    private List<UIBehaviour> uiWidgets = new List<UIBehaviour>();
    //ui游戏对象列表
    private List<GameObject> uiObjects = new List<GameObject>();
    //视图宽度一半
    private float halfViewWidth;
    //视图高度一半
    private float halfViewHeight;

    private Vector2 scrollWidgetPos;
    private Vector2 scrollObjectPos;
    private Vector2 scrollTextPos;

    private int selectedBar = 0;
    private bool isMono = true;
    private bool NeedSafeAreaPanel = true;

    #region 代码生成
    private StringBuilder codeStateText;
    private StringBuilder codeEventText;
    private StringBuilder codeEventFuncText;
    private StringBuilder codeAssignText;
    private StringBuilder codeAllText;

    //缓存所有变量名和对应控件对象，对重名作处理
    private Dictionary<string, object> variableNameDic = new Dictionary<string, object>();
    //变量编号
    private int variableNum;
    //需要注册事件的控件,可通过toggle选择
    private Dictionary<string, bool> selectedEventWidgets = new Dictionary<string, bool>();
    //UI 类名
    private string className;
    //生成脚本的类型
    private Type scriptType;
    //输出文件的目录
    private string outFilePath;


    private ReorderableList reorderableList;
    //private ReorderableList reorderableList1;

    private UIGameObjectRef sObjectRef;
    private SerializedObject serObjectRef;
    private SerializedProperty serObjectRefProp;
    #endregion

    #region 代码格式分类
    private string regionStartFmt { get { return selectedBar == 0 ? GenUICodeTemp.regionStartFmt : ""; } }
    private string regionEnd { get { return selectedBar == 0 ? GenUICodeTemp.regionEnd : ""; } }
    private string statementRegion { get { return GenUICodeTemp.statementRegion; } }
    private string statementRegionEnd { get { return GenUICodeTemp.statementRegionEnd; } }
    private string eventRegion { get { return selectedBar == 0 ? GenUICodeTemp.eventRegion : ""; } }
    private string eventRegionEnd { get { return selectedBar == 0 ? GenUICodeTemp.eventRegionEnd : ""; } }
    private string eventFuncRegion { get { return selectedBar == 0 ? GenUICodeTemp.eventFuncRegion : ""; } }
    private string eventFuncRegionEnd { get { return selectedBar == 0 ? GenUICodeTemp.eventFuncRegionEnd : ""; } }
    private string assignRegion { get { return selectedBar == 0 ? GenUICodeTemp.assignRegion : ""; } }
    private string assignRegionEnd { get { return selectedBar == 0 ? GenUICodeTemp.assignRegionEnd : ""; } }
    private string methodStartFmt { get { return selectedBar == 0 ? GenUICodeTemp.methodStartFmt : ""; } }
    private string methodEnd { get { return selectedBar == 0 ? GenUICodeTemp.methodEnd : ""; } }
    private string assignCodeFmt { get { return selectedBar == 0 ? GenUICodeTemp.assignCodeFmt : ""; } }
    private string assignGameObjectCodeFmt { get { return selectedBar == 0 ? GenUICodeTemp.assignGameObjectCodeFmt : ""; } }
    private string assignRootCodeFmt { get { return selectedBar == 0 ? GenUICodeTemp.assignRootCodeFmt : ""; } }
    private string onClickSerilCode { get { return selectedBar == 0 ? GenUICodeTemp.onClickSerilCode : ""; } }
    private string onValueChangeSerilCode { get { return selectedBar == 0 ? GenUICodeTemp.onValueChangeSerilCode : ""; } }
    private string btnCallbackSerilCode { get { return selectedBar == 0 ? GenUICodeTemp.btnCallbackSerilCode : ""; } }
    private string eventCallbackSerilCode { get { return selectedBar == 0 ? GenUICodeTemp.eventCallbackSerilCode : ""; } }

    #endregion

    void OnEnable()
    {
        serializedObj = new SerializedObject(this);
        uiWidgets.Clear();
        reorderableList = new ReorderableList(uiWidgets, typeof(UIBehaviour), true, true, true, true);
        reorderableList.drawElementCallback = (rect, index, active, focused) =>
        {
            GUI.Label(rect, uiWidgets[index].name + "  [" + uiWidgets[index].GetType() + "]");
            if (GUI.Button(new Rect(rect.x + rect.width - 40, rect.y, 40, rect.height), "X"))
            {
                uiWidgets.RemoveAt(index);
            }
            if (GUI.Button(new Rect(rect.x + rect.width - 80, rect.y, 40, rect.height), "↓"))
            {
                uiObjects.Add(uiWidgets[index].gameObject);
                uiWidgets.RemoveAt(index);
            }
        };
        reorderableList.onSelectCallback = list =>
        {
            Selection.activeTransform = uiWidgets[list.index].transform;
        };
        reorderableList.onRemoveCallback = list =>
         {
             Selection.activeTransform = uiWidgets[list.index].transform;
         };


        //reorderableList1 = new ReorderableList(uiObjects, typeof(GameObject), true, true, true, true);
        //reorderableList1.drawElementCallback = (rect, index, active, focused) =>
        //{
        //    GUI.Label(rect, uiObjects[index].name + " [GameObject]");
        //    if (GUI.Button(new Rect(rect.x + rect.width - 40, rect.y, 40, rect.height), "X"))
        //    {
        //        uiObjects.RemoveAt(index);
        //    }
        //};

        sObjectRef = new UIGameObjectRef();
        serObjectRef = new SerializedObject(sObjectRef);
        serObjectRefProp = serObjectRef.FindProperty("uiObjects");
    }

    void OnGUI()
    {
        serializedObj.Update();

        if (codeWindow == null)
        {
            codeWindow = GetWindow<GenUICodeWindow>();
        }
        halfViewWidth = EditorGUIUtility.currentViewWidth / 2f;
        halfViewHeight = codeWindow.position.height / 2f;

        using (new EditorGUILayout.HorizontalScope())
        {
            //左半部分
            using (EditorGUILayout.VerticalScope vScope = new EditorGUILayout.VerticalScope(GUILayout.Width(EditorGUIUtility.currentViewWidth * 0.5f)))
            {
                GUI.backgroundColor = Color.white;
                Rect rect = vScope.rect;
                rect.height = codeWindow.position.height;
                GUI.Box(rect, "");

                DrawSelectUI();
                DrawFindWidget();
                DrawWidgetList();
                DrawCustomObjectList();
            }
            //右半部分
            using (new EditorGUILayout.VerticalScope(GUILayout.Width(EditorGUIUtility.currentViewWidth * 0.5f)))
            {
                DrawCodeGenTitle();
                DrawCodeGenToolBar();
            }
        }

        serializedObj.ApplyModifiedProperties();
    }

    /// <summary>
    /// 绘制 选择要分析的UI
    /// </summary>
    private void DrawSelectUI()
    {
        EditorGUILayout.Space();
        using (EditorGUILayout.HorizontalScope hScope = new EditorGUILayout.HorizontalScope())
        {
            GUI.backgroundColor = Color.white;
            Rect rect = hScope.rect;
            rect.height = EditorGUIUtility.singleLineHeight;
            GUI.Box(rect, "");

            EditorGUILayout.LabelField("选择待处理UI:", GUILayout.Width(halfViewWidth / 4f));
            GameObject lastRoot = root;
            root = EditorGUILayout.ObjectField(root, typeof(GameObject), true) as GameObject;

            if (lastRoot != null && lastRoot != root)
            {
                uiWidgets.Clear();
                uiObjects.Clear();
            }
        }
    }

    /// <summary>
    /// 绘制 查找UI控件
    /// </summary>
    private void DrawFindWidget()
    {
        EditorGUILayout.Space();
        using (EditorGUILayout.HorizontalScope hScope = new EditorGUILayout.HorizontalScope())
        {
            GUI.backgroundColor = Color.white;
            Rect rect = hScope.rect;
            rect.height = EditorGUIUtility.singleLineHeight;
            GUI.Box(rect, "");

            if (GUILayout.Button("查找UI控件"))
            {
                if (root == null)
                {
                    Debug.LogWarning("请先选择一个UI物体!");
                    return;
                }

                RecursiveUI(root.transform, (tran) =>
                {
                    UIBehaviour[] widgets = tran.GetComponents<UIBehaviour>();
                    for (int i = 0; i < widgets.Length; i++)
                    {
                        var widget = widgets[i];
                        if (widget != null && !uiWidgets.Contains(widget))
                        {
                            uiWidgets.Add(widget);
                        }
                    }
                });
            }
            if (GUILayout.Button("只查找操作控件"))
            {
                if (root == null)
                {
                    Debug.LogWarning("请先选择一个UI物体!");
                    return;
                }

                uiWidgets.Clear();
                RecursiveUI(root.transform, (tran) =>
                {
                    UIBehaviour[] widgets = tran.GetComponents<UIBehaviour>();
                    for (int i = 0; i < widgets.Length; i++)
                    {
                        var widget = widgets[i];
                        if (widget != null && !uiWidgets.Contains(widget) && (
                                widget is UnityEngine.UI.Text ||
                                widget is UnityEngine.UI.Button ||
                                widget is UnityEngine.UI.Toggle ||
                                widget is UnityEngine.UI.ToggleGroup ||
                                widget is UnityEngine.UI.Slider ||
                                widget is UnityEngine.UI.InputField ||
                                widget is UnityEngine.UI.ScrollRect ||
                                widget is UnityEngine.UI.Scrollbar ||
                                widget is UnityEngine.UI.Dropdown ||
                                widget is UnityEngine.UI.RawImage

                            ))
                        {
                            uiWidgets.Add(widget);
                        }
                    }
                    if (tran.name.Contains("trans_"))
                    {
                        uiObjects.Add(tran.gameObject);
                    }
                });
            }
            if (GUILayout.Button("清除控件"))
            {
                uiWidgets.Clear();
            }
            if (GUILayout.Button("清除其他"))
            {
                uiObjects.Clear();
            }
        }
    }

    /// <summary>
    /// 绘制 控件列表
    /// </summary>
    private void DrawWidgetList()
    {
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("UI控件");
        scrollWidgetPos = EditorGUILayout.BeginScrollView(scrollWidgetPos);
        reorderableList.displayRemove = true;
        reorderableList.displayAdd = false;
        reorderableList.DoLayoutList();
        EditorGUILayout.EndScrollView();
    }

    /// <summary>
    /// 绘制 其他ui gameobject,比如某些节点要控制下层的隐藏显示
    /// </summary>
    private void DrawCustomObjectList()
    {
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("其他UI对象");
        scrollObjectPos = EditorGUILayout.BeginScrollView(scrollObjectPos);
        //reorderableList1.DoLayoutList();
        EditorGUILayout.PropertyField(serObjectRefProp);
        serObjectRef.ApplyModifiedProperties();
        uiObjects.Clear();
        uiObjects.AddRange(sObjectRef.uiObjects);
        EditorGUILayout.EndScrollView();
    }

    private void DrawCodeGenTitle()
    {
        EditorGUILayout.Space();
        using (var hScope = new EditorGUILayout.HorizontalScope(GUILayout.Height(EditorGUIUtility.singleLineHeight)))
        {
            GUI.backgroundColor = Color.white;
            Rect rect = hScope.rect;
            GUI.Box(rect, "");

            EditorGUILayout.LabelField("代码生成:");
        }
    }

    private void DrawCodeGenToolBar()
    {
        EditorGUILayout.Space();

        //selectedBar = GUILayout.Toolbar(selectedBar, new string[] { "C#", "Lua" });

        //switch (selectedBar)
        //{
        //    case 0:
        //        DrawCsPage();
        //        break;
        //    case 1:
        //        DrawLuaPage();
        //        break;
        //    default:
        //        break;
        //}


        DrawCsPage();
    }

    private void DrawCsPage()
    {
        EditorGUILayout.Space();
        isMono = GUILayout.Toggle(isMono, "继承BaseUIForm");
        EditorGUILayout.Space();
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("变量声明", GUILayout.Width(halfViewWidth / 3f)))
        {
            BuildStatementCode();
        }
        if (GUILayout.Button("查找赋值", GUILayout.Width(halfViewWidth / 3f)))
        {
            BuildAssignmentCode();
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space();
        using (EditorGUILayout.VerticalScope vScope = new EditorGUILayout.VerticalScope())
        {
            GUI.backgroundColor = Color.white;
            GUI.Box(vScope.rect, "");

            EditorGUILayout.LabelField("选择需要注册事件回调的控件:");
            DrawEventWidget();

            EditorGUILayout.Space();
            if (GUILayout.Button("注册事件", GUILayout.Width(halfViewWidth / 3f)))
            {
                BuildEventCode();
                BuildEventFuncCode();
            }
            if (GUILayout.Button("变量声明赋值事件注册一键完成", GUILayout.Width(halfViewWidth / 3f)))
            {
                BuildStatementCode();
                BuildAssignmentCode();
                BuildEventCode();
                BuildEventFuncCode();
            }


        }

        EditorGUILayout.Space();
        using (EditorGUILayout.HorizontalScope hScope = new EditorGUILayout.HorizontalScope())
        {
            if (GUILayout.Button("复制代码"))
            {
                TextEditor p = new TextEditor();
                codeAllText = new StringBuilder(codeStateText.ToString());
                codeAllText.Append(codeAssignText);
                codeAllText.Append(codeEventText);
                codeAllText.Append(BuildEventFuncCode());
                p.text = codeAllText.ToString();
                p.OnFocus();
                p.Copy();

                EditorUtility.DisplayDialog("提示", "代码复制成功", "OK");
            }
            if (GUILayout.Button("生成脚本"))
            {
                CreateCsUIScript();
            }
        }

        EditorGUILayout.Space();
        using (EditorGUILayout.HorizontalScope hScope = new EditorGUILayout.HorizontalScope())
        {
            if (isMono)
            {
                NeedSafeAreaPanel = GUILayout.Toggle(NeedSafeAreaPanel, "添加SafeAreaPanel");

                if (GUILayout.Button("挂载脚本组件"))
                {
                    AddScriptComponent();
                }
                //if (GUILayout.Button("绑定UI(无需查找赋值)"))
                //{
                //    BindSerializeWidget();
                //}
            }
        }

        EditorGUILayout.Space();
        using (EditorGUILayout.HorizontalScope hScope = new EditorGUILayout.HorizontalScope())
        {
            if (string.IsNullOrEmpty(outFilePath))
            {
                outFilePath = EditorPrefs.GetString("GenUICode_exportPath", "");
            }

            outFilePath = GUILayout.TextField(outFilePath);
            if (GUILayout.Button("选择导出的目录"))
            {
                if (string.IsNullOrEmpty(outFilePath)) outFilePath = Application.dataPath;
                outFilePath = EditorUtility.OpenFolderPanel("选择导出目录", outFilePath, outFilePath);
                EditorPrefs.SetString("GenUICode_exportPath", outFilePath);
            }
            //检查脚本挂载脚本
            if (GUILayout.Button("导出prefab&form配置"))
            {
                //导出prefab
                string path = outFilePath + "/" + root.name + ".prefab";
                bool isExists = File.Exists(path);
                if (!isExists) PrefabUtility.SaveAsPrefabAsset(root, outFilePath + "/" + root.name + ".prefab");
                else PrefabUtility.ApplyObjectOverride(root, path, InteractionMode.UserAction);



            }
        }
        DrawPreviewText();
    }

    /// <summary>
    /// 遍历UI
    /// </summary>
    /// <param name="parent">父节点</param>
    /// <param name="callback">回调</param>
    public void RecursiveUI(Transform parent, UnityAction<Transform> callback)
    {
        if (callback != null)
            callback(parent);

        if (parent.childCount >= 0)
        {
            for (int i = 0; i < parent.childCount; i++)
            {
                Transform child = parent.GetChild(i);

                RecursiveUI(child, callback);
            }
        }
    }

    private string BuildStatementCode()
    {
        variableNum = 0;
        variableNameDic.Clear();

        codeStateText = null;
        codeStateText = new StringBuilder();

        codeStateText.Append(statementRegion);
        //非mono类声明一个transform
        if (!isMono)
        {
            codeStateText.Append(GenUICodeTemp.stateTransform);
        }

        //控件列表
        for (int i = 0; i < uiWidgets.Count; i++)
        {
            if (uiWidgets[i] == null) continue;

            Type type = uiWidgets[i].GetType();
            if (type == null)
            {
                Debug.LogError("BuildUICode type error !");
                return "";
            }

            string typeName = type.Name;
            string variableName = string.Format("{0}_{1}", typeName.ToLower(), uiWidgets[i].name);
            variableName = variableName.Replace(' ', '_');   //命名有空格的情况
            variableName = variableName.Replace("-", "_");
            //重名处理
            ++variableNum;
            if (variableNameDic.ContainsKey(variableName))
            {
                variableName += variableNum;
            }
            variableNameDic.Add(variableName, uiWidgets[i]);

            if (isMono)
            {
                codeStateText.AppendFormat(GenUICodeTemp.serilStateCodeFmt, typeName, variableName);
            }
            else
            {
                codeStateText.AppendFormat(GenUICodeTemp.stateCodeFmt, typeName, variableName);
            }
        }
        //其他对象列表，目前都是GameObject
        for (int i = 0; i < uiObjects.Count; i++)
        {
            if (uiObjects[i] == null) continue;

            Type type = uiObjects[i].GetType();
            if (type == null)
            {
                Debug.LogError("BuildUICode type error !");
                return "";
            }

            string typeName = type.Name;
            string variableName = string.Format("go_{0}", uiObjects[i].name);
            variableName = variableName.Replace(' ', '_');   //命名有空格的情况
            variableName = variableName.Replace("-", "_");
            //重名处理                                                
            ++variableNum;
            if (variableNameDic.ContainsKey(variableName))
            {
                variableName += variableNum;
            }
            variableNameDic.Add(variableName, uiObjects[i]);

            if (isMono)
            {
                codeStateText.AppendFormat(GenUICodeTemp.serilStateCodeFmt, typeName, variableName);
            }
            else
            {
                codeStateText.AppendFormat(GenUICodeTemp.stateCodeFmt, typeName, variableName);
            }
        }

        codeStateText.Append(statementRegionEnd);
        return codeStateText.ToString();
    }

    private void DrawEventWidget()
    {
        using (EditorGUILayout.HorizontalScope hScope = new EditorGUILayout.HorizontalScope())
        {
            //筛选当前UI的事件控件
            foreach (var elem in Enum.GetValues(typeof(GenUICodeTemp.EventWidgetType)))
            {
                for (int i = 0; i < uiWidgets.Count; i++)
                {
                    if (uiWidgets[i] == null) continue;

                    Type type = uiWidgets[i].GetType();
                    if (type == null)
                    {
                        Debug.LogError("BuildUICode type error !");
                        continue;
                    }

                    if (type.Name == elem.ToString() && !selectedEventWidgets.ContainsKey(type.Name))
                    {
                        selectedEventWidgets.Add(type.Name, true);
                    }
                }
            }

            //绘制toggle,注意不能遍历dic的同时赋值
            List<string> list = new List<string>(selectedEventWidgets.Keys);
            foreach (string wedagetName in list)
            {
                selectedEventWidgets[wedagetName] = EditorGUILayout.ToggleLeft(wedagetName, selectedEventWidgets[wedagetName],
                    GUILayout.Width(halfViewWidth / 8f));
            }
        }
    }

    /// <summary>
    /// 构建注册控件事件的代码
    /// </summary>
    /// <returns></returns>
    private string BuildEventCode()
    {
        codeEventText = null;
        codeEventText = new StringBuilder();

        codeEventText.Append(eventRegion);
        codeEventText.AppendFormat(methodStartFmt, "AddEvent");

        bool hasEventWidget = false;    //标识是否有控件注册了事件
        for (int i = 0; i < uiWidgets.Count; i++)
        {
            if (uiWidgets[i] == null) continue;

            //剔除不是事件或者是事件但未勾选toggle的控件
            string typeName = uiWidgets[i].GetType().Name;
            if (!selectedEventWidgets.ContainsKey(typeName) || !selectedEventWidgets[typeName])
            {
                continue;
            }

            foreach (var vName in variableNameDic.Keys)
            {
                if (uiWidgets[i].Equals(variableNameDic[vName]))
                {
                    string variableName = vName;
                    if (!string.IsNullOrEmpty(variableName))
                    {
                        variableName = variableName.Replace("-", "_");
                        string methodName = variableName.Substring(variableName.IndexOf('_') + 1);
                        if (uiWidgets[i] is UnityEngine.UI.Button)
                        {
                            string onClickStr = string.Format(onClickSerilCode, variableName, methodName);
                            if (hasEventWidget)
                            {
                                string str = codeEventText.ToString();
                                codeEventText.Insert(str.LastIndexOf(';') + 1, "\n" + onClickStr);
                            }
                            else
                            {
                                codeEventText.Append(onClickStr);
                            }
                            hasEventWidget = true;
                        }
                        else
                        {
                            string addEventStr = string.Format(onValueChangeSerilCode, variableName, methodName);
                            if (hasEventWidget)
                            {
                                codeEventText.Insert(codeEventText.ToString().LastIndexOf(';') + 1, addEventStr);
                            }
                            else
                            {
                                codeEventText.Append(addEventStr);
                            }
                            hasEventWidget = true;
                        }
                    }
                    break;
                }
            }
        }

        string codeStr = codeEventText.ToString();
        if (hasEventWidget)
        {
            codeEventText.Insert(codeStr.LastIndexOf(';') + 1, methodEnd);
        }
        else
        {
            codeEventText.Append(methodEnd);
        }

        codeEventText.Insert(codeEventText.Length - 2, eventRegionEnd);
        codeEventText.Remove(codeEventText.Length - 2, 2);
        return codeEventText.ToString();
    }

    private string BuildEventFuncCode()
    {
        codeEventFuncText = null;
        codeEventFuncText = new StringBuilder();

        codeEventFuncText.Append(eventFuncRegion);
        foreach (var widget in uiWidgets)
        {
            if (widget == null) continue;
            foreach (var vName in variableNameDic.Keys)
            {
                if (!widget.Equals(variableNameDic[vName])) continue;
                string variableName = vName;
                if (!string.IsNullOrEmpty(variableName))
                {
                    string methodName = variableName.Substring(variableName.IndexOf('_') + 1);
                    if (widget is Button)
                    {
                        codeEventFuncText.AppendFormat(btnCallbackSerilCode, methodName);
                    }
                    else
                    {
                        string typeName = widget.GetType().Name;
                        string paramType = "";
                        if (GenUICodeTemp.eventCBParamDic.ContainsKey(typeName))
                            paramType = GenUICodeTemp.eventCBParamDic[typeName];
                        if (!string.IsNullOrEmpty(paramType))
                            codeEventFuncText.AppendFormat(eventCallbackSerilCode, methodName, paramType);
                    }
                }
            }
        }
        codeEventFuncText.Append(eventFuncRegionEnd);

        codeEventFuncText.AppendLine();

        //OnAwake函数
        codeEventFuncText.AppendFormat(methodStartFmt, "OnAwake");
        codeEventFuncText.AppendLine("\t\tCurrentUIType.UIForms_Type = UIFormType.Normal;");
        codeEventFuncText.AppendLine("\t\tCurrentUIType.UIForms_ShowMode = UIFormShowMode.Normal;");
        codeEventFuncText.AppendLine("\t\tCurrentUIType.UIForm_LucencyType = UIFormLucenyType.Lucency;");
        codeEventFuncText.Append(methodEnd);

        //四个UI系统函数
        codeEventFuncText.AppendLine();
        codeEventFuncText.AppendFormat(GenUICodeTemp.methodOverroidStartFmt, "Display");
        codeEventFuncText.AppendFormat(GenUICodeTemp.methodCallbackFmt, "Display");
        codeEventFuncText.Append(methodEnd);

        codeEventFuncText.AppendLine();
        codeEventFuncText.AppendFormat(GenUICodeTemp.methodOverroidStartFmt, "Hiding");
        codeEventFuncText.AppendFormat(GenUICodeTemp.methodCallbackFmt, "Hiding");
        codeEventFuncText.Append(methodEnd);

        codeEventFuncText.AppendLine();
        codeEventFuncText.AppendFormat(GenUICodeTemp.methodOverroidStartFmt, "Redisplay");
        codeEventFuncText.AppendFormat(GenUICodeTemp.methodCallbackFmt, "Redisplay");
        codeEventFuncText.Append(methodEnd);

        codeEventFuncText.AppendLine();
        codeEventFuncText.AppendFormat(GenUICodeTemp.methodOverroidStartFmt, "Freeze");
        codeEventFuncText.AppendFormat(GenUICodeTemp.methodCallbackFmt, "Freeze");
        codeEventFuncText.Append(methodEnd);

        return codeEventFuncText.ToString();
    }

    /// <summary>
    /// 当前操作生成的代码预览
    /// </summary>
    private void DrawPreviewText()
    {
        EditorGUILayout.Space();

        using (var ver = new EditorGUILayout.VerticalScope())
        {
            GUI.backgroundColor = Color.white;
            GUI.Box(ver.rect, "");

            EditorGUILayout.HelpBox("代码预览:", MessageType.None);
            using (var scr = new EditorGUILayout.ScrollViewScope(scrollTextPos))
            {
                scrollTextPos = scr.scrollPosition;

                if (codeStateText != null && !string.IsNullOrEmpty(codeStateText.ToString()) && selectedBar == 0)
                {
                    //GUILayout.TextArea(codeStateText.ToString());
                    GUILayout.Label(codeStateText.ToString());
                }

                if (codeAssignText != null && !string.IsNullOrEmpty(codeAssignText.ToString()))
                {
                    GUILayout.Label(codeAssignText.ToString());
                }

                if (codeEventText != null && !string.IsNullOrEmpty(codeEventText.ToString()))
                {
                    //GUILayout.TextArea(codeEventText.ToString());
                    GUILayout.Label(codeEventText.ToString());
                }

                if (codeEventFuncText != null && !string.IsNullOrEmpty(codeEventFuncText.ToString()))
                {
                    GUILayout.Label(codeEventFuncText.ToString());
                }
            }
        }
    }

    /// <summary>
    /// 生成C# UI脚本
    /// </summary>
    private void CreateCsUIScript()
    {
        string path = EditorPrefs.GetString("create_script_folder", "");
        path = EditorUtility.SaveFilePanel("Create Script", path, root.name + ".cs", "cs");
        if (string.IsNullOrEmpty(path)) return;
        int index = path.LastIndexOf('/');
        className = path.Substring(index + 1, path.LastIndexOf('.') - index - 1);
        //查看当前是否已有生成好的脚本
        if (File.Exists(path)) ExistsCsModScript(className, path);
        else CreateCsScript(className, path);
    }

    private void ExistsCsModScript(string className, string path)
    {
        string[] codeLines = File.ReadAllLines(path);
        List<string> codeList = new List<string>(codeLines);
        //代码开头到using部分
        StringBuilder annotationSb = new StringBuilder();
        //class开头部分
        int classEndIndex = codeList.FindIndex(0, str => str.Contains("UI VARIABLE STATEMENT START"));
        ForeachListAppend(codeList, annotationSb, 0, classEndIndex);

        //event 实现部分的代码  分拣出已实现的事件代码
        StringBuilder eventBuilder = new StringBuilder();
        int eventStartIdx = codeList.FindIndex(classEndIndex, str => str.Contains("UI EVENT FUNC START"));
        int eventEndIdx = codeList.FindIndex(eventStartIdx, str => str.Contains("UI EVENT FUNC END"));
        List<string> eventCodes = new List<string>(eventEndIdx - eventStartIdx);
        for (int i = eventStartIdx; i < eventEndIdx; i++)
        {
            eventCodes.Add(codeList[i]);
            eventBuilder.AppendLine(codeList[i]);
        }
        ForeachEventCodeAppend(eventCodes, eventBuilder);
        eventBuilder.AppendLine(/*eventFuncRegionEnd*/"	// UI EVENT FUNC END");

        //自己写的代码
        StringBuilder selfcodeSb = new StringBuilder();
        ForeachListAppend(codeList, selfcodeSb, eventEndIdx + 1, codeList.Count, true);

        StringBuilder allCode = new StringBuilder();
        allCode.Append(annotationSb);
        codeStateText.Remove(0, 1);
        allCode.Append(codeStateText);
        allCode.Append(codeAssignText);
        allCode.Append(codeEventText);
        allCode.AppendLine();
        allCode.Append(eventBuilder);
        allCode.AppendLine();
        allCode.Append(selfcodeSb);
        File.WriteAllText(path, allCode.ToString(), new UTF8Encoding(false));
        AssetDatabase.Refresh();

        Debug.Log("脚本修改成功,生成路径为:" + path);
        EditorPrefs.SetString("create_script_folder", path);
    }
    private void ForeachListAppend(List<string> lines, StringBuilder sb, int start, int end, bool firstAppend = false)
    {
        for (int i = start; i < end; i++)
        {
            if (i == start && firstAppend) sb.Append(lines[i]);
            else sb.AppendLine(lines[i]);
        }
    }

    private void ForeachEventCodeAppend(List<string> includList, StringBuilder outBuilder)
    {
        foreach (var widget in uiWidgets)
        {
            if (widget == null) continue;
            foreach (var vName in variableNameDic.Keys)
            {
                if (!widget.Equals(variableNameDic[vName])) continue;
                string variableName = vName;
                if (!string.IsNullOrEmpty(variableName))
                {
                    string methodName = variableName.Substring(variableName.IndexOf('_') + 1);
                    if (widget is UnityEngine.UI.Button)
                    {
                        string funcName = string.Format("On{0}Clicked(", methodName);
                        int idx = includList.FindIndex(str => str.Contains(funcName));
                        if (idx < 0) outBuilder.AppendFormat(btnCallbackSerilCode, methodName);
                    }
                    else
                    {
                        string funcName = string.Format("On{0}ValueChanged(", methodName);
                        int idx = includList.FindIndex(str => str.Contains(funcName));
                        if (idx > 0) continue;
                        string typeName = widget.GetType().Name;
                        string paramType = "";
                        if (GenUICodeTemp.eventCBParamDic.ContainsKey(typeName))
                            paramType = GenUICodeTemp.eventCBParamDic[typeName];
                        if (!string.IsNullOrEmpty(paramType))
                            outBuilder.AppendFormat(eventCallbackSerilCode, methodName, paramType);
                    }
                }
            }
        }
    }

    private void CreateCsScript(string className, string path)
    {
        StringBuilder scriptBuilder = new StringBuilder();
        scriptBuilder.Append(GenUICodeTemp.codeAnnotation);
        scriptBuilder.Append(GenUICodeTemp.usingNamespace);

        if (isMono) scriptBuilder.AppendFormat(GenUICodeTemp.classMonoStart, className);
        else scriptBuilder.AppendFormat(GenUICodeTemp.classStart, className);

        scriptBuilder.Append(codeStateText);
        scriptBuilder.Append(codeAssignText);
        scriptBuilder.Append(codeEventText);
        scriptBuilder.Append(BuildEventFuncCode());
        scriptBuilder.Append(GenUICodeTemp.classEnd);

        File.WriteAllText(path, scriptBuilder.ToString(), new UTF8Encoding(false));
        AssetDatabase.Refresh();

        Debug.Log("脚本生成成功,生成路径为:" + path);
        EditorPrefs.SetString("create_script_folder", path);
    }


    /// <summary>
    /// 在根物体上挂载生成的脚本(必须继承monobehavior)
    /// </summary>
    private void AddScriptComponent()
    {
        if (EditorApplication.isCompiling)
        {
            EditorUtility.DisplayDialog("警告", "请等待编辑器完成编译再执行此操作", "OK");
            return;
        }

        if (root == null || string.IsNullOrEmpty(className))
        {
            EditorUtility.DisplayDialog("警告", "请先按顺序生成UI脚本再执行此操作", "OK");
            return;
        }

        //通过Assembly-CSharp程序集挂载脚本
        Assembly[] AssbyCustmList = System.AppDomain.CurrentDomain.GetAssemblies();
        Assembly asCSharp = null;
        for (int i = 0; i < AssbyCustmList.Length; i++)
        {
            string assbyName = AssbyCustmList[i].GetName().Name;
            if (assbyName == "Assembly-CSharp")
            {
                asCSharp = AssbyCustmList[i];
                break;
            }
        }

        scriptType = asCSharp.GetType(className);
        if (scriptType == null)
        {
            EditorUtility.DisplayDialog("警告", "挂载失败，请先检查脚本是否正确生成", "OK");
            return;
        }

        var target = root.GetComponent(scriptType);
        if (target) Destroy(target);
        else target = root.AddComponent(scriptType);

        if (NeedSafeAreaPanel)
        {
            target = root.GetComponent<SafeAreaPanel>();
            if (target == null) root.AddComponent<SafeAreaPanel>();
        }

    }

    private void BuildAssignmentCode()
    {
        codeAssignText = new StringBuilder();
        codeAssignText.Append(assignRegion);
        codeAssignText.AppendFormat(methodStartFmt, "Awake");
        if (!isMono && selectedBar == 0)
        {
            codeAssignText.Append(GenUICodeTemp.assignTransform);
        }

        var allPath = GetChildrenPaths(root);

        if (variableNameDic == null)
        {
            return;
        }

        //格式：变量名 = transform.Find("").Getcomponent<>();
        foreach (var name in variableNameDic.Keys)
        {
            var obj = variableNameDic[name];
            if (obj == null) continue;

            string path = "";
            bool isRootComponent = false;
            foreach (var tran in allPath.Keys)
            {
                if (tran == null) continue;

                UIBehaviour behav = obj as UIBehaviour;
                if (behav != null)
                {
                    //判断是否挂在根上，根上不需要路径
                    isRootComponent = behav.gameObject == root;
                    if (isRootComponent) break;

                    if (behav.gameObject == tran.gameObject)
                    {
                        path = allPath[tran];
                        break;
                    }
                }
                else
                {
                    if (tran.gameObject == obj)
                    {
                        path = allPath[tran];
                        break;
                    }
                }
            }

            if (obj is GameObject)
            {
                codeAssignText.AppendFormat(assignGameObjectCodeFmt, name, path);
            }
            else
            {
                if (isRootComponent)
                {
                    codeAssignText.AppendFormat(assignRootCodeFmt, name, obj.GetType().Name);
                }
                else
                {
                    codeAssignText.AppendFormat(assignCodeFmt, name, path, obj.GetType().Name);
                }
            }
        }

        codeAssignText.Append("\n");
        codeAssignText.Append("\t\tOnAwake(); \n");
        codeAssignText.Append(GenUICodeTemp.addEventCodeFmt);
        codeAssignText.Append(methodEnd);
        codeAssignText.Append(assignRegionEnd);
        //Debug.Log(codeAssignText.ToString());
    }


    private Dictionary<Transform, string> GetChildrenPaths(GameObject rootGo)
    {
        Dictionary<Transform, string> pathDic = new Dictionary<Transform, string>();
        string path = string.Empty;
        Transform[] tfArray = rootGo.GetComponentsInChildren<Transform>(true);
        for (int i = 0; i < tfArray.Length; i++)
        {
            Transform node = tfArray[i];

            string str = node.name;
            while (node.parent != null && node.gameObject != rootGo && node.parent.gameObject != rootGo)
            {
                str = string.Format("{0}/{1}", node.parent.name, str);
                node = node.parent;
            }
            path += string.Format("{0}\n", str);

            if (!pathDic.ContainsKey(tfArray[i]))
            {
                pathDic.Add(tfArray[i], str);
            }
        }
        //Debug.Log(path);

        return pathDic;
    }

}

