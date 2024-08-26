using UnityEngine;
using UnityEngine.Advertisements;
using System;

public class BannerAds : MonoBehaviour, IUnityAdsLoadListener, IUnityAdsShowListener
{
    [SerializeField]
    private string bannerAdUnitId; // The Ad Unit ID for banner ads

    private void Start()
    {
        // Initialize Unity Ads and load the banner ad
        if (Advertisement.isInitialized)
        {
            LoadBannerAd();
        }
        else
        {
            // Debug.LogLogWarning("Unity Ads is not initialized.");
        }
    }

    private void LoadBannerAd()
    {
        if (PlayerPrefs.HasKey("AdFree"))
            return; // no ads

        Advertisement.Banner.Load(bannerAdUnitId);
    }

    public void OnUnityAdsAdLoaded(string placementId)
    {
        if (placementId == bannerAdUnitId)
        {
            // Show the banner ad when it's successfully loaded
            Advertisement.Banner.SetPosition(BannerPosition.BOTTOM_CENTER); // Set position to the bottom center
            Advertisement.Banner.Show(bannerAdUnitId);
            // Debug.LogLog("Banner ad loaded and displayed.");
        }
    }

    public void OnUnityAdsFailedToLoad(string placementId, UnityAdsLoadError error, string message)
    {
        if (placementId == bannerAdUnitId)
        {
            // Debug.LogLogWarning($"Banner ad failed to load: {message}");
        }
    }

    public void OnUnityAdsShowFailure(string placementId, UnityAdsShowError error, string message)
    {
        // Debug.LogLogWarning($"Banner ad failed to show: {message}");
    }

    public void OnUnityAdsShowStart(string placementId)
    {
        // Debug.LogLog("Banner ad started showing.");
    }

    public void OnUnityAdsShowClick(string placementId)
    {
        // Debug.LogLog("Banner ad clicked.");
    }

    public void OnUnityAdsShowComplete(string placementId, UnityAdsShowCompletionState showCompletionState)
    {
        // Debug.LogLog("Banner ad completed showing.");
    }

    private void OnDestroy()
    {
        // Hide the banner ad when the script is destroyed
        Advertisement.Banner.Hide();
    }
}
