using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class NavbarItem : MonoBehaviour
{
    [SerializeField] private Image itemIcon;
    [SerializeField] private GameObject itemText;
    [SerializeField] private bool isActived = false;
    Color normalColor;
    Color highlightedColor;

    private void Start()
    {
        ColorUtility.TryParseHtmlString("#84A6AD", out normalColor);
        ColorUtility.TryParseHtmlString("#26B8C9", out highlightedColor);

        SetActive(isActived);
    }

    public void SetActive(bool active)
    {
        itemIcon.DOColor(active ? highlightedColor : normalColor, 0.2f);
        itemIcon.rectTransform.DOScale(active ? 1.1f : 1f, 0.2f).SetEase(Ease.OutBack);

        isActived = active;
        itemText.SetActive(active);
    }
}
