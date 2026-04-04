using System;
using System.Collections;
using UnityEngine;

public static class GameEvents
{
    public static event Action OnGamePaused;
    public static event Action<Vector3> OnPlayerMoving;
    public static event Action<Vector3> OnPlayerStopped;
    public static event Action OnCollectiblePicked;
    public static event Action<bool> OnLevelFinished;
    public static event Action<Vector3, bool> OnPathValidated;

    public static void TriggerGamePaused()
    {
        OnGamePaused?.Invoke();
    }

    public static void TriggerPlayerMoving(Vector3 targetPos)
    {
        OnPlayerMoving?.Invoke(targetPos);
    }

    public static void TriggerPlayerStopped(Vector3 stopPos)
    {
        OnPlayerStopped?.Invoke(stopPos);
    }

    public static void TriggerCollectiblePicked()
    {
        OnCollectiblePicked?.Invoke();
    }

    public static void TriggerLevelFinished(bool isWinning)
    {
        OnLevelFinished?.Invoke(isWinning);
    }

    public static void TriggerPathValidated(Vector3 targetPos, bool isValid)
    {
        OnPathValidated?.Invoke(targetPos, isValid);
    }
}
