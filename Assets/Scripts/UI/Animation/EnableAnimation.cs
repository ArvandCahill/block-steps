using UnityEngine;
using DG.Tweening;

public class UIEnterAnimator : MonoBehaviour
{
    public enum AnimationTrigger { OnEnable, OnStart }
    public enum AnimationType { Scale, Position, Fade }
    public AnimationTrigger animationTrigger = AnimationTrigger.OnEnable;
    public AnimationType animationType = AnimationType.Scale;

    [Header("General Settings")]
    public float duration = 0.5f;
    public float delay = 0f;
    public Ease ease = Ease.OutBack;

    [Header("Scale Settings")]
    public Vector3 startScale = Vector3.zero;

    [Header("Position Settings")]
    public Vector3 offset = new Vector3(0, -500, 0); 

    [Header("Fade Settings")]
    public CanvasGroup canvasGroup;

    private Vector3 originalPosition;
    private Vector3 originalScale;

    void Awake()
    {
        originalPosition = transform.localPosition;
        originalScale = transform.localScale;

        if (animationType == AnimationType.Fade && canvasGroup == null)
            canvasGroup = GetComponent<CanvasGroup>();
    }

    private void Start()
    {
        if (animationTrigger == AnimationTrigger.OnStart)
        {
            switch (animationType)
            {
                case AnimationType.Scale:
                    AnimateScale();
                    break;

                case AnimationType.Position:
                    AnimatePosition();
                    break;

                case AnimationType.Fade:
                    AnimateFade();
                    break;
            }
        }
    }

    void OnEnable()
    {
        if (animationTrigger == AnimationTrigger.OnEnable)
        {
            switch (animationType)
            {
                case AnimationType.Scale:
                    AnimateScale();
                    break;

                case AnimationType.Position:
                    AnimatePosition();
                    break;

                case AnimationType.Fade:
                    AnimateFade();
                    break;
            }
        }
    }

    private void OnDisable()
    {
        transform.localScale = originalScale;
        transform.localPosition = originalPosition;

        if (canvasGroup != null)
            canvasGroup.alpha = 1f; 
    }

    void AnimateScale()
    {
        transform.localScale = startScale;
        transform.DOScale(originalScale, duration).SetEase(ease).SetDelay(delay).SetUpdate(true);
    }

    void AnimatePosition()
    {
        transform.localPosition = originalPosition + offset;
        transform.DOLocalMove(originalPosition, duration).SetEase(ease).SetDelay(delay).SetUpdate(true);
        Debug.Log("AnimatePosition called");
    }

    void AnimateFade()
    {
        if (canvasGroup == null) return;

        canvasGroup.alpha = 0f;
        canvasGroup.DOFade(1f, duration).SetEase(ease).SetDelay(delay).SetUpdate(true);
    }
}
