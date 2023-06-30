using UnityEngine;
using UnityEngine.UI;

public class HandPointer : MonoBehaviour
{
	[HideInInspector]
	public GamePlayManager _gamePlayManager;

	private Image sprite;

	private float speed = 0.8f;

	private float delayBeforeStart = 2f;

	public AnimationCurve alphaCurve;

	private bool HandActive;

	private float t;

	private float startTime;

	private Color _currentColor;

	private void Awake()
	{
		sprite = GetComponent<Image>();
		sprite.enabled = false;
		_currentColor = sprite.color;
	}

	private void Update()
	{
		if (t > 0f && Time.time > startTime)
		{
			sprite.enabled = true;
			t -= Time.deltaTime * speed;
			sprite.color = new Color(_currentColor.r, _currentColor.g, _currentColor.b, alphaCurve.Evaluate(1f - t));
		}
		else
		{
			sprite.enabled = false;
		}
		if (HandActive && t <= 0f && Time.time > startTime)
		{
			t = 1f;
			_gamePlayManager._makeNoise.PlaySFX("HandPointer");
		}
	}

	public void ActivateHandPointer()
	{
		startTime = Time.time + delayBeforeStart;
		HandActive = true;
	}

	public void DeActivateHandPointer()
	{
		HandActive = false;
	}
}
