using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Adview : MonoBehaviour {
    public Admobs admobs;
    public GameObject review;
    private int playtime;
	// Use this for initialization
	void Start () {
        //adsinit
        //GetComponent<Admobs>().RequestInterstitial();

        //GetComponent<Admobs>().RequestRewardBasedVideo();

        //GetComponent<Admobs>().ShowAds();
        //RateGame.Instance.IncreaseCustomEvents();
       // admobs.ShowAds();
        //RateGame.Instance.ShowRatePopup(PopupClosedMethod);
        playtime = PlayerPrefs.GetInt("PlayTime", 0);
        playtime += 1;
        if (playtime==2 || playtime==4 || playtime == 6 || playtime == 8 || playtime == 10 || playtime == 12 || playtime == 14)
        {
            review.SetActive(true);
        }
        PlayerPrefs.SetInt("PlayTime", playtime);
        PlayerPrefs.Save();
    }

    // Update is called once per frame
    void Update () {
		
	}

    private void PopupClosedMethod()
    {
        Debug.Log("Popup Closed -> Resume Game");
    }
}
