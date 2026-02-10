using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Collections.Generic;
using DG.Tweening;
using DG.Tweening.Core.Easing;

public class Polaroid : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private RectTransform bg;
    [SerializeField] private Image image;
    [SerializeField] private Image lockImage;
    [SerializeField] private TextMeshProUGUI title;
    [SerializeField] private TextMeshProUGUI levelNameText;
    [SerializeField] private Button levelButton;

    [Header("Apple UI")]
    [SerializeField] private Transform appleImageParent;
    [SerializeField] private Image appleImage;
    [SerializeField] private List<Image> appleImages = new();

    [Header("Pocket Animation")]
    [SerializeField] private float pullOutOffset = 300f;
    [SerializeField] private float pocketAnimDuration = 0.35f;

    [Header("Focus Animation")]
    [SerializeField] private float zoomScale = 1.15f;
    [SerializeField] private float focusAnimDuration = 0.4f;

    public PolaroidViewState State { get; private set; } = PolaroidViewState.Grid;

    private Vector2 bgHiddenPos;
    private Vector2 bgShownPos;

    private Transform bgOriginalParent;
    private int bgOriginalSiblingIndex;
    private Vector3 bgOriginalScale;

    private Action<Polaroid> onPolaroidClicked;

    private void Awake()
    {
        bgHiddenPos = bg.anchoredPosition;
        bgShownPos = bgHiddenPos + Vector2.up * pullOutOffset;

        bgOriginalParent = bg.parent;
        bgOriginalSiblingIndex = bg.GetSiblingIndex();
        bgOriginalScale = bg.localScale;
    }

    #region Init

    public void Init(
        LevelData levelData,
        Action<LevelData> startLevel,
        Action<Polaroid> onClicked)
    {
        onPolaroidClicked = onClicked;

        levelButton.onClick.RemoveAllListeners();
        levelButton.onClick.AddListener(() =>
        {
            onPolaroidClicked?.Invoke(this);
        });

        InstantiateAppleImage(levelData);

        appleImageParent.gameObject.SetActive(true);
        Refresh(levelData);
    }

    public void Init(
        AnimalUnit animalUnit,
        Action<AnimalUnit> onAnimalClicked)
    {
        levelButton.onClick.RemoveAllListeners();
        levelButton.onClick.AddListener(() =>
        {
            onAnimalClicked?.Invoke(animalUnit);
        });

        appleImageParent.gameObject.SetActive(false);
        Refresh(animalUnit);
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

    #endregion

    #region Refresh

    public void Refresh(LevelData levelData)
    {
        image.sprite = levelData.levelImage;

        bool locked = !levelData.isUnlocked;
        lockImage.gameObject.SetActive(locked);
        levelButton.interactable = !locked;

        title.text = $"Level - {levelData.levelNumber}";
        levelNameText.text = levelData.levelName;
    }

    public void Refresh(AnimalUnit animalUnit)
    {
        var data = animalUnit.animalData;

        image.sprite = data.animalImage;
        title.text = data.animalName;

        bool unlocked = data.CheckMilestone(GameManager.instance.Currency);
        lockImage.gameObject.SetActive(!unlocked);
    }

    #endregion

    #region Pocket + Focus Animation (CHAINED)

    public void PlayFocusAnimation(Transform focusParent)
    {
        if (State != PolaroidViewState.Grid)
            return;

        State = PolaroidViewState.Animating;

        Sequence seq = DOTween.Sequence();
        GameManager.instance.audioManager.PlaySFX("Paper");

        seq.Append(
            bg.DOAnchorPos(bgShownPos, pocketAnimDuration)
              .SetEase(Ease.OutBack)
        );

        seq.AppendCallback(() =>
        {
            Vector3 worldPos = bg.position;

            bg.SetParent(focusParent, true); 
            bg.SetAsLastSibling();
            bg.position = worldPos;
        });

        seq.Append(
            bg.DOMove(focusParent.position, focusAnimDuration)
              .SetEase(Ease.OutCubic)
        );

        seq.Join(
            bg.DOScale(zoomScale, focusAnimDuration)
              .SetEase(Ease.OutSine)
        );

        seq.OnComplete(() =>
        {
            State = PolaroidViewState.Zoomed;
        });
    }


    public void RestoreFromFocus()
    {
        if (State != PolaroidViewState.Zoomed)
            return;

        State = PolaroidViewState.Animating;

        Sequence seq = DOTween.Sequence();

        Vector3 targetWorldPos = bgOriginalParent.TransformPoint(bgHiddenPos);

        seq.Append(
            bg.DOMove(targetWorldPos, focusAnimDuration)
              .SetEase(Ease.InCubic)
        );

        seq.Join(
            bg.DOScale(bgOriginalScale, focusAnimDuration)
              .SetEase(Ease.InBack)
        );

        seq.AppendCallback(() =>
        {
            bg.SetParent(bgOriginalParent, true);
            bg.SetSiblingIndex(bgOriginalSiblingIndex);
        });

        seq.Append(
            bg.DOAnchorPos(bgHiddenPos, pocketAnimDuration)
              .SetEase(Ease.InBack)
        );

        seq.OnComplete(() =>
        {
            State = PolaroidViewState.Grid;
        });
    }


    #endregion


    #region Utility

    public void EnableApple(bool enable)
    {
        appleImageParent.gameObject.SetActive(enable);
    }

    #endregion
}
