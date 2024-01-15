using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillManager : MonoBehaviour
{
    public void defenseSkillFun(Skill skill, GameObject tarObj)
    {
        GameObject defenseSkillObj = Instantiate(Resources.Load("Prefabs/Skill/" + skill.skill_animation, typeof(GameObject)) as GameObject);
        defenseSkillObj.AddComponent<SkillDeployer>();
        SkillDeployer skillDeployer = defenseSkillObj.GetComponent<SkillDeployer>();
        skillDeployer.skillData = skill;
    }
    public void SkillBeginFun()
    { 
    
    }
}
