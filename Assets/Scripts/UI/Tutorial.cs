using UnityEngine;
using UnityEngine.UI;
using TMPro;

[System.Serializable]
public class TutorialData
{
    public Sprite image;
    public string title;
    [TextArea] public string description;
}

public class Tutorial : MonoBehaviour
{
    [Header("Data")]
    [SerializeField] private TutorialData[] tutorials;

    [Header("UI References")]
    [SerializeField] private Image tutorialImage;
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI descriptionText;

    [Header("Buttons")]
    [SerializeField] private Button nextButton;
    [SerializeField] private Button backButton;
    [SerializeField] private TextMeshProUGUI nextButtonText;


    private int currentIndex = 0;

    private void OnEnable()
    {
        Time.timeScale = 0f;
        currentIndex = 0;
        ShowTutorial(currentIndex);
    }

    private void OnDisable()
    {
        Time.timeScale = 1f;
    }

    private void ShowTutorial(int index)
    {
        if (index < 0 || index >= tutorials.Length) return;

        var data = tutorials[index];

        tutorialImage.sprite = data.image;
        titleText.text = data.title;
        descriptionText.text = data.description;

        nextButtonText.text = (index == tutorials.Length - 1) ? "Close" : "Next";

        backButton.interactable = (index > 0);
    }

    public void OnNextClicked()
    {
        currentIndex++;

        if (currentIndex >= tutorials.Length)
        {
            CloseTutorial();
            return;
        }

        ShowTutorial(currentIndex);

    }

    public void OnBackClicked()
    {
        currentIndex--;

        if (currentIndex < 0) currentIndex = 0;

        ShowTutorial(currentIndex);
    }

    private void CloseTutorial()
    {
        gameObject.SetActive(false);
    }
}