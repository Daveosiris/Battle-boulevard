using UnityEngine;
using UnityEngine.UI;

public class ShowInterstitialScript : MonoBehaviour
{
	private GameObject InitText;

	private GameObject LoadButton;

	private GameObject LoadText;

	private GameObject ShowButton;

	private GameObject ShowText;

	public static string INTERSTITIAL_INSTANCE_ID = "0";

	private void Start()
	{
		UnityEngine.Debug.Log("unity-script: ShowInterstitialScript Start called");
		LoadButton = GameObject.Find("LoadInterstitial");
		LoadText = GameObject.Find("LoadInterstitialText");
		LoadText.GetComponent<Text>().color = Color.blue;
		ShowButton = GameObject.Find("ShowInterstitial");
		ShowText = GameObject.Find("ShowInterstitialText");
		ShowText.GetComponent<Text>().color = Color.red;
		IronSourceEvents.onInterstitialAdReadyEvent += InterstitialAdReadyEvent;
		IronSourceEvents.onInterstitialAdLoadFailedEvent += InterstitialAdLoadFailedEvent;
		IronSourceEvents.onInterstitialAdShowSucceededEvent += InterstitialAdShowSucceededEvent;
		IronSourceEvents.onInterstitialAdShowFailedEvent += InterstitialAdShowFailedEvent;
		IronSourceEvents.onInterstitialAdClickedEvent += InterstitialAdClickedEvent;
		IronSourceEvents.onInterstitialAdOpenedEvent += InterstitialAdOpenedEvent;
		IronSourceEvents.onInterstitialAdClosedEvent += InterstitialAdClosedEvent;
		IronSourceEvents.onInterstitialAdReadyDemandOnlyEvent += InterstitialAdReadyDemandOnlyEvent;
		IronSourceEvents.onInterstitialAdLoadFailedDemandOnlyEvent += InterstitialAdLoadFailedDemandOnlyEvent;
		IronSourceEvents.onInterstitialAdShowSucceededDemandOnlyEvent += InterstitialAdShowSucceededDemandOnlyEvent;
		IronSourceEvents.onInterstitialAdShowFailedDemandOnlyEvent += InterstitialAdShowFailedDemandOnlyEvent;
		IronSourceEvents.onInterstitialAdClickedDemandOnlyEvent += InterstitialAdClickedDemandOnlyEvent;
		IronSourceEvents.onInterstitialAdOpenedDemandOnlyEvent += InterstitialAdOpenedDemandOnlyEvent;
		IronSourceEvents.onInterstitialAdClosedDemandOnlyEvent += InterstitialAdClosedDemandOnlyEvent;
		IronSourceEvents.onInterstitialAdRewardedEvent += InterstitialAdRewardedEvent;
	}

	private void Update()
	{
	}

	public void LoadInterstitialButtonClicked()
	{
		UnityEngine.Debug.Log("unity-script: LoadInterstitialButtonClicked");
		IronSource.Agent.loadInterstitial();
	}

	public void ShowInterstitialButtonClicked()
	{
		UnityEngine.Debug.Log("unity-script: ShowInterstitialButtonClicked");
		if (IronSource.Agent.isInterstitialReady())
		{
			IronSource.Agent.showInterstitial();
		}
		else
		{
			UnityEngine.Debug.Log("unity-script: IronSource.Agent.isInterstitialReady - False");
		}
	}

	private void LoadDemandOnlyInterstitial()
	{
		UnityEngine.Debug.Log("unity-script: LoadDemandOnlyInterstitialButtonClicked");
		IronSource.Agent.loadISDemandOnlyInterstitial(INTERSTITIAL_INSTANCE_ID);
	}

	private void ShowDemandOnlyInterstitial()
	{
		UnityEngine.Debug.Log("unity-script: ShowDemandOnlyInterstitialButtonClicked");
		if (IronSource.Agent.isISDemandOnlyInterstitialReady(INTERSTITIAL_INSTANCE_ID))
		{
			IronSource.Agent.showISDemandOnlyInterstitial(INTERSTITIAL_INSTANCE_ID);
		}
		else
		{
			UnityEngine.Debug.Log("unity-script: IronSource.Agent.isISDemandOnlyInterstitialReady - False");
		}
	}

	private void InterstitialAdReadyEvent()
	{
		UnityEngine.Debug.Log("unity-script: I got InterstitialAdReadyEvent");
		ShowText.GetComponent<Text>().color = Color.blue;
	}

	private void InterstitialAdLoadFailedEvent(IronSourceError error)
	{
		UnityEngine.Debug.Log("unity-script: I got InterstitialAdLoadFailedEvent, code: " + error.getCode() + ", description : " + error.getDescription());
	}

	private void InterstitialAdShowSucceededEvent()
	{
		UnityEngine.Debug.Log("unity-script: I got InterstitialAdShowSucceededEvent");
		ShowText.GetComponent<Text>().color = Color.red;
	}

	private void InterstitialAdShowFailedEvent(IronSourceError error)
	{
		UnityEngine.Debug.Log("unity-script: I got InterstitialAdShowFailedEvent, code :  " + error.getCode() + ", description : " + error.getDescription());
		ShowText.GetComponent<Text>().color = Color.red;
	}

	private void InterstitialAdClickedEvent()
	{
		UnityEngine.Debug.Log("unity-script: I got InterstitialAdClickedEvent");
	}

	private void InterstitialAdOpenedEvent()
	{
		UnityEngine.Debug.Log("unity-script: I got InterstitialAdOpenedEvent");
	}

	private void InterstitialAdClosedEvent()
	{
		UnityEngine.Debug.Log("unity-script: I got InterstitialAdClosedEvent");
	}

	private void InterstitialAdRewardedEvent()
	{
		UnityEngine.Debug.Log("unity-script: I got InterstitialAdRewardedEvent");
	}

	private void InterstitialAdReadyDemandOnlyEvent(string instanceId)
	{
		UnityEngine.Debug.Log("unity-script: I got InterstitialAdReadyDemandOnlyEvent for instance: " + instanceId);
		ShowText.GetComponent<Text>().color = Color.blue;
	}

	private void InterstitialAdLoadFailedDemandOnlyEvent(string instanceId, IronSourceError error)
	{
		UnityEngine.Debug.Log("unity-script: I got InterstitialAdLoadFailedDemandOnlyEvent for instance: " + instanceId + ", error code: " + error.getCode() + ",error description : " + error.getDescription());
	}

	private void InterstitialAdShowSucceededDemandOnlyEvent(string instanceId)
	{
		UnityEngine.Debug.Log("unity-script: I got InterstitialAdShowSucceededDemandOnlyEvent for instance: " + instanceId);
		ShowText.GetComponent<Text>().color = Color.red;
	}

	private void InterstitialAdShowFailedDemandOnlyEvent(string instanceId, IronSourceError error)
	{
		UnityEngine.Debug.Log("unity-script: I got InterstitialAdShowFailedDemandOnlyEvent for instance: " + instanceId + ", error code :  " + error.getCode() + ",error description : " + error.getDescription());
		ShowText.GetComponent<Text>().color = Color.red;
	}

	private void InterstitialAdClickedDemandOnlyEvent(string instanceId)
	{
		UnityEngine.Debug.Log("unity-script: I got InterstitialAdClickedDemandOnlyEvent for instance: " + instanceId);
	}

	private void InterstitialAdOpenedDemandOnlyEvent(string instanceId)
	{
		UnityEngine.Debug.Log("unity-script: I got InterstitialAdOpenedDemandOnlyEvent for instance: " + instanceId);
	}

	private void InterstitialAdClosedDemandOnlyEvent(string instanceId)
	{
		UnityEngine.Debug.Log("unity-script: I got InterstitialAdClosedDemandOnlyEvent for instance: " + instanceId);
	}

	private void InterstitialAdRewardedDemandOnlyEvent(string instanceId)
	{
		UnityEngine.Debug.Log("unity-script: I got InterstitialAdRewardedDemandOnlyEvent for instance: " + instanceId);
	}
}
