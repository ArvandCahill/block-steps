using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SettingPanel : MonoBehaviour
{
    [Header("UI Toggles")]
    [SerializeField] private Toggle bgmToggle;
    [SerializeField] private Toggle sfxToggle;
    [SerializeField] private TextMeshProUGUI bgmText;
    [SerializeField] private TextMeshProUGUI sfxText;

    public void Start()
    {
        bgmToggle.SetIsOnWithoutNotify(SaveManager.instance.IsBgmOn);
        sfxToggle.SetIsOnWithoutNotify(SaveManager.instance.IsSfxOn);

        bgmText.text = bgmToggle.isOn ? "ON" : "OFF";
        sfxText.text = sfxToggle.isOn ? "ON" : "OFF";

        bgmToggle.onValueChanged.AddListener(SetBGM);
        sfxToggle.onValueChanged.AddListener(SetSFX);
    }

    public void SetBGM(bool isOn)
    {
        bgmText.text = isOn ? "ON" : "OFF";
        SaveManager.instance.IsBgmOn = isOn;
    }

    public void SetSFX(bool isOn)
    {
        sfxText.text = isOn ? "ON" : "OFF";
        SaveManager.instance.IsSfxOn = isOn;
    }
}
