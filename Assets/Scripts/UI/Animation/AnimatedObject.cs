using DG.Tweening;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class AnimatedObject
{
    public enum AnimationType { Move, Scale, Rotate, Fade }
    public enum SpaceMode { Absolute, Relative }

    public GameObject target;
    public AnimationType type;
    public Vector3 targetValue = Vector3.one;
    public float duration = 0.5f;
    public float delay = 0f;
    public Ease ease = Ease.OutQuad;
    public int sequenceIndex = 0;
    public bool isReverse = false;
    public SpaceMode spaceMode = SpaceMode.Absolute;

    [Header("Loop Settings")]
    public int loopCount = 0;
    public LoopType loopType = LoopType.Restart;

    [Header("Event")]
    public UnityEvent onComplete;

    [NonSerialized] private Tween currentTween;

    private static readonly Dictionary<AnimationType, Func<AnimatedObject, Tween>> tweenStrategies = new()
    {
        { AnimationType.Move,   a => a.CreateMoveTween().SetUpdate(true) },
        { AnimationType.Scale,  a => a.CreateScaleTween().SetUpdate(true) },
        { AnimationType.Rotate, a => a.CreateRotateTween().SetUpdate(true) },
        { AnimationType.Fade,   a => a.CreateFadeTween().SetUpdate(true) },
    };

    public Tween CreateTween()
    {
        if (target == null) return null;

        if (tweenStrategies.TryGetValue(type, out var strategy))
        {
            currentTween = strategy(this)
                .SetEase(ease)
                .SetDelay(delay)
                .SetLoops(loopCount, loopType)
                .SetUpdate(true)
                .OnComplete(() => onComplete?.Invoke());
        }

        return currentTween;
    }

    public void KillTween()
    {
        if (currentTween != null && currentTween.IsActive())
            currentTween.Kill();
    }

    private Tween CreateMoveTween()
    {
        var tf = target.transform;
        var end = isReverse ? -targetValue : targetValue;
        end = spaceMode == SpaceMode.Relative ? tf.localPosition + end : end;
        return tf.DOLocalMove(end, duration).SetUpdate(true);
    }

    private Tween CreateScaleTween()
    {
        var tf = target.transform;
        var end = isReverse ? -targetValue : targetValue;
        end = spaceMode == SpaceMode.Relative ? tf.localScale + end : end;
        return tf.DOScale(end, duration).SetUpdate(true);
    }

    private Tween CreateRotateTween()
    {
        var tf = target.transform;
        var end = isReverse ? -targetValue : targetValue;
        end = spaceMode == SpaceMode.Relative ? tf.localEulerAngles + end : end;
        return tf.DOLocalRotate(end, duration, RotateMode.FastBeyond360).SetUpdate(true);
    }

    private Tween CreateFadeTween()
    {
        var cg = target.GetComponent<CanvasGroup>() ?? target.AddComponent<CanvasGroup>();
        var endAlpha = isReverse ? -targetValue.x : targetValue.x;
        var finalAlpha = spaceMode == SpaceMode.Relative ? cg.alpha + endAlpha : endAlpha;
        return cg.DOFade(finalAlpha, duration).SetUpdate(true);
    }
}
