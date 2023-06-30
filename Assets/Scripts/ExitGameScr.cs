using ControlFreak2;
using UnityEngine;

public class ExitGameScr : MonoBehaviour
{
	public GameObject _menu;

	private float currentTimeScale;

	private void Awake()
	{
		_menu.SetActive(value: false);
	}

	private void Update()
	{
		if (CF2Input.GetKeyDown(KeyCode.Escape))
		{
			OpenCloseMenu();
		}
	}

	public void OpenCloseMenu()
	{
		if (_menu.activeSelf)
		{
			_menu.SetActive(value: false);
			Time.timeScale = currentTimeScale;
		}
		else
		{
			currentTimeScale = Time.timeScale;
			_menu.SetActive(value: true);
			Time.timeScale = 0f;
		}
	}

	public void ExitGame()
	{
		Application.Quit();
	}
}
