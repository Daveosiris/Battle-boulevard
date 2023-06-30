using System.Collections.Generic;
using UnityEngine;

public class DebugTenjin : BaseTenjin
{
	public override void Connect()
	{
		UnityEngine.Debug.Log("Connecting " + base.ApiKey);
	}

	public override void Connect(string deferredDeeplink)
	{
		UnityEngine.Debug.Log("Connecting with deferredDeeplink " + deferredDeeplink);
	}

	public override void Init(string apiKey)
	{
		UnityEngine.Debug.Log("Initializing " + apiKey);
		base.ApiKey = apiKey;
	}

	public override void Init(string apiKey, string sharedSecret)
	{
		UnityEngine.Debug.Log("Initializing with secret " + apiKey);
		base.ApiKey = apiKey;
		base.SharedSecret = sharedSecret;
	}

	public override void SendEvent(string eventName)
	{
		UnityEngine.Debug.Log("Sending Event " + eventName);
	}

	public override void SendEvent(string eventName, string eventValue)
	{
		UnityEngine.Debug.Log("Sending Event " + eventName + " : " + eventValue);
	}

	public override void Transaction(string productId, string currencyCode, int quantity, double unitPrice, string transactionId, string receipt, string signature)
	{
		UnityEngine.Debug.Log("Transaction " + productId + ", " + currencyCode + ", " + quantity + ", " + unitPrice + ", " + transactionId + ", " + receipt + ", " + signature);
	}

	public override void GetDeeplink(Tenjin.DeferredDeeplinkDelegate deferredDeeplinkDelegate)
	{
		UnityEngine.Debug.Log("Sending DebugTenjin::GetDeeplink");
	}

	public override void OptIn()
	{
		UnityEngine.Debug.Log("OptIn ");
	}

	public override void OptOut()
	{
		UnityEngine.Debug.Log("OptOut ");
	}

	public override void OptInParams(List<string> parameters)
	{
		UnityEngine.Debug.Log("OptInParams");
	}

	public override void OptOutParams(List<string> parameters)
	{
		UnityEngine.Debug.Log("OptOutParams");
	}
}
