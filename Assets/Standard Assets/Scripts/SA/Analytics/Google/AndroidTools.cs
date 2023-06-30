using System;
using System.Collections.Generic;
using UnityEngine;

namespace SA.Analytics.Google
{
	public class AndroidTools
	{
		private static string ReferalIntentReciever = "com.androidnative.analytics.ReferalIntentReciever";

		private static Dictionary<string, AndroidJavaObject> pool = new Dictionary<string, AndroidJavaObject>();

		public static void RequestReffer()
		{
			CallStatic(ReferalIntentReciever, "RequestReferrer");
		}

		public static void CallStatic(string className, string methodName, params object[] args)
		{
			if (Application.platform == RuntimePlatform.Android)
			{
				UnityEngine.Debug.Log("AN: Using proxy for class: " + className + " method:" + methodName);
				try
				{
					AndroidJavaObject bridge;
					if (pool.ContainsKey(className))
					{
						bridge = pool[className];
					}
					else
					{
						bridge = new AndroidJavaObject(className);
						pool.Add(className, bridge);
					}
					AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
					AndroidJavaObject @static = androidJavaClass.GetStatic<AndroidJavaObject>("currentActivity");
					@static.Call("runOnUiThread", (AndroidJavaRunnable)delegate
					{
						bridge.CallStatic(methodName, args);
					});
				}
				catch (Exception ex)
				{
					UnityEngine.Debug.LogWarning(ex.Message);
				}
			}
		}
	}
}
