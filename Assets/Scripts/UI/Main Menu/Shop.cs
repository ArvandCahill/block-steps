using UnityEngine;
using System.Collections.Generic;
using DG.Tweening;

public class Shop : MonoBehaviour
{
    [SerializeField] private Transform polaroidContainer;
    [SerializeField] private Polaroid polaroidPrefab;
    [SerializeField] private Polaroid selectedPolaroid;

    List<Polaroid> polaroids = new List<Polaroid>();

    private void Awake()
    {
        InstantiatePolaroid();
    }

    private void InstantiatePolaroid()
    {
        for (int i = 0; i < GameManager.instance.allAnimalData.Count; i++)
        {
            AnimalData animalData = GameManager.instance.allAnimalData[i];
            Polaroid polaroid = Instantiate(polaroidPrefab, polaroidContainer);
            polaroids.Add(polaroid);
            polaroid.Init(animalData, DisplayAnimal);
        }

        DisplayAnimal(polaroids[0]);
    }

    private void DisplayAnimal(Polaroid polaroid)
    {
        selectedPolaroid?.transform.DOScale(0.65f, 0.2f).SetEase(Ease.OutBack);
        selectedPolaroid = polaroid;
        polaroid.transform.DOScale(0.75f, 0.2f).SetEase(Ease.OutBack);
    }
}
