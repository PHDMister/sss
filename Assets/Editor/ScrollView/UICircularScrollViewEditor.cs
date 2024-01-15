using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace CircularScrollView
{
    [CustomEditor(typeof(CircularScrollView.UICircularScrollView))]
    public class UICircularScrollViewEditor : Editor
    {
        UICircularScrollView scrollView;
        public override void OnInspectorGUI()
        {
            scrollView = (CircularScrollView.UICircularScrollView)target;
            scrollView.m_Direction = (e_Direction)EditorGUILayout.EnumPopup("Direction:", scrollView.m_Direction);
            scrollView.m_Row = EditorGUILayout.IntField("Row or column:", scrollView.m_Row);
            scrollView.m_Spacing = EditorGUILayout.FloatField("Spacing:", scrollView.m_Spacing);
            scrollView.m_SpacingY = EditorGUILayout.FloatField("SpacingY:", scrollView.m_SpacingY);
            scrollView.m_ContentOffsetY = EditorGUILayout.FloatField("ContentOffsetY:", scrollView.m_ContentOffsetY);
            scrollView.m_CellGameObject = (GameObject)EditorGUILayout.ObjectField("Cell: ", scrollView.m_CellGameObject, typeof(GameObject), true);
            scrollView.m_IsShowArrow = EditorGUILayout.ToggleLeft(" IsShowArrow", scrollView.m_IsShowArrow);
            if (scrollView.m_IsShowArrow)
            {
                scrollView.m_PointingFirstArrow = (GameObject)EditorGUILayout.ObjectField("Up or Left Arrow: ", scrollView.m_PointingFirstArrow, typeof(GameObject), true);
                scrollView.m_PointingEndArrow = (GameObject)EditorGUILayout.ObjectField("Down or Right Arrow: ", scrollView.m_PointingEndArrow, typeof(GameObject), true);
            }
        }
    }
}


