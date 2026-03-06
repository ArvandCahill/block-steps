using AYellowpaper.SerializedCollections;
using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

[Serializable]
public class SceneEnvironment
{
    [SerializeField] private Light directionalLight;
    [SerializeField] Volume postProcessVolume;

    [SerializedDictionary("Skybox Type", "Skybox Material")]
    public AYellowpaper.SerializedCollections.SerializedDictionary<SkyboxType, Material> skyboxMaterials = new();

    private GameplayManager gameplayManager => GameplayManager.instance;

    public void Init(LevelData levelData)
    {
        UpdateSkybox(levelData);
        SetLightIntensity(levelData);
        SetExposure(levelData);
    }

    void UpdateSkybox(LevelData levelData)
    {
        if (skyboxMaterials.TryGetValue(levelData.skyboxType, out Material newSkybox))
        {
            RenderSettings.skybox = newSkybox;
            DynamicGI.UpdateEnvironment();
        }
        else
        {
            Debug.LogWarning($"Skybox material for {levelData.skyboxType} not found in SkyboxChanger.");
        }
    }

    void SetLightIntensity(LevelData levelData)
    {
        if (!levelData.isNightMode)
        {
            directionalLight.intensity = 1f;
        }
        else
        {
            directionalLight.intensity = 0.2f;
        }
    }

    void SetExposure(LevelData levelData)
    {
        if (postProcessVolume.profile.TryGet(out ColorAdjustments exposure))
        {
            float targetValue = levelData.isNightMode ? -0.5f : 0f;

            exposure.postExposure.value = targetValue;
            Debug.Log("Exposure set to " + exposure.postExposure.value);
        }
        else
        {
            Debug.LogWarning("ColorAdjustments not found in Volume!");
        }
    }
}
