using UnityEngine;

public class AdManager : MonoBehaviour
{
	private int adShowLimit;

	public static string uniqueUserId = "demoUserUnity";

	public static string appKey = "7d865975";

	private string _callBackFunction;

	private GameObject _callBackObject;

	private bool bannerLoaded;

	private bool rewardedLoaded;

	private void Awake()
	{
		Object.DontDestroyOnLoad(base.gameObject);
		adShowLimit = 0;
		bannerLoaded = false;
		IronSourceConfig.Instance.setClientSideCallbacks(status: true);
		IronSource.Agent.validateIntegration();
		IronSource.Agent.setUserId("uniqueUserId");
		IronSource.Agent.init(appKey);
		IronSourceEvents.onBannerAdLoadedEvent += BannerAdLoadedEvent;
		IronSourceEvents.onInterstitialAdShowSucceededEvent += InterstitialAdShowSucceededEvent;
		IronSourceEvents.onInterstitialAdClosedEvent += InterstitialAdClosedEvent;
		IronSourceEvents.onRewardedVideoAvailabilityChangedEvent += RewardedVideoAvailabilityChangedEvent;
		IronSourceEvents.onRewardedVideoAdRewardedEvent += RewardedVideoAdRewardedEvent;
	}

	private void OnApplicationPause(bool isPaused)
	{
		IronSource.Agent.onApplicationPause(isPaused);
	}

	public void RequestBanner()
	{
		if (DataFunctions.GetIntData(Config.PP_Removed_Ads) != 1)
		{
			IronSource.Agent.loadBanner(IronSourceBannerSize.BANNER, IronSourceBannerPosition.TOP);
			IronSource.Agent.hideBanner();
		}
	}

	public void ShowBanner()
	{
		if (DataFunctions.GetIntData(Config.PP_Removed_Ads) != 1)
		{
			if (!bannerLoaded)
			{
				IronSource.Agent.loadBanner(IronSourceBannerSize.BANNER, IronSourceBannerPosition.TOP);
			}
			else
			{
				IronSource.Agent.displayBanner();
			}
		}
	}

	public void HideBanner()
	{
		if (bannerLoaded)
		{
			IronSource.Agent.hideBanner();
		}
	}

	public void DestroyBanner()
	{
		if (bannerLoaded)
		{
			IronSource.Agent.destroyBanner();
			bannerLoaded = false;
		}
	}

	private void BannerAdLoadedEvent()
	{
		bannerLoaded = true;
	}

	public void RequestInterstitial()
	{
		if (DataFunctions.GetIntData(Config.PP_Removed_Ads) != 1)
		{
			IronSource.Agent.loadInterstitial();
		}
	}

	public void ShowAd(GameObject go, string callbackFunction)
	{
		if (DataFunctions.GetIntData(Config.PP_Removed_Ads) == 1 || adShowLimit < 0)
		{
			if (!string.IsNullOrEmpty(callbackFunction))
			{
				go.SendMessage(callbackFunction);
			}
			adShowLimit++;
			return;
		}
		adShowLimit = 0;
		if (!string.IsNullOrEmpty(callbackFunction))
		{
			_callBackFunction = callbackFunction;
			_callBackObject = go;
		}
		else
		{
			_callBackFunction = string.Empty;
			_callBackObject = null;
		}
		if (IronSource.Agent.isInterstitialReady())
		{
			IronSource.Agent.showInterstitial();
		}
	}

	private void InterstitialAdShowSucceededEvent()
	{
		AnalyticsManager.SendEventInfo("inter");
	}

	private void InterstitialAdClosedEvent()
	{
		RunVideoCallbackFunction();
	}

	public void ShowRewardedAd(GameObject go, string callbackFunction)
	{
		_callBackFunction = callbackFunction;
		_callBackObject = go;
		ShowIronSourceRewardedVideo();
	}

	public void ShowIronSourceRewardedVideo()
	{
		if (IronSource.Agent.isRewardedVideoAvailable())
		{
			IronSource.Agent.showRewardedVideo();
		}
	}

	private void RewardedVideoAvailabilityChangedEvent(bool canShowAd)
	{
		rewardedLoaded = canShowAd;
	}

	private void RewardedVideoAdRewardedEvent(IronSourcePlacement ssp)
	{
		RunVideoCallbackFunction();
	}

	private void RunVideoCallbackFunction()
	{
		if (!string.IsNullOrEmpty(_callBackFunction) && _callBackObject != null)
		{
			Invoke("CallFunction", 0.5f);
		}
	}

	private void CallFunction()
	{
		adShowLimit = 0;
		_callBackObject.SendMessage(_callBackFunction);
	}
}
