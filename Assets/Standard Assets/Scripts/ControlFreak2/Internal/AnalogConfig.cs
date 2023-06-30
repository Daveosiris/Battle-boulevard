using System;
using UnityEngine;

namespace ControlFreak2.Internal
{
	[Serializable]
	public class AnalogConfig
	{
		public float analogDeadZone;

		public float analogEndZone;

		public float analogRangeStartValue;

		public float digitalEnterThresh;

		public float digitalLeaveThresh;

		public float digitalToAnalogPressSpeed;

		public float digitalToAnalogReleaseSpeed;

		public const float DIGITAL_TO_ANALOG_SMOOTHING_TIME = 0.2f;

		public bool useRamp;

		public AnimationCurve ramp;

		private const float MIN_DIGITAL_DEADZONE = 0.05f;

		private const float MAX_DIGITAL_DEADZONE = 0.9f;

		private const float MIN_ANALOG_DEADZONE = 0f;

		private const float MAX_ANALOG_DEADZONE = 0.9f;

		private const float MIN_ANALOG_ENDZONE = 0.1f;

		private const float MAX_ANALOG_ENDZONE = 1f;

		private const float MAX_DIGI_LEAVE_MARGIN = 0.1f;

		private const float MIN_DIGI_LEAVE_THRESH = 0.1f;

		public AnalogConfig()
		{
			analogDeadZone = 0.1f;
			analogEndZone = 1f;
			analogRangeStartValue = 0f;
			digitalEnterThresh = 0.5f;
			digitalLeaveThresh = 0.2f;
			digitalToAnalogPressSpeed = 0.5f;
			digitalToAnalogReleaseSpeed = 0.5f;
			ramp = new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(0.5f, 0.5f), new Keyframe(1f, 1f));
		}

		public float GetAnalogVal(float rawVal)
		{
			float num = Mathf.Abs(rawVal);
			if (num <= analogDeadZone)
			{
				return 0f;
			}
			if (num >= analogEndZone)
			{
				return (rawVal >= 0f) ? 1 : (-1);
			}
			float num2 = (num - analogDeadZone) / (analogEndZone - analogDeadZone);
			num2 = ((!useRamp || ramp == null) ? Mathf.Lerp(analogRangeStartValue, 1f, num2) : Mathf.Clamp01(ramp.Evaluate(num2)));
			return (!(rawVal >= 0f)) ? (0f - num2) : num2;
		}

		public int GetSignedDigitalVal(float rawVal, int prevDigiVal)
		{
			if (rawVal >= 0f)
			{
				return (rawVal > ((prevDigiVal != 1) ? digitalEnterThresh : digitalLeaveThresh)) ? 1 : 0;
			}
			return (rawVal < ((prevDigiVal != -1) ? (0f - digitalEnterThresh) : (0f - digitalLeaveThresh))) ? (-1) : 0;
		}

		public bool GetDigitalVal(float rawVal, bool prevDigiVal)
		{
			return rawVal > ((!prevDigiVal) ? digitalEnterThresh : digitalLeaveThresh);
		}

		public float AnimateDigitalToAnalog(float curVal, float targetVal, bool pressed)
		{
			return CFUtils.SmoothTowards(curVal, targetVal, ((!pressed) ? digitalToAnalogReleaseSpeed : digitalToAnalogPressSpeed) * 0.2f, CFUtils.realDeltaTime, 0.001f);
		}
	}
}
