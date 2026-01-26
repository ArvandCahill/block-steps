using System;
using UnityEngine;

public static class GameEvents
{
    public static event Action OnGamePaused;
    public static event Action<Vector3> OnPlayerMoving;
    public static event Action<Vector3> OnPlayerStopped;
    public static event Action<bool,int> OnPlayerFinished;

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

    public static void TriggerPlayerFinished(bool isWinning, int collectibles)
    {
        OnPlayerFinished?.Invoke(isWinning, collectibles);
    }
}
