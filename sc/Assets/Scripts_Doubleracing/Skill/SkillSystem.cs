using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// ����ϵͳ�࣬���ⲿ�ṩ�����ͷŷ���
/// </summary>
/// 
public class SkillSystem : MonoBehaviour
{
    public SkillManager _skillManager;
    /// <summary>
    ///  �����ͷ�
    /// </summary>
    /// <param name="skillID">����ID</param>
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
