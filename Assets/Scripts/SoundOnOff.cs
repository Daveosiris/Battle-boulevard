using UnityEngine;
using UnityEngine.UI;

public class SoundOnOff : MonoBehaviour
{
	private Image _image;

	public Sprite[] _sprites;

	private int _ind;

	private void Awake()
	{
		_image = GetComponent<Image>();
		_ind = DataFunctions.GetIntData(Config.PP_Sound);
		_image.sprite = _sprites[_ind];
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
		DataFunctions.SaveData(Config.PP_Sound, _ind);
		_image.sprite = _sprites[_ind];
		MakeNoise makeNoise = UnityEngine.Object.FindObjectOfType<MakeNoise>();
		makeNoise.ChangeVolumeImmediate();
		makeNoise.PlaySFX("CharSelect");
	}
}
