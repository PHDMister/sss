using LogicOwner = IFsm<IFsmLogicManager>;

public abstract class FsmLogicBase : FsmState<IFsmLogicManager>
{
    /// <summary>
    /// 状态初始化时调用。
    /// </summary>
    /// <param name="logicOwner">逻辑持有者。</param>
    protected internal override void OnInit(LogicOwner logicOwner)
    {
        base.OnInit(logicOwner);
    }

    /// <summary>
    /// 进入状态时调用。
    /// </summary>
    /// <param name="logicOwner">逻辑持有者。</param>
    protected internal override void OnEnter(LogicOwner logicOwner)
    {
        base.OnEnter(logicOwner);
    }

    /// <summary>
    /// 状态轮询时调用。
    /// </summary>
    /// <param name="logicOwner">逻辑持有者。</param>
    protected internal override void OnUpdate(LogicOwner logicOwner)
    {
        base.OnUpdate(logicOwner);
    }

    /// <summary>
    /// 离开状态时调用。
    /// </summary>
    /// <param name="logicOwner">逻辑持有者。</param>
    /// <param name="isShutdown">是否是关闭状态机时触发。</param>
    protected internal override void OnLeave(LogicOwner logicOwner, bool isShutdown)
    {
        base.OnLeave(logicOwner, isShutdown);
    }

    /// <summary>
    /// 状态销毁时调用。
    /// </summary>
    /// <param name="logicOwner">逻辑持有者。</param>
    protected internal override void OnDestroy(LogicOwner logicOwner)
    {
        base.OnDestroy(logicOwner);
    }
}