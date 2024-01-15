using UnityEngine;

public class SafeAreaPanel : MonoBehaviour
{
    RectTransform RT = null;
    public bool Debug = false;
    void Awake()
    {
        RT = GetComponent<RectTransform>();
        RT.pivot = new Vector2(0.5f, 0.5f);
        RT.anchorMax = Vector2.one;
        RT.anchorMin = Vector2.zero;
        RT.localRotation = Quaternion.identity;
        RT.localScale = Vector2.one;
        RT.localPosition = Vector3.zero;
        RT.offsetMax = Vector3.zero;
        RT.offsetMin = Vector3.zero;

        AutoSetSafeArea();
    }

    void AutoSetSafeArea()
    {
        //unity �°汾�ṩ��  Screen.safeArea ���������Ի�ȡ����Ļ�İ�ȫ����
        //�������߶�����һ������������
        RT.anchorMax = new Vector2(1 - (Screen.safeArea.x / Screen.width), 1);
        RT.anchorMin = new Vector2(Screen.safeArea.x / Screen.width, 0);
    }

#if UNITY_EDITOR_WIN
    void Update()
    {
        if (Debug)
        {
            AutoSetSafeArea();
        }

    }
#endif

}