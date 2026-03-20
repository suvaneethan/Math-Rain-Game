using UnityEngine;
using GoogleMobileAds.Api;
using System;
using System.Collections;
using System.Collections.Generic;

public class AdManager : MonoBehaviour
{
    public static AdManager Instance;

    private RewardedAd rewardedAd;

    private Action onRewardCallback;
    private bool rewardEarned = false;

    public Action OnAdClosed;

    public bool IsAdReady => rewardedAd != null && rewardedAd.CanShowAd();

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    void Start()
    {
        MobileAds.Initialize(initStatus => { });

#if UNITY_EDITOR || DEVELOPMENT_BUILD
        RequestConfiguration requestConfiguration = new RequestConfiguration
        {
            TestDeviceIds = new List<string> { "ZN4223HPKD" }
        };
        MobileAds.SetRequestConfiguration(requestConfiguration);
#endif

        LoadRewardedAd();
    }

    string GetAdUnitId()
    {
#if UNITY_EDITOR
        return "ca-app-pub-3940256099942544/5224354917";
#else
        return "ca-app-pub-5187377766948762/9517032674";
#endif
    }

    void LoadRewardedAd()
    {
        string adUnitId = GetAdUnitId();

        AdRequest request = new AdRequest();

        RewardedAd.Load(adUnitId, request,
            (RewardedAd ad, LoadAdError error) =>
            {
                if (error != null || ad == null)
                {
                    Debug.Log("Ad Load Failed: " + error);
                    Invoke(nameof(LoadRewardedAd), 2f);
                    return;
                }

                rewardedAd = ad;
                RegisterEvents(ad);

                Debug.Log("Ad Loaded");
            });
    }

    void RegisterEvents(RewardedAd ad)
    {
        ad.OnAdFullScreenContentOpened += () =>
        {
            StartCoroutine(SetAudioPause(true));
        };

        ad.OnAdFullScreenContentClosed += () =>
        {
            StartCoroutine(HandleAdClosed());
        };

        ad.OnAdFullScreenContentFailed += (AdError error) =>
        {
            Debug.Log("Ad Failed: " + error);

            AudioListener.pause = false;
            LoadRewardedAd();
        };
    }
    IEnumerator HandleAdClosed()
    {
        yield return null; // 🔥 switch to main thread

        AudioListener.pause = false;

        if (rewardEarned && onRewardCallback != null)
        {
            Action callback = onRewardCallback;
            onRewardCallback = null;

            callback.Invoke(); // ✅ SAFE
        }

        rewardEarned = false;

        OnAdClosed?.Invoke(); // ✅ now safe

        LoadRewardedAd();
    }
    public void ShowRewardedAd(Action onReward)
    {
        if (rewardedAd != null && rewardedAd.CanShowAd())
        {
            onRewardCallback = onReward;
            rewardEarned = false;

            rewardedAd.Show((Reward reward) =>
            {
               
                rewardEarned = true; // ✅ mark only
                Debug.Log("Reward Callback Triggered");
            });
        }
        else
        {
            Debug.Log("Ad not ready...");
        }
    }

    IEnumerator InvokeOnMainThread()
    {
        yield return null;

        if (onRewardCallback != null)
        {
            Action callback = onRewardCallback;
            onRewardCallback = null;

            callback.Invoke();
        }
    }
    IEnumerator SetAudioPause(bool state)
    {
        yield return null; // move to main thread
        AudioListener.pause = state;
    }
}