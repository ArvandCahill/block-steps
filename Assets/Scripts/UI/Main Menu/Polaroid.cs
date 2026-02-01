using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System;

public class Polaroid : MonoBehaviour
{
    public Image image; 
    [SerializeField] private Image lockImage;
    [SerializeField] List<Image> appleImages = new();
    [SerializeField] private TextMeshProUGUI title;
    [SerializeField] private TextMeshProUGUI levelNameText;
    [SerializeField] private Button levelButton;
    [SerializeField] private Transform appleImageParent;
    [SerializeField] private Image appleImage;

    public void Init(LevelData levelData, Action<LevelData> startLevel)
    {
        image.sprite = levelData?.levelImage;
        lockImage.gameObject.SetActive(levelData.isLocked);
        title.text = $"Level - {levelData.levelNumber.ToString()}";
        levelNameText.text = levelData.levelPrefab.name;
        InstantiateAppleImage(levelData);
        levelButton.interactable = !levelData.isLocked;
        levelButton.onClick.AddListener(() => startLevel(levelData));
    }

    public void Init(AnimalData animalData, Action<Polaroid> displayAnimal)
    {
        Instantiate(appleImage, appleImageParent);

        image.sprite = animalData?.animalImage;
        lockImage.gameObject.SetActive(!animalData.isUnlocked);
        title.text = animalData.animalName;
        levelButton.onClick.AddListener(() => displayAnimal(this));
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
}
