//***循环列表基类***//

//初始化：
//   Init(callbackFun)
//刷新整个列表
//    ShowList(int=数量)
//刷新单个项
// UpdataCell(int =索引)
//刷新列表数据（无数量变化时调用）
//     UpdataList()
//回调：
//Func(GameObject=Cell,int =Index)  //刷新列表
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace CircularScrollView
{
    public class UIUtils
    {
        public static void SetActive(GameObject obj, bool isActive)
        {
            if (obj != null)
            {
                obj.SetActive(isActive);
            }
        }
    }
    public enum e_Direction
    {
        Horizontal,
        Vertical
    }
    public class UICircularScrollView : MonoBehaviour
    {
        public delegate void ListenerHandler(Vector2 a);
        public event ListenerHandler Listener = null;

        public GameObject m_PointingFirstArrow;
        public GameObject m_PointingEndArrow;

        protected Action<GameObject, int> m_FuncCallBackFunc;
        protected Action<GameObject, int> m_FuncOnClickCallBack;

        protected GameObject m_Content;
        protected RectTransform m_ContentRectTrans;

        public int m_Row = 1;

        //预制体的宽和高
        protected float m_CellObjectWidth;
        protected float m_CellObjectHeight;
        // scrollView的宽高
        protected float m_PlaneWidth;
        protected float m_PlaneHeight;
        // scrollView中的content的宽高
        protected float m_ContentWidth;
        protected float m_ContentHeight;
        //间距
        public float m_Spacing = 0f;
        //Y轴间距
        public float m_SpacingY = 0f;
        public float m_ContentOffsetY = 0;
        protected RectTransform rectTrans;
        //本身的scrollect组件
        protected ScrollRect m_ScrollRect;
        public GameObject m_CellGameObject; //指定的cell也就是列表中的预制体
        public e_Direction m_Direction = e_Direction.Horizontal;

        public bool m_IsShowArrow = true;

        protected int m_MaxCount = -1; //列表数量
        protected int m_MinIndex = -1;
        protected int m_MaxIndex = -1;
        protected bool m_IsClearList = false; //是否清空列表

        protected bool m_Isinited = false;

        protected struct CellInfo
        {
            public Vector3 pos;
            public GameObject obj;
        };

        //对象池 
        protected Stack<GameObject> poolsObj = new Stack<GameObject>();
        //所有的预制体
        protected CellInfo[] m_CellInfos;

        public virtual void Init(Action<GameObject, int> callBack)
        {
            Init(callBack, null);
        }
        public virtual void Init(Action<GameObject, int> callBack, Action<GameObject, int> OnClickCallBack)
        {
            //初始化事件
            DisposeAll();
            m_FuncCallBackFunc = callBack;
            if (OnClickCallBack != null)
            {
                m_FuncOnClickCallBack = OnClickCallBack;
            }
            if (m_Isinited)
            {
                return;
            }
            m_Content = this.GetComponent<ScrollRect>().content.gameObject;
            if (m_CellGameObject == null)
            {
                m_CellGameObject = m_Content.transform.GetChild(0).gameObject;
            }
            SetPoolsObj(m_CellGameObject);
            RectTransform cellRectTrans = m_CellGameObject.GetComponent<RectTransform>();
            cellRectTrans.pivot = new Vector2(0f, 1f);
            //检查anchor是否正确
            CheckAnchor(cellRectTrans);
            cellRectTrans.anchoredPosition = Vector2.zero;
            //记录cell信息
            m_CellObjectHeight = cellRectTrans.rect.height;
            m_CellObjectWidth = cellRectTrans.rect.width;
            //记录panel信息
            rectTrans = GetComponent<RectTransform>();
            Rect planeRect = rectTrans.rect;
            m_PlaneHeight = planeRect.height;
            m_PlaneWidth = planeRect.width;
            //记录content信息
            m_ContentRectTrans = m_Content.GetComponent<RectTransform>();
            Rect contentRect = m_ContentRectTrans.rect;
            m_ContentHeight = contentRect.height;
            m_ContentWidth = contentRect.width;
            m_ContentRectTrans.pivot = new Vector2(0f, 1f);
            //检查anchor是否正确
            CheckAnchor(m_ContentRectTrans);
            m_ScrollRect = this.GetComponent<ScrollRect>();
            //先去除掉所有的监听
            m_ScrollRect.onValueChanged.RemoveAllListeners();
            //添加滑动事件
            m_ScrollRect.onValueChanged.AddListener(delegate (Vector2 value) { ScrollRectListener(value); });
            m_Isinited = true;
        }
        /// <summary>
        /// 刷新整个列表
        /// </summary>
        /// <param name="num"></param>
        public virtual void ShowList(int num)
        {
            m_MinIndex = -1;
            m_MaxIndex = -1;
            //计算content尺寸
            SetContentSizeFun(num);
            //计算开始索引
            int lastEndIndex = 0;
            // 过多的物体扔到对象池（首次调用showList函数时，则无效）
            if (m_Isinited)
            {
                lastEndIndex = num - m_MaxCount > 0 ? m_MaxCount : num;
                lastEndIndex = m_IsClearList ? 0 : lastEndIndex;
                int count = m_IsClearList ? m_CellInfos.Length : m_MaxCount;
                for (int i = lastEndIndex; i < count; i++)
                {
                    if (m_CellInfos[i].obj != null)
                    {
                        SetPoolsObj(m_CellInfos[i].obj);
                        m_CellInfos[i].obj = null;
                    }
                }
            }
            CellInfo[] tempCellInfos = m_CellInfos;
            m_CellInfos = new CellInfo[num];

            for (int i = 0; i < num; i++)
            {
                if (m_MaxCount != -1 && i < lastEndIndex)
                {
                    CellInfo tempCellinfo = tempCellInfos[i];
                    float rPos = m_Direction == e_Direction.Vertical ? tempCellinfo.pos.y : tempCellinfo.pos.x;
                    if (!IsOutRange(rPos))
                    {
                        m_MinIndex = m_MinIndex == -1 ? i : m_MinIndex;
                        m_MaxIndex = i;

                        if (tempCellinfo.obj == null)
                        {
                            //从对象池取对象
                            tempCellinfo.obj = GetPoolObj();
                        }
                        tempCellinfo.obj.transform.GetComponent<RectTransform>().anchoredPosition = tempCellinfo.pos;
                        tempCellinfo.obj.name = i.ToString();
                        tempCellinfo.obj.SetActive(true);
                        Func(m_FuncCallBackFunc, tempCellinfo.obj);
                    }
                    else
                    {
                        SetPoolsObj(tempCellinfo.obj);
                        tempCellinfo.obj = null;
                    }
                    m_CellInfos[i] = tempCellinfo;
                    continue;
                }
                CellInfo cellInfo = new CellInfo();
                float pos = 0;
                float rowPos = 0;//预制体在每排里面的坐标
                if (m_Direction == e_Direction.Vertical)
                {
                    pos = m_CellObjectHeight * Mathf.FloorToInt(i / m_Row) + (m_Spacing + m_SpacingY) * Mathf.FloorToInt(i / m_Row);
                    rowPos = m_CellObjectWidth * (i % m_Row) + m_Spacing * (i % m_Row);
                    cellInfo.pos = new Vector3(rowPos, -pos, 0);
                }
                else
                {
                    pos = m_CellObjectWidth * Mathf.FloorToInt(i / m_Row) + m_Spacing * Mathf.FloorToInt(i / m_Row);
                    rowPos = m_CellObjectHeight * (i % m_Row) + m_Spacing * (i % m_Row);
                    cellInfo.pos = new Vector3(pos, -rowPos, 0);
                }
                //计算是否超出范围
                float cellPos = m_Direction == e_Direction.Vertical ? cellInfo.pos.y : cellInfo.pos.x;
                if (IsOutRange(cellPos))
                {
                    cellInfo.obj = null;
                    m_CellInfos[i] = cellInfo;
                    continue;
                }
                //->记录显示范围中的首位index和末尾index
                m_MinIndex = m_MinIndex == -1 ? i : m_MinIndex;//首位index
                m_MaxIndex = i;
                //取cell
                GameObject cell = GetPoolObj();
                cell.transform.GetComponent<RectTransform>().anchoredPosition = cellInfo.pos;
                cell.gameObject.name = i.ToString();
                //->存数据
                cellInfo.obj = cell;
                m_CellInfos[i] = cellInfo;
                //->回调函数
                Func(m_FuncCallBackFunc, cell);
            }
            m_MaxCount = num;
            m_Isinited = true;
        }
        /// <summary>
        /// 设置content尺寸
        /// </summary>
        /// <param name="num"></param>
        void SetContentSizeFun(int num)
        {
            if (m_Direction == e_Direction.Vertical)
            {
                float contentSize = (m_Spacing + m_SpacingY + m_ContentOffsetY + m_CellObjectHeight) * Mathf.CeilToInt((float)num / m_Row);
                if (Mathf.CeilToInt((float)num / m_Row) >= 1)
                {
                    contentSize -= m_Spacing + m_SpacingY + m_ContentOffsetY;
                }
                m_ContentHeight = contentSize;
                m_ContentWidth = m_ContentRectTrans.sizeDelta.x;
                contentSize = contentSize < rectTrans.rect.height ? rectTrans.rect.height : contentSize;
                m_ContentRectTrans.sizeDelta = new Vector2(m_ContentWidth, contentSize);
                if (num != m_MaxCount)
                {
                    m_ContentRectTrans.anchoredPosition = new Vector2(m_ContentRectTrans.anchoredPosition.x, 0);
                }
            }
            else
            {
                float contentSize = (m_Spacing + m_CellObjectWidth) * Mathf.CeilToInt((float)num / m_Row);
                if (Mathf.CeilToInt((float)num / m_Row) >= 1)
                {
                    contentSize -= m_Spacing;
                }
                m_ContentWidth = contentSize;
                m_ContentHeight = m_ContentRectTrans.sizeDelta.x;
                contentSize = contentSize < rectTrans.rect.width ? rectTrans.rect.width : contentSize;
                m_ContentRectTrans.sizeDelta = new Vector2(contentSize, m_ContentHeight);
                if (num != m_MaxCount)
                {
                    m_ContentRectTrans.anchoredPosition = new Vector2(0, m_ContentRectTrans.anchoredPosition.y);
                }
            }
        }
        /// <summary>
        /// 滑动事件
        /// </summary>
        /// <param name="value"></param>
        protected virtual void ScrollRectListener(Vector2 value)
        {
            UpdateCheck();
            if (Listener != null)
            {
                Listener(value);
            }
        }
        /// <summary>
        /// 滑动事件方法
        /// </summary>
        private void UpdateCheck()
        {
            if (m_CellInfos == null)
            {
                return;
            }

            //检查超出范围
            for (int i = 0; i < m_CellInfos.Length; i++)
            {
                CellInfo cellInfo = m_CellInfos[i];

                GameObject obj = cellInfo.obj;
                Vector3 pos = cellInfo.pos;
                float rangePos = m_Direction == e_Direction.Vertical ? pos.y : pos.x;
                if (IsOutRange(rangePos))
                {
                    //把超出范围的cell扔进poolsObj里
                    if (obj != null)
                    {
                        SetPoolsObj(obj);
                        m_CellInfos[i].obj = null;
                    }
                }
                else
                {
                    if (obj == null)
                    {
                        //从对象池中取出
                        GameObject cell = GetPoolObj();
                        cell.transform.localPosition = pos;
                        cell.gameObject.name = i.ToString();
                        m_CellInfos[i].obj = cell;
                        Func(m_FuncCallBackFunc, cell);
                    }
                }
            }
        }



        /// <summary>
        /// 刷新所有列表
        /// </summary>
        public virtual void UpdateList()
        {
            if (m_CellInfos == null)
            {
                Debug.Log("这里为空的值");
            }
            else
            {
                Debug.Log("食醋胡嫌疑啊的付款的酷酷酷酷酷L " + m_CellInfos.Length);

            }
            for (int i = 0; i < m_CellInfos.Length; i++)
            {
                CellInfo cellinfo = m_CellInfos[i];
                if (cellinfo.obj != null)
                {
                    float rangPos = m_Direction == e_Direction.Vertical ? cellinfo.pos.y : cellinfo.pos.x;
                    if (!IsOutRange(rangPos))
                    {
                        Func(m_FuncCallBackFunc, cellinfo.obj, true);
                    }
                }
            }
        }
        /// <summary>
        /// 刷新其中一项列表
        /// </summary>
        /// <param name="index"></param>
        public void UpdateCell(int index)
        {
            CellInfo cellInfo = m_CellInfos[index - 1];
            if (cellInfo.obj != null)
            {
                float rangePos = m_Direction == e_Direction.Vertical ? cellInfo.pos.y : cellInfo.pos.x;
                if (!IsOutRange(rangePos))
                {
                    Func(m_FuncCallBackFunc, cellInfo.obj, true);
                }
            }
        }
        /// <summary>
        /// 更新滚动区域大小
        /// </summary>
        public void UpdateSize()
        {
            Rect rect = GetComponent<RectTransform>().rect;
            m_PlaneHeight = rect.height;
            m_PlaneWidth = rect.width;
        }



        protected void OnDragListener(Vector2 value)
        {
            float normalizedPos = m_Direction == e_Direction.Vertical ? m_ScrollRect.verticalNormalizedPosition : m_ScrollRect.horizontalNormalizedPosition;

            if (m_Direction == e_Direction.Vertical)
            {
                if (m_ContentHeight - rectTrans.rect.height < 10)
                {
                    UIUtils.SetActive(m_PointingFirstArrow, false);
                    UIUtils.SetActive(m_PointingEndArrow, false);
                    return;
                }
            }
            else
            {
                if (m_ContentWidth - rectTrans.rect.width < 10)
                {
                    UIUtils.SetActive(m_PointingFirstArrow, false);
                    UIUtils.SetActive(m_PointingEndArrow, false);
                    return;
                }
            }
            if (normalizedPos >= 0.9)
            {
                UIUtils.SetActive(m_PointingFirstArrow, false);
                UIUtils.SetActive(m_PointingEndArrow, true);
            }
            else if (normalizedPos <= 0.1)
            {
                UIUtils.SetActive(m_PointingFirstArrow, true);
                UIUtils.SetActive(m_PointingEndArrow, false);
            }
            else
            {
                UIUtils.SetActive(m_PointingFirstArrow, true);
                UIUtils.SetActive(m_PointingEndArrow, true);
            }
        }


        /// <summary>
        /// 检查 Anchor 是否正确
        /// </summary>
        /// <param name="rectTrans"></param>
        private void CheckAnchor(RectTransform rectTrans)
        {
            if (m_Direction == e_Direction.Vertical)
            {
                if (!((rectTrans.anchorMin == new Vector2(0, 1) && rectTrans.anchorMax == new Vector2(0, 1)) ||
                         (rectTrans.anchorMin == new Vector2(0, 1) && rectTrans.anchorMax == new Vector2(1, 1))))
                {
                    rectTrans.anchorMin = new Vector2(0, 1);
                    rectTrans.anchorMax = new Vector2(1, 1);
                }
            }
            else
            {
                if (!((rectTrans.anchorMin == new Vector2(0, 1) && rectTrans.anchorMax == new Vector2(0, 1)) ||
                         (rectTrans.anchorMin == new Vector2(0, 0) && rectTrans.anchorMax == new Vector2(0, 1))))
                {
                    rectTrans.anchorMin = new Vector2(0, 0);
                    rectTrans.anchorMax = new Vector2(0, 1);
                }
            }
        }

        /// <summary>
        /// 判断是否超出显示范围
        /// </summary>
        /// <param name="pos"></param>
        /// <returns></returns>
        protected bool IsOutRange(float pos)
        {
            Vector3 contPos = m_ContentRectTrans.anchoredPosition;

            if (m_Direction == e_Direction.Vertical)
            {
                if (pos + contPos.y > m_CellObjectHeight || pos + contPos.y < -rectTrans.rect.width)
                {
                    return true;
                }
            }
            else
            {
                if (pos + contPos.x < -m_CellObjectWidth || pos + contPos.x > rectTrans.rect.width)
                {
                    return true;
                }
            }
            return false;
        }
        public void DisposeAll()
        {
            if (m_FuncCallBackFunc != null)
            {
                m_FuncCallBackFunc = null;
            }
            if (m_FuncOnClickCallBack != null)
            {
                m_FuncOnClickCallBack = null;
            }
        }
        protected void OnDestroy()
        {
            DisposeAll();
        }
        /// <summary>
        /// 这个回调用来刷新数据
        /// </summary>
        /// <param name="func"></param>
        /// <param name="selectObject"></param>
        /// <param name="isUpdate"></param>
        protected void Func(Action<GameObject, int> func, GameObject selectObject, bool isUpdate = false)
        {
            int num = int.Parse(selectObject.name) + 1;
            if (func != null)
            {
                func(selectObject, num);
            }
        }

        /// <summary>
        /// 对象池取出
        /// </summary>
        /// <returns></returns>
        protected virtual GameObject GetPoolObj()
        {
            GameObject cell = null;
            if (poolsObj.Count > 0)
            {
                cell = poolsObj.Pop();
            }
            if (cell == null)
            {
                cell = Instantiate(m_CellGameObject) as GameObject;
            }
            //设置父物体
            cell.transform.SetParent(m_Content.transform);
            cell.transform.localScale = Vector3.one;
            UIUtils.SetActive(cell, true);
            return cell;
        }
        /// <summary>
        /// 对象池存入
        /// </summary>
        /// <param name="cell"></param>
        public virtual void SetPoolsObj(GameObject cell)
        {
            if (cell != null)
            {
                poolsObj.Push(cell);
                UIUtils.SetActive(cell, false);
            }
        }

        public void ResetScrollRect()
        {
            if (m_ScrollRect != null)
                m_ScrollRect.normalizedPosition = new Vector2(0, 1);
        }
    }
}


