using UnityEngine;
using GoogleMobileAds.Api;
using System;
using System.Collections.Generic;

public class AdManager : MonoBehaviour
{
    public static AdManager Instance;

    private RewardedAd rewardedAd;
    private bool isShowingAd = false;

    private Action pendingRewardCallback;
    private bool shouldReward = false;

    public Action<bool> OnAdAvailabilityChanged;

    public bool IsAdReady => rewardedAd != null && rewardedAd.CanShowAd();

    // ✅ Retry System
    private int retryCount = 0;

    //// ✅ Cooldown System
    //private float lastAdTime = -100f;
    //private float adCooldown = 30f;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        Debug.Log("🚀 Initializing AdMob...");

        MobileAds.Initialize(initStatus =>
        {
            Debug.Log("✅ AdMob Initialized");

            MainThreadDispatcher.RunOnMainThread(() =>
            {
                LoadRewardedAd();
            });
        });

#if UNITY_EDITOR || DEVELOPMENT_BUILD
        RequestConfiguration requestConfiguration = new RequestConfiguration
        {
            TestDeviceIds = new List<string> { "ZN4223HPKD" }
        };
        MobileAds.SetRequestConfiguration(requestConfiguration);
#endif
    }

    string GetAdUnitId()
    {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
        return "ca-app-pub-3940256099942544/5224354917"; // Test
#else
        return "ca-app-pub-5187377766948762/9517032674"; // Live
#endif
    }

    void LoadRewardedAd()
    {
        // ✅ Internet Check
        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            Debug.Log("❌ No Internet - retry later");
            Invoke(nameof(LoadRewardedAd), 5f);
            return;
        }

        string adUnitId = GetAdUnitId();
        AdRequest request = new AdRequest();

        if (rewardedAd != null)
        {
            rewardedAd.Destroy();
            rewardedAd = null;
        }

        Debug.Log("🔄 Loading Rewarded Ad...");

        RewardedAd.Load(adUnitId, request,
            (RewardedAd ad, LoadAdError error) =>
            {
                MainThreadDispatcher.RunOnMainThread(() =>
                {
                    if (error != null || ad == null)
                    {
                        Debug.Log("❌ Ad Load Failed: " + error);

                        retryCount++;

                        float delay = Mathf.Min(60f, Mathf.Pow(2, retryCount));
                        Debug.Log($"⏳ Retry in {delay} sec");

                        Invoke(nameof(LoadRewardedAd), delay);

                        UpdateAdAvailability(false);
                        return;
                    }

                    // ✅ SUCCESS
                    retryCount = 0;

                    rewardedAd = ad;
                    RegisterEvents(ad);

                    Debug.Log("✅ Ad Loaded Successfully!");
                    UpdateAdAvailability(true);
                });
            });
    }

    void RegisterEvents(RewardedAd ad)
    {
        ad.OnAdFullScreenContentOpened += () =>
        {
            MainThreadDispatcher.RunOnMainThread(() =>
            {
                Debug.Log("📺 Ad Opened");
                isShowingAd = true;
                AudioListener.pause = true;
            });
        };

        ad.OnAdFullScreenContentClosed += () =>
        {
            MainThreadDispatcher.RunOnMainThread(() =>
            {
                Debug.Log("📉 Ad Closed");

                isShowingAd = false;
                AudioListener.pause = false;

                if (shouldReward && pendingRewardCallback != null)
                {
                    Debug.Log("🎁 Reward Granted");
                    pendingRewardCallback.Invoke();
                }

                shouldReward = false;
                pendingRewardCallback = null;

                LoadRewardedAd();
            });
        };

        ad.OnAdFullScreenContentFailed += (AdError error) =>
        {
            MainThreadDispatcher.RunOnMainThread(() =>
            {
                Debug.Log("❌ Ad Show Failed: " + error);

                isShowingAd = false;
                AudioListener.pause = false;

                shouldReward = false;
                pendingRewardCallback = null;

                LoadRewardedAd();
            });
        };
    }

    public bool ShowRewardedAd(Action onReward)
    {
        // ✅ Already showing
        if (isShowingAd)
        {
            Debug.Log("⚠️ Ad already showing");
            return false;
        }

        // ✅ Cooldown check
        //if (Time.time - lastAdTime < adCooldown)
        //{
        //    Debug.Log("⏳ Ad cooldown active");
        //    return false;
        //}

        // ✅ Proper readiness check
        if (IsAdReady)
        {
            Debug.Log("▶️ Showing Rewarded Ad");

           // lastAdTime = Time.time;

            pendingRewardCallback = onReward;
            shouldReward = false;

            try
            {
                rewardedAd.Show((Reward reward) =>
                {
                    Debug.Log("🎯 User Earned Reward");
                    shouldReward = true;
                });

                return true;
            }
            catch (Exception e)
            {
                Debug.Log("❌ Show Exception: " + e);
                return false;
            }
        }
        else
        {
            Debug.Log("❌ Ad not ready yet");
            return false;
        }
    }

    void UpdateAdAvailability(bool available)
    {
        OnAdAvailabilityChanged?.Invoke(available);
    }
}