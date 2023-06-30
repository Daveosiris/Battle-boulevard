using System.Collections;
using UnityEngine;

public class CamSlowMotionDelay : MonoBehaviour
{
	public float slowMotionTimeScale = 0.2f;

	public void StartSlowMotionDelay(float duration)
	{
		StopAllCoroutines();
		StartCoroutine(SlowMotionRoutine(duration));
	}

	private IEnumerator SlowMotionRoutine(float duration)
	{
		Time.timeScale = slowMotionTimeScale;
		float startTime = Time.realtimeSinceStartup;
		while (Time.realtimeSinceStartup < startTime + duration)
		{
			yield return null;
		}
		Time.timeScale = 1f;
	}
}
