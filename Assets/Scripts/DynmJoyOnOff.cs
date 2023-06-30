using UnityEngine;
using UnityEngine.UI;

public class DynmJoyOnOff : MonoBehaviour
{
	private Text _text;

	public string[] texts;

	private int _ind;

	private void Awake()
	{
		_text = GetComponentInChildren<Text>();
		_ind = DataFunctions.GetIntData(Config.PP_DynmJoy);
		_text.text = texts[_ind];
	}

	public void OnClickEvent()
	{
		if (_ind == 0)
		{
			_ind = 1;
		}
		else
		{
			_ind = 0;
		}
		DataFunctions.SaveData(Config.PP_DynmJoy, _ind);
		_text.text = texts[_ind];
		MakeNoise makeNoise = UnityEngine.Object.FindObjectOfType<MakeNoise>();
		makeNoise.PlaySFX("CharSelect");
	}
}
