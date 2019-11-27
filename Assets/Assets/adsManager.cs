using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Monetization;
public class adsManager : MonoBehaviour
{
    public static adsManager AM;
    private string store_id = "3356942";
    private string rewardedVideo = "rewardedVideo";
    private string video = "video";
    private string banner = "banner";
    void Start()
    {
        Monetization.Initialize(store_id, false);
        if (AM != null)
            Destroy(gameObject);
        else
        {
            AM = this;
            DontDestroyOnLoad(AM);
        }
            
    }

    public void showRewardedVideoAd()
    {
        showAd(rewardedVideo);
    }

    public void showVideoAd()
    {
        showAd(video);
    }

    public void showBanner()
    {
        showAd(banner);
    }

    private void showAd(string adType)
    {
        if (Monetization.IsReady(rewardedVideo))
        {
            ShowAdPlacementContent ad = null;
            ad = Monetization.GetPlacementContent(adType) as ShowAdPlacementContent;

            if (ad != null)
            {
                ad.Show();
            }
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            showAd("video");
        }
    }
}
