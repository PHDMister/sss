using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillPrefabBuilder : MonoBehaviour
{

    public static SkillPrefabBuilder SkillPrefabBuilder_Instance;


    /// <summary>
    /// 技能对象池子
    /// </summary>
    public Dictionary<int, List<GameObject>> SkillPrefabsPool = new Dictionary<int, List<GameObject>>();

    private void Awake()
    {
        SkillPrefabBuilder_Instance = this;
    }


    // Start is called before the first frame update
    void Start()
    {
        //初始化时，先生成一些
        InstactiaterPreFabPoll();
    }
    /// <summary>
    /// 初始化技能预制体
    /// </summary>
    public void StartSkillPrefabFun()
    {

    }


    public GameObject InstantiatePreFun(Skill skill)
    {
        if (SkillPrefabsPool.ContainsKey(skill.skill_id))
        {
            if (SkillPrefabsPool[skill.skill_id].Count > 0)
            {
                GameObject prefab = SkillPrefabsPool[skill.skill_id][0];
                SkillPrefabsPool[skill.skill_id].RemoveAt(0);
                return prefab;
            }
        }
        GameObject defenseSkillObj = Instantiate(Resources.Load("Prefabs/Skill/" + skill.skill_animation, typeof(GameObject)) as GameObject, transform);
        return defenseSkillObj;
    }

    /// <summary>
    /// 技能预制体对象池
    /// </summary>
    public void InstactiaterPreFabPoll()
    {



        SkillContainer skillContainer = BinaryDataMgr.Instance.LoadTableById<SkillContainer>("SkillContainer");
        var skillKey = skillContainer.dataDic.Keys;

        foreach (var key in skillKey)
        {
            for (int i = 0; i < 10; i++)
            {
                GameObject defenseSkillObj = Instantiate(Resources.Load("Prefabs/Skill/" + skillContainer.dataDic[key].skill_animation, typeof(GameObject)) as GameObject, transform);
                defenseSkillObj.SetActive(false);
                if (!SkillPrefabsPool.ContainsKey(skillContainer.dataDic[key].skill_id))
                {
                    List<GameObject> gameObjects = new List<GameObject>();
                    SkillPrefabsPool.Add(skillContainer.dataDic[key].skill_id, gameObjects);
                }
                SkillPrefabsPool[skillContainer.dataDic[key].skill_id].Add(defenseSkillObj);
            }
        }

    }

}
