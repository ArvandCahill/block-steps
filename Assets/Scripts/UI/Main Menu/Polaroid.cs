using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Polaroid : MonoBehaviour
{
    [SerializeField] private Image levelImage; 
    [SerializeField] private Image lockImage;
    [SerializeField] private TextMeshProUGUI levelNumber;
    [SerializeField] private TextMeshProUGUI levelNameText;

    public void Init(LevelData levelData)
    {
        levelImage.sprite = levelData.levelImage;
        lockImage.gameObject.SetActive(levelData.isLocked);
        levelNumber.text = $"Level - {levelData.levelNumber.ToString()}";
        levelNameText.text = levelData.levelPrefab.name;
    }

    public void OnClick()
    {

    }
}
