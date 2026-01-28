using UnityEngine;
using DG.Tweening;
using System;
using System.Collections.Generic;
using System.Linq;

[System.Serializable]
public class GroupSettings
{
    public bool playInSequence = false;
    public float groupDelay = 0f;
    public bool autoReverse = false;
}

[Serializable]
public class AnimationGroup
{
    public bool useSequence = false;
    public List<AnimatedObject> animatedObjects = new();

    [Header("Group Settings")]
    public GroupSettings groupSettings = new();

    public void Play()
    {
        if (useSequence)
        {
            Sequence seq = DOTween.Sequence().SetUpdate(true);
            foreach (var obj in animatedObjects.OrderBy(o => o.sequenceIndex))
            {
                Tween t = obj.CreateTween().SetUpdate(true);
                if (t != null) seq.Append(t).SetUpdate(true);
            }
        }
        else
        {
            foreach (var obj in animatedObjects)
            {
                obj.CreateTween().SetUpdate(true);
            }
        }
    }

    public void Stop()
    {
        foreach (var obj in animatedObjects)
        {
            obj.KillTween();
        }
    }
}
