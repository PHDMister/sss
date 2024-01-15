using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public class ResourceController
{
    /// <summary>
    /// 根据名字查找所有Resource文件夹的物体
    /// </summary>
    /// <param name="ObjName"></param>
    /// <returns></returns>
    public GameObject ResLoadObjByNameFun(string ObjName)
    {
        return Resources.Load(ObjName, typeof(GameObject)) as GameObject;
    }
    /// <summary>
    /// 指定物品类型路径下名字查找文件夹的物体
    /// </summary>
    /// <param name="ObjName"></param>
    /// <returns></returns>
    public GameObject ResLoadObjByPathNameFun(string ObjName)
    {
        return Resources.Load("Model/" + ObjName, typeof(GameObject)) as GameObject;
    }
    /// <summary>
    /// 根据名字查找所有resouce文件夹下的texture贴图
    /// </summary>
    /// <param name="textureName"></param>
    /// <returns></returns>
    public Texture ResLoadTextureByNameFun(string textureName)
    {
        return Resources.Load(textureName, typeof(Texture)) as Texture;
    }
    /// <summary>
    ///  指定物品类型路径下的Icon图
    /// </summary>
    /// <param name="textureName"></param>
    /// <returns></returns>
    public Sprite ResLoadIconByPathNameFun(string textureName)
    {
        Debug.Log("输出一下这里的icon的图的名称：  " + textureName);
        return Resources.Load("UIRes/UISprite/Icon/" + textureName, typeof(Sprite)) as Sprite;

        /*  SpriteAtlas atlas = Resources.Load<SpriteAtlas>("UIRes/Atlas/Icon");
          Sprite sprite = atlas.GetSprite(textureName);
          return sprite;*/
    }

    /// <summary>
    ///  指定物品类型路径下的Icon图(Texture2d格式)
    /// </summary>
    /// <param name="textureName"></param>
    /// <returns></returns>
    public Texture2D ResLoadIconTextureByPathNameFun(string textureName)
    {
        Debug.Log("输出一下这里的icon的图的名称：  " + textureName);
        return Resources.Load("UIRes/UISprite/Icon/" + textureName, typeof(Texture2D)) as Texture2D;

        /*  SpriteAtlas atlas = Resources.Load<SpriteAtlas>("UIRes/Atlas/Icon");
          Sprite sprite = atlas.GetSprite(textureName);
          return sprite;*/
    }


    /// <summary>
    /// 指定物品类型路径下的Shop图
    /// </summary>
    /// <param name="textureName"></param>
    /// <returns></returns>
    public Sprite ResLoadShopByPathNameFun(string textureName)
    {
        // return Resources.Load("UIRes/UISprite/Shop/" + textureName, typeof(Sprite)) as Sprite;

        SpriteAtlas atlas = Resources.Load<SpriteAtlas>("UIRes/Atlas/Shop");
        Sprite sprite = atlas.GetSprite(textureName);
        return sprite;
    }


    /// <summary>
    /// 指定物品类型路径下的PetHUD图
    /// </summary>
    /// <param name="textureName"></param>
    /// <returns></returns>
    public Sprite ResLoadPetHUDByPathNameFun(string textureName)
    {
        return Resources.Load("UIRes/UISprite/PetHUD/" + textureName, typeof(Sprite)) as Sprite;

        /*  SpriteAtlas atlas = Resources.Load<SpriteAtlas>("UIRes/Atlas/PetHUD");
          Sprite sprite = atlas.GetSprite(textureName);
          return sprite;*/
    }


    /// <summary>
    /// 指定物品类型路径下的Common图
    /// </summary>
    /// <param name="textureName"></param>
    /// <returns></returns>
    public Sprite ResLoadCommonByPathNameFun(string textureName)
    {
        //  return Resources.Load("UIRes/UISprite/Common/" + textureName, typeof(Sprite)) as Sprite;

        SpriteAtlas atlas = Resources.Load<SpriteAtlas>("UIRes/Atlas/Common");
        Sprite sprite = atlas.GetSprite(textureName);
        return sprite;
    }


    /// <summary>
    /// 指定物品类型路径下的Joystick图
    /// </summary>
    /// <param name="textureName"></param>
    /// <returns></returns>
    public Sprite ResLoadJoyStickByPathNameFun(string textureName)
    {
        return Resources.Load("UIRes/UISprite/JoyStick/" + textureName, typeof(Sprite)) as Sprite;

        /*  SpriteAtlas atlas = Resources.Load<SpriteAtlas>("UIRes/Atlas/JoyStick");
          Sprite sprite = atlas.GetSprite(textureName);
          return sprite;*/
    }

    /// <summary>
    /// 指定物品类型路径下的HUD图
    /// </summary>
    /// <param name="textureName"></param>
    /// <returns></returns>
    public Sprite ResLoadHUDByPathNameFun(string textureName)
    {
        return Resources.Load("UIRes/UISprite/HUD/" + textureName, typeof(Sprite)) as Sprite;

        /*  SpriteAtlas atlas = Resources.Load<SpriteAtlas>("UIRes/Atlas/HUD");
          Sprite sprite = atlas.GetSprite(textureName);
          return sprite;*/
    }



    public Sprite ResLoadPetShopIconFun(string textureName)
    {
        return Resources.Load("UIRes/Texture/PetShopIcon/" + textureName, typeof(Sprite)) as Sprite;
    }
    /// <summary>
    /// 根据名字查找所有resouce文件夹下的texture贴图
    /// </summary>
    /// <param name="textureName"></param>
    /// <returns></returns>
    public Sprite ResLoadSpriteByNameFun(string spriteName)
    {
        return Resources.Load(spriteName, typeof(Sprite)) as Sprite;
    }
    /// <summary>
    ///  指定物品类型路径下的sprite图
    /// </summary>
    /// <param name="textureName"></param>
    /// <returns></returns>
    public Sprite ResLoadSpriteByPathNameFun(string spriteName)
    {
        return Resources.Load("Sprite" + spriteName, typeof(Sprite)) as Sprite;
    }
    /// <summary>
    /// 根据名字查找所有resouce文件夹下的texture贴图
    /// </summary>
    /// <param name="textureName"></param>
    /// <returns></returns>
    public Material ResLoadMaterialByNameFun(string spriteName)
    {
        return Resources.Load(spriteName, typeof(Material)) as Material;
    }
    /// <summary>
    ///  指定物品类型路径下的sprite图
    /// </summary>
    /// <param name="textureName"></param>
    /// <returns></returns>
    public Material ResLoadMaterialByPathNameFun(string spriteName)
    {
        return Resources.Load("Material/" + spriteName, typeof(Material)) as Material;
    }

    /// <summary>
    /// 查找衣服
    /// </summary>
    /// <param name="ObjName"></param>
    /// <returns></returns>
    public GameObject ResLoadAvatarByTypeFun(int avatar_Type2, string ObjName)
    {
        string folderPath = "";
        switch ((Avatar_Type2)avatar_Type2)
        {
            case Avatar_Type2.suit:
                folderPath = "Cloth";
                break;
            case Avatar_Type2.hair:
                folderPath = "Hair";
                break;
            case Avatar_Type2.glasses:
                folderPath = "Glasses";
                break;
            case Avatar_Type2.earring:
                folderPath = "Earring";
                break;
            case Avatar_Type2.necklace:
                folderPath = "Necklace";
                break;
            case Avatar_Type2.Coat:
                folderPath = "Coat";
                break;
            case Avatar_Type2.Pants:
                folderPath = "Pants";
                break;
            case Avatar_Type2.shoes:
                folderPath = "Shoes";
                break;
            case Avatar_Type2.wrist:
                folderPath = "Wrist";
                break;
            case Avatar_Type2.eyebrow:
                folderPath = "Eyebrow";
                break;
            default:
                break;
        }
        string loadPath = string.Format("Prefabs/CharacterChanging/{0}/{1}", folderPath, ObjName);
        return Resources.Load(loadPath, typeof(GameObject)) as GameObject;
    }
    /// <summary>
    /// 查找躯体
    /// </summary>
    /// <param name="bodyName"></param>
    /// <returns></returns>
    public GameObject ResLoadBodyByTypeFun(int bodyType)
    {
        string bodyName = "";
        switch ((BodyType)bodyType)
        {
            case BodyType.Arm:
                bodyName = "DefaultArm";
                break;
            case BodyType.Hand:
                bodyName = "DefaultHand";
                break;
            case BodyType.Head:
                bodyName = "DefaultHead";
                break;
            case BodyType.Leg:
                bodyName = "DefaultLeg";
                break;
            default:
                break;
        }
        string loadPath = string.Format("Prefabs/CharacterChanging/Body/{0}", bodyName);
        return Resources.Load(loadPath, typeof(GameObject)) as GameObject;
    }

}
