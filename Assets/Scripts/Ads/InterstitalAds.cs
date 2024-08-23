using UnityEngine;
using UnityEngine.Advertisements;
using System.Collections;
using System;

public class InterstitialAds : Singleton<InterstitialAds>, IUnityAdsLoadListener, IUnityAdsShowListener
{
    [SerializeField]
    private string AdUnitId;

    public static Action PlayInterstitalAd;
    public static Action FinishInterstitalAd;

    private void Update()
    {
        if (FinishInterstitalAd != null)
            Debug.Log("FinishInterstitalAd is not null");
        else
        {
            Debug.Log("FinishInterstitalAd is null");
        }
    }

    public void LoadInterstitialAds()
    {
        Advertisement.Load(AdUnitId, this);
    }

    public void ShowInterstitialAd()
    {
        PlayInterstitalAd?.Invoke();
        Advertisement.Show(AdUnitId, this);
    }


    public void OnUnityAdsAdLoaded(string placementId)
    {
        Debug.Log("On Unity interstital Ads loaded");
    }

    public void OnUnityAdsFailedToLoad(string placementId, UnityAdsLoadError error, string message)
    {
        Debug.LogWarning("On Unity Ads failed to load");
        //FinishInterstitalAd?.Invoke();
    }

    public void OnUnityAdsShowFailure(string placementId, UnityAdsShowError error, string message)
    {
        //FinishInterstitalAd?.Invoke();
        Debug.Log("Interstitial Ad Show Failed");
    }

    public void OnUnityAdsShowStart(string placementId)
    {
        Debug.Log("Interstitial Ad Show Start");
    }

    public void OnUnityAdsShowClick(string placementId)
    {
        Debug.Log("Interstitial Ad Clicked");
    }

    public void OnUnityAdsShowComplete(string placementId, UnityAdsShowCompletionState showCompletionState)
    {
        Debug.Log("Interstitial Ad Show Complete");

        FinishInterstitalAd?.Invoke();
    }

    private void OnDestroy()
    {
        Debug.Log("Iam destroyed");
    }
}
