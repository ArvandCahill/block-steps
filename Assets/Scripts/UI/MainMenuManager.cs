using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField] private List<NavbarItem> navbarItems;
    [SerializeField] private Transform levelContainer;
    [SerializeField] Polaroid polaroidPrefab;

    private GameManager gameManager => GameManager.instance;

    private void Start()
    {
        InstantiateLevels();
    }

    private void InstantiateLevels()
    {
        for (int i = 0; i < gameManager.allLevelData.Count; i++)
        {
            LevelData levelData = gameManager.allLevelData[i];
            Polaroid polaroid = Instantiate(polaroidPrefab, levelContainer);
            polaroid.Init(levelData);
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
}
