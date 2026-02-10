using System;
using System.Collections;
using UnityEngine;
using GoogleMobileAds.Api;

[Serializable]
public class AdmobInit
{
    public bool isAdActive;

    [Header("Jangan dimatikan")]
    [SerializeField] private bool testMode;

    public int adsFrequency = 5;

    private InterstitialAd interstitialAd;
    private BannerView bannerAd;
    private Action onAdClosedCallback;

    public bool IsInterstitialAdReady => interstitialAd != null && interstitialAd.CanShowAd();

    public void Init()
    {
        MobileAds.Initialize(initStatus => {
            RequestAds();
        });
    }

    private void RequestAds()
    {
        RequestInterstitial();
    }

    private void RequestInterstitial()
    {
        string adUnitId = testMode ? "ca-app-pub-3940256099942544/1033173712" : "ca-app-pub-9002983919929663/6840936034";

        var adRequest = new AdRequest();
        InterstitialAd.Load(adUnitId, adRequest, (ad, error) =>
        {
            if (error != null)
            {
                Debug.LogError($"Failed to load interstitial ad: {error.GetMessage()}");
                return;
            }
            interstitialAd = ad;
        
            interstitialAd.OnAdFullScreenContentClosed += HandleInterstitialClosed;
        });
    }

    public void ShowInterstitial(Action onAdClosed)
    {
        if (interstitialAd != null && interstitialAd.CanShowAd())
        {
            onAdClosedCallback = onAdClosed; 
            interstitialAd.Show();
            Debug.Log("show ads");
        }
        else
        {
            Debug.Log("Interstitial Ad is not ready yet.");
            onAdClosed?.Invoke();
        }
    }

    public IEnumerator ShowInterstitalWithDelay(int sceneCount)
    {
        yield return new WaitForSeconds(1);

        if (sceneCount == adsFrequency && isAdActive == true)
        {
            ShowInterstitial(() =>
            {
                Debug.Log("show ad");
                GameManager.instance.ResetSceneCount();
            });
        }
    }

    public void ShowInterstitial()
    {
        ShowInterstitial(null);
    }


    private void HandleInterstitialClosed()
    {
        onAdClosedCallback?.Invoke(); 
        RequestInterstitial();
    }

    public void RequestBanner()
    {
        if (isAdActive == true)
        {
            if (bannerAd != null)
            {
                DestroyBanner();
            }

            string adUnitId = testMode ? "ca-app-pub-3940256099942544/6300978111" : "ca-app-pub-9002983919929663/5364202837";
            bannerAd = new BannerView(adUnitId, AdSize.Banner, AdPosition.Bottom);
            var adRequest = new AdRequest();
            bannerAd.LoadAd(adRequest);
        }
    }

    public void ShowBanner()
    {
        bannerAd?.Show();
    }

    public void HideBanner()
    {
        bannerAd?.Hide();
    }

    public void DestroyBanner()
    {
        bannerAd?.Destroy();    
        bannerAd = null;
    }
}