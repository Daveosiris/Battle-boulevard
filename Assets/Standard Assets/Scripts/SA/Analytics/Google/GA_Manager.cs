using System;
using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SA.Analytics.Google
{
	public class GA_Manager : MonoBehaviour
	{
		public const string GOOGLE_ANALYTICS_CLIENTID_PREF_KEY = "google_analytics_clientid_pref_key";

		public const string GOOGLE_ANALYTICS_SYSTEM_INFO_SUBMITED = "system_info_submited";

		public const string SYSTEM_INFO = "SystemInfo";

		private static GA_Manager Instance = null;

		private static string _ClientId;

		private static Client cleint = null;

		private static string CurrentLevelName;

		private static bool IsSessionStarted = false;

		private static System.Random random = new System.Random((int)DateTime.Now.Ticks);

		public string CampaignSource
		{
			get;
			private set;
		}

		public string CampaignName
		{
			get;
			private set;
		}

		public string CampaignContent
		{
			get;
			private set;
		}

		public static Client Client
		{
			get
			{
				if (cleint == null)
				{
					StartTracking();
				}
				return cleint;
			}
		}

		public static string ClientId => _ClientId;

		public static string LoadedLevelName => SceneManager.GetActiveScene().name;

		private void Awake()
		{
			if (Instance != null)
			{
				UnityEngine.Object.DestroyImmediate(base.gameObject);
				return;
			}
			Instance = this;
			UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
			base.name = "Google Analytics";
			GenerateClientId();
			cleint = new Client(_ClientId);
			if (!IsSessionStarted)
			{
				Client.CreateHit(HitType.SCREENVIEW);
				Client.StartSession();
				Client.SetUserLanguage(Application.systemLanguage.ToString());
				Client.SetScreenResolution(Screen.width, Screen.height);
				Client.Send();
				IsSessionStarted = true;
			}
			SendFirstScreenHit();
			SubmitSystemInfo();
			if (GA_Settings.Instance.AutoExceptionTracking)
			{
				Application.logMessageReceived += HandleLog;
			}
			if (GA_Settings.Instance.AutoCampaignTracking)
			{
				AndroidTools.RequestReffer();
			}
			SceneManager.sceneLoaded += delegate
			{
				TrackNewLevel();
			};
		}

		public static void SetTrackingId(string TrackingId)
		{
			StartTracking();
			cleint.GenerateHeaders(TrackingId);
		}

		public static void StartTracking()
		{
			Instance = UnityEngine.Object.FindObjectOfType<GA_Manager>();
			if (Instance == null)
			{
				string name = "Google Analytics";
				GameObject gameObject = GameObject.Find(name);
				if (gameObject == null)
				{
					gameObject = new GameObject(name);
				}
				gameObject.AddComponent<GA_Manager>();
			}
		}

		private void OnApplicationPause(bool paused)
		{
			if (GA_Settings.Instance.AutoAppResumeTracking)
			{
				if (paused)
				{
					Client.CreateHit(HitType.APPVIEW);
					Client.EndSession();
					Client.Send();
				}
				else
				{
					Client.CreateHit(HitType.APPVIEW);
					Client.StartSession();
					Client.Send();
				}
			}
		}

		private void OnApplicationQuit()
		{
			if (GA_Settings.Instance.AutoAppQuitTracking)
			{
				Client.CreateHit(HitType.APPVIEW);
				Client.EndSession();
				Client.Send();
			}
		}

		private void HandleLog(string logString, string stackTrace, LogType type)
		{
			if (GA_Settings.Instance.AutoExceptionTracking)
			{
				if (type == LogType.Exception)
				{
					Client.CreateHit(HitType.EXCEPTION);
					Client.SetExceptionDescription(logString);
					Client.SetScreenName(LoadedLevelName);
					Client.SetDocumentTitle(stackTrace);
					Client.SetIsFatalException(isFatal: false);
					Client.Send();
				}
				if (type == LogType.Error)
				{
					Client.CreateHit(HitType.EXCEPTION);
					Client.SetExceptionDescription(logString);
					Client.SetScreenName(LoadedLevelName);
					Client.SetDocumentTitle(stackTrace);
					Client.SetIsFatalException(isFatal: false);
					Client.Send();
				}
			}
		}

		private void OnReferalIntentReciver(string referrerString)
		{
			try
			{
				UnityEngine.Debug.Log("[OnReferalIntentReciver] referrerString:" + referrerString);
				int num = referrerString.LastIndexOf('?');
				UnityEngine.Debug.Log("[OnReferalIntentReciver] urlBeginIndex:" + num);
				if (num != -1)
				{
					num++;
					referrerString = referrerString.Substring(num, referrerString.Length - num);
				}
				Client.CreateHit(HitType.APPVIEW);
				Client.SetScreenName(GA_Settings.Instance.LevelPrefix + CurrentLevelName + GA_Settings.Instance.LevelPostfix);
				string[] array = referrerString.Split('&');
				string[] array2 = array;
				foreach (string text in array2)
				{
					if (!text.Equals(string.Empty))
					{
						UnityEngine.Debug.Log("[OnReferalIntentReciver] StringParams:kv " + text);
						string[] array3 = text.Split('=');
						string text2 = array3[0];
						string text3 = array3[1];
						if (!string.IsNullOrEmpty(text2) && !string.IsNullOrEmpty(text3) && text2 != null)
						{
							if (!(text2 == "utm_campaign"))
							{
								if (!(text2 == "utm_source"))
								{
									if (!(text2 == "utm_medium"))
									{
										if (!(text2 == "utm_term"))
										{
											if (!(text2 == "utm_content"))
											{
												if (text2 == "gclid")
												{
													Client.SetGoogleAdWordsID(text3);
												}
											}
											else
											{
												Client.SetCampaignContent(text3);
												CampaignContent = text3;
											}
										}
										else
										{
											string[] array4 = text3.Split('+');
											string[] array5 = array4;
											foreach (string keyword in array5)
											{
												Client.AddCampaignKeyword(keyword);
											}
										}
									}
									else
									{
										Client.SetCampaignMedium(text3);
									}
								}
								else
								{
									Client.SetCampaignSource(text3);
									CampaignSource = text3;
								}
							}
							else
							{
								Client.SetCampaignName(text3);
								CampaignName = text3;
							}
						}
					}
				}
				Client.Send();
			}
			catch (Exception ex)
			{
				UnityEngine.Debug.Log("OnReferalIntentReciver:: parsing error");
				UnityEngine.Debug.Log(ex.Message);
			}
		}

		public static void SubmitSystemInfo()
		{
			if (GA_Settings.Instance.SubmitSystemInfoOnFirstLaunch && !PlayerPrefs.HasKey("system_info_submited"))
			{
				PlayerPrefs.SetInt("system_info_submited", 1);
				Client.SendEventHit("SystemInfo", "deviceModel", SystemInfo.deviceModel);
				Client.SendEventHit("SystemInfo", "deviceName", SystemInfo.deviceName);
				Client.SendEventHit("SystemInfo", "deviceType", SystemInfo.deviceType.ToString());
				Client.SendEventHit("SystemInfo", "graphicsDeviceID", SystemInfo.graphicsDeviceID.ToString(), SystemInfo.graphicsDeviceID);
				Client.SendEventHit("SystemInfo", "graphicsDeviceVendorID", SystemInfo.graphicsDeviceVendorID.ToString(), SystemInfo.graphicsDeviceVendorID);
				Client.SendEventHit("SystemInfo", "graphicsDeviceName", SystemInfo.graphicsDeviceName);
				Client.SendEventHit("SystemInfo", "graphicsDeviceVendor", SystemInfo.graphicsDeviceVendor);
				Client.SendEventHit("SystemInfo", "graphicsDeviceVersion", SystemInfo.graphicsDeviceVersion);
				Client.SendEventHit("SystemInfo", "graphicsShaderLevel", SystemInfo.graphicsShaderLevel.ToString(), SystemInfo.graphicsShaderLevel);
				Client.SendEventHit("SystemInfo", "graphicsMemorySize", SystemInfo.graphicsMemorySize.ToString() + "MB", SystemInfo.graphicsMemorySize);
				Client.SendEventHit("SystemInfo", "systemMemorySize", SystemInfo.systemMemorySize.ToString() + "MB", SystemInfo.systemMemorySize);
				Client.SendEventHit("SystemInfo", "systemLanguage", Application.systemLanguage.ToString());
				Client.SendEventHit("SystemInfo", "operatingSystem", SystemInfo.operatingSystem);
				Client.SendEventHit("SystemInfo", "processorType", SystemInfo.processorType);
				Client.SendEventHit("SystemInfo", "processorCount", SystemInfo.processorCount.ToString(), SystemInfo.processorCount);
				Client.SendEventHit("SystemInfo", "supportsAccelerometer", (!SystemInfo.supportsAccelerometer) ? "false" : "true", SystemInfo.supportsAccelerometer ? 1 : 0);
				Client.SendEventHit("SystemInfo", "supportsLocationService", (!SystemInfo.supportsLocationService) ? "false" : "true", SystemInfo.supportsLocationService ? 1 : 0);
				Client.SendEventHit("SystemInfo", "supportsVibration", (!SystemInfo.supportsVibration) ? "false" : "true", SystemInfo.supportsVibration ? 1 : 0);
				Client.SendEventHit("SystemInfo", "supportsImageEffects", (!SystemInfo.supportsImageEffects) ? "false" : "true", SystemInfo.supportsImageEffects ? 1 : 0);
				Client.SendEventHit("SystemInfo", "supportsShadows", (!SystemInfo.supportsShadows) ? "false" : "true", SystemInfo.supportsShadows ? 1 : 0);
			}
		}

		public static void RestorePrefKeys()
		{
			PlayerPrefs.SetString("google_analytics_clientid_pref_key", _ClientId);
			PlayerPrefs.SetInt("system_info_submited", 1);
		}

		public static void Send(string request)
		{
			byte[] bytes = Encoding.UTF8.GetBytes(request);
			if (GA_Settings.Instance.IsRequetsCachingEnabled)
			{
				Instance.StartCoroutine(Instance.SendAnalytics(bytes, request));
			}
			else
			{
				SendSkipCache(request);
			}
		}

		public static WWW SendSkipCache(string request)
		{
			byte[] bytes = Encoding.UTF8.GetBytes(request);
			return Client.GenerateWWW(bytes);
		}

		private IEnumerator SendAnalytics(byte[] data, string cache)
		{
			WWW www = Client.GenerateWWW(data);
			yield return www;
			if (www.error != null)
			{
				RequestCache.SaveRequest(cache);
			}
			else
			{
				RequestCache.SendCachedRequests();
			}
			yield return null;
		}

		private static void SendFirstScreenHit()
		{
			if (GA_Settings.Instance.AutoLevelTracking)
			{
				CurrentLevelName = LoadedLevelName;
				Client.CreateHit(HitType.APPVIEW);
				Client.SetScreenResolution(Screen.currentResolution.width, Screen.currentResolution.height);
				Client.SetViewportSize(Screen.width, Screen.height);
				Client.SetUserLanguage(Application.systemLanguage.ToString());
				Client.SetScreenName(GA_Settings.Instance.LevelPrefix + CurrentLevelName + GA_Settings.Instance.LevelPostfix);
				Client.Send();
			}
		}

		private static void TrackNewLevel()
		{
			if (GA_Settings.Instance.AutoLevelTracking && !CurrentLevelName.Equals(LoadedLevelName))
			{
				CurrentLevelName = LoadedLevelName;
				Client.SendScreenHit(GA_Settings.Instance.LevelPrefix + CurrentLevelName + GA_Settings.Instance.LevelPostfix);
			}
		}

		private static void GenerateClientId()
		{
			if (PlayerPrefs.HasKey("google_analytics_clientid_pref_key"))
			{
				_ClientId = PlayerPrefs.GetString("google_analytics_clientid_pref_key");
				return;
			}
			_ClientId = RandomString(32);
			PlayerPrefs.SetString("google_analytics_clientid_pref_key", _ClientId);
		}

		private static string RandomString(int size)
		{
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = 0; i < size; i++)
			{
				char value = Convert.ToChar(Convert.ToInt32(Math.Floor(26.0 * random.NextDouble() + 65.0)));
				stringBuilder.Append(value);
			}
			return stringBuilder.ToString();
		}
	}
}
