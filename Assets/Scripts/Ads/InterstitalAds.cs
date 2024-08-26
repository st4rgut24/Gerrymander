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
    public static Action FailedToLoadAdEvent;

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
        // Debug.LogLog("On Unity interstital Ads loaded");
    }

    public void OnUnityAdsFailedToLoad(string placementId, UnityAdsLoadError error, string message)
    {
        // Debug.LogLogWarning("On Unity Ads failed to load");
        FailedToLoadAdEvent?.Invoke();

    }

    public void OnUnityAdsShowFailure(string placementId, UnityAdsShowError error, string message)
    {
        //FinishInterstitalAd?.Invoke();
        // Debug.LogLog("Interstitial Ad Show Failed");
    }

    public void OnUnityAdsShowStart(string placementId)
    {
        // Debug.LogLog("Interstitial Ad Show Start");
    }

    public void OnUnityAdsShowClick(string placementId)
    {
        // Debug.LogLog("Interstitial Ad Clicked");
    }

    public void OnUnityAdsShowComplete(string placementId, UnityAdsShowCompletionState showCompletionState)
    {
        // Debug.LogLog("Interstitial Ad Show Complete");

        //FinishInterstitalAd?.Invoke();
        StartCoroutine(FinishInterstitalAdSideEffect());
    }

    IEnumerator FinishInterstitalAdSideEffect()
    {
        yield return new WaitForSeconds(1);
        FinishInterstitalAd?.Invoke();
    }

    private void OnDestroy()
    {
        // Debug.LogLog("Iam destroyed");
    }
}
