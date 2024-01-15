using System.Collections;
using System.Collections.Generic;
using UIFW;
using UnityEngine;

public class RainbowIocnGuidePanel : GuidePanel
{
    protected override void OnAwake()
    {
        base.OnAwake();
        CurrentUIType.UIForm_LucencyType = UIFormLucenyType.ImPenetrable;
    }
}
