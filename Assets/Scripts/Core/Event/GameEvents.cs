using System;
using System.Collections;
using UnityEngine;

public static class GameEvents
{
    public static event Action OnGamePaused;
    public static event Action<Vector3> OnPlayerMoving;
    public static event Action<Vector3> OnPlayerStopped;
    public static event Action OnCollectiblePicked;
    public static event Action<bool> OnPlayerFinished;
    public static event Action<int> OnCurrencyValueChanged;

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

    public static IEnumerator TriggerPlayerFinished(bool isWinning, int collectibles)
    {
        yield return new WaitForSeconds(2f);
        OnPlayerFinished?.Invoke(isWinning);
    }

    public static void TriggerCurrencyValueChanged(int currency)
    {
        OnCurrencyValueChanged?.Invoke(currency);
    }
}
