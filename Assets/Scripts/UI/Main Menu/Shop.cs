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
    [SerializeField] private Transform polaroidContainer;
    [SerializeField] private Transform animalContainer;
    [SerializeField] private ShopButton shopButton;

    [Header("Prefabs")]
    [SerializeField] private Polaroid polaroidPrefab;
    [SerializeField] private AnimalUnit animalUnitPrefab;

    [SerializedDictionary("Polaroid", "Animal Unit")]
    [SerializeField] private SerializedDictionary<Polaroid, AnimalUnit> animals = new();

    private GameManager gameManager => GameManager.instance;

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

            polaroid.Init(animalData, OnPolaroidSelected);
            animalUnit.Init(animalData);
            animalUnit.name = animalData.animalName;

            animals.Add(polaroid, animalUnit);

            if (animalData.animalID == gameManager.GetSelectedAnimal().animalID) SetSelectedAnimal(animalUnit);
        }
    }

    private void DisplayFirstAnimal()
    {
        if (animals.Count == 0) return;
        StartCoroutine(DisplayAnimal(animals.Keys.First()));
    }

    private void OnPolaroidSelected(Polaroid polaroid)
    {
        StartCoroutine(DisplayAnimal(polaroid));
    }

    private IEnumerator DisplayAnimal(Polaroid polaroid)
    {
        if (displayedPolaroid == polaroid) yield break;

        ResetPreviousSelection();

        SelectPolaroid(polaroid);
        shopButton.UpdateBuyButtonState(animals[polaroid]);
        yield return MoveAnimalToBlock(animals[polaroid]);
    }

    private void ResetPreviousSelection()
    {
        if(displayedAnimal != null) PathFinding.instance.Move(displayedAnimal, new Vector3Int(0, 0, 6));
        displayedAnimal?.EnableAI(true);
        displayedPolaroid?.transform
            .DOScale(0.65f, 0.2f)
            .SetEase(Ease.OutBack);
    }

    private void SelectPolaroid(Polaroid polaroid)
    {
        displayedPolaroid = polaroid;
        polaroid.transform
            .DOScale(0.75f, 0.2f)
            .SetEase(Ease.OutBack);

        displayedAnimal = animals[polaroid];
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
        selectedPolaroid = animals.FirstOrDefault(x => x.Value == animalUnit).Key;
        selectedPolaroid.EnableApple(true);
        gameManager.SetSelectedAnimal(animalUnit.animalData);
    }
}