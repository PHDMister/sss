using UIFW;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class CamCtrl : MonoBehaviour
{
    public Transform modelObj;//围绕的物体
    private Vector3 pivotPos = new Vector3();
    ShopNewUIForm shopNewUIForm;
    AppearanceEditorUIForm appearanceEditorUIForm;
    PersonalDataPanel personalDataPanel;
    EditPersonalDataPanel editPersonalDataPanel;
    void Start()
    {
        //modelObj = GameObject.Find("Model").transform;
    }
    void Update()
    {
        Cam_Ctrl_Rotation();
    }
    //摄像机的旋转
    public void Cam_Ctrl_Rotation()
    {
        if (modelObj == null)
            return;

        bool bShopOpend = UIManager.GetInstance().IsOpend(FormConst.SHOPNEWUIFORM);
        bool bShopItemPriviewOpend = UIManager.GetInstance().IsOpend(FormConst.SHOPITEMPRVIEW_UIFORM);
        bool bAppearanceOpend = UIManager.GetInstance().IsOpend(FormConst.APPEARANCEEDITORUIFORM);
        bool bPersonalDataPanel = UIManager.GetInstance().IsOpend(FormConst.PERSONALDATAPANEL);
        bool bEditPersonalDataPanel = UIManager.GetInstance().IsOpend(FormConst.EDITPERSONALDATAPANEL);

        shopNewUIForm = (ShopNewUIForm)UIManager.GetInstance().GetUIForm(FormConst.SHOPNEWUIFORM);
        appearanceEditorUIForm = (AppearanceEditorUIForm)UIManager.GetInstance().GetUIForm(FormConst.APPEARANCEEDITORUIFORM);
        personalDataPanel = (PersonalDataPanel)UIManager.GetInstance().GetUIForm(FormConst.PERSONALDATAPANEL);
        editPersonalDataPanel = (EditPersonalDataPanel)UIManager.GetInstance().GetUIForm(FormConst.EDITPERSONALDATAPANEL);

        if (!bShopOpend && !bAppearanceOpend && !bShopItemPriviewOpend && !bPersonalDataPanel)
            return;

        if (bShopOpend)
        {
            if (shopNewUIForm != null && shopNewUIForm.m_ScrollViewBg != null)
            {
                if (RectTransformUtility.RectangleContainsScreenPoint(shopNewUIForm.m_ScrollViewBg, Input.mousePosition))
                {
                    return;
                }
            }
        }
        if (bAppearanceOpend)
        {
            if (appearanceEditorUIForm != null && appearanceEditorUIForm.m_ScrollViewBg != null)
            {
                if (RectTransformUtility.RectangleContainsScreenPoint(appearanceEditorUIForm.m_ScrollViewBg, Input.mousePosition))
                {
                    return;
                }
            }
        }

        if (bPersonalDataPanel)
        {
            if (personalDataPanel != null && personalDataPanel.Bg_Img != null)
            {
                if (RectTransformUtility.RectangleContainsScreenPoint(personalDataPanel.Bg_Img.rectTransform, Input.mousePosition))
                {
                    return;
                }
            }
        }

        if (bEditPersonalDataPanel)
        {
            if (editPersonalDataPanel != null && editPersonalDataPanel.thisBg != null)
            {
                if (RectTransformUtility.RectangleContainsScreenPoint(editPersonalDataPanel.thisBg.rectTransform, Input.mousePosition))
                {
                    return;
                }
            }
        }


        if (ManageMentClass.DataManagerClass.PlatformType == 2)
        {
            if (bShopOpend)
            {
                if (shopNewUIForm != null && (shopNewUIForm.GetPageType() == ShopNewUIForm.PageType.Appearance || shopNewUIForm.GetPageType() == ShopNewUIForm.PageType.Action))
                {
                    if (Input.GetMouseButton(0))
                    {
                        if (Input.touchCount == 1)
                        {
                            Touch touch = Input.GetTouch(0);
                            Vector2 deltaPos = touch.deltaPosition;
                            if (touch.phase == TouchPhase.Moved)
                            {
                                if (ManageMentClass.DataManagerClass.CameraController)
                                {
                                    ManageMentClass.DataManagerClass.CameraController = false;
                                }
                                modelObj.transform.Rotate(Vector3.up * deltaPos.x * 2);
                            }
                            else
                            {
                                if (!ManageMentClass.DataManagerClass.CameraController)
                                {
                                    ManageMentClass.DataManagerClass.CameraController = true;
                                }
                            }
                        }
                    }
                }
                else
                {
                    if (Input.GetMouseButton(0))
                    {
                        if (Input.touchCount == 1)
                        {
                            Touch touch = Input.GetTouch(0);
                            Vector2 deltaPos = touch.deltaPosition;
                            if (touch.phase == TouchPhase.Moved)
                            {
                                if (ManageMentClass.DataManagerClass.CameraController)
                                {
                                    ManageMentClass.DataManagerClass.CameraController = false;
                                }
                                for (int i = 0; i < modelObj.transform.childCount; i++)
                                {
                                    Transform childTrans = modelObj.transform.GetChild(i);
                                    MeshRenderer meshRender = childTrans.GetComponent<MeshRenderer>();
                                    if (meshRender != null)
                                    {
                                        pivotPos = meshRender.bounds.center;
                                    }
                                    else
                                    {
                                        for (int j = 0; j < childTrans.childCount; j++)
                                        {
                                            if (childTrans.GetChild(j).GetComponent<MeshRenderer>() != null)
                                            {
                                                pivotPos = childTrans.GetChild(j).GetComponent<MeshRenderer>().bounds.center;
                                                break;
                                            }
                                        }
                                    }
                                }
                                modelObj.RotateAround(pivotPos, Vector3.up, deltaPos.x * 2);
                            }
                            else
                            {
                                if (!ManageMentClass.DataManagerClass.CameraController)
                                {
                                    ManageMentClass.DataManagerClass.CameraController = true;
                                }
                            }
                        }
                    }
                }
            }

            if (bAppearanceOpend)
            {
                if (Input.GetMouseButton(0))
                {
                    if (Input.touchCount == 1)
                    {
                        Touch touch = Input.GetTouch(0);
                        Vector2 deltaPos = touch.deltaPosition;
                        if (touch.phase == TouchPhase.Moved)
                        {
                            if (ManageMentClass.DataManagerClass.CameraController)
                            {
                                ManageMentClass.DataManagerClass.CameraController = false;
                            }
                            modelObj.transform.Rotate(Vector3.up * deltaPos.x * 2);
                        }
                        else
                        {
                            if (!ManageMentClass.DataManagerClass.CameraController)
                            {
                                ManageMentClass.DataManagerClass.CameraController = true;
                            }
                        }
                    }
                }
            }
            if (bPersonalDataPanel)
            {
                if (Input.GetMouseButton(0))
                {
                    if (Input.touchCount == 1)
                    {
                        Touch touch = Input.GetTouch(0);
                        Vector2 deltaPos = touch.deltaPosition;
                        if (touch.phase == TouchPhase.Moved)
                        {
                            if (ManageMentClass.DataManagerClass.CameraController)
                            {
                                ManageMentClass.DataManagerClass.CameraController = false;
                            }
                            modelObj.transform.Rotate(Vector3.up * deltaPos.x * 2);
                        }
                        else
                        {
                            if (!ManageMentClass.DataManagerClass.CameraController)
                            {
                                ManageMentClass.DataManagerClass.CameraController = true;
                            }
                        }
                    }
                }
            }


            if (bShopItemPriviewOpend)
            {
                if (Input.GetMouseButton(0))
                {
                    if (Input.touchCount == 1)
                    {
                        Touch touch = Input.GetTouch(0);
                        Vector2 deltaPos = touch.deltaPosition;
                        if (touch.phase == TouchPhase.Moved)
                        {
                            if (ManageMentClass.DataManagerClass.CameraController)
                            {
                                ManageMentClass.DataManagerClass.CameraController = false;
                            }
                            for (int i = 0; i < modelObj.transform.childCount; i++)
                            {
                                Transform childTrans = modelObj.transform.GetChild(i);
                                MeshRenderer meshRender = childTrans.GetComponent<MeshRenderer>();
                                if (meshRender != null)
                                {
                                    pivotPos = meshRender.bounds.center;
                                }
                                else
                                {
                                    for (int j = 0; j < childTrans.childCount; j++)
                                    {
                                        if (childTrans.GetChild(j).GetComponent<MeshRenderer>() != null)
                                        {
                                            pivotPos = childTrans.GetChild(j).GetComponent<MeshRenderer>().bounds.center;
                                            break;
                                        }
                                    }
                                }
                            }
                            modelObj.RotateAround(pivotPos, Vector3.up, deltaPos.x * 5);
                        }
                        else
                        {
                            if (!ManageMentClass.DataManagerClass.CameraController)
                            {
                                ManageMentClass.DataManagerClass.CameraController = true;
                            }
                        }
                    }
                }
            }
        }
        else
        {
            var mouse_x = Input.GetAxis("Mouse X");//获取鼠标X轴移动
            var mouse_y = -Input.GetAxis("Mouse Y");//获取鼠标Y轴移动
            if (bShopOpend)
            {
                if (shopNewUIForm != null && (shopNewUIForm.GetPageType() == ShopNewUIForm.PageType.Appearance || shopNewUIForm.GetPageType() == ShopNewUIForm.PageType.Action))
                {
                    Debug.Log("在A点循环");
                    if (Input.GetKey(KeyCode.Mouse0))
                    {
                        modelObj.transform.Rotate(Vector3.up * mouse_x * 5);
                    }
                }
                else
                {
                    Debug.Log("在B点循环");
                    if (Input.GetKey(KeyCode.Mouse0))
                    {
                        for (int i = 0; i < modelObj.transform.childCount; i++)
                        {
                            Transform childTrans = modelObj.transform.GetChild(i);
                            MeshRenderer meshRender = childTrans.GetComponent<MeshRenderer>();
                            if (meshRender != null)
                            {
                                pivotPos = meshRender.bounds.center;
                            }
                            else
                            {
                                for (int j = 0; j < childTrans.childCount; j++)
                                {
                                    if (childTrans.GetChild(j).GetComponent<MeshRenderer>() != null)
                                    {
                                        pivotPos = childTrans.GetChild(j).GetComponent<MeshRenderer>().bounds.center;
                                        break;
                                    }
                                }
                            }
                            break;
                        }
                        modelObj.RotateAround(pivotPos, Vector3.up, mouse_x * 5);
                    }
                }
            }

            if (bAppearanceOpend)
            {
                if (Input.GetKey(KeyCode.Mouse0))
                {
                    modelObj.transform.Rotate(Vector3.up * mouse_x * 5);
                }
            }

            if (bPersonalDataPanel)
            {
                if (Input.GetKey(KeyCode.Mouse0))
                {
                    modelObj.transform.Rotate(Vector3.up * mouse_x * 5);
                }
            }

            if (bShopItemPriviewOpend)
            {
                if (Input.GetKey(KeyCode.Mouse0))
                {
                    for (int i = 0; i < modelObj.transform.childCount; i++)
                    {
                        Transform childTrans = modelObj.transform.GetChild(i);
                        MeshRenderer meshRender = childTrans.GetComponent<MeshRenderer>();
                        if (meshRender != null)
                        {
                            pivotPos = meshRender.bounds.center;
                        }
                        else
                        {
                            for (int j = 0; j < childTrans.childCount; j++)
                            {
                                if (childTrans.GetChild(j).GetComponent<MeshRenderer>() != null)
                                {
                                    pivotPos = childTrans.GetChild(j).GetComponent<MeshRenderer>().bounds.center;
                                    break;
                                }
                            }
                        }
                        break;
                    }
                    modelObj.RotateAround(pivotPos, Vector3.up, mouse_x * 5);
                }
            }
        }
    }
}