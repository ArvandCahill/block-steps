using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class NavbarItem : MonoBehaviour
{
    [SerializeField] private Image itemIcon;
    [SerializeField] private GameObject itemText;
    [SerializeField] private GameObject highlight;
    [SerializeField] private bool isActived = false;

    private void Start()
    {
        SetActive(isActived);
    }

    public void SetActive(bool active)
    {
        itemIcon.rectTransform.DOScale(active ? 1.1f : 1f, 0.2f).SetEase(Ease.OutBack);

        isActived = active;
        highlight.SetActive(active);
        itemText.SetActive(active);
    }
}
