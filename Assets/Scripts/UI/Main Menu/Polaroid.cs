using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System;

public class Polaroid : MonoBehaviour
{
    [SerializeField] private Image image; 
    [SerializeField] private Image lockImage;
    [SerializeField] List<Image> appleImages = new();
    [SerializeField] private TextMeshProUGUI title;
    [SerializeField] private TextMeshProUGUI levelNameText;
    [SerializeField] private Button levelButton;
    [SerializeField] private Transform appleImageParent;
    [SerializeField] private Image appleImage;

    public void Init(LevelData levelData, Action<LevelData> startLevel)
    {
        levelButton.onClick.RemoveAllListeners();
        levelButton.onClick.AddListener(() => startLevel(levelData));

        Refresh(levelData);
    }


    public void Init(AnimalUnit animalUnit, Action<AnimalUnit> displayAnimal)
    {
        levelButton.onClick.RemoveAllListeners();
        levelButton.onClick.AddListener(() => displayAnimal(animalUnit));

        Refresh(animalUnit);
    }

    private void InstantiateAppleImage(LevelData levelData)
    {
        for (int i = 0; i < levelData.maxCollectibles; i++)
        {
            Image img = Instantiate(appleImage, appleImageParent);
            appleImages.Add(img);

            if (i < levelData.collectiblesCollected)
            {
                appleImages[i].color = Color.white; 
            }
            else
            {
                appleImages[i].color = Color.gray; 
            }
        }
    }

    public void Refresh(AnimalUnit animalUnit)
    {
        var data = animalUnit.animalData;

        image.sprite = data.animalImage;
        title.text = data.animalName;

        bool unlocked = data.CheckMilestone(GameManager.instance.Currency);

        lockImage.gameObject.SetActive(!unlocked);
        levelButton.interactable = unlocked;
    }

    public void Refresh(LevelData levelData)
    {
        image.sprite = levelData.levelImage;

        bool locked = !levelData.isUnlocked;

        lockImage.gameObject.SetActive(locked);
        levelButton.interactable = !locked;

        Debug.Log($"Refreshing Polaroid for Level {levelData.levelNumber}: Unlocked = {levelData.isUnlocked}, Collectibles Collected = {levelData.collectiblesCollected}/{levelData.maxCollectibles}");

        title.text = $"Level - {levelData.levelNumber}";
        levelNameText.text = levelData.levelName;
    }


    public void EnableApple(bool enable)
    {
        appleImageParent.gameObject.SetActive(enable);
        Debug.Log("Apple Enabled: " + enable);
    }
}
