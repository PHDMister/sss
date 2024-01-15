using System;

public interface IFsmLogicManager
{
    /// <summary>
    /// 获取当前逻辑。
    /// </summary>
    FsmLogicBase CurrentLogic
    {
        get;
    }

    /// <summary>
    /// 获取当前逻辑持续时间。
    /// </summary>
    float CurrentLogicTime
    {
        get;
    }

    /// <summary>
    /// 初始化逻辑管理器。
    /// </summary>
    /// <param name="fsmManager">有限状态机管理器。</param>
    /// <param name="logics">逻辑管理器包含的逻辑。</param>
    void Initialize(IFsmManager fsmManager, params FsmLogicBase[] logics);

    /// <summary>
    /// 开始逻辑。
    /// </summary>
    /// <typeparam name="T">要开始的逻辑类型。</typeparam>
    void StartLogic<T>() where T : FsmLogicBase;

    /// <summary>
    /// 开始逻辑。
    /// </summary>
    /// <param name="logicType">要开始的逻辑类型。</param>
    void StartLogic(Type logicType);

    /// <summary>
    /// 是否存在逻辑。
    /// </summary>
    /// <typeparam name="T">要检查的逻辑类型。</typeparam>
    /// <returns>是否存在逻辑。</returns>
    bool HasLogic<T>() where T : FsmLogicBase;

    /// <summary>
    /// 是否存在逻辑。
    /// </summary>
    /// <param name="logicType">要检查的逻辑类型。</param>
    /// <returns>是否存在逻辑。</returns>
    bool HasLogic(Type logicType);

    /// <summary>
    /// 获取逻辑。
    /// </summary>
    /// <typeparam name="T">要获取的逻辑类型。</typeparam>
    /// <returns>要获取的逻辑。</returns>
    FsmLogicBase GetLogic<T>() where T : FsmLogicBase;

    /// <summary>
    /// 获取逻辑。
    /// </summary>
    /// <param name="logicType">要获取的逻辑类型。</param>
    /// <returns>要获取的逻辑。</returns>
    FsmLogicBase GetLogic(Type logicType);
}