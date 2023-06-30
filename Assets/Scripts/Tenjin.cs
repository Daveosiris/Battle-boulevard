using System.Collections.Generic;
using UnityEngine;

public static class Tenjin
{
	public delegate void DeferredDeeplinkDelegate(Dictionary<string, string> deferredLinkData);

	private static Dictionary<string, BaseTenjin> _instances = new Dictionary<string, BaseTenjin>();

	public static BaseTenjin getInstance(string apiKey)
	{
		if (!_instances.ContainsKey(apiKey))
		{
			_instances.Add(apiKey, createTenjin(apiKey, null));
		}
		return _instances[apiKey];
	}

	public static BaseTenjin getInstance(string apiKey, string sharedSecret)
	{
		string key = apiKey + "." + sharedSecret;
		if (!_instances.ContainsKey(key))
		{
			_instances.Add(key, createTenjin(apiKey, sharedSecret));
		}
		return _instances[key];
	}

	private static BaseTenjin createTenjin(string apiKey, string sharedSecret)
	{
		GameObject gameObject = new GameObject("Tenjin");
		gameObject.hideFlags = HideFlags.HideAndDontSave;
		Object.DontDestroyOnLoad(gameObject);
		BaseTenjin baseTenjin = gameObject.AddComponent<AndroidTenjin>();
		if (sharedSecret != null)
		{
			baseTenjin.Init(apiKey, sharedSecret);
		}
		else
		{
			baseTenjin.Init(apiKey);
		}
		return baseTenjin;
	}
}
