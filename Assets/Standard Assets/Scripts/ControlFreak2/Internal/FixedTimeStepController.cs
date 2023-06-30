using System;
using UnityEngine;

namespace ControlFreak2.Internal
{
	public class FixedTimeStepController
	{
		public static float deltaTime;

		private float fixedTime;

		private float fixedDeltaTime;

		private float fixedDeltaTimeCombined;

		private float timeAccum;

		private int totalFrameCount;

		private int frameSteps;

		public FixedTimeStepController(int framesPerSecond)
		{
			SetFPS(framesPerSecond);
			Reset();
		}

		public float GetDeltaTime()
		{
			return fixedDeltaTime;
		}

		public float GetDeltaTimeCombined()
		{
			return fixedDeltaTimeCombined;
		}

		public int GetFrameCount()
		{
			return totalFrameCount;
		}

		public int GetFrameSteps()
		{
			return frameSteps;
		}

		public float GetTime()
		{
			return fixedTime;
		}

		public void Reset()
		{
			fixedTime = 0f;
			totalFrameCount = 0;
			frameSteps = 0;
			fixedDeltaTimeCombined = 0f;
		}

		public void SetFPS(int framesPerSecond)
		{
			fixedDeltaTime = 1f / (float)Mathf.Max(1, framesPerSecond);
		}

		public void Update(float deltaTime)
		{
			timeAccum += deltaTime;
			fixedDeltaTimeCombined = 0f;
			for (frameSteps = 0; timeAccum > fixedDeltaTime; timeAccum -= fixedDeltaTime)
			{
				fixedDeltaTimeCombined += fixedDeltaTime;
				frameSteps++;
			}
			totalFrameCount += frameSteps;
			SetStaticData();
		}

		public void Execute(Action updateCallback)
		{
			if (frameSteps > 0)
			{
				SetStaticData();
				for (int i = 0; i < frameSteps; i++)
				{
					updateCallback();
				}
			}
		}

		public void SetStaticData()
		{
			deltaTime = fixedDeltaTime;
		}
	}
}
