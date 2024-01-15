using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 技能系统类，给外部提供技能释放方法
/// </summary>
/// 
public class SkillSystem : MonoBehaviour
{
    public SkillManager _skillManager;
    /// <summary>
    ///  技能释放
    /// </summary>
    /// <param name="skillID">技能ID</param>
    public virtual void ReleaseUseSkill(int skillID, GameObject targetObj)
    {
        Skill skill = ManageMentClass.DataManagerClass.GetSkillTable(skillID);
        switch (skill.skill_character)
        {
            case 1:
                break;
            case 2:
                break;
        }
    }
    public void ToSelfSkill(Skill skill)
    {

    }
    public void ToOtherSkill(Skill skill, GameObject targetObj)
    {

    }

}
