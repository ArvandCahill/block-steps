using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class GameplayUI : MonoBehaviour
{
    [SerializeField] private Transform appleImageParent;
    [SerializeField] private List<Image> appleImages = new();
    [SerializeField] private Image applePrefab;
    private int index = 0;

    private GameplayManager GameplayManager => GameplayManager.instance;

    private void OnEnable()
    {
        GameEvents.OnCollectiblePicked += UpdateCollectiblesUI;
    }

    private void OnDisable()
    {
        GameEvents.OnCollectiblePicked -= UpdateCollectiblesUI;
    }

    void Start()
    {
        InstantiateAppleImages();
    }

    void InstantiateAppleImages()
    {
        for (int i = 0; i < GameplayManager.levelData.maxCollectibles; i++)
        {
            var img = Instantiate(applePrefab, appleImageParent);
            appleImages.Add(img);
            SetAppleColor(i, false);
        }
    }

    void SetAppleColor(int index, bool isCollected)
    {
        if (index < 0 || index >= appleImages.Count) return;
        appleImages[index].color = isCollected ? Color.white : Color.gray;
    }

    public void UpdateCollectiblesUI()
    {
        SetAppleColor(index, true);
        index++;
    }
}
