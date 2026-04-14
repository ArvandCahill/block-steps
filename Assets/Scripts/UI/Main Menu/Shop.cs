using UnityEngine;
using System.Collections;
using System.Linq;
using DG.Tweening;
using AYellowpaper.SerializedCollections;
using UnityEngine.UI;

public class Shop : MonoBehaviour
{
    [Header("State")]
    [SerializeField] private Polaroid selectedPolaroid;
    [SerializeField] private Polaroid displayedPolaroid;
    [SerializeField] private AnimalUnit displayedAnimal;

    [Header("References")]
    [SerializeField] private Block blockTarget;
    [SerializeField] private Block blockDefault;
    [SerializeField] private RectTransform polaroidContainer;
    [SerializeField] private Transform animalContainer;
    [SerializeField] private ShopButton shopButton;

    [Header("Prefabs")]
    [SerializeField] private Polaroid polaroidPrefab;
    [SerializeField] private AnimalUnit animalUnitPrefab;

    [SerializedDictionary("Polaroid", "Animal Unit")]
    [SerializeField] private SerializedDictionary<AnimalUnit, Polaroid> animals = new();

    [Header("Scroll View Animation")]
    [SerializeField] private RectTransform scrollView;
    [SerializeField] private float scrollAnimDuration = 0.4f;
    [SerializeField] private float scrollOffset = -600f;
    private Vector2 scrollOriginalPos;
    private ScrollRect scrollRect;


    private GameManager gameManager => GameManager.instance;

    public void OnEnable()
    {
        AnimateScrollViewIn();
        DisplayEquippedAnimal();
    }

    private void Awake()
    {
        scrollRect = scrollView.GetComponent<ScrollRect>();
    }

    public void InstantiatePolaroids()
    {
        var sortedAnimalData = GameManager.instance.allAnimalData.OrderBy(data => data.price).ToList();

        foreach (AnimalData animalData in sortedAnimalData)
        {
            Polaroid polaroid = Instantiate(polaroidPrefab, polaroidContainer);
            AnimalUnit animalUnit = Instantiate(
                animalUnitPrefab,
                animalContainer.position,
                Quaternion.identity,
                animalContainer
            );

            animalUnit.Init(animalData);
            polaroid.Init(animalUnit, OnPolaroidSelected);
            animalUnit.name = animalData.animalName;

            animals.Add(animalUnit, polaroid);

            if (animalData.animalID == gameManager.GetSelectedAnimal().animalID) SetSelectedAnimal(animalUnit);
        }

        scrollOriginalPos = scrollView.anchoredPosition;
    }

    private void DisplayEquippedAnimal()
    {
        var selectedData = gameManager.GetSelectedAnimal();

        foreach (var pair in animals)
        {
            if (pair.Key.animalData.animalID == selectedData.animalID)
            {
                StartCoroutine(DisplayAnimal(pair.Key));
                return;
            }
        }
    }

    private void OnPolaroidSelected(AnimalUnit animal)
    {
        StartCoroutine(DisplayAnimal(animal));
    }

    private IEnumerator DisplayAnimal(AnimalUnit animal)
    {
        StartCoroutine(tes(animals[animal]));
/*        FocusToPolaroid(animals[animal]);*/
        if (displayedPolaroid == animals[animal]) yield break;

        ResetPreviousSelection();

        SelectPolaroid(animal);
        shopButton.UpdateBuyButtonState(animal);
        yield return MoveAnimalToBlock(animal);
    }

    private void ResetPreviousSelection()
    {
        if (displayedAnimal != null)
        {
            PathFinding.instance.Move(displayedAnimal, blockDefault.GetPosition());
        }
        displayedAnimal?.EnableAI(true);
        displayedPolaroid?.transform
            .DOScale(0.65f, 0.2f)
            .SetEase(Ease.OutBack);
    }

    private void SelectPolaroid(AnimalUnit animal)
    {
        displayedPolaroid = animals[animal];
        displayedPolaroid.transform
            .DOScale(0.75f, 0.2f)
            .SetEase(Ease.OutBack);

        displayedAnimal = animal;
        displayedAnimal.EnableAI(false);
    }

    private IEnumerator MoveAnimalToBlock(AnimalUnit animal)
    {
        blockTarget.isWalkable = true;
        animal.stopMovement = true;
        PathFinding.instance.Move(animal, blockTarget.GetPosition());

        yield return new WaitForSeconds(0.5f);

        blockTarget.isWalkable = false;
    }

    public void SetSelectedAnimal(AnimalUnit animalUnit)
    {
        selectedPolaroid?.EnableApple(false);
        selectedPolaroid = animals[animalUnit];
        selectedPolaroid.EnableApple(true);
        gameManager.SetSelectedAnimal(animalUnit.animalData);
    }

    private void UpdateMilestones(int currency)
    {
        foreach (var pair in animals)
        {
            pair.Key.animalData.CheckMilestone(currency);
            pair.Value.Refresh(pair.Key);
        }
    }

    private void AnimateScrollViewIn()
    {
        shopButton.transform.parent.gameObject.SetActive(true);

        scrollView.DOKill();

        scrollView.anchoredPosition = scrollOriginalPos + Vector2.up * scrollOffset;

        scrollView
            .DOAnchorPos(scrollOriginalPos, scrollAnimDuration)
            .SetEase(Ease.OutBack);
    }

    private IEnumerator FocusDelayed(Polaroid target)
    {
        yield return null; 
        yield return null; 

        FocusToPolaroid(target);
    }

    private IEnumerator tes(Polaroid polaroid)
    {
        yield return new WaitUntil(() => polaroidContainer.rect.width > 100);
        FocusToPolaroid(polaroid);
    }

    private void FocusToPolaroid(Polaroid target)
    {
        polaroidContainer.DOKill();
        Canvas.ForceUpdateCanvases();

        RectTransform targetRect = target.GetComponent<RectTransform>();

        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            polaroidContainer,
            RectTransformUtility.WorldToScreenPoint(null, targetRect.position),
            null,
            out localPoint
        );

        float viewportCenterX = scrollRect.viewport.rect.width / 2f;

        float targetX = -localPoint.x + viewportCenterX;

        float minX = -(polaroidContainer.rect.width - scrollRect.viewport.rect.width);
        float maxX = 0f;

        targetX = Mathf.Clamp(targetX, minX, maxX);

        polaroidContainer.DOAnchorPosX(targetX, 0.3f).SetEase(Ease.OutCubic);
    }

    public void CloseShop()
    {
        if (scrollView == null) return;

        shopButton.transform.parent.gameObject.SetActive(false);

        scrollView.DOKill();

        scrollView
            .DOAnchorPos(scrollOriginalPos + Vector2.up * scrollOffset, scrollAnimDuration)
            .SetEase(Ease.InCubic)
            .OnComplete(() => gameObject.SetActive(false));
    }
}