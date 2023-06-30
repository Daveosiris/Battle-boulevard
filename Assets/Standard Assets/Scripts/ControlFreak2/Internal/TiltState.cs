using System;
using UnityEngine;

namespace ControlFreak2.Internal
{
	[Serializable]
	public class TiltState
	{
		public Vector2 analogRange;

		private Vector3 curVec;

		private Vector3 nextFrVec;

		private Vector2 anglesNeutral;

		private Vector2 anglesAbsCur;

		private Vector2 anglesAbsRaw;

		private Vector2 anglesCur;

		private Vector2 analogCur;

		private bool calibrated;

		private bool setRefAnglesFlag;

		public TiltState()
		{
			analogRange = new Vector2(25f, 25f);
			anglesNeutral = new Vector2(0f, 135f);
			calibrated = false;
			Reset();
		}

		public void Reset()
		{
			calibrated = false;
			analogCur = Vector2.zero;
			anglesCur = Vector2.zero;
		}

		public void InternalApplyVector(Vector3 v)
		{
			nextFrVec += v;
		}

		public static bool IsAvailable()
		{
			return Input.acceleration != Vector3.zero;
		}

		public void Calibate()
		{
			setRefAnglesFlag = true;
		}

		public bool IsCalibrated()
		{
			return calibrated;
		}

		public void Update()
		{
			curVec = nextFrVec;
			nextFrVec = Vector3.zero;
			float sqrMagnitude = curVec.sqrMagnitude;
			if (sqrMagnitude < 1E-06f)
			{
				curVec = Vector3.zero;
				anglesAbsRaw = anglesNeutral;
			}
			else
			{
				if (Mathf.Abs(sqrMagnitude - 1f) > 0.001f)
				{
					curVec.Normalize();
				}
				anglesAbsRaw.x = Mathf.Atan2(curVec.x, Mathf.Max(Mathf.Abs(curVec.y), Mathf.Abs(curVec.z))) * 57.29578f;
				anglesAbsRaw.y = Mathf.Atan2(0f - curVec.y, curVec.z) * 57.29578f;
			}
			anglesAbsCur = anglesAbsRaw;
			if (setRefAnglesFlag)
			{
				calibrated = true;
				setRefAnglesFlag = false;
				anglesNeutral = anglesAbsRaw;
				anglesNeutral.x = 0f;
			}
			anglesCur = anglesAbsCur - anglesNeutral;
			if (!calibrated)
			{
				anglesCur.y = 0f;
			}
			for (int i = 0; i < 2; i++)
			{
				float num = anglesCur[i];
				float num2 = Mathf.Abs(num);
				float num3 = analogRange[i];
				analogCur[i] = ((!(num2 >= num3)) ? (num2 / num3) : 1f) * (float)((!(num < 0f)) ? 1 : (-1));
			}
		}

		public Vector2 GetAngles()
		{
			return anglesCur;
		}

		public Vector2 GetAnalog()
		{
			return analogCur;
		}
	}
}
