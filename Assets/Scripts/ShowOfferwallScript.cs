using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShowOfferwallScript : MonoBehaviour
{
	private GameObject InitText;

	private GameObject ShowButton;

	private GameObject ShowText;

	private GameObject AmountText;

	private int userCredits;

	private void Start()
	{
		UnityEngine.Debug.Log("ShowOfferwallScript Start called");
		ShowButton = GameObject.Find("ShowOfferwall");
		ShowText = GameObject.Find("ShowOfferwallText");
		ShowText.GetComponent<Text>().color = Color.red;
		AmountText = GameObject.Find("OWAmount");
		IronSourceEvents.onOfferwallClosedEvent += OfferwallClosedEvent;
		IronSourceEvents.onOfferwallOpenedEvent += OfferwallOpenedEvent;
		IronSourceEvents.onOfferwallShowFailedEvent += OfferwallShowFailedEvent;
		IronSourceEvents.onOfferwallAdCreditedEvent += OfferwallAdCreditedEvent;
		IronSourceEvents.onGetOfferwallCreditsFailedEvent += GetOfferwallCreditsFailedEvent;
		IronSourceEvents.onOfferwallAvailableEvent += OfferwallAvailableEvent;
	}

	private void Update()
	{
	}

	public void ShowOfferwallButtonClicked()
	{
		if (IronSource.Agent.isOfferwallAvailable())
		{
			IronSource.Agent.showOfferwall();
		}
		else
		{
			UnityEngine.Debug.Log("IronSource.Agent.isOfferwallAvailable - False");
		}
	}

	private void OfferwallOpenedEvent()
	{
		UnityEngine.Debug.Log("I got OfferwallOpenedEvent");
	}

	private void OfferwallClosedEvent()
	{
		UnityEngine.Debug.Log("I got OfferwallClosedEvent");
	}

	private void OfferwallShowFailedEvent(IronSourceError error)
	{
		UnityEngine.Debug.Log("I got OfferwallShowFailedEvent, code :  " + error.getCode() + ", description : " + error.getDescription());
	}

	private void OfferwallAdCreditedEvent(Dictionary<string, object> dict)
	{
		UnityEngine.Debug.Log("I got OfferwallAdCreditedEvent, current credits = " + dict["credits"] + " totalCredits = " + dict["totalCredits"]);
		userCredits += Convert.ToInt32(dict["credits"]);
		AmountText.GetComponent<Text>().text = string.Empty + userCredits;
	}

	private void GetOfferwallCreditsFailedEvent(IronSourceError error)
	{
		UnityEngine.Debug.Log("I got GetOfferwallCreditsFailedEvent, code :  " + error.getCode() + ", description : " + error.getDescription());
	}

	private void OfferwallAvailableEvent(bool canShowOfferwal)
	{
		UnityEngine.Debug.Log("I got OfferwallAvailableEvent, value = " + canShowOfferwal);
		if (canShowOfferwal)
		{
			ShowText.GetComponent<Text>().color = Color.blue;
		}
		else
		{
			ShowText.GetComponent<Text>().color = Color.red;
		}
	}
}
