using UnityEngine;

namespace ControlFreak2.Internal
{
	public class JoystickState
	{
		public JoystickConfig config;

		private float tilt;

		private float angle;

		private float safeAngle;

		private Vector2 pos;

		private Vector2 posRaw;

		private Vector2 normDir;

		private DirectionState dirState;

		private DirectionState dirState4;

		private DirectionState dirState8;

		private Dir dirLastNonNeutral4;

		private Dir dirLastNonNeutral8;

		private Vector2 nextFramePos;

		private bool nextFrameDigiU;

		private bool nextFrameDigiR;

		private bool nextFrameDigiD;

		private bool nextFrameDigiL;

		private Vector2 digiInputAnalogVecCur;

		private const float MIN_SAFE_TILT = 0.01f;

		private const float MIN_SAFE_TILT_SQ = 0.0001f;

		public JoystickState(JoystickConfig config)
		{
			this.config = config;
			dirState = new DirectionState();
			dirState4 = new DirectionState();
			dirState8 = new DirectionState();
			Reset();
		}

		public Vector2 GetVector()
		{
			return pos;
		}

		public Vector2 GetVectorRaw()
		{
			return posRaw;
		}

		public Vector2 GetVectorEx(bool squareMode)
		{
			if (config.clampMode == JoystickConfig.ClampMode.Square == squareMode)
			{
				return GetVector();
			}
			return (!squareMode) ? CFUtils.SquareToCircularJoystickVec(GetVector()) : CFUtils.CircularToSquareJoystickVec(GetVector());
		}

		public Vector2 GetVectorRawEx(bool squareMode)
		{
			if (config.clampMode == JoystickConfig.ClampMode.Square == squareMode)
			{
				return GetVectorRaw();
			}
			return (!squareMode) ? CFUtils.SquareToCircularJoystickVec(GetVectorRaw()) : CFUtils.CircularToSquareJoystickVec(GetVectorRaw());
		}

		public float GetAngle()
		{
			return angle;
		}

		public float GetTilt()
		{
			return tilt;
		}

		public DirectionState GetDirState()
		{
			return dirState;
		}

		public DirectionState GetDirState4()
		{
			return dirState4;
		}

		public DirectionState GetDirState8()
		{
			return dirState8;
		}

		public Dir GetDir()
		{
			return dirState.GetCur();
		}

		public Dir GetDir4()
		{
			return dirState4.GetCur();
		}

		public Dir GetDir8()
		{
			return dirState8.GetCur();
		}

		public bool JustPressedDir(Dir dir)
		{
			return dirState.JustPressed(dir);
		}

		public bool JustPressedDir4(Dir dir)
		{
			return dirState4.JustPressed(dir);
		}

		public bool JustPressedDir8(Dir dir)
		{
			return dirState8.JustPressed(dir);
		}

		public bool JustReleasedDir(Dir dir)
		{
			return dirState.JustReleased(dir);
		}

		public bool JustReleasedDir4(Dir dir)
		{
			return dirState4.JustReleased(dir);
		}

		public bool JustReleasedDir8(Dir dir)
		{
			return dirState8.JustReleased(dir);
		}

		public void SetConfig(JoystickConfig config)
		{
			this.config = config;
		}

		public void Reset()
		{
			normDir = Vector2.up;
			safeAngle = 0f;
			dirState.Reset();
			dirState4.Reset();
			dirState8.Reset();
			nextFrameDigiD = (nextFrameDigiL = (nextFrameDigiR = (nextFrameDigiU = false)));
			nextFramePos = Vector2.zero;
			digiInputAnalogVecCur = Vector2.zero;
			angle = 0f;
			dirLastNonNeutral4 = Dir.N;
			dirLastNonNeutral8 = Dir.N;
			normDir = new Vector2(0f, 1f);
			pos = (posRaw = Vector2.zero);
		}

		public void ApplyUnclampedVec(Vector2 v)
		{
			v = ((config.clampMode != 0) ? CFUtils.ClampInsideUnitSquare(v) : CFUtils.ClampInsideUnitCircle(v));
			nextFramePos.x = CFUtils.ApplyDeltaInput(nextFramePos.x, v.x);
			nextFramePos.y = CFUtils.ApplyDeltaInput(nextFramePos.y, v.y);
		}

		public void ApplyClampedVec(Vector2 v, JoystickConfig.ClampMode clampMode)
		{
			if (clampMode != config.clampMode)
			{
				v = ((config.clampMode != 0) ? CFUtils.CircularToSquareJoystickVec(v) : CFUtils.SquareToCircularJoystickVec(v));
			}
			nextFramePos.x = CFUtils.ApplyDeltaInput(nextFramePos.x, v.x);
			nextFramePos.y = CFUtils.ApplyDeltaInput(nextFramePos.y, v.y);
		}

		public void ApplyDir(Dir dir)
		{
			ApplyDigital(dir == Dir.U || dir == Dir.UL || dir == Dir.UR, dir == Dir.R || dir == Dir.UR || dir == Dir.DR, dir == Dir.D || dir == Dir.DL || dir == Dir.DR, dir == Dir.L || dir == Dir.UL || dir == Dir.DL);
		}

		public void ApplyDigital(bool digiU, bool digiR, bool digiD, bool digiL)
		{
			if (digiU)
			{
				nextFrameDigiU = true;
			}
			if (digiR)
			{
				nextFrameDigiR = true;
			}
			if (digiD)
			{
				nextFrameDigiD = true;
			}
			if (digiL)
			{
				nextFrameDigiL = true;
			}
		}

		public void ApplyState(JoystickState state)
		{
			if (config.stickMode == JoystickConfig.StickMode.Analog)
			{
				ApplyClampedVec(state.GetVectorRaw(), state.config.clampMode);
			}
			else if (config.stickMode == JoystickConfig.StickMode.Digital4)
			{
				ApplyDir(state.GetDir4());
			}
			else
			{
				ApplyDir(state.GetDir8());
			}
		}

		public void Update()
		{
			Dir targetDir = CFUtils.DigitalToDir(nextFrameDigiU, nextFrameDigiR, nextFrameDigiD, nextFrameDigiL);
			digiInputAnalogVecCur = config.AnimateDirToAnalogVec(digiInputAnalogVecCur, targetDir);
			posRaw.x = CFUtils.ApplyDeltaInput(nextFramePos.x, digiInputAnalogVecCur.x);
			posRaw.y = CFUtils.ApplyDeltaInput(nextFramePos.y, digiInputAnalogVecCur.y);
			nextFramePos = Vector2.zero;
			nextFrameDigiU = false;
			nextFrameDigiR = false;
			nextFrameDigiD = false;
			nextFrameDigiL = false;
			if (config.blockX)
			{
				posRaw.x = 0f;
			}
			if (config.blockY)
			{
				posRaw.y = 0f;
			}
			Vector2 vector = posRaw;
			posRaw = config.ClampNormalizedPos(posRaw);
			float num = posRaw.magnitude;
			float ang = safeAngle;
			if (posRaw.sqrMagnitude < 0.0001f)
			{
				num = 0f;
				tilt = 0f;
			}
			else
			{
				normDir = posRaw.normalized;
				tilt = num;
				ang = Mathf.Atan2(normDir.x, normDir.y) * 57.29578f;
				ang = CFUtils.NormalizeAnglePositive(ang);
			}
			Dir dir = GetDir4();
			Dir dir2 = GetDir8();
			float num2 = num;
			float num3 = num;
			if (config.digitalDetectionMode == JoystickConfig.DigitalDetectionMode.Touch)
			{
				num2 = Mathf.Max(Mathf.Abs(vector.x), Mathf.Abs(vector.y));
				num3 = num2;
				num3 = Mathf.Max(num3, Mathf.Abs(0.707106769f * vector.x + 0.707106769f * vector.y));
				num3 = Mathf.Max(num3, Mathf.Abs(0.707106769f * vector.x - 0.707106769f * vector.y));
			}
			if (num < 0.001f)
			{
				dirLastNonNeutral4 = (dirLastNonNeutral8 = Dir.N);
			}
			float num4 = (dirLastNonNeutral4 == Dir.N) ? (0.5f * (config.digitalEnterThresh + config.digitalLeaveThresh)) : config.digitalEnterThresh;
			dir = ((num2 > num4) ? CFUtils.DirFromAngleEx(ang, as8way: false, dirLastNonNeutral4, config.angularMagnet) : ((num2 > config.digitalLeaveThresh) ? dir : Dir.N));
			float num5 = (dirLastNonNeutral8 == Dir.N) ? (0.5f * (config.digitalEnterThresh + config.digitalLeaveThresh)) : config.digitalEnterThresh;
			dir2 = ((num3 > num5) ? CFUtils.DirFromAngleEx(ang, as8way: true, dirLastNonNeutral8, config.angularMagnet) : ((num3 > config.digitalLeaveThresh) ? dir2 : Dir.N));
			if (dir != 0)
			{
				dirLastNonNeutral4 = dir;
			}
			if (dir2 != 0)
			{
				dirLastNonNeutral8 = dir2;
			}
			dirState.BeginFrame();
			dirState4.BeginFrame();
			dirState8.BeginFrame();
			dirState4.SetDir(dir, config.originalDirResetMode);
			dirState8.SetDir(dir2, config.originalDirResetMode);
			dirState.SetDir((config.stickMode != JoystickConfig.StickMode.Digital4) ? GetDir8() : GetDir4(), config.originalDirResetMode);
			switch (config.stickMode)
			{
			case JoystickConfig.StickMode.Analog:
				angle = ang;
				if (config.perAxisDeadzones)
				{
					pos.x = config.GetAnalogVal(posRaw.x);
					pos.y = config.GetAnalogVal(posRaw.y);
					tilt = pos.magnitude;
					break;
				}
				tilt = config.GetAnalogVal(num);
				pos = normDir * tilt;
				if (config.clampMode == JoystickConfig.ClampMode.Square)
				{
					pos = CFUtils.CircularToSquareJoystickVec(pos);
				}
				break;
			case JoystickConfig.StickMode.Digital4:
			case JoystickConfig.StickMode.Digital8:
				pos = CFUtils.DirToVector(GetDir(), config.clampMode == JoystickConfig.ClampMode.Circle);
				angle = CFUtils.DirToAngle(GetDir());
				tilt = ((GetDir() != 0) ? 1 : 0);
				if (GetDir() != 0)
				{
					normDir = pos.normalized;
				}
				break;
			}
			safeAngle = angle;
		}
	}
}
