using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
	[Header("Character")]
	public Transform _playersHolder;

	public GameObject[] _characters;

	private GameObject[] _chs;

	private int _characterInd;

	private PlayerControl _playerControl;

	[Header("Orange")]
	private int _orangeCount;

	public Text _orangeCountText;

	[Header("UI")]
	public GameObject _commingSoonText;

	public GameObject _playBtn;

	public GameObject _unlockBtn;

	public Text _unlockPriceText;

	[Header("Upgrade")]
	public CanvasGroup _upgradePanel;

	public Text _playerName;

	public UpgradeUI _healthUI;

	public UpgradeUI _weaponTecniqueUI;

	public Text _comboCount;

	public UpgradeUI _punchPowerUI;

	public UpgradeUI _kickPowerUI;

	[Header("Level")]
	public Text _levelText;

	public Button _prewLevelBtn;

	public Button _nextLevelBtn;

	private int _level;

	private int _lastLevel;

	[Header("Diffuculty")]
	public Text _diffucultyText;

	public Color[] _diffucultyColors;

	private int _diffuculty;

	[Header("Store")]
	public CanvasGroup _storeMenu;

	public GameObject _adNotRemoved;

	public GameObject _adRemoved;

	[Header("Credits")]
	public GameObject _credits;

	public Text _creditsInfoText;

	[Header("Fader")]
	public UIFader _uIfader;

	[HideInInspector]
	public MakeNoise _makeNoise;

	private void Awake()
	{
		if ((bool)UnityEngine.Object.FindObjectOfType<AdManager>())
		{
			AdManager adManager = UnityEngine.Object.FindObjectOfType<AdManager>();
			adManager.HideBanner();
		}
		_makeNoise = GetComponent<MakeNoise>();
		_diffuculty = DataFunctions.GetIntData(Config.PP_Diffuculty);
		_diffucultyText.text = Config.Diffuculties[_diffuculty];
		_diffucultyText.color = _diffucultyColors[_diffuculty];
		SetLevelInfoForDiffuculty();
		_orangeCount = DataFunctions.GetIntData(Config.PP_Orange_Count);
		UpdateOrangeCountText();
		_chs = new GameObject[_characters.Length];
		for (int i = 0; i < _characters.Length; i++)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate(_characters[i], Vector3.zero, Quaternion.identity);
			gameObject.transform.parent = _playersHolder;
			gameObject.transform.localPosition = Vector3.zero;
			gameObject.transform.localEulerAngles = Vector3.zero;
			_chs[i] = gameObject;
		}
		if (!DataFunctions.HasData(Config.PP_SelectedCharacter_ID))
		{
			_characterInd = 0;
		}
		else
		{
			for (int j = 0; j < _chs.Length; j++)
			{
				if (_chs[j].GetComponent<PlayerControl>()._id == DataFunctions.GetIntData(Config.PP_SelectedCharacter_ID))
				{
					_characterInd = j;
					break;
				}
			}
		}
		ChangeCharacter();
		_storeMenu.alpha = 0f;
		_storeMenu.interactable = false;
		_storeMenu.blocksRaycasts = false;
		_credits.SetActive(value: false);
		_creditsInfoText.text = "Settings\n" + Config.version;
		_uIfader.gameObject.SetActive(value: true);
		_uIfader.Fade(UIFader.FADE.FadeIn, 1f, 0.3f);
	}

	public void ChangeDifficulty(int i)
	{
		_diffuculty += i;
		if (_diffuculty < 0)
		{
			_diffuculty = Config.Diffuculties.Length - 1;
		}
		if (_diffuculty > Config.Diffuculties.Length - 1)
		{
			_diffuculty = 0;
		}
		DataFunctions.SaveData(Config.PP_Diffuculty, _diffuculty);
		_diffucultyText.text = Config.Diffuculties[_diffuculty];
		_diffucultyText.color = _diffucultyColors[_diffuculty];
		_makeNoise.PlaySFX("CharSelect");
		SetLevelInfoForDiffuculty();
	}

	public void SetOrangeCount(int count)
	{
		_orangeCount += count;
		DataFunctions.SaveData(Config.PP_Orange_Count, _orangeCount);
		UpdateOrangeCountText();
		_makeNoise.PlaySFX("ButtonStart");
	}

	private void UpdateOrangeCountText()
	{
		_orangeCountText.text = _orangeCount.ToString();
	}

	public void SelectLevel(int i)
	{
		_makeNoise.PlaySFX("CharSelect");
		_level += i;
		if (_level < 1)
		{
			_level = 1;
		}
		else if (_level > _lastLevel)
		{
			_level = _lastLevel;
		}
		ChangeLevel();
	}

	private void SetLevelInfoForDiffuculty()
	{
		_lastLevel = DataFunctions.GetIntData(Config.PP_CityLevel_Index + Config.LevelDiffucultyPrefix[_diffuculty]);
		if (_lastLevel == 0)
		{
			_lastLevel = 1;
		}
		if (_lastLevel > Config.CityTotalLevel)
		{
			_lastLevel--;
		}
		_level = _lastLevel;
		ChangeLevel();
	}

	private void SetLevelButtons()
	{
		if (_level > 1)
		{
			_prewLevelBtn.interactable = true;
		}
		else
		{
			_prewLevelBtn.interactable = false;
		}
		if (_level < _lastLevel)
		{
			_nextLevelBtn.interactable = true;
		}
		else
		{
			_nextLevelBtn.interactable = false;
		}
	}

	private void ChangeLevel()
	{
		SetLevelButtons();
		_levelText.text = "Level " + _level;
	}

	public void SelectChar(int i)
	{
		_makeNoise.PlaySFX("CharSelect");
		_characterInd += i;
		if (_characterInd < 0)
		{
			_characterInd = _characters.Length - 1;
		}
		else if (_characterInd > _characters.Length - 1)
		{
			_characterInd = 0;
		}
		ChangeCharacter();
	}

	private void ChangeCharacter()
	{
		_playerControl = _chs[_characterInd].GetComponent<PlayerControl>();
		_unlockBtn.SetActive(value: false);
		_playBtn.SetActive(value: false);
		_commingSoonText.SetActive(value: false);
		GameObject[] chs = _chs;
		foreach (GameObject gameObject in chs)
		{
			gameObject.transform.GetChild(0).GetChild(0).gameObject.SetActive(value: false);
			gameObject.transform.GetChild(0).GetChild(1).gameObject.SetActive(value: false);
		}
		_chs[_characterInd].transform.GetChild(0).GetChild(0).gameObject.SetActive(value: true);
		_chs[_characterInd].transform.GetChild(0).GetChild(1).gameObject.SetActive(value: true);
		if (_characterInd == _chs.Length - 1)
		{
			_commingSoonText.SetActive(value: true);
			_upgradePanel.alpha = 0f;
			return;
		}
		_playerControl.SetValues();
		FillUpgradeUI();
		if (_playerControl._unlocked)
		{
			_playBtn.SetActive(value: true);
			ActiveDeactiveUpgradeButtons(AcDe: true);
		}
		else if (DataFunctions.GetIntData(Config.PP_Character + _playerControl._id) == 0)
		{
			_unlockBtn.SetActive(value: true);
			_unlockPriceText.text = _playerControl._price.ToString();
			ActiveDeactiveUpgradeButtons(AcDe: false);
		}
		else
		{
			_playBtn.SetActive(value: true);
			ActiveDeactiveUpgradeButtons(AcDe: true);
		}
	}

	private void ActiveDeactiveUpgradeButtons(bool AcDe)
	{
		if (AcDe)
		{
			_upgradePanel.alpha = 1f;
		}
		else
		{
			_upgradePanel.alpha = 0.75f;
		}
		_healthUI.Btn.SetActive(AcDe);
		_weaponTecniqueUI.Btn.SetActive(AcDe);
		_punchPowerUI.Btn.SetActive(AcDe);
		_kickPowerUI.Btn.SetActive(AcDe);
		CheckUpgradeCounts();
	}

	private void CheckUpgradeCounts()
	{
		if (DataFunctions.GetIntData(_playerControl._id.ToString() + Config.PP_CharacterHealthUpgradeCount) >= Config.MaximumUpgradeCount)
		{
			_healthUI.Btn.SetActive(value: false);
		}
		if (DataFunctions.GetIntData(_playerControl._id.ToString() + Config.PP_CharacterWeaponTecniqueUpgradeCount) >= Config.MaximumUpgradeCount)
		{
			_weaponTecniqueUI.Btn.SetActive(value: false);
		}
		if (DataFunctions.GetIntData(_playerControl._id.ToString() + Config.PP_CharacterPunchPowerUpgradeCount) >= Config.MaximumUpgradeCount)
		{
			_punchPowerUI.Btn.SetActive(value: false);
		}
		if (DataFunctions.GetIntData(_playerControl._id.ToString() + Config.PP_CharacterKickPowerUpgradeCount) >= Config.MaximumUpgradeCount)
		{
			_kickPowerUI.Btn.SetActive(value: false);
		}
	}

	public void UpgradeHealth()
	{
		if (!NotEnoughOranges(_playerControl._healthUpPrice))
		{
			string text = _playerControl._id.ToString() + Config.PP_CharacterHealthUpgradeCount;
			int intData = DataFunctions.GetIntData(text);
			UpgradePlayer(text, intData + 1, _playerControl._healthUpPrice);
		}
	}

	public void UpgradeWeaponTecnique()
	{
		if (!NotEnoughOranges(_playerControl._weaponTecUpPrice))
		{
			string text = _playerControl._id.ToString() + Config.PP_CharacterWeaponTecniqueUpgradeCount;
			int intData = DataFunctions.GetIntData(text);
			UpgradePlayer(text, intData + 1, _playerControl._weaponTecUpPrice);
		}
	}

	public void UpgradePunchPower()
	{
		if (!NotEnoughOranges(_playerControl._punchUpgradePrice))
		{
			string text = _playerControl._id.ToString() + Config.PP_CharacterPunchPowerUpgradeCount;
			int intData = DataFunctions.GetIntData(text);
			UpgradePlayer(text, intData + 1, _playerControl._punchUpgradePrice);
		}
	}

	public void UpgradeKickPower()
	{
		if (!NotEnoughOranges(_playerControl._kickUpgradePrice))
		{
			string text = _playerControl._id.ToString() + Config.PP_CharacterKickPowerUpgradeCount;
			int intData = DataFunctions.GetIntData(text);
			UpgradePlayer(text, intData + 1, _playerControl._kickUpgradePrice);
		}
	}

	private void UpgradePlayer(string key, int value, int price)
	{
		SetOrangeCount(-price);
		DataFunctions.SaveData(key, value);
		_playerControl.SetValues();
		FillUpgradeUI();
		CheckUpgradeCounts();
	}

	public void UnlockCharacter()
	{
		if (!NotEnoughOranges(_playerControl._price))
		{
			SetOrangeCount(-_playerControl._price);
			DataFunctions.SaveData(Config.PP_Character + _playerControl._id, 1);
			_unlockBtn.SetActive(value: false);
			_playBtn.SetActive(value: true);
			ActiveDeactiveUpgradeButtons(AcDe: true);
		}
	}

	private void FillUpgradeUI()
	{
		_playerName.text = _playerControl._name;
		_healthUI.Value.text = _playerControl._healthU.ToString("f1");
		_healthUI.Price.text = _playerControl._healthUpPrice.ToString();
		_healthUI.Bar.fillAmount = _playerControl._healthU / Config.MaximumPlayerHealth;
		_weaponTecniqueUI.Value.text = (_playerControl._weaponTecniqueU * 10f).ToString("f1");
		_weaponTecniqueUI.Price.text = _playerControl._weaponTecUpPrice.ToString();
		_weaponTecniqueUI.Bar.fillAmount = _playerControl._weaponTecniqueU / Config.MaximumPlayerWeaponTecnique;
		_comboCount.text = _playerControl.GetComponent<CharacterControl>()._punches.Length + _playerControl.GetComponent<CharacterControl>()._kicks.Length + " Combos";
		_punchPowerUI.Value.text = _playerControl._punchPowerU.ToString("f1");
		_punchPowerUI.Price.text = _playerControl._punchUpgradePrice.ToString();
		_punchPowerUI.Bar.fillAmount = _playerControl._punchPowerU / Config.MaximumPlayerPunchPower;
		_kickPowerUI.Value.text = _playerControl._kickPowerU.ToString("f1");
		_kickPowerUI.Price.text = _playerControl._kickUpgradePrice.ToString();
		_kickPowerUI.Bar.fillAmount = _playerControl._kickPowerU / Config.MaximumPlayerKickPower;
	}

	private bool NotEnoughOranges(int price)
	{
		if (price > _orangeCount)
		{
			//OpenCloseStoreMenu();
			return true;
		}
		return false;
	}

	public void OpenCloseStoreMenu()
	{
        /*
		_makeNoise.PlaySFX("CharSelect");
		if (_storeMenu.interactable)
		{
			_storeMenu.alpha = 0f;
			_storeMenu.interactable = false;
			_storeMenu.blocksRaycasts = false;
			return;
		}
		_adNotRemoved.SetActive(value: false);
		_adRemoved.SetActive(value: false);
		if (DataFunctions.GetIntData(Config.PP_Removed_Ads) == 1)
		{
			_adRemoved.SetActive(value: true);
		}
		else
		{
			_adNotRemoved.SetActive(value: true);
		}
		_storeMenu.alpha = 1f;
		_storeMenu.interactable = true;
		_storeMenu.blocksRaycasts = true;
        */
	}

	public void OpenCloseCreditMenu()
	{
		_makeNoise.PlaySFX("CharSelect");
		_credits.SetActive(!_credits.activeSelf);
	}

	public void PrivacyURL()
	{
		Application.OpenURL("https://www.parakogames.com/privacy-policy");
	}

	public void TermsURL()
	{
		Application.OpenURL("https://www.parakogames.com/terms-conditions");
	}

	public void PlayGame()
	{
		_makeNoise.PlaySFX("ButtonStart");
		GlobalVariables._player = _characters[_characterInd];
		GlobalVariables._level = _level;
		DataFunctions.SaveData(Config.PP_SelectedCharacter_ID, _chs[_characterInd].GetComponent<PlayerControl>()._id);
		AnalyticsManager.SendEventInfo(Config.Diffuculties[DataFunctions.GetIntData(Config.PP_Diffuculty)]);
		_uIfader.gameObject.SetActive(value: true);
		_uIfader.Fade(UIFader.FADE.FadeOut, 0.3f, 0f);
		Invoke("loadLevel", 0.5f);
	}

	private void loadLevel()
	{
		SceneManager.LoadScene("Level");
	}
}
