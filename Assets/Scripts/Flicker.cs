using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flicker : MonoBehaviour
{
	private float flickerSpeedStart = 15f;

	private float flickerSpeedEnd = 35f;

	private float Duration = 1.5f;

	public List<GameObject> GFX = new List<GameObject>();

	public IEnumerator FlickerCoroutine(float delay)
	{
		yield return new WaitForSeconds(delay);
		float t = 0f;
		while (t < 1f)
		{
			float speed = Mathf.Lerp(flickerSpeedStart, flickerSpeedEnd, MathUtilities.Coserp(0f, 1f, t));
			float i = Mathf.Sin(Time.time * speed);
			foreach (GameObject item in GFX)
			{
				item.SetActive(i < 0f);
			}
			t += Time.deltaTime / Duration;
			yield return null;
		}
		base.gameObject.SetActive(value: false);
	}
}
