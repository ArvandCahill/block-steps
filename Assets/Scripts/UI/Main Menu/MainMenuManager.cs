using AYellowpaper.SerializedCollections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    [SerializedDictionary("Level Data", "Polaroids")]
    private SerializedDictionary<LevelData, Polaroid> polaroids = new();

    [SerializeField] private List<NavbarItem> navbarItems;
    [SerializeField] private TextMeshProUGUI currencyText;
    [SerializeField] private Transform levelContainer;
    [SerializeField] private Polaroid polaroidPrefab;
    [SerializeField] private Shop shop;

    private GameManager gameManager => GameManager.instance;

    private void OnEnable()
    {
        GameEvents.OnCurrencyValueChanged += OnCurrencyChanged;
    }

    private void OnDisable()
    {
        GameEvents.OnCurrencyValueChanged -= OnCurrencyChanged;
    }

    private void Start()
    {
        InstantiateLevels();
        shop.InstantiatePolaroids();
        OnCurrencyChanged(gameManager.Currency);
    }

    private void InstantiateLevels()
    {
        for (int i = 0; i < gameManager.allLevelData.Count; i++)
        {
            LevelData levelData = gameManager.allLevelData[i];
            Polaroid polaroid = Instantiate(polaroidPrefab, levelContainer);
            polaroid.Init(levelData, StartLevel);
            polaroids.Add(levelData, polaroid);
        }
    }

    public void ClickNavbar(NavbarItem navbarItem)
    {
        foreach (NavbarItem item in navbarItems)
        {
            if (navbarItem == item) item.SetActive(true);
            else item.SetActive(false);
        }
    }

    public void StartLevel(LevelData levelData)
    {
        GameManager.instance.SetSelectedLevel(levelData);
        GameManager.instance.SceneLoader.LoadScene("Gameplay");
    }

    private void OnCurrencyChanged(int currency)
    {
        currencyText.text = currency.ToString();
    }
}
