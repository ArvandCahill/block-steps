using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class Polaroid : MonoBehaviour
{
    [SerializeField] private Image levelImage; 
    [SerializeField] private Image lockImage;
    [SerializeField] List<Image> appleImages = new();
    [SerializeField] private TextMeshProUGUI levelNumber;
    [SerializeField] private TextMeshProUGUI levelNameText;
    [SerializeField] private Button levelButton;
    [SerializeField] private Transform appleImageParent;
    [SerializeField] private Image appleImage;

    public void Init(LevelData levelData)
    {
        levelImage.sprite = levelData?.levelImage;
        lockImage.gameObject.SetActive(levelData.isLocked);
        levelNumber.text = $"Level - {levelData.levelNumber.ToString()}";
        levelNameText.text = levelData.levelPrefab.name;
        InstantiateAppleImage(levelData);
        levelButton.interactable = !levelData.isLocked;
        levelButton.onClick.AddListener(() => StartLevel(levelData));
    }

    public void Init(AnimalData animalData)
    {

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

    public void StartLevel(LevelData levelData)
    {
        GameManager.instance.SetSelectedLevel(levelData);
        GameManager.instance.SceneLoader.LoadScene("Gameplay");
    }
}
