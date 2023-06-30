using UnityEngine;

public static class MathUtilities
{
	public static float Sinerp(float start, float end, float value)
	{
		return Mathf.Lerp(start, end, Mathf.Sin(value * 3.14159274f * 0.5f));
	}

	public static float Coserp(float start, float end, float value)
	{
		return Mathf.Lerp(start, end, 1f - Mathf.Cos(value * 3.14159274f * 0.5f));
	}

	public static float CoSinLerp(float start, float end, float value)
	{
		return Mathf.Lerp(start, end, value * value * (3f - 2f * value));
	}
}
