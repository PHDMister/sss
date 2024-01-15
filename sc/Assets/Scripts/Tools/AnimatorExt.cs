using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class AnimatorExt 
{

    public static float GetAnimLength(this Animator animator, string animName)
    {
        float length = 0;
        AnimationClip[] clips = animator.runtimeAnimatorController.animationClips;
        foreach (AnimationClip clip in clips)
        {
            if (clip.name.Equals(animName))
            {
                length = clip.length;
                break;
            }
        }
        return length;
    }

    public static AnimationClip GetAnimationClip(this Animator animator, string animName)
    {
        AnimationClip[] clips = animator.runtimeAnimatorController.animationClips;
        foreach (AnimationClip clip in clips)
        {
            if (clip.name.Equals(animName))
            {
                return clip;
            }
        }
        return null;
    }
}
