using System;
using System.Collections.Generic;
using UnityEngine;

public class AndroidTenjin : BaseTenjin
{
	private class DeferredDeeplinkListener : AndroidJavaProxy
	{
		private Tenjin.DeferredDeeplinkDelegate callback;

		public DeferredDeeplinkListener(Tenjin.DeferredDeeplinkDelegate deferredDeeplinkCallback)
			: base("com.tenjin.android.Callback")
		{
			callback = deferredDeeplinkCallback;
		}

		public void onSuccess(bool clickedTenjinLink, bool isFirstSession, AndroidJavaObject data)
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			string value = data.Call<string>("get", new object[1]
			{
				"ad_network"
			});
			string value2 = data.Call<string>("get", new object[1]
			{
				"campaign_id"
			});
			string value3 = data.Call<string>("get", new object[1]
			{
				"advertising_id"
			});
			string value4 = data.Call<string>("get", new object[1]
			{
				"deferred_deeplink_url"
			});
			if (!string.IsNullOrEmpty(value))
			{
				dictionary["ad_network"] = value;
			}
			if (!string.IsNullOrEmpty(value2))
			{
				dictionary["campaign_id"] = value2;
			}
			if (!string.IsNullOrEmpty(value3))
			{
				dictionary["advertising_id"] = value3;
			}
			if (!string.IsNullOrEmpty(value4))
			{
				dictionary["deferred_deeplink_url"] = value4;
			}
			dictionary.Add("clicked_tenjin_link", Convert.ToString(clickedTenjinLink));
			dictionary.Add("is_first_session", Convert.ToString(isFirstSession));
			callback(dictionary);
		}
	}

	private const string AndroidJavaTenjinClass = "com.tenjin.android.TenjinSDK";

	private AndroidJavaObject tenjinJava;

	private AndroidJavaObject activity;

	public override void Init(string apiKey)
	{
		if (Debug.isDebugBuild)
		{
			UnityEngine.Debug.Log("Android Initializing");
		}
		base.ApiKey = apiKey;
		initActivity();
		AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.tenjin.android.TenjinSDK");
		if (androidJavaClass == null)
		{
			throw new MissingReferenceException(string.Format("AndroidTenjin failed to load {0} class", "com.tenjin.android.TenjinSDK"));
		}
		tenjinJava = androidJavaClass.CallStatic<AndroidJavaObject>("getInstance", new object[2]
		{
			activity,
			apiKey
		});
	}

	public override void Init(string apiKey, string sharedSecret)
	{
		if (Debug.isDebugBuild)
		{
			UnityEngine.Debug.Log("Android Initializing with Shared Secret");
		}
		base.ApiKey = apiKey;
		base.SharedSecret = sharedSecret;
		initActivity();
		AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.tenjin.android.TenjinSDK");
		if (androidJavaClass == null)
		{
			throw new MissingReferenceException(string.Format("AndroidTenjin failed to load {0} class", "com.tenjin.android.TenjinSDK"));
		}
		tenjinJava = androidJavaClass.CallStatic<AndroidJavaObject>("getInstance", new object[3]
		{
			activity,
			apiKey,
			sharedSecret
		});
	}

	private void initActivity()
	{
		AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
		activity = androidJavaClass.GetStatic<AndroidJavaObject>("currentActivity");
	}

	public override void Connect()
	{
		string text = null;
		if (optIn)
		{
			text = "optin";
		}
		else if (optOut)
		{
			text = "optout";
		}
		object[] args = new object[2]
		{
			null,
			text
		};
		tenjinJava.Call("connect", args);
	}

	public override void Connect(string deferredDeeplink)
	{
		string text = null;
		if (optIn)
		{
			text = "optin";
		}
		else if (optOut)
		{
			text = "optout";
		}
		object[] args = new object[2]
		{
			deferredDeeplink,
			text
		};
		tenjinJava.Call("connect", args);
	}

	public override void SendEvent(string eventName)
	{
		object[] args = new object[1]
		{
			eventName
		};
		tenjinJava.Call("eventWithName", args);
	}

	public override void SendEvent(string eventName, string eventValue)
	{
		object[] args = new object[2]
		{
			eventName,
			eventValue
		};
		tenjinJava.Call("eventWithNameAndValue", args);
	}

	public override void Transaction(string productId, string currencyCode, int quantity, double unitPrice, string transactionId, string receipt, string signature)
	{
		transactionId = null;
		if (receipt != null && signature != null)
		{
			object[] args = new object[6]
			{
				productId,
				currencyCode,
				quantity,
				unitPrice,
				receipt,
				signature
			};
			if (Debug.isDebugBuild)
			{
				UnityEngine.Debug.Log("Android Transaction " + productId + ", " + currencyCode + ", " + quantity + ", " + unitPrice + ", " + receipt + ", " + signature);
			}
			tenjinJava.Call("transaction", args);
		}
		else
		{
			object[] args2 = new object[4]
			{
				productId,
				currencyCode,
				quantity,
				unitPrice
			};
			if (Debug.isDebugBuild)
			{
				UnityEngine.Debug.Log("Android Transaction " + productId + ", " + currencyCode + ", " + quantity + ", " + unitPrice);
			}
			tenjinJava.Call("transaction", args2);
		}
	}

	public override void GetDeeplink(Tenjin.DeferredDeeplinkDelegate deferredDeeplinkDelegate)
	{
		DeferredDeeplinkListener deferredDeeplinkListener = new DeferredDeeplinkListener(deferredDeeplinkDelegate);
		tenjinJava.Call("getDeeplink", deferredDeeplinkListener);
	}

	public override void OptIn()
	{
		optIn = true;
		tenjinJava.Call("optIn");
	}

	public override void OptOut()
	{
		optOut = true;
		tenjinJava.Call("optOut");
	}

	public override void OptInParams(List<string> parameters)
	{
		tenjinJava.Call("optInParams", new object[1]
		{
			parameters.ToArray()
		});
	}

	public override void OptOutParams(List<string> parameters)
	{
		tenjinJava.Call("optOutParams", new object[1]
		{
			parameters.ToArray()
		});
	}
}
