using UIFW;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityTimer;

public class TreasureBox : MonoBehaviour, IPointerClickHandler
{
    private float lastClickTime = 0;
    private Timer openTimer = null;
    private Animator animator;
    public bool IsAutoOpen = true;
    [Range(1, 5)]
    public float AutoOpenInterMultiple = 1;
    private Timer reTimer;
    protected void Awake()
    {
        animator = GetComponent<Animator>();
    }

    protected void Start()
    {
        if (IsAutoOpen)
        {
            lastClickTime = Time.realtimeSinceStartup;
            float time = animator.GetAnimLength("Idle") * AutoOpenInterMultiple;
            reTimer = Timer.RegisterRealTimeNoLoop(time, () =>
            {
                reTimer = null;
                lastClickTime = 0;
                OnPointerClick(null);
            });
        }
    }

    protected void OnDestroy()
    {
        if (reTimer != null) reTimer.Cancel();
        reTimer = null;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (Time.realtimeSinceStartup - lastClickTime < 5) return;
        lastClickTime = Time.realtimeSinceStartup;

        if (!UIManager.GetInstance().IsOpend(FormConst.TREASUREREWARDPANEL))
        {
            animator.ResetTrigger("Idle");
            animator.SetTrigger("Open");

            if (openTimer == null)
            {
                float time = animator.GetAnimLength("Open");
                openTimer = Timer.RegisterRealTimeNoLoop(time, () =>
                {
                    openTimer = null;
                    Singleton<TreasuringController>.Instance.SetTeamPlayerAnim("Idle");
                    if (!UIManager.GetInstance().IsOpend(FormConst.TREASUREREWARDPANEL))
                        UIManager.GetInstance().ShowUIForms(FormConst.TREASUREREWARDPANEL);
                    this.enabled = false;
                    GameObject.Destroy(this.gameObject, 1);
                });
            }
        }
    }

}
