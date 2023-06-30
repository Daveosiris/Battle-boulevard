using System.Collections;
using UnityEngine;

public class ColorEffectScript : MonoBehaviour
{
	public GameObject _effectVignette;

	private CanvasGroup _canvasGroup;

	private ImageEffectt _imageEffectt;

	public Gradient _specialBoss;

	public Gradient _powerUp;

	private bool _specialBossScene;

	public void AwakeProc()
	{
		_canvasGroup = _effectVignette.GetComponent<CanvasGroup>();
		_imageEffectt = _effectVignette.GetComponent<ImageEffectt>();
	}

	public void SpecialBossEffect()
	{
		_specialBossScene = true;
		_imageEffectt.ColorTransition = _specialBoss;
		_imageEffectt.speed = 1f;
		_canvasGroup.alpha = 1f;
		_effectVignette.SetActive(value: true);
	}

	public void PowerUpEffect()
	{
		if (!_specialBossScene)
		{
			StartCoroutine(FadeCoroutine(0f, 1f, 0.5f));
		}
		_imageEffectt.ColorTransition = _powerUp;
		_imageEffectt.speed = 1.6f;
	}

	public void DisableEffect()
	{
		if (_specialBossScene)
		{
			_imageEffectt.ColorTransition = _specialBoss;
			_imageEffectt.speed = 1f;
		}
		else
		{
			StartCoroutine(FadeCoroutine(1f, 0f, 0.5f));
		}
	}

	private IEnumerator FadeCoroutine(float From, float To, float Duration)
	{
		_effectVignette.SetActive(value: true);
		float t = 0f;
		while (t < 1f)
		{
			_canvasGroup.alpha = Mathf.Lerp(From, To, t);
			t += Time.deltaTime / Duration;
			yield return 0;
		}
		_canvasGroup.alpha = To;
		if (To == 0f)
		{
			_effectVignette.SetActive(value: false);
		}
	}
}
