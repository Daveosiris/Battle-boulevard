using UnityEngine;
using UnityEngine.UI;

public class HUDScript : MonoBehaviour
{
	[Header("Player")]
	public Image _playerPortrait;

	public Text _playerNameText;

	public Image _playerHealthBar;

	[Header("Enemy")]
	public GameObject _enemyStatusHolder;

	public Text _enemyNameText;

	public Image _enemyHealthBar;

	private GameObject _currentEnemyObject;

	[Header("Level")]
	public Text[] _levelNameText;

	public Text[] _levelDiffucultyText;

	public Color[] _diffucultyColors;

	public Text[] _levelProgressText;

	public Image[] _levelBar;

	public GameObject _levelBarAdRemoved;

	public GameObject _levelBarAdNotRemoved;

	[Header("Orange")]
	public Text _collectedOrangeText;

	[Header("Effects")]
	public Color[] _colors;

	public Text[] _hitText;

	private Animator[] _hitTextAnim;

	private int _hitValue;

	public Text[] _InfoText;

	private Animator[] _InfoTextAnim;

	private int effectInd;

	private void Awake()
	{
		_levelBarAdRemoved.SetActive(value: false);
		_levelBarAdNotRemoved.SetActive(value: false);
		if (DataFunctions.GetIntData(Config.PP_Removed_Ads) == 1)
		{
			_levelBarAdRemoved.SetActive(value: true);
		}
		else
		{
			_levelBarAdNotRemoved.SetActive(value: true);
		}
		_enemyStatusHolder.SetActive(value: false);
		_hitTextAnim = new Animator[_hitText.Length];
		for (int i = 0; i < _hitText.Length; i++)
		{
			_hitText[i].text = string.Empty;
			_hitTextAnim[i] = _hitText[i].GetComponent<Animator>();
		}
		_InfoTextAnim = new Animator[_InfoText.Length];
		for (int j = 0; j < _InfoText.Length; j++)
		{
			_InfoText[j].text = string.Empty;
			_InfoTextAnim[j] = _InfoText[j].GetComponent<Animator>();
		}
	}

	public void ClearInfoTexts()
	{
		for (int i = 0; i < _hitText.Length; i++)
		{
			_hitText[i].text = string.Empty;
		}
		for (int j = 0; j < _InfoText.Length; j++)
		{
			_InfoText[j].text = string.Empty;
		}
	}

	public void HitTextEffect(int direction)
	{
		if (direction == 1)
		{
			effectInd = 0;
		}
		else
		{
			effectInd = 1;
		}
		_hitValue++;
		_hitText[effectInd].color = _colors[Random.Range(0, _colors.Length)];
		_hitText[effectInd].transform.localEulerAngles = new Vector3(0f, 0f, UnityEngine.Random.Range(-20, 20));
		_hitText[effectInd].text = _hitValue + " HIT!";
		_hitTextAnim[effectInd].SetTrigger("Hit");
		CancelInvoke("ResetHitValue");
		Invoke("ResetHitValue", 0.75f);
	}

	private void ResetHitValue()
	{
		_hitValue = 0;
	}

	public void InfoTextEffect(string text, int direction)
	{
		if (direction == 1)
		{
			effectInd = 0;
		}
		else
		{
			effectInd = 1;
		}
		_InfoText[effectInd].color = _colors[Random.Range(0, _colors.Length)];
		_InfoText[effectInd].transform.localEulerAngles = new Vector3(0f, 0f, UnityEngine.Random.Range(-20, 20));
		_InfoText[effectInd].text = text;
		CancelInvoke("InfoAnim");
		Invoke("InfoAnim", 0.2f);
	}

	private void InfoAnim()
	{
		_InfoTextAnim[effectInd].SetTrigger("Hit");
	}

	public void ChangePlayerNamePortrait(string name, Sprite portrait)
	{
		_playerNameText.text = name;
		_playerPortrait.sprite = portrait;
	}

	public void ChangePlayerHealth(float maxValue, float currentValue)
	{
		_playerHealthBar.fillAmount = currentValue / maxValue;
	}

	public void ChangeEnemyStatus(float maxValue, float currentValue, string name, GameObject gameObject)
	{
		_currentEnemyObject = gameObject;
		_enemyNameText.text = name;
		_enemyHealthBar.fillAmount = currentValue / maxValue;
	}

	public void ChangeLevelName(int level)
	{
		Text[] levelNameText = _levelNameText;
		foreach (Text text in levelNameText)
		{
			text.text = "Level " + level + " complete";
		}
		int intData = DataFunctions.GetIntData(Config.PP_Diffuculty);
		Text[] levelDiffucultyText = _levelDiffucultyText;
		foreach (Text text2 in levelDiffucultyText)
		{
			text2.text = Config.Diffuculties[intData];
			text2.color = _diffucultyColors[intData];
		}
	}

	public void ChangeLevelStatus(float percent)
	{
		Text[] levelProgressText = _levelProgressText;
		foreach (Text text in levelProgressText)
		{
			text.text = "% " + percent.ToString("f1");
		}
		Image[] levelBar = _levelBar;
		foreach (Image image in levelBar)
		{
			image.fillAmount = percent / 100f;
		}
	}

	public void ChangeCollectedOrangeText(int value)
	{
		_collectedOrangeText.text = value.ToString();
	}

	private void Update()
	{
		if ((bool)_currentEnemyObject)
		{
			_enemyStatusHolder.SetActive(_currentEnemyObject.activeSelf);
		}
	}
}
