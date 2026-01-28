using UnityEngine;
using DG.Tweening;
using System.Linq;
using AYellowpaper.SerializedCollections;

public class AnimationManager : MonoBehaviour
{
    [SerializeField] public bool useEvents = true;
    [SerializedDictionary("Group Name", "Animation Group")]
    public SerializedDictionary<string, AnimationGroup> animationGroups = new();

    void OnEnable()
    {
        if (useEvents)
        {
            AnimationEvents.OnPlayGroups += PlayGroups;
            AnimationEvents.OnStopGroup += StopGroup;
        }
    }

    void OnDisable()
    {
        if (useEvents)
        {
            AnimationEvents.OnPlayGroups -= PlayGroups;
            AnimationEvents.OnStopGroup -= StopGroup;
        }
    }

    public void PlayGroup(string groupName)
    {
        if (GameManager.instance.isAnimating) return;

        GameManager.instance.isAnimating = true;

        if (!animationGroups.TryGetValue(groupName, out var group))
        {
            Debug.LogWarning($"Animation group '{groupName}' not found!");
            return;
        }

        var sorted = group.animatedObjects.OrderBy(x => x.sequenceIndex);
        var sequence = DOTween.Sequence().SetDelay(group.groupSettings.groupDelay).SetUpdate(true);

        foreach (var anim in sorted)
        {
            anim.KillTween();
            var tween = anim.CreateTween().SetUpdate(true);
            if (group.groupSettings.playInSequence) sequence.Append(tween).SetUpdate(true);
            else sequence.Join(tween).SetUpdate(true);
        }

        if (group.groupSettings.autoReverse)
        {
            sequence.OnComplete(() =>
            {
                var reverseSeq = DOTween.Sequence().SetUpdate(true);
                foreach (var anim in sorted)
                {
                    anim.isReverse = !anim.isReverse;
                    reverseSeq.Join(anim.CreateTween()).SetUpdate(true);
                }
                reverseSeq.Play().SetUpdate(true);
            });
        }

        sequence.Play().SetUpdate(true);

        GameManager.instance.isAnimating = false;
    }

    public void PlayGroups(params string[] groupNames)
    {
        foreach (var groupName in groupNames)
        {
            PlayGroup(groupName);
        }
    }

    public void StopGroup(string groupName)
    {
        if (animationGroups.TryGetValue(groupName, out var group))
            group.animatedObjects.ForEach(a => a.KillTween());
    }
}
