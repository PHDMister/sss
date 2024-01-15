using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSkillSystem : SkillSystem
{
    // Start is called before the first frame update
    private void Awake()
    {
        _skillManager = new SkillManager();
    }
    public override void ReleaseUseSkill(int skillID,GameObject targetObj)
    {
        base.ReleaseUseSkill(skillID,targetObj);
    }
}
