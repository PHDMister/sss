using System;
using System.Linq;

public class FsmLogicManager: IFsmLogicManager, ISingleton
{
    private IFsmManager m_FsmManager;
    private IFsm<IFsmLogicManager> m_LogicFsm;

    private bool m_Initialized;
    /// <summary>
    /// 初始化逻辑管理器的新实例。
    /// </summary>
    public FsmLogicManager()
    {
        m_FsmManager = null;
        m_LogicFsm = null;
        m_Initialized = false;
    }
    
    /// <summary>
    /// 获取当前逻辑。
    /// </summary>
    public FsmLogicBase CurrentLogic
    {
        get
        {
            if (m_LogicFsm == null)
            {
                throw new Exception("You must initialize logic first.");
            }

            return (FsmLogicBase)m_LogicFsm.CurrentState;
        }
    }
    
    /// <summary>
    /// 获取当前逻辑持续时间。
    /// </summary>
    public float CurrentLogicTime
    {
        get
        {
            if (m_LogicFsm == null)
            {
                throw new Exception("You must initialize logic first.");
            }

            return m_LogicFsm.CurrentStateTime;
        }
    }
    
    /// <summary>
    /// 关闭并清理逻辑管理器。
    /// </summary>
    internal void Shutdown()
    {
        if (m_FsmManager != null)
        {
            if (m_LogicFsm != null)
            {
                m_FsmManager.DestroyFsm(m_LogicFsm);
                m_LogicFsm = null;
            }

            m_FsmManager = null;
        }
        m_Initialized = false;
    }

    public void Initialize(IFsmManager fsmManager, params FsmLogicBase[] logics)
    {
        if (fsmManager == null)
        {
            throw new Exception("FSM manager is invalid.");
        }

        m_FsmManager = fsmManager;
        m_LogicFsm = m_FsmManager.CreateFsm(this, logics);
        
    }

    public void StartLogic<T>() where T : FsmLogicBase
    {
        if (!m_Initialized)
        {
            Initialize();
        }
        if (m_LogicFsm == null)
        {
            throw new Exception("You must initialize logic first.");
        }

        m_LogicFsm.Start<T>();
    }

    public void StartLogic(Type logicType)
    {
        if (!m_Initialized)
        {
            Initialize();
        }
        if (m_LogicFsm == null)
        {
            throw new Exception("You must initialize logic first.");
        }
        m_LogicFsm.Start(logicType);
    }

    public bool HasLogic<T>() where T : FsmLogicBase
    {
        if (m_LogicFsm == null)
        {
            throw new Exception("You must initialize logic first.");
        }

        return m_LogicFsm.HasState<T>();
    }

    public bool HasLogic(Type logicType)
    {
        if (m_LogicFsm == null)
        {
            throw new Exception("You must initialize logic first.");
        }

        return m_LogicFsm.HasState(logicType);
    }

    public FsmLogicBase GetLogic<T>() where T : FsmLogicBase
    {
        if (m_LogicFsm == null)
        {
            throw new Exception("You must initialize logic first.");
        }

        return m_LogicFsm.GetState<T>();
    }

    public FsmLogicBase GetLogic(Type logicType)
    {
        if (m_LogicFsm == null)
        {
            throw new Exception("You must initialize logic first.");
        }

        return (FsmLogicBase)m_LogicFsm.GetState(logicType);
    }
    
    public void Init()
    {
        
    }

    public void Initialize()
    {
        var fsmManager = FsmManager.Instance();

        var types = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(a => a.GetTypes().Where(t => t.BaseType == typeof(FsmLogicBase))).ToArray();

        var logics = new FsmLogicBase [types.Count()];
        for (int i = 0; i < types.Count(); i++)
        {
            var type = types[i];
            var logic = (FsmLogicBase)Activator.CreateInstance(type);
            logics[i] = logic;
        }
        
        Initialize(fsmManager, logics);

        m_Initialized = true;
    }
}