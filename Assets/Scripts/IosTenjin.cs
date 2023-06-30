using System.Collections.Generic;
using UnityEngine;

public class IosTenjin : BaseTenjin
{
	public override void Init(string apiKey)
	{
		UnityEngine.Debug.Log("iOS Initializing");
		base.ApiKey = apiKey;
	}

	public override void Init(string apiKey, string sharedSecret)
	{
		UnityEngine.Debug.Log("iOS Initializing with Shared Secret");
		base.ApiKey = apiKey;
		base.SharedSecret = sharedSecret;
	}

	public override void Connect()
	{
		UnityEngine.Debug.Log("iOS Connecting");
	}

	public override void Connect(string deferredDeeplink)
	{
		UnityEngine.Debug.Log("Connecting with deferredDeeplink " + deferredDeeplink);
	}

	public override void SendEvent(string eventName)
	{
		UnityEngine.Debug.Log("iOS Sending Event " + eventName);
	}

	public override void SendEvent(string eventName, string eventValue)
	{
		UnityEngine.Debug.Log("iOS Sending Event " + eventName + " : " + eventValue);
	}

	public override void Transaction(string productId, string currencyCode, int quantity, double unitPrice, string transactionId, string receipt, string signature)
	{
		UnityEngine.Debug.Log("iOS Transaction " + productId + ", " + currencyCode + ", " + quantity + ", " + unitPrice + ", " + transactionId + ", " + receipt + ", " + signature);
	}

	public override void GetDeeplink(Tenjin.DeferredDeeplinkDelegate deferredDeeplinkDelegate)
	{
		UnityEngine.Debug.Log("Sending IosTenjin::GetDeeplink");
	}

	public override void OptIn()
	{
		UnityEngine.Debug.Log("iOS OptIn");
	}

	public override void OptOut()
	{
		UnityEngine.Debug.Log("iOS OptOut");
	}

	public override void OptInParams(List<string> parameters)
	{
		UnityEngine.Debug.Log("iOS OptInParams");
	}

	public override void OptOutParams(List<string> parameters)
	{
		UnityEngine.Debug.Log("iOS OptOutParams");
	}
}
