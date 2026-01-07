using UnityEngine;
using DG.Tweening;
using AYellowpaper.SerializedCollections;
using System;

[Serializable]
public class UIManager
{
    public GameObject Panel;

    [Serializable]
    public class PopupEntry
    {
        public GameObject popupObject;

        [Header("Show Animation Settings")]
        public float showDuration = 0.3f;
        public Ease showEase = Ease.OutBack;
        public Vector3 showScale = Vector3.one;

        [Header("Hide Animation Settings")]
        public float hideDuration = 0.3f;
        public Ease hideEase = Ease.InBack;
        public Vector3 hideScale = Vector3.zero;
    }

    [SerializedDictionary("Popup Name", "Popup Data")]
    public SerializedDictionary<string, PopupEntry> popupList;

    public void ShowPopup(string popupName, bool panel)
    {
        ShowPanel(panel);

        if (popupList.TryGetValue(popupName, out PopupEntry popup))
        {
            popup.popupObject.SetActive(true);
            popup.popupObject.transform.localScale = popup.hideScale;
            popup.popupObject.transform.DOScale(popup.showScale, popup.showDuration).SetEase(popup.showEase);
        }
        else
        {
            Debug.LogWarning($"Popup '{popupName}' tidak ditemukan!");
        }
    }

    public void HidePopup(string popupName)
    {
        HidePanel();
        if (popupList.TryGetValue(popupName, out PopupEntry popup))
        {
            popup.popupObject.transform.DOScale(popup.hideScale, popup.hideDuration).SetEase(popup.hideEase).OnComplete(() =>
            {
                popup.popupObject.SetActive(false);
            });
        }
    }

    public void HideAllPopups()
    {
        foreach (var popup in popupList.Values)
        {
            popup.popupObject.SetActive(false);
        }
    }

    private void ShowPanel(bool panel)
    {
        if (panel == true) Panel.SetActive(true);
    }
    
    private void HidePanel()
    {
        Panel?.SetActive(false);
    }
}