using System.Collections;
using System.Collections.Generic;
using UIFW;
using UnityEngine;

public class RainbowSeabedGuidePanel : GuidePanel
{
    protected override void OnAwake()
    {
        base.OnAwake();
        CurrentUIType.UIForm_LucencyType = UIFormLucenyType.ImPenetrable;
    }
}
