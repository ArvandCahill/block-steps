using System;

public static class AnimationEvents
{
    public static event Action<string[]> OnPlayGroups;
    public static event Action<string> OnStopGroup;

    public static void TriggerPlayGroups(params string[] groupNames)
    {
        OnPlayGroups?.Invoke(groupNames);
    }

    public static void TriggerStopGroup(string groupName)
    {
        OnStopGroup?.Invoke(groupName);
    }
}
