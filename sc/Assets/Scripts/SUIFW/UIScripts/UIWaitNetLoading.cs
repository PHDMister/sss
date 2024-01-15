using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UIFW;
using UnityEngine;
using UnityEngine.UI;

public class UIWaitNetLoading : BaseUIForm
{
    // UI VARIABLE STATEMENT START
    private Image image_Image1;
    private Image image_Image2;
    private Image image_Image3;
    private Text text_Text;
    // UI VARIABLE STATEMENT END

    // UI VARIABLE ASSIGNMENT START
    private void Awake()
    {
        image_Image1 = FindComp<Image>("Image");
        image_Image2 = FindComp<Image>("Image1");
        image_Image3 = FindComp<Image>("Image2");
        text_Text = FindComp<Text>("Text");

        OnAwake();
        AddEvent();

    }
    // UI VARIABLE ASSIGNMENT END

    // UI EVENT REGISTER START
    private void AddEvent()
    {

    }
    // UI EVENT REGISTER END

    // UI EVENT FUNC START
    // UI EVENT FUNC END


    private int WaitRecordNum = 0;
    private const float autoCloseTime = 30;
    private float rtTime;
    private void OnAwake()
    {
        CurrentUIType.UIForms_Type = UIFormType.PopUp;
        CurrentUIType.UIForms_ShowMode = UIFormShowMode.Normal;
        CurrentUIType.UIForm_LucencyType = UIFormLucenyType.Lucency;

    }

    public void AddWaitRecordNum()
    {
        WaitRecordNum++;
    }

    public void DecWaitRecordNum()
    {
        WaitRecordNum = Mathf.Max(0, WaitRecordNum - 1);
    }

    protected void Update()
    {
        //计数清零 关闭UI
        if (WaitRecordNum <= 0)
        {
            CloseUIForm();
            return;
        }
        //30s后自动关闭
        rtTime -= Time.deltaTime;
        if (rtTime <= 0) CloseUIForm();
    }

    protected void OnEnable()
    {
        Vector3 rVec = new Vector3(0, 0, -360f);
        image_Image1.transform.DOLocalRotate(rVec, 2f, RotateMode.FastBeyond360)
            .SetLoops(-1, LoopType.Incremental)
            .SetEase(Ease.Linear);
        Vector3 rVec1 = new Vector3(0, 0, 360f);
        image_Image2.transform.DOLocalRotate(rVec1, 2f, RotateMode.FastBeyond360)
            .SetLoops(-1, LoopType.Incremental)
            .SetEase(Ease.Linear);
        Vector3 rVec2 = new Vector3(0, 0, -360f);
        image_Image3.transform.DOLocalRotate(rVec2, 2f, RotateMode.FastBeyond360)
            .SetLoops(-1, LoopType.Incremental)
            .SetEase(Ease.Linear);
    }

    protected void OnDisable()
    {
        image_Image1.transform.DOKill();
        image_Image2.transform.DOKill();
        image_Image3.transform.DOKill();
    }


    public override void Display()
    {
        base.Display();
        WaitRecordNum = 1;
        rtTime = autoCloseTime;

        text_Text.text = "";
    }

    public override void Hiding()
    {
        base.Hiding();
        WaitRecordNum = 0;
    }

    public override void Redisplay()
    {
        base.Redisplay();
        WaitRecordNum = 1;
        rtTime = autoCloseTime;
    }

    public override void Freeze()
    {
        base.Freeze();
    }

}
