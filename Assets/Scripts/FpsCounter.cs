using UnityEngine;
using UnityEngine.UI;

public class FPSCounter : MonoBehaviour
{
    [Header("UI")]
    public Text fpsText; 

    [Header("Settings")]
    [Range(0.1f, 1f)]
    public float updateInterval = 0.5f;

    private float timeLeft;
    private int frameCount;
    private float fps;

    void Start()
    {
        Application.targetFrameRate = 60;
        timeLeft = updateInterval;
    }

    void Update()
    {
        timeLeft -= Time.unscaledDeltaTime;
        frameCount++;

        if (timeLeft <= 0.0f)
        {
            fps = frameCount / updateInterval;

            fpsText.text = $"{Mathf.RoundToInt(fps)} FPS";

            timeLeft = updateInterval;
            frameCount = 0;
        }
    }
}
