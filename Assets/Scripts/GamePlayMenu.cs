using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GamePlayMenu : MonoBehaviour
{
	public UIFader _uIfader;

	public GameObject[] _menus;

	[HideInInspector]
	public GamePlayManager _gamePlayManager;

	private GameObject _buttonClicked;

	private void Awake()
	{
		_uIfader.gameObject.SetActive(value: true);
		_uIfader.Fade(UIFader.FADE.FadeIn, 1f, 0.3f);
	}

	public void HUDMenu()
	{
		if (!MenuIsAlreadyOpen(0))
		{
			_gamePlayManager._makeNoise.PlaySFX("Pick");
			CloseAllMenus();
			OpenMenu(0);
			OpenMenu(4);
			_gamePlayManager._hUDScript.ClearInfoTexts();
			_gamePlayManager._playerControl.CancelInactiveInvoke();
			_gamePlayManager._playerControl._inactive = true;
			_gamePlayManager._playerControl._waitForInput = true;
			Time.timeScale = 1f;
		}
	}

	public void PauseMenu()
	{
		if (!MenuIsAlreadyOpen(1) && !_gamePlayManager._gameOver)
		{
			_gamePlayManager._makeNoise.PlaySFX("Pick");
			CloseAllMenus();
			OpenMenu(1);
			Time.timeScale = 0f;
		}
	}

	public void GameOverMenu()
	{
		if (!MenuIsAlreadyOpen(2))
		{
			CloseAllMenus();
			OpenMenu(2);
		}
	}

	public void LevelCompletedMenu()
	{
		if (!MenuIsAlreadyOpen(3))
		{
			CloseAllMenus();
			OpenMenu(3);
		}
	}

	private void OpenMenu(int ind)
	{
		_menus[ind].SetActive(value: true);
	}

	private void CloseAllMenus()
	{
		GameObject[] menus = _menus;
		foreach (GameObject gameObject in menus)
		{
			gameObject.SetActive(value: false);
		}
	}

	private bool MenuIsAlreadyOpen(int ind)
	{
		if (_menus[ind].activeSelf)
		{
			return true;
		}
		return false;
	}

	public void ReviveForOrange()
	{
		//AnalyticsManager.SendEventInfo("SC_Orange");
		_gamePlayManager._totalOranges -= _gamePlayManager._levelOrangeCount / 2;
		if (_gamePlayManager._totalOranges < 0)
		{
			_gamePlayManager._totalOranges = 0;
		}
		DataFunctions.SaveData(Config.PP_Orange_Count, _gamePlayManager._totalOranges);
		_gamePlayManager.RevivePlayer();
	}

	public void WatchVideoToRevive()
	{
        Object.FindObjectOfType<Admobs>().RewardShow();//(base.gameObject, "Revive");

        Object.FindObjectOfType<Admobs>().reward_type = "revive";

    }

	public void Revive()
	{
		//AnalyticsManager.SendEventInfo("SC_Rwrd");
		_gamePlayManager.RevivePlayer();
	}

	public void WatchVideoToDoubleOrange()
	{
        Object.FindObjectOfType<Admobs>().RewardShow();//(base.gameObject, "DoubleOrange");

        Object.FindObjectOfType<Admobs>().reward_type = "doubleorange";

    }

	public void DoubleOrange()
	{
		//AnalyticsManager.SendEventInfo("2X_Rwrd");
		_gamePlayManager.DoubleOranges();
	}

	public void GoToMenu(GameObject go)
	{
		_buttonClicked = go;
		LoadScene("Menu");
	}

	public void RestartLevel(GameObject go)
	{
		_buttonClicked = go;
       // Object.FindObjectOfType<Admobs>().ShowAds();// (base.gameObject, "RestartLevelExecute");
        RestartLevelExecute();

    }

	public void RestartLevelExecute()
	{
		LoadScene("Level");
	}

	public void NextLevel(GameObject go)
	{
		_buttonClicked = go;
		if (!_gamePlayManager._levelCompletedScript._doubleOrangeBtn.interactable)
		{
			NextLevelExecute();
		}
		else
		{
            Object.FindObjectOfType<Admobs>().ShowAds();//base.gameObject, "NextLevelExecute");
            NextLevelExecute();
		}
	}

	public void NextLevelExecute()
	{
		GlobalVariables._level = _gamePlayManager._currentLevel + 1;
		LoadScene("Level");
	}

	private void LoadScene(string name)
	{
		Time.timeScale = 1f;
		_gamePlayManager._makeNoise.PlaySFX("Pick");
		_buttonClicked.GetComponent<ButtonFlicker>().StartButtonFlicker();
		StartCoroutine(LoadAsync(name));
		_uIfader.gameObject.SetActive(value: true);
		_uIfader.Fade(UIFader.FADE.FadeOut, 0.5f, 0.5f);
	}

	private IEnumerator LoadAsync(string sceneName)
	{
		AsyncOperation asy = SceneManager.LoadSceneAsync(sceneName);
		while (!asy.isDone)
		{
			yield return null;
		}
	}
}
