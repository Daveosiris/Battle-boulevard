using System.Collections.Generic;
using UnityEngine;

public abstract class BaseTenjin : MonoBehaviour
{
	protected string apiKey;

	protected string sharedSecret;

	protected bool optIn;

	protected bool optOut;

	public string ApiKey
	{
		get
		{
			return apiKey;
		}
		set
		{
			apiKey = value;
		}
	}

	public string SharedSecret
	{
		get
		{
			return sharedSecret;
		}
		set
		{
			sharedSecret = value;
		}
	}

	public abstract void Init(string apiKey);

	public abstract void Init(string apiKey, string sharedSecret);

	public abstract void Connect();

	public abstract void Connect(string deferredDeeplink);

	public abstract void OptIn();

	public abstract void OptOut();

	public abstract void OptInParams(List<string> parameters);

	public abstract void OptOutParams(List<string> parameters);

	public abstract void SendEvent(string eventName);

	public abstract void SendEvent(string eventName, string eventValue);

	public abstract void Transaction(string productId, string currencyCode, int quantity, double unitPrice, string transactionId, string receipt, string signature);

	public abstract void GetDeeplink(Tenjin.DeferredDeeplinkDelegate deferredDeeplinkDelegate);
}
