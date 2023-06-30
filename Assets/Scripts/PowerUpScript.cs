using UnityEngine;
using UnityEngine.UI;

public class PowerUpScript : MonoBehaviour
{
	[HideInInspector]
	public GamePlayManager _gamePlayManager;

	public Text _powerUpInfoText;

	public Image _powerUpBar;

	private float _dropValue = 0.04f;

	private void Awake()
	{
		ResetUI();
	}

	public void ResetUI()
	{
		_powerUpInfoText.gameObject.SetActive(value: false);
		_powerUpBar.fillAmount = 0f;
	}

	private void Update()
	{
		if (_powerUpBar.fillAmount > 0f && Time.timeScale > 0f)
		{
			_powerUpBar.fillAmount -= _dropValue * Time.deltaTime;
			if (_gamePlayManager._playerControl._poweredUp && _powerUpBar.fillAmount == 0f)
			{
				_gamePlayManager._playerControl._characterControl.PowerUpEnd();
				_powerUpInfoText.gameObject.SetActive(value: false);
			}
		}
	}

	public void SetPowerUpBarValue(float value)
	{
		_powerUpBar.fillAmount = Mathf.Clamp01(_powerUpBar.fillAmount + value);
		if (_powerUpBar.fillAmount == 1f && !_gamePlayManager._playerControl._poweredUp)
		{
			_gamePlayManager._playerControl._powerUpCharacter = true;
		}
		if (_gamePlayManager._playerControl._poweredUp && _powerUpBar.fillAmount == 0f)
		{
			_gamePlayManager._playerControl._characterControl.PowerUpEnd();
			_powerUpInfoText.gameObject.SetActive(value: false);
		}
	}
}
