using UnityEngine;
using UnityEditor;
using UnityEngine.UI;

public class ChangeFontWindow : EditorWindow
{
    [MenuItem("Tools/更换字体")]
    public static void Open()
    {
        EditorWindow.GetWindow(typeof(ChangeFontWindow));
    }

    Font toChange;
    static Font toChangeFont;
    FontStyle toFontStyle;
    static FontStyle toChangeFontStyle;

    void OnGUI()
    {
        toChange = (Font)EditorGUILayout.ObjectField(toChange, typeof(Font), true, GUILayout.MinWidth(100f));
        toChangeFont = toChange;
        toFontStyle = (FontStyle)EditorGUILayout.EnumPopup(toFontStyle, GUILayout.MinWidth(100f));
        toChangeFontStyle = toFontStyle;
        if (GUILayout.Button("更换"))
        {
            Change();
        }
    }

    public static void Change()
    {
        Transform canvas = GameObject.Find("Canvas").transform;
        if (!canvas)
        {
            Debug.Log("NO Canvas");
            return;
        }
        Transform[] tArray = canvas.GetComponentsInChildren<Transform>();
        for (int i = 0; i < tArray.Length; i++)
        {
            Text t = tArray[i].GetComponent<Text>();
            if (t)
            {
                //如果不加这个代码  在做完更改后 自己随便修改下场景里物体的状态 在保存就好了 ，不然他会觉得没变 就不会保存 就失败了
                Undo.RecordObject(t, t.gameObject.name);
                t.font = toChangeFont;
                t.fontStyle = toChangeFontStyle;
                //刷新下
                EditorUtility.SetDirty(t);
            }
        }
        Debug.Log("Succed");
    }
}