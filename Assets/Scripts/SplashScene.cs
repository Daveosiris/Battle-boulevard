
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SplashScene : MonoBehaviour
{
	private Animator _animator;

	private AsyncOperation asy;

	private void Start()
	{
		if (!DataFunctions.HasData(Config.PP_Sound))
		{
			DataFunctions.SaveData(Config.PP_Sound, 1);
		}
		if (!DataFunctions.HasData(Config.PP_Music))
		{
			DataFunctions.SaveData(Config.PP_Music, 1);
		}
		if (!DataFunctions.HasData(Config.PP_Haptic))
		{
			DataFunctions.SaveData(Config.PP_Haptic, 1);
		}
		_animator = GetComponent<Animator>();
		Application.targetFrameRate = 60;
		//Object.Instantiate(Resources.Load("AdManager"));
		//Object.FindObjectOfType<AdmobBanner>().RequestBanner();
		//AnalyticsManager.SendEventInfo(Config.version);
		//FB.Init();
	}

	public void CallLoadScene()
	{
		StartCoroutine(LoadAsync("Menu"));
	}

	private IEnumerator LoadAsync(string sceneName)
	{
		asy = SceneManager.LoadSceneAsync(sceneName);
		asy.allowSceneActivation = false;
		while (asy.progress < 0.9f)
		{
			yield return null;
		}
		_animator.SetTrigger("Out");
	}

	public void GoToScene()
	{
		asy.allowSceneActivation = true;
	}
}
