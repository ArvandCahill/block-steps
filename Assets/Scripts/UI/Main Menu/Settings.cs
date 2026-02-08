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
        bgmToggle.SetIsOnWithoutNotify(GameManager.instance.IsBgmOn);
        sfxToggle.SetIsOnWithoutNotify(GameManager.instance.IsSfxOn);

        bgmText.text = bgmToggle.isOn ? "ON" : "OFF";
        sfxText.text = sfxToggle.isOn ? "ON" : "OFF";

        bgmToggle.onValueChanged.AddListener(SetBGM);
        sfxToggle.onValueChanged.AddListener(SetSFX);
    }

    public void SetBGM(bool isOn)
    {
        bgmText.text = isOn ? "ON" : "OFF";
        GameManager.instance.IsBgmOn = isOn;
        GameManager.instance.saveManager.SaveSettings();
    }

    public void SetSFX(bool isOn)
    {
        sfxText.text = isOn ? "ON" : "OFF";
        GameManager.instance.IsSfxOn = isOn;
        GameManager.instance.saveManager.SaveSettings();
    }
}
