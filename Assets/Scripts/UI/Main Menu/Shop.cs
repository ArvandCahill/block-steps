using UnityEngine;
using System.Collections;
using System.Linq;
using DG.Tweening;
using AYellowpaper.SerializedCollections;

public class Shop : MonoBehaviour
{
    [Header("State")]
    [SerializeField] private Polaroid selectedPolaroid;
    [SerializeField] private Polaroid displayedPolaroid;
    [SerializeField] private AnimalUnit displayedAnimal;

    [Header("References")]
    [SerializeField] private Block blockTarget;
    [SerializeField] private Block blockDefault;
    [SerializeField] private Transform polaroidContainer;
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


    private GameManager gameManager => GameManager.instance;

    private void OnEnable()
    {
        GameEvents.OnCurrencyValueChanged += UpdateMilestones;
        AnimateScrollViewIn();
    }

    private void OnDisable()
    {
        GameEvents.OnCurrencyValueChanged -= UpdateMilestones;
    }

    private void Awake()
    {
        scrollOriginalPos = scrollView.anchoredPosition;
    }

    private void Start()
    {
        DisplayFirstAnimal();
    }

    public void InstantiatePolaroids()
    {
        foreach (AnimalData animalData in GameManager.instance.allAnimalData)
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
    }

    private void DisplayFirstAnimal()
    {
        if (animals.Count == 0) return;
        StartCoroutine(DisplayAnimal(animals.Keys.First()));
    }

    private void OnPolaroidSelected(AnimalUnit animal)
    {
        StartCoroutine(DisplayAnimal(animal));
    }

    private IEnumerator DisplayAnimal(AnimalUnit animal)
    {
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
            Debug.Log("move previous animal");
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
        Debug.Log($"Moving {animal.name} to {blockTarget.GetPosition()}");

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
        if (scrollView == null) return;

        shopButton.transform.parent.gameObject.SetActive(true);

        scrollView.DOKill();

        scrollView.anchoredPosition = scrollOriginalPos + Vector2.up * scrollOffset;

        scrollView
            .DOAnchorPos(scrollOriginalPos, scrollAnimDuration)
            .SetEase(Ease.OutBack);
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