//using UnityEngine;
//using GoogleMobileAds.Api;
//using System;
//using System.Collections.Generic;

//public class AdManager : MonoBehaviour
//{
//    public static AdManager Instance;

//    private RewardedAd rewardedAd;
//    private bool isShowingAd = false;

//    private Action pendingRewardCallback;
//    private bool shouldReward = false;

//    public Action<bool> OnAdAvailabilityChanged;

//    public bool IsAdReady => rewardedAd != null;

//    void Awake()
//    {
//        if (Instance == null)
//        {
//            Instance = this;
//            DontDestroyOnLoad(gameObject);
//        }
//        else
//        {
//            Destroy(gameObject);
//        }
//    }

//    void Start()
//    {
//        Debug.Log("🚀 Initializing AdMob...");

//        MobileAds.Initialize(initStatus =>
//        {
//            Debug.Log("✅ AdMob Initialized");

//            // ✅ FIX: Safely route the first ad load back to the Main Thread
//            MainThreadDispatcher.RunOnMainThread(() =>
//            {
//                LoadRewardedAd();
//            });
//        });

//#if UNITY_EDITOR || DEVELOPMENT_BUILD
//        RequestConfiguration requestConfiguration = new RequestConfiguration
//        {
//            TestDeviceIds = new List<string> { "ZN4223HPKD" }
//        };
//        MobileAds.SetRequestConfiguration(requestConfiguration);
//#endif
//    }   

//    string GetAdUnitId()
//    {


//        // We will put this back when uploading to the Play Store:
//        #if UNITY_EDITOR || DEVELOPMENT_BUILD
//        return "ca-app-pub-3940256099942544/5224354917"; // Test ID
//        #else
//        return "ca-app-pub-5187377766948762/9517032674"; // Live Id
//        #endif

//    }

//    void LoadRewardedAd()
//    {
//        string adUnitId = GetAdUnitId();
//        AdRequest request = new AdRequest();

//        if (rewardedAd != null)
//        {
//            rewardedAd.Destroy();
//            rewardedAd = null;
//        }

//        Debug.Log("🔄 Loading Rewarded Ad...");

//        RewardedAd.Load(adUnitId, request,
//            (RewardedAd ad, LoadAdError error) =>
//            {
//                // 🔥 FIX: The load result comes back on a background thread. 
//                // We must push it to the main thread before touching Unity APIs!
//                MainThreadDispatcher.RunOnMainThread(() =>
//                {
//                    if (error != null || ad == null)
//                    {
//                        Debug.Log("❌ Ad Load Failed: " + error);

//                        // Invoke is a Unity API, it now safely runs on the Main Thread
//                        Invoke(nameof(LoadRewardedAd), 2f);

//                        // Safely update UI (if any buttons are listening)
//                        UpdateAdAvailability(false);
//                        return;
//                    }

//                    rewardedAd = ad;
//                    RegisterEvents(ad);

//                    Debug.Log("✅ Ad Loaded Successfully!");
//                    UpdateAdAvailability(true); // Safely turns on your Ad Buttons
//                });
//            });
//    }

//    void RegisterEvents(RewardedAd ad)
//    {
//        // 1. OPENED EVENT
//        ad.OnAdFullScreenContentOpened += () =>
//        {
//            // ✅ Wrapped in Dispatcher
//            MainThreadDispatcher.RunOnMainThread(() =>
//            {
//                Debug.Log("📺 Ad Opened");
//                isShowingAd = true;
//                AudioListener.pause = true;
//            });
//        };

//        // 2. CLOSED EVENT
//        ad.OnAdFullScreenContentClosed += () =>
//        {
//            // ✅ Wrapped in Dispatcher
//            MainThreadDispatcher.RunOnMainThread(() =>
//            {
//                Debug.Log("📉 Ad Closed");

//                isShowingAd = false;
//                AudioListener.pause = false;

//                if (shouldReward && pendingRewardCallback != null)
//                {
//                    Debug.Log("🎁 Reward Granted");
//                    pendingRewardCallback.Invoke();
//                }

//                shouldReward = false;
//                pendingRewardCallback = null;

//                LoadRewardedAd(); // preload next
//            });
//        };

//        // 3. FAILED EVENT
//        ad.OnAdFullScreenContentFailed += (AdError error) =>
//        {
//            // ✅ Wrapped in Dispatcher
//            MainThreadDispatcher.RunOnMainThread(() =>
//            {
//                Debug.Log("❌ Ad Show Failed: " + error);

//                isShowingAd = false;
//                AudioListener.pause = false;

//                shouldReward = false;
//                pendingRewardCallback = null;

//                LoadRewardedAd();
//            });
//        };
//    }

//    public bool ShowRewardedAd(Action onReward)
//    {
//        if (isShowingAd)
//        {
//            Debug.Log("⚠️ Ad already showing");
//            return false;
//        }

//        if (rewardedAd != null)
//        {
//            Debug.Log("▶️ Showing Rewarded Ad");

//            pendingRewardCallback = onReward;
//            shouldReward = false;

//            try
//            {
//                rewardedAd.Show((Reward reward) =>
//                {
//                    Debug.Log("🎯 User Earned Reward");
//                    shouldReward = true;
//                });

//                return true;
//            }
//            catch (Exception e)
//            {
//                Debug.Log("❌ Show Exception: " + e);
//                return false;
//            }
//        }
//        else
//        {
//            Debug.Log("❌ Ad not ready yet");
//            return false;
//        }
//    }

//    void UpdateAdAvailability(bool available)
//    {
//        OnAdAvailabilityChanged?.Invoke(available);
//    }
//}

//deep seek ai code
//////////////////////////////////////////////////////////////////////////////////////
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

    public bool IsAdReady => rewardedAd != null;

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
            LoadRewardedAd();   // No dispatcher needed
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
        // 🛠️ Temporarily force the test ID for testing on the phone
     //   return "ca-app-pub-3940256099942544/5224354917";

        //* We will put this back when uploading to the Play Store:
        #if UNITY_EDITOR || DEVELOPMENT_BUILD
        return "ca-app-pub-3940256099942544/5224354917"; // Test ID
        #else
        return "ca-app-pub-5187377766948762/9517032674"; // Live Id
        #endif
        
    }

    void LoadRewardedAd()
    {
        string adUnitId = GetAdUnitId();
        AdRequest request = new AdRequest();

        // Destroy existing ad if any to prevent memory leaks
        if (rewardedAd != null)
        {
            rewardedAd.Destroy();
            rewardedAd = null;
        }

        Debug.Log("🔄 Loading Rewarded Ad...");

        RewardedAd.Load(adUnitId, request, (RewardedAd ad, LoadAdError error) =>
        {
            // Callback is already on main thread
            if (error != null || ad == null)
            {
                Debug.Log("❌ Ad Load Failed: " + error);
                Invoke(nameof(LoadRewardedAd), 2f);   // retry after 2 seconds
                UpdateAdAvailability(false);
                return;
            }

            rewardedAd = ad;
            RegisterEvents(ad);

            Debug.Log("✅ Ad Loaded Successfully!");
            UpdateAdAvailability(true);
        });
    }

    void RegisterEvents(RewardedAd ad)
    {
        // Unsubscribe any previous handlers to avoid double events
        ad.OnAdFullScreenContentOpened -= OnAdOpened;
        ad.OnAdFullScreenContentClosed -= OnAdClosed;
        ad.OnAdFullScreenContentFailed -= OnAdFailed;

        ad.OnAdFullScreenContentOpened += OnAdOpened;
        ad.OnAdFullScreenContentClosed += OnAdClosed;
        ad.OnAdFullScreenContentFailed += OnAdFailed;
    }

    private void OnAdOpened()
    {
        Debug.Log("📺 Ad Opened");
        isShowingAd = true;
        AudioListener.pause = true;
    }

    private void OnAdClosed()
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

        // Load the next ad
        LoadRewardedAd();
    }

    private void OnAdFailed(AdError error)
    {
        Debug.Log("❌ Ad Show Failed: " + error);
        isShowingAd = false;
        AudioListener.pause = false;

        shouldReward = false;
        pendingRewardCallback = null;

        // Retry loading
        LoadRewardedAd();
    }

    public bool ShowRewardedAd(Action onReward)
    {
        if (isShowingAd)
        {
            Debug.Log("⚠️ Ad already showing");
            return false;
        }

        if (rewardedAd != null)
        {
            Debug.Log("▶️ Showing Rewarded Ad");

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