/***
 * 
 *    Title: "UIFW" UI框架项目
 *           主题： 资源加载管理器      
 *    Description: 
 *           功能： 本功能是在Unity的Resources类的基础之上，增加了“缓存”的处理。
 *                  本脚本适用于
 *    Date: 2017
 *    Version: 0.1版本
 *    Modify Recoder: 
 *    
 *   
 */
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.U2D;
using UnityEngine.Networking;
using System.IO;

namespace UIFW
{
    public class ResourcesMgr : MonoBehaviour
    {
        /* 字段 */
        private static ResourcesMgr _Instance;              //本脚本私有单例实例
        private Hashtable ht = null;                        //容器键值对集合
        private Hashtable htFromBundle = null;              //使用bundle管理的模块化容器键值对集合


        /// <summary>
        /// 得到实例(单例)
        /// </summary>
        /// <returns></returns>
        public static ResourcesMgr GetInstance()
        {
            if (_Instance == null)
            {
                _Instance = new GameObject("_ResourceMgr").AddComponent<ResourcesMgr>();
            }
            return _Instance;
        }

        void Awake()
        {
            ht = new Hashtable();
            htFromBundle = new Hashtable();
        }

        /// <summary>
        /// 调用资源（带对象缓冲技术）
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="path"></param>
        /// <param name="isCatch"></param>
        /// <returns></returns>
        public T LoadResource<T>(string path, bool isCatch) where T : UnityEngine.Object
        {
            if (ht.Contains(path))
            {
                return ht[path] as T;
            }

            T TResource = Resources.Load<T>(path);
            if (TResource == null)
            {
                Debug.LogError(GetType() + "/GetInstance()/TResource 提取的资源找不到，请检查。 path=" + path);
            }
            else if (isCatch)
            {
                ht.Add(path, TResource);
            }

            return TResource;
        }

        /// <summary>
        /// 调用资源（带对象缓冲技术）
        /// </summary>
        /// <param name="path"></param>
        /// <param name="isCatch"></param>
        /// <returns></returns>
        public GameObject LoadAsset(string path, bool isCatch)
        {
            GameObject goObj = LoadResource<GameObject>(path, isCatch);
            GameObject goObjClone = GameObject.Instantiate<GameObject>(goObj);
            if (goObjClone == null)
            {
                Debug.LogError(GetType() + "/LoadAsset()/克隆资源不成功，请检查。 path=" + path);
            }
            //goObj = null;//??????????
            return goObjClone;
        }

        /// <summary>
        /// 加载图集下Sprite
        /// </summary>
        /// <param name="atlasName"></param>
        /// <param name="spriteName"></param>
        /// <returns></returns>
        public Sprite LoadSprrite(string atlasName,string spriteName)
        {
            SpriteAtlas atlas = Resources.Load<SpriteAtlas>(string.Format("UIRes/Atlas/{0}", atlasName));
            if (atlas == null)
                return null;
            Sprite sprite = atlas.GetSprite(spriteName);
            return sprite;
        }

        #region 模块化资源对业务层开放接口

        /// <summary>
        /// 调用属于当前模块的某个bundle内的某个资源（带对象缓冲技术）
        /// </summary>
        /// <returns></returns>
        public GameObject LoadAssetInBundle(string bundleName, string assetName, bool isCache)
        {
            GameObject goObj = LoadResourceInBundle<GameObject>(bundleName, assetName, isCache);
            GameObject goObjClone = GameObject.Instantiate<GameObject>(goObj);
            if (goObjClone == null)
            {
                Debug.LogError($"{GetType()} /LoadAssetInBundle()  克隆资源不成功 请检查 bundleName:{bundleName} assetName:{assetName}");
            }
            //goObj = null;//??????????
            return goObjClone;
        }

        /// <summary>
        /// 加载属于当前模块的某个bundle内的某个图集下Sprite
        /// </summary>
        public Sprite LoadSpriteInBundle(string bundleName, string atlasName, string spriteName)
        {
            SpriteAtlas atlas = LoadResourceInBundle<SpriteAtlas>(bundleName, atlasName, false);
            if (atlas == null)
            {
                Debug.LogError($"{GetType()} /LoadSpriteInBundle()  获取图集不成功 请检查 bundleName:{bundleName} atlasName:{atlasName}");
                return null;
            }
            Sprite sprite = atlas.GetSprite(spriteName);
            if (sprite == null)
            {
                Debug.LogError($"{GetType()} /LoadSpriteInBundle()  获取图集内单个Sprite不成功 请检查 bundleName:{bundleName} atlasName:{atlasName} spriteName:{spriteName}");

                return null;
            }
            return sprite;
        }
        /// <summary>
        /// 调用资源（带对象缓冲技术）
        /// </summary>
        public T LoadResourceInBundle<T>(string bundleName, string assetName, bool isCache) where T : UnityEngine.Object
        {
            string htKey = bundleName + "_" + assetName;
            if (htFromBundle.Contains(htKey))
            {
                return htFromBundle[htKey] as T;
            }

            AssetBundle ab = GetLoadedBundleByName(bundleName);
            if (ab == null)
            {
                Debug.LogError($"{GetType()} /LoadResourceInBundle()/TResource 想要提取的asset: {assetName} 所属的bundle: {bundleName} 找不到，请检查。");
                return null;
            }

            T TResource = ab.LoadAsset<T>(assetName);
            if (TResource == null)
            {
                Debug.LogError($"{GetType()} /LoadResourceInBundle()/TResource 想要提取的asset: {assetName} 在bundle: {bundleName} 内部找不到，请检查。");
                return null;
            }

            if (isCache)
            {
                htFromBundle.Add(htKey, TResource);
            }

            return TResource;
        }
        #endregion

        #region 模块化资源相关非业务层接口
        public void ClearHTFromBundle()
        {
            htFromBundle.Clear();
        }
      

        private AssetBundle GetLoadedBundleByName(string bundleName)
        {
            var allBundles = AssetBundle.GetAllLoadedAssetBundles();
            foreach (var bundle in allBundles)
            {
                //Debug.Log($"bundle name: {bundle.name}");
                if (bundle.name == bundleName)
                {
                    return bundle;
                }
            }
            return null;
        }
        #endregion




    }//Class_end



}