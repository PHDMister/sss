using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UIFW;

public class PersonPicItem : BaseUIForm
{
    private RawImage icon_RawImage;
    private Text name_Text;
    private Text count_Text;
    private void Awake()
    {
        icon_RawImage = FindComp<RawImage>("ItemImage_RawImage");
        name_Text = FindComp<Text>("ItemName_Text");
        count_Text = FindComp<Text>("ItemCount_Text");
    }

    public void SetItemIcon(PersonPicture personPicture)
    {
        if (personPicture == null)
        {
            return;
        }
        if (personPicture.sprite != null && icon_RawImage != null)
        {
            icon_RawImage.texture = personPicture.sprite.texture;
        }
        name_Text.text = personPicture.product_title;
        count_Text.text = personPicture.num.ToString() + "¸ö";
    }

}
