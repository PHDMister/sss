using System.Collections;
using System.Collections.Generic;
using UIFW;
using UnityEngine;

public class treasureHuntNpc : BaseUIForm
{
    private void Start()
    {
        GameObject obj = ResourcesMgr.GetInstance().LoadAsset(SysDefine.SYS_PATH_TREASUREHUNTTOASTUI, true);
        obj.SetActive(true);
        UIFollowWorldObject uIFollowWorldObject = obj.transform.Find("Toast").GetComponent<UIFollowWorldObject>();
        if (uIFollowWorldObject != null)
        {
            Transform target = UnityHelper.FindTheChildNode(transform.gameObject, "UIRoot");

            Canvas canvas = obj.GetComponent<Canvas>();
            if (canvas != null)
            {
                canvas.renderMode = RenderMode.WorldSpace;
                canvas.sortingLayerID = 10;
                obj.transform.position = target.position;
                obj.transform.localScale = new Vector3(0.005f, 0.005f, 0.005f);
                canvas.worldCamera = Camera.main;
                uIFollowWorldObject.Init(Camera.main, target, obj.GetComponent<Canvas>());
            }

            Transform toastClick = UnityHelper.FindTheChildNode(obj, "ToastClick");
            EventTriggerListener.Get(toastClick.gameObject).onClick = p =>
            {
                OpenUIForm(FormConst.TREASUREDIGGINGTICKETEXCHANGE);
            };
        }
    }

}
