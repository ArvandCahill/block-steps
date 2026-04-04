using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;
using DG.Tweening;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "AI Detection", story: "Update [Agent] [Player] [Detector] and Wait for [DetectionTime]", category: "Action", id: "7ed23eab34ac4f543c5ad3db01a90fd9")]
public partial class AiDetectionAction : Action
{
    [SerializeReference] public BlackboardVariable<AnimalUnit> Agent;
    [SerializeReference] public BlackboardVariable<AnimalUnit> Player;
    [SerializeReference] public BlackboardVariable<AIDetector> Detector;
    [SerializeReference] public BlackboardVariable<float> DetectionTime;

    private float timer;
    private bool isDetecting;
    private Tween scaleTween;
    private bool hasPlayedTween;
    private GameObject alertIcon => Agent.Value.alertIcon;

    protected override Status OnStart()
    {
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        if (Detector.Value == null) return Status.Failure;

        Player.Value = Detector.Value.UpdateRangeDetector();

        if (Player.Value == null)
        {
            ResetAll();
            return Status.Failure;
        }

        HandleDetection();

        if (timer < DetectionTime.Value)
            return Status.Running;

        Agent.Value.stopMovement = true;
        return Status.Success;
    }

    private void HandleDetection()
    {
        if (!isDetecting)
        {
            isDetecting = true;
            timer = 0f;

            ShowAlert();
            PlayAlertAnimationOnce();
        }

        timer += Time.deltaTime;
    }

    private void ShowAlert()
    {
        if (alertIcon == null) return;

        alertIcon.SetActive(true);
    }

    private void PlayAlertAnimationOnce()
    {
        if (alertIcon == null || hasPlayedTween) return;

        GameManager.instance.audioManager.PlayRandomSFX("Bear");
        scaleTween?.Kill();

        alertIcon.transform.localScale = Vector3.zero;
        scaleTween = alertIcon.transform
            .DOScale(Vector3.one, DetectionTime.Value)
            .SetEase(Ease.OutBack);

        hasPlayedTween = true;
    }

    private void HideAlert()
    {
        if (alertIcon == null) return;

        scaleTween?.Kill();
        alertIcon.SetActive(false);
        alertIcon.transform.localScale = Vector3.zero;
    }

    private void ResetAll()
    {
        timer = 0f;
        isDetecting = false;
        hasPlayedTween = false;

        HideAlert();
    }
}
