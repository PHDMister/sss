using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SceneEditorItem : MonoBehaviour
{
    private Button button_scene_zhu;
    private Image image_Render;
    private Image image_shucaitu;
    private Image image_scene_kuang;
    private Image image_scene_weijiesuo;
    private Image image_icon_password;
    private Image image_icon_star;
    private Image image_jiagebeijing;
    private Image image_BUTTON_aixin;
    private Text text_pay;
    private Text text_txt_name;
   
    private void Awake()
    {
        button_scene_zhu = transform.GetComponent<Button>();
        image_Render = transform.Find("scene-beijingban/Render").GetComponent<Image>();
        image_shucaitu = transform.Find("shucaitu").GetComponent<Image>();
        image_scene_kuang = transform.Find("scene-kuang").GetComponent<Image>();
        image_scene_weijiesuo = transform.Find("scene-weijiesuo").GetComponent<Image>();
        image_icon_password = transform.Find("icon_password/icon_password").GetComponent<Image>();
        image_icon_star = transform.Find("icon-star/icon-star").GetComponent<Image>();
        image_jiagebeijing = transform.Find("jiagebeijing").GetComponent<Image>();
        image_BUTTON_aixin = transform.Find("jiage/BUTTON-aixin").GetComponent<Image>();
        text_pay = transform.Find("jiage/pay").GetComponent<Text>();
        text_txt_name = transform.Find("txt-name").GetComponent<Text>();

        OnAwake();
        AddEvent();

    }

    private void AddEvent()
    {
        button_scene_zhu.onClick.RemoveAllListeners();
        button_scene_zhu.onClick.AddListener(Onscene_zhuClicked);
    }

    private void Onscene_zhuClicked()
    {

    }


    private void OnAwake()
    {

    }



}
