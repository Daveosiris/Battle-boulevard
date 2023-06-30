using System.Collections;
using System.Collections.Generic;
using GoogleMobileAds;
using GoogleMobileAds.Api;
using UnityEngine;

public class AdsLoad : MonoBehaviour {
    public Admobs Admob;
	// Use this for initialization
	public void Start ()
    {
        Admob.RequestInterstitial();
        //Admob.RequestBanner();
    }
	
	// Update is called once per frame
	void Update ()
    {
		
	}
}
