using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Advertisements;

public class InitializeAds : MonoBehaviour, IUnityAdsInitializationListener
{
    [SerializeField]
    private string AndroidGameId;

    [SerializeField]
    private bool IsTesting;

    private string GameId;

    public void OnInitializationComplete()
    {
        // // Debug.LogLog("Ad initialization complete");
    }

    public void OnInitializationFailed(UnityAdsInitializationError error, string message)
    {
        // // Debug.LogLog("Ad initialization failed");
    }

    private void Awake()
    {
        GameId = AndroidGameId;

        if (!Advertisement.isInitialized && Advertisement.isSupported)
        {
            Advertisement.Initialize(GameId, IsTesting, this);
        }
    }
}