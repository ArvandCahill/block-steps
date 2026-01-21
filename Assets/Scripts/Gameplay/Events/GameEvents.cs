using System;
using UnityEngine;

public static class GameEvents
{
    public static event Action OnGamePaused;
    public static event Action<Vector3> OnPlayerMoving;
    public static event Action<Vector3> OnPlayerStopped;
    public static event Action<int> OnPlayerFinish;

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

    public static void TriggerPlayerFinish(int collectibles)
    {
        OnPlayerFinish?.Invoke(collectibles);
    }
}
