using System.Collections;
using UnityEngine;

public class CamShake : MonoBehaviour
{
	public AnimationCurve camShakeY;

	public AnimationCurve camShakeX;

	public AnimationCurve camShakeZ;

	public float multiplier = 1f;

	public bool randomize;

	public float time = 0.5f;

	public void Shake(float intensity)
	{
		StartCoroutine(DoShake(intensity));
	}

	private IEnumerator DoShake(float scale)
	{
		Vector3 rand = new Vector3(getRandomValue(), getRandomValue(), getRandomValue());
		scale *= multiplier;
		float t = 0f;
		while (t < time)
		{
			if (randomize)
			{
				base.transform.localPosition = new Vector3(camShakeX.Evaluate(t) * scale * rand.x, camShakeY.Evaluate(t) * scale * rand.y, camShakeZ.Evaluate(t) * scale * rand.z);
			}
			else
			{
				base.transform.localPosition = new Vector3(camShakeX.Evaluate(t) * scale, camShakeY.Evaluate(t) * scale, camShakeZ.Evaluate(t) * scale);
			}
			t += Time.deltaTime / time;
			yield return null;
		}
		base.transform.localPosition = Vector3.zero;
	}

	private int getRandomValue()
	{
		int[] array = new int[2]
		{
			-1,
			1
		};
		return array[Random.Range(0, 2)];
	}
}
