using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class UIFader : MonoBehaviour
{
	public enum FADE
	{
		FadeIn,
		FadeOut
	}

	public Image img;

	public void Fade(FADE fadeDir, float fadeDuration, float StartDelay)
	{
		if (img != null)
		{
			if (fadeDir == FADE.FadeIn)
			{
				StartCoroutine(FadeCoroutine(1f, 0f, fadeDuration, StartDelay, DisableOnFinish: true));
			}
			if (fadeDir == FADE.FadeOut)
			{
				StartCoroutine(FadeCoroutine(0f, 1f, fadeDuration, StartDelay, DisableOnFinish: false));
			}
		}
	}

	private IEnumerator FadeCoroutine(float From, float To, float Duration, float StartDelay, bool DisableOnFinish)
	{
		yield return new WaitForSeconds(StartDelay);
		float t = 0f;
		Color col = img.color;
		img.enabled = true;
		img.color = new Color(col.r, col.g, col.b, From);
		while (t < 1f)
		{
			float alpha = Mathf.Lerp(From, To, t);
			img.color = new Color(col.r, col.g, col.b, alpha);
			t += Time.deltaTime / Duration;
			yield return 0;
		}
		img.color = new Color(col.r, col.g, col.b, To);
		base.gameObject.SetActive(!DisableOnFinish);
	}
}
