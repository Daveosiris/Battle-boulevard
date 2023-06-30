using UnityEngine;

public class MyAppStart : MonoBehaviour
{
	public static string uniqueUserId = "demoUserUnity";

	public static string appKey = "7d865975";

	private void Start()
	{
		UnityEngine.Debug.Log("unity-script: MyAppStart Start called");
		IronSourceConfig.Instance.setClientSideCallbacks(status: true);
		string advertiserId = IronSource.Agent.getAdvertiserId();
		UnityEngine.Debug.Log("unity-script: IronSource.Agent.getAdvertiserId : " + advertiserId);
		UnityEngine.Debug.Log("unity-script: IronSource.Agent.validateIntegration");
		IronSource.Agent.validateIntegration();
		UnityEngine.Debug.Log("unity-script: unity version" + IronSource.unityVersion());
		UnityEngine.Debug.Log("unity-script: IronSource.Agent.init");
		IronSource.Agent.setUserId("uniqueUserId");
		IronSource.Agent.init(appKey);
	}

	private void Update()
	{
	}

	private void OnApplicationPause(bool isPaused)
	{
		UnityEngine.Debug.Log("unity-script: OnApplicationPause = " + isPaused);
		IronSource.Agent.onApplicationPause(isPaused);
	}
}
