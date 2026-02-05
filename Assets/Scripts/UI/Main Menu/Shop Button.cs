using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopButton : MonoBehaviour
{
    [SerializeField] private Shop shop;
    [SerializeField] private Image background;
    [SerializeField] private GameObject apple;
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private Button button;
    [SerializeField] private Color normalColor;
    [SerializeField] private Color highlightColor;

    private GameManager gameManager => GameManager.instance;

    public void UpdateBuyButtonState(AnimalUnit unit)
    {
        button.onClick.RemoveAllListeners();

        if (unit.animalData.CheckMilestone(gameManager.Currency))
        {
            button.onClick.AddListener(() => SelectAnimal(unit));

            if (unit.animalData == gameManager.GetSelectedAnimal())
            {
                apple.SetActive(false);
                button.interactable = false;
                text.text = "Selected";
            }

            else
            {
                apple.SetActive(false);
                button.interactable = true;
                text.text = "Select";
            }

            apple.SetActive(false);
        }
        else
        {
            button.onClick.AddListener(() => BuyAnimal(unit.animalData));
            button.interactable = true;
            text.text = unit.animalData.price.ToString();
            apple.SetActive(true);
        }
    }

    private void SelectAnimal(AnimalUnit unit)
    {
        shop.SetSelectedAnimal(unit);
        UpdateBuyButtonState(unit);
    }

    private void BuyAnimal(AnimalData data)
    {

    }
}
