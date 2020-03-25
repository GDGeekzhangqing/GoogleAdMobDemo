using GoogleMobileAds.Api;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class GameManager : MonoBehaviour
{

    private BannerView bannerView;
    public Text Logger;
    public enum CallBackType
    {
        AdLoaded,
        FailedToLoad,
        AdOpened,
        AdClosed,
        AdLeavingApplication,
        Initialized,
    }

    CallBackType m_CallBackType;
    string msg;
    // Use this for initialization
    void Awake()
    {
#if UNITY_ANDROID
        string appId = "ca-app-pub-2311179396200958~3834235781";
#elif UNITY_IPHONE
            string appId = "ca-app-pub-2311179396200958~5299290886";
#else
            string appId = "unexpected_platform";
#endif
        //GoogleMobileAds.Api.MobileAds.Initialize(initStatus => {
        //    m_CallBackType = CallBackType.Initialized;
        //});//中介形式 + 账号测试ID 就能得到报告


        // Initialize the Google Mobile Ads SDK.
        MobileAds.Initialize(appId); //非中介形式
    }

    private InterstitialAd interstitial;

    public void RequestInterstitiall()
    {

#if UNITY_ANDROID
        //string adUnitId = "ca-app-pub-2311179396200958/1311519383";//正式
        string adUnitId = "ca-app-pub-3940256099942544/1033173712";
       // string d =        "ca-app-pub-3940256099942544/1033173712";
#elif UNITY_IPHONE
        string adUnitId = "ca-app-pub-3940256099942544/4411468910";
#else
        string adUnitId = "unexpected_platform";
#endif
        // Initialize an InterstitialAd.
        this.interstitial = new InterstitialAd(adUnitId);

        // Called when an ad request has successfully loaded.
        this.interstitial.OnAdLoaded += HandleOnAdLoaded;
        // Called when an ad request failed to load.
        this.interstitial.OnAdFailedToLoad += HandleOnAdFailedToLoad;
        // Called when an ad is shown.
        this.interstitial.OnAdOpening += HandleOnAdOpened;
        // Called when the ad is closed.
        this.interstitial.OnAdClosed += HandleOnAdClosed;
        // Called when the ad click caused the user to leave the application.
        this.interstitial.OnAdLeavingApplication += HandleOnAdLeavingApplication;

        // Create an empty ad request.
        AdRequest request = new AdRequest.Builder().Build();
        // Load the interstitial with the request.
        this.interstitial.LoadAd(request);
        
    }

    public void ShowAds()
    {
        if (this.interstitial.IsLoaded())
        {
            this.interstitial.Show();
        }
    }


    public void RequestBanner()
    {
#if UNITY_ANDROID
        //正式banner
        //string adUnitId = "ca-app-pub-2311179396200958/3830366354";
        //string adUnitId = "ca-app-pub-2311179396200958/9689127881";//正式应用广告ID：必须拥有要在google play服务不然会报错Failed to find provider info for com.google.android.gms.chimera

        //TEST
        string adUnitId = "ca-app-pub-3940256099942544/6300978111";//示例广告ID，能够不经过审核进行多次点击，并且唯一成功的
#elif UNITY_IPHONE
            string adUnitId = "ca-app-pub-2311179396200958/3603065834";
#else
            string adUnitId = "unexpected_platform";
#endif
        Logger.text = adUnitId + ",开始请求广告";

        // Create a 320x50 banner at the top of the screen.
        bannerView = new BannerView(adUnitId, AdSize.Banner, AdPosition.Top);

        // Called when an ad request has successfully loaded.
        bannerView.OnAdLoaded += HandleOnAdLoaded;
        // Called when an ad request failed to load.
        bannerView.OnAdFailedToLoad += HandleOnAdFailedToLoad;
        // Called when an ad is clicked.
        bannerView.OnAdOpening += HandleOnAdOpened;
        // Called when the user returned from the app after an ad click.
        bannerView.OnAdClosed += HandleOnAdClosed;
        // Called when the ad click caused the user to leave the application.
        bannerView.OnAdLeavingApplication += HandleOnAdLeavingApplication;

        //AddTestDevice("D2274BD43A961E608AB96AF29D9E6239")是用于测试的ID，在经过AS调试在logcat搜索到（具体在官网说明）
        //未通过审核情况下，
        //如果没有测试ID，且未通过审核，虽然可以请求正式广告ID，但是不能多次点击广告进入广告！同时也无法拿到报告单(即那个网站上不会有记录
        //如果有测试ID，你就能进行请求正式广告ID，可以点击，但是也不会记录到网站上，而且也算是测试模式
        //如果是中介方式初始化，那么就能够拿到报告单，但是至今未测试成功
        //在模拟器上默认是测试模式，广告请求出来后会有一句话弹出：Test Ad
        //测试ID + 中介形式请求的广告是不会带Test Ad，并且后台会记录报告（有报告）[不需要审核的！] 测试失败，原因未知，注意是必须带测试ID

        //在通过审核情况下，以上都属于废话，肯定是能成功并且有报告单
        // Create an empty ad request.
        //GoogleMobileAds.Api.AdRequest request = new GoogleMobileAds.Api.AdRequest.Builder().AddTestDevice("D2274BD43A961E608AB96AF29D9E6239").Build();
        AdRequest request = new AdRequest.Builder().Build();//在模拟器上，广告ID不需要测试ID【测试广告ID也不需要】

        // Load the banner with the request.
        bannerView.LoadAd(request);
        
    }

    private void Update()
    {
        switch (m_CallBackType)
        {
            case CallBackType.AdLoaded:
                Logger.text = "HandleAdLoaded event received";
                break;
            case CallBackType.FailedToLoad:
                Logger.text = "HandleFailedToReceiveAd event received with message: "
                     + msg;
                break;
            case CallBackType.AdOpened:
                Logger.text = "HandleAdOpened event received";
                break;
            case CallBackType.AdClosed:
                Logger.text = "HandleAdClosed event received";
                break;
            case CallBackType.AdLeavingApplication:
                Logger.text = "HandleAdLeavingApplication event received";
                break;
            case CallBackType.Initialized:
                Logger.text = "中介形式初始化";
                break;
            default:
                break;
        }
    }
    //注意下面的这些回调都是在非主线程进行的，所以不能加入Unity的对象修改方法
    public void HandleOnAdLoaded(object sender, System.EventArgs args)
    {
        MonoBehaviour.print("HandleAdLoaded event received");
        m_CallBackType = CallBackType.AdLoaded;
    }

    public void HandleOnAdFailedToLoad(object sender, AdFailedToLoadEventArgs args)
    {
        MonoBehaviour.print("HandleFailedToReceiveAd event received with message: "
                            + args.Message);
        msg = args.Message;
        m_CallBackType = CallBackType.FailedToLoad;
    }

    public void HandleOnAdOpened(object sender, System.EventArgs args)
    {
        MonoBehaviour.print("HandleAdOpened event received");
        m_CallBackType = CallBackType.AdOpened;
    }

    public void HandleOnAdClosed(object sender, System.EventArgs args)
    {
        MonoBehaviour.print("HandleAdClosed event received");
        m_CallBackType = CallBackType.AdClosed;
    }

    public void HandleOnAdLeavingApplication(object sender, System.EventArgs args)
    {
        MonoBehaviour.print("HandleAdLeavingApplication event received");
        m_CallBackType = CallBackType.AdLeavingApplication;
    }

    private void OnDestroy()
    {
        bannerView.Destroy();
        bannerView = null;

        interstitial.Destroy();
        interstitial = null;
    }

}
