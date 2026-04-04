using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

public class IncrementalBundleVersion : IPreprocessBuildWithReport
{
    public int callbackOrder => 0;

    public void OnPreprocessBuild(BuildReport report)
    {
        if (report.summary.platform == BuildTarget.Android && EditorUserBuildSettings.buildAppBundle)
        {
            IncrementVersionCode();
        }
    }

    private static void IncrementVersionCode()
    {
        int currentVersion = PlayerSettings.Android.bundleVersionCode;
        PlayerSettings.Android.bundleVersionCode = currentVersion + 1;
        Debug.Log($"Auto-Increment Version Code for AAB: {currentVersion} → {PlayerSettings.Android.bundleVersionCode}");
    }
}
