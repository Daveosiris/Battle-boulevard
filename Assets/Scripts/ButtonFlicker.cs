using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ButtonFlicker : MonoBehaviour
{
	public Text buttonText;

	public float flickerSpeed = 40f;

	public float flickerDuration = 1f;

	private void OnDisable()
	{
		buttonText.enabled = true;
	}

	public void StartButtonFlicker()
	{
		StartCoroutine(ButtonFlickerCoroutine());
	}

	private IEnumerator ButtonFlickerCoroutine()
	{
		float t = 0f;
		while (t < flickerDuration)
		{
			float i = Mathf.Sin(Time.time * flickerSpeed);
			buttonText.enabled = (i > 0f);
			t += Time.deltaTime;
			yield return null;
		}
		buttonText.enabled = true;
	}
}
