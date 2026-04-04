using AYellowpaper.SerializedCollections;
using DG.Tweening;
using System.Collections.Generic;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    [Header("Polaroids")]
    [SerializedDictionary("Level Data", "Polaroid")]
    private SerializedDictionary<LevelData, Polaroid> polaroids = new();

    [SerializeField] private Transform levelContainer;
    [SerializeField] private Transform focusContainer;
    [SerializeField] private Polaroid polaroidPrefab;

    [Header("UI")]
    [SerializeField] private Image panelBackground;
    [SerializeField] private TextMeshProUGUI currencyText;
    [SerializeField] private List<NavbarItem> navbarItems;

    [Header("Bookmark Animation")]
    [SerializeField] private RectTransform bookmarkIcon;
    [SerializeField] private float bookmarkPullOffset = 40f;
    [SerializeField] private float bookmarkAnimDuration = 0.25f;
    private Vector2 bookmarkHiddenPos;
    private Vector2 bookmarkShownPos;


    [Header("Other")]
    [SerializeField] private Shop shop;

    private GameManager gameManager => GameManager.instance;
    private Polaroid focusedPolaroid;

    private void Awake()
    {
        bookmarkHiddenPos = bookmarkIcon.anchoredPosition;
        bookmarkShownPos = bookmarkHiddenPos + Vector2.up * bookmarkPullOffset;
    }

    private IEnumerator Start()
    {
        InstantiateLevels();

        yield return new WaitUntil(() => SaveManager.instance.saveData.levelProgress.Count > 0);
        shop.InstantiatePolaroids();

        panelBackground.gameObject.SetActive(false);

        SetCurrencyText(SaveManager.instance.Currency);
    }

    private void InstantiateLevels()
    {
        foreach (LevelData levelData in gameManager.allLevelData)
        {
            Polaroid polaroid = Instantiate(polaroidPrefab, levelContainer);
            polaroid.Init(levelData, OnPolaroidClicked);
            polaroids.Add(levelData, polaroid);
        }
    }

    private void OnPolaroidClicked(Polaroid polaroid)
    {
        if (polaroid.State == PolaroidViewState.Animating)
            return;

        if (polaroid.State == PolaroidViewState.Zoomed)
        {
            LevelData data = GetLevelDataFromPolaroid(polaroid);
            if (data != null)
                StartLevel(data);
            return;
        }

        if (focusedPolaroid != null)
            return;

        focusedPolaroid = polaroid;

        panelBackground.gameObject.SetActive(true);

        PlayBookmarkPull();
        polaroid.PlayFocusAnimation(focusContainer);
    }

    public void CloseFocusedPolaroid()
    {
        if (focusedPolaroid.State == PolaroidViewState.Animating)
            return;

        panelBackground.gameObject.SetActive(false);

        HideBookmark();
        focusedPolaroid.RestoreFromFocus();
        focusedPolaroid = null;
    }

    private LevelData GetLevelDataFromPolaroid(Polaroid polaroid)
    {
        foreach (var pair in polaroids)
        {
            if (pair.Value == polaroid)
                return pair.Key;
        }
        return null;
    }

    public void ClickNavbar(NavbarItem navbarItem)
    {
        foreach (NavbarItem item in navbarItems)
        {
            item.SetActive(item == navbarItem);
        }
    }

    private void SetCurrencyText(int currency)
    {
        currencyText.text = currency.ToString();
    }

    private void StartLevel(LevelData levelData)
    {
        gameManager.SetSelectedLevel(levelData);
        gameManager.SceneLoader.LoadScene("Gameplay");
    }

    #region Bookmark Animation
    private void PlayBookmarkPull()
    {
        if (bookmarkIcon == null) return;

        bookmarkIcon.anchoredPosition = bookmarkHiddenPos;
        bookmarkIcon.localScale = Vector3.one;

        Sequence seq = DOTween.Sequence();
        seq.Append(
            bookmarkIcon.DOAnchorPos(bookmarkShownPos, bookmarkAnimDuration)
                .SetEase(Ease.OutBack)
        );
    }

    private void HideBookmark()
    {
        if (bookmarkIcon == null) return;

        Sequence seq = DOTween.Sequence();
        seq.Append(
            bookmarkIcon.DOAnchorPos(bookmarkHiddenPos, bookmarkAnimDuration)
                .SetEase(Ease.InBack)
        );
    }
    #endregion


}
