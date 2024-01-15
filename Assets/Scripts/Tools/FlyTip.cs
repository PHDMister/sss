using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FlyTip : MonoBehaviour
{
    public const float DURING_LONG = 1f;
    public const float DURING_SHOR = 0.5f;

    public enum Position
    {
        Bottom,
        Top,
        Center
    }

    public Image backGround;
    public Text toastStr;

    #region Top位置固定参数
    private static Vector2 TopAnchorMin = new Vector2(0, 1);
    private static Vector2 TopAnchorMax = new Vector2(1, 1);
    private static Vector2 TopPivot = new Vector2(0.5f, 1);
    private static Vector3 TopAnchoredPosition3D = new Vector3(0, 0, 0);
    private static Vector2 TopSizeDelta = new Vector3(0, 50);
    #endregion

    #region Bottom位置固定参数
    private static Vector2 BottomAnchorMin = new Vector2(0, 0);
    private static Vector2 BottomAnchorMax = new Vector2(1, 0);
    private static Vector2 BottomPivot = new Vector2(0.5f, 0);
    private static Vector3 BottomAnchoredPosition3D = new Vector3(0, 0, 0);
    private static Vector2 BottomSizeDelta = new Vector3(0, 50);
    #endregion

    #region 自定义位置参数--居中
    private Vector2 customAnchorMin = new Vector2(0, 0);
    private Vector2 customAnchorMax = new Vector2(1, 1);
    private Vector2 customPivot = new Vector2(0.5f, 1);
    private Vector3 customAnchoredPosition3D = new Vector3(0, 0, 0);
    private Vector2 customSizeDelta = new Vector3(0, 0);
    #endregion

    private RectTransform rectTransform;
    private Animator mAnimator;
    private float duringTime = DURING_SHOR;
    private Position showPosition = Position.Center;

    private void Awake()
    {
        rectTransform = this.GetComponent<RectTransform>();
        mAnimator = backGround.GetComponent<Animator>();
    }

    public void Show()
    {
        gameObject.SetActive(true);
        transform.localScale = Vector3.one;

        SetPositionAndSize();

        mAnimator.Play("FlyTextAni");
        StartCoroutine("EndAni");

        LayoutRebuilder.ForceRebuildLayoutImmediate(backGround.GetComponent<RectTransform>());
    }

    IEnumerator EndAni()
    {
        yield return new WaitForSeconds(duringTime);
        gameObject.SetActive(false);
        ToastManager.Instance.CheckToast(this);
    }
    public void SetColor(Color backColor, Color textColor)
    {
        backGround.color = backColor;
        toastStr.color = textColor;
    }

    public void SetDuring(float duringTime)
    {
        this.duringTime = duringTime;
    }

    public void SetText(string str)
    {
        toastStr.text = str;
        LayoutRebuilder.ForceRebuildLayoutImmediate(backGround.GetComponent<RectTransform>());
    }

    public void SetPositionAndSize(Position showPosition)
    {
        this.showPosition = showPosition;
    }

    public void SetPositionAndSize(Vector2 min, Vector2 max, Vector2 pivot, Vector3 anchoredPosition, Vector2 size)
    {
        showPosition = Position.Center;
        customAnchorMin = min;
        customAnchorMax = max;
        customPivot = pivot;
        customAnchoredPosition3D = anchoredPosition;
        customSizeDelta = size;
    }

    private void SetPositionAndSize()
    {
        if (showPosition == Position.Top)
        {
            rectTransform.anchorMin = TopAnchorMin;
            rectTransform.anchorMax = TopAnchorMax;
            rectTransform.pivot = TopPivot;
            rectTransform.anchoredPosition3D = TopAnchoredPosition3D;
            rectTransform.sizeDelta = TopSizeDelta;
        }
        else if (showPosition == Position.Bottom)
        {
            rectTransform.anchorMin = BottomAnchorMin;
            rectTransform.anchorMax = BottomAnchorMax;
            rectTransform.pivot = BottomPivot;
            rectTransform.anchoredPosition3D = BottomAnchoredPosition3D;
            rectTransform.sizeDelta = BottomSizeDelta;
        }
        else if (showPosition == Position.Center)
        {
            rectTransform.anchorMin = customAnchorMin;
            rectTransform.anchorMax = customAnchorMax;
            rectTransform.pivot = customPivot;
            rectTransform.anchoredPosition3D = customAnchoredPosition3D;
            rectTransform.sizeDelta = customSizeDelta;
        }
        else
        {
            rectTransform.anchorMin = customAnchorMin;
            rectTransform.anchorMax = customAnchorMax;
            rectTransform.pivot = customPivot;
            rectTransform.anchoredPosition3D = customAnchoredPosition3D;
            rectTransform.sizeDelta = customSizeDelta;
        }

    }
}