using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleMobileAds;
using GoogleMobileAds.Api;
using System;
/*
public class Admobs : MonoBehaviour
{

    public RewardBasedVideoAd rewardBasedVideo;
    public InterstitialAd interstitial;
    //public int coinToAdd = 100;
    public string appid_android;
    public string inter_adid_android;
    public string reward_adid_android;
    public string appid_ios;
    public string inter_adid_ios;
    public string reward_adid_ios;
    public string reward_type;
    // public GamePlayManager _gamePlayManager;
    // Use this for initialization
    void Start()
    {

#if UNITY_ANDROID
        string appId = appid_android;
#elif UNITY_IPHONE
        string appId = appid_ios;
#else
            string appId = "unexpected_platform";
#endif

        // Initialize the Google Mobile Ads SDK.
        MobileAds.Initialize(appId);

        // Get singleton reward based video ad reference.
        this.rewardBasedVideo = RewardBasedVideoAd.Instance;

        this.RequestRewardBasedVideo();

        // Called when an ad request has successfully loaded.
        rewardBasedVideo.OnAdLoaded += HandleRewardBasedVideoLoaded;
        // Called when an ad request failed to load.
        rewardBasedVideo.OnAdFailedToLoad += HandleRewardBasedVideoFailedToLoad;
        // Called when an ad is shown.
        rewardBasedVideo.OnAdOpening += HandleRewardBasedVideoOpened;
        // Called when the ad starts to play.
        rewardBasedVideo.OnAdStarted += HandleRewardBasedVideoStarted;
        // Called when the user should be rewarded for watching a video.
        rewardBasedVideo.OnAdRewarded += HandleRewardBasedVideoRewarded;
        // Called when the ad is closed.
        rewardBasedVideo.OnAdClosed += HandleRewardBasedVideoClosed;
        // Called when the ad click caused the user to leave the application.
        rewardBasedVideo.OnAdLeavingApplication += HandleRewardBasedVideoLeftApplication;


        rewardBasedVideo.OnAdClosed += HandleRewardBasedVideoClosed;

    }

    //handleinter:

    public void HandleOnAdLoaded(object sender, EventArgs args)
    {

    }

    public void HandleOnAdFailedToLoad(object sender, AdFailedToLoadEventArgs args)
    {

    }

    public void HandleOnAdOpened(object sender, EventArgs args)
    {

    }

    public void HandleOnAdClosed(object sender, EventArgs args)
    {
        interstitial.Destroy();
    }

    public void HandleOnAdLeavingApplication(object sender, EventArgs args)
    {

    }



    //handlereward:


    public void HandleRewardBasedVideoLoaded(object sender, EventArgs args)
    {

    }

    public void HandleRewardBasedVideoFailedToLoad(object sender, AdFailedToLoadEventArgs args)
    {
        MonoBehaviour.print(
            "HandleRewardBasedVideoFailedToLoad event received with message: "
                             + args.Message);
    }

    public void HandleRewardBasedVideoOpened(object sender, EventArgs args)
    {
        MonoBehaviour.print("HandleRewardBasedVideoOpened event received");
    }

    public void HandleRewardBasedVideoStarted(object sender, EventArgs args)
    {
        MonoBehaviour.print("HandleRewardBasedVideoStarted event received");
    }

    public void HandleRewardBasedVideoClosed(object sender, EventArgs args)
    {
        this.RequestRewardBasedVideo();
    }

    public void HandleRewardBasedVideoRewarded(object sender, Reward args)
    {
        string type = args.Type;
        double amount = args.Amount;

        print("User rewarded with: " + amount.ToString() + " " + type);
        if (reward_type == "revive") { FindObjectOfType<GamePlayManager>().RevivePlayer(); }
        else if (reward_type == "doubleorange") { FindObjectOfType<GamePlayManager>().DoubleOranges(); }
        //GameManager.Instance.AddCoin(coinToAdd);
        //var hint = PlayerPrefs.GetInt("MONEY", 0);

        //hint += 5;
        //PlayerPrefs.SetInt(GlobalValue.Coins, coins);
        //PlayerPrefs.SetInt("MONEY", money);
        // GameManager.Instance.AddHints();
        //PlayerPrefs.Save();
    }

    public void HandleRewardBasedVideoLeftApplication(object sender, EventArgs args)
    {
        MonoBehaviour.print("HandleRewardBasedVideoLeftApplication event received");
    }






    //request Inter


    public void RequestInterstitial()
    {
#if UNITY_ANDROID
        string adUnitId = inter_adid_android;
#elif UNITY_IPHONE
        string adUnitId = inter_adid_ios;
#else
        string adUnitId = "unexpected_platform";
#endif

        // Initialize an InterstitialAd.
        interstitial = new InterstitialAd(adUnitId);


        // Called when an ad request has successfully loaded.
        interstitial.OnAdLoaded += HandleOnAdLoaded;
        // Called when an ad request failed to load.
        interstitial.OnAdFailedToLoad += HandleOnAdFailedToLoad;
        // Called when an ad is shown.
        interstitial.OnAdOpening += HandleOnAdOpened;
        // Called when the ad is closed.
        interstitial.OnAdClosed += HandleOnAdClosed;
        // Called when the ad click caused the user to leave the application.
        interstitial.OnAdLeavingApplication += HandleOnAdLeavingApplication;

        // Create an empty ad request.
        AdRequest request = new AdRequest.Builder().Build();

        // Load the interstitial with the request.
        interstitial.LoadAd(request);


        Debug.Log("Leule");
    }

    //showinter

    public void ShowAds()
    {



        if (interstitial.IsLoaded())
        {
            interstitial.Show();
            Debug.Log("Showroi");

        }

    }

    //requestreward:

    public void RequestRewardBasedVideo()
    {
#if UNITY_ANDROID
        string adUnitId = reward_adid_android;
#elif UNITY_IPHONE
        string adUnitId = reward_adid_ios;
#else
            string adUnitId = "unexpected_platform";
#endif

        // Create an empty ad request.
        AdRequest request = new AdRequest.Builder().Build();
        // Load the rewarded video ad with the request.
        this.rewardBasedVideo.LoadAd(request, adUnitId);
    }

    //rewardshow:


    public void RewardShow()


    {

        //GameManager.Instance.AddHints();

        if (rewardBasedVideo.IsLoaded())
        {
            rewardBasedVideo.Show();
        }
    }

}
*/