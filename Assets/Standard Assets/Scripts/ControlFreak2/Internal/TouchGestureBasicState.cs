using UnityEngine;

namespace ControlFreak2.Internal
{
	public class TouchGestureBasicState
	{
		protected bool controlledByMouse;

		protected bool pollStartedByMouse;

		protected bool isPressureSensitive;

		protected bool pollStartPressureSensitive;

		protected float pressureCur;

		protected float pollStartPressure;

		protected bool pressCur;

		protected bool pressPrev;

		protected float releasedDur;

		protected float elapsedSincePress;

		protected float elapsedSinceRelease;

		protected float curDist;

		protected float releasedDist;

		protected Vector2 posCurRaw;

		protected Vector2 posPrevRaw;

		protected Vector2 posCurSmooth;

		protected Vector2 posPrevSmooth;

		protected Vector2 posNext;

		protected Vector2 posStart;

		protected float elapsedSinceLastMove;

		protected Vector2 relStartPos;

		protected Vector2 relEndPos;

		protected Vector2 relExtremeDistPerAxis;

		protected float relDur;

		protected float relExtremeDistSq;

		protected float relDistSq;

		protected float extremeDistCurSq;

		protected float extremeDistPrevSq;

		protected Vector2 extremeDistPerAxisCur;

		protected Vector2 extremeDistPerAxisPrev;

		protected float smoothingTime;

		protected bool prepollState;

		protected bool pollStartedPress;

		protected bool pollReleased;

		protected bool pollReleasedAsCancel;

		protected float pollCurPressure;

		protected Vector2 pollCurPos;

		protected Vector2 pollPressStartPos;

		protected Vector2 pollReleasePos;

		protected float pollPressStartDelay;

		public virtual void Reset()
		{
			pressCur = false;
			pressPrev = false;
			isPressureSensitive = false;
			pressureCur = 1f;
			prepollState = false;
			pollReleased = false;
			pollReleasedAsCancel = false;
			pollStartedPress = false;
			pollStartPressureSensitive = false;
			pollStartPressure = 1f;
			pollCurPressure = 1f;
			pollCurPos = Vector2.zero;
			pollPressStartPos = Vector2.zero;
			pollReleasePos = Vector2.zero;
			pollPressStartDelay = 0f;
			curDist = 0f;
			extremeDistCurSq = 0f;
			extremeDistPrevSq = 0f;
			releasedDist = 0f;
			extremeDistPerAxisCur = Vector2.zero;
			extremeDistPerAxisPrev = Vector2.zero;
			posCurRaw = Vector2.zero;
			posCurSmooth = Vector2.zero;
			posPrevRaw = Vector2.zero;
			posPrevSmooth = Vector2.zero;
			relEndPos = Vector2.zero;
			relDur = 0f;
			releasedDist = 0f;
			relStartPos = Vector2.zero;
		}

		public void SetSmoothingTime(float smt)
		{
			smoothingTime = smt;
		}

		public bool PressedRaw()
		{
			return pressCur;
		}

		public bool JustPressedRaw()
		{
			return pressCur && !pressPrev;
		}

		public bool JustReleasedRaw()
		{
			return !pressCur && pressPrev;
		}

		public Vector2 GetCurPosRaw()
		{
			return posCurRaw;
		}

		public Vector2 GetCurPosSmooth()
		{
			return posCurSmooth;
		}

		public Vector2 GetStartPos()
		{
			return posStart;
		}

		public Vector2 GetReleasedStartPos()
		{
			return relStartPos;
		}

		public Vector2 GetReleasedEndPos()
		{
			return relEndPos;
		}

		public Vector2 GetSwipeVecRaw()
		{
			return posCurRaw - posStart;
		}

		public Vector2 GetSwipeVecSmooth()
		{
			return posCurSmooth - posStart;
		}

		public Vector2 GetDeltaVecRaw()
		{
			return posCurRaw - posPrevRaw;
		}

		public Vector2 GetDeltaVecSmooth()
		{
			return posCurSmooth - posPrevSmooth;
		}

		public bool Moved(float threshSquared)
		{
			return extremeDistCurSq > threshSquared;
		}

		public bool JustMoved(float threshSquared)
		{
			return extremeDistCurSq > threshSquared && extremeDistPrevSq <= threshSquared;
		}

		public bool IsControlledByMouse()
		{
			return controlledByMouse;
		}

		public bool IsPressureSensitive()
		{
			return PressedRaw() && isPressureSensitive;
		}

		public float GetPressure()
		{
			return (!PressedRaw()) ? 0f : pressureCur;
		}

		public virtual void Update()
		{
			InternalUpdate();
			InternalPostUpdate();
		}

		protected void InternalUpdate()
		{
			posPrevRaw = posCurRaw;
			posPrevSmooth = posCurSmooth;
			pressPrev = pressCur;
			extremeDistPrevSq = extremeDistCurSq;
			extremeDistPerAxisPrev = extremeDistPerAxisCur;
			if (pollReleased && prepollState)
			{
				posCurRaw = pollReleasePos;
				posCurSmooth = posCurRaw;
				OnRelease(pollReleasePos, pollReleasedAsCancel);
				pollReleased = false;
				prepollState = pressCur;
				if (pollStartedPress)
				{
					pollPressStartDelay += CFUtils.realDeltaTime;
				}
			}
			else if (pollStartedPress && !prepollState)
			{
				OnPress(pollPressStartPos, pollCurPos, pollPressStartDelay, pollStartedByMouse, pollStartPressureSensitive, pollStartPressure);
				pollStartedPress = false;
				prepollState = pressCur;
			}
			else
			{
				posCurRaw = pollCurPos;
				posCurSmooth = CFUtils.SmoothTowardsVec2(posCurSmooth, posCurRaw, smoothingTime, CFUtils.realDeltaTime, 0f);
				pressureCur = pollCurPressure;
			}
			if (pressCur)
			{
				CheckMovement(itsFinalUpdate: false);
			}
		}

		protected void InternalPostUpdate()
		{
			elapsedSincePress += CFUtils.realDeltaTime;
			elapsedSinceRelease += CFUtils.realDeltaTime;
			elapsedSinceLastMove += CFUtils.realDeltaTime;
		}

		public void OnTouchStart(Vector2 startPos, Vector2 curPos, float timeElapsed, TouchObject touchObj)
		{
			OnTouchStart(startPos, curPos, timeElapsed, touchObj.IsMouse(), touchObj.IsPressureSensitive(), touchObj.GetPressure());
		}

		public void OnTouchStart(Vector2 startPos, Vector2 curPos, float timeElapsed, bool controlledByMouse, bool isPressureSensitive, float pressure)
		{
			if (!prepollState)
			{
				pollReleased = false;
			}
			else if (!pollReleased)
			{
				return;
			}
			pollStartedPress = true;
			pollPressStartPos = startPos;
			pollPressStartDelay = timeElapsed;
			pollStartedByMouse = controlledByMouse;
			pollStartPressureSensitive = isPressureSensitive;
			pollStartPressure = pressure;
			pollCurPos = curPos;
		}

		public void OnTouchPressureChange(float pressure)
		{
			isPressureSensitive = true;
			pollCurPressure = pressure;
		}

		public void OnTouchMove(Vector2 pos)
		{
			pollCurPos = pos;
		}

		public void OnTouchMoveByDelta(Vector2 delta)
		{
			pollCurPos += delta;
		}

		public void OnTouchEnd(Vector2 pos, bool cancel)
		{
			if (prepollState)
			{
				if (pollReleased)
				{
					return;
				}
			}
			else if (!pollStartedPress)
			{
				return;
			}
			pollReleased = true;
			pollReleasedAsCancel = cancel;
			pollReleasePos = pos;
		}

		public void OnTouchEnd(bool cancel)
		{
			OnTouchEnd(pollCurPos, cancel);
		}

		protected virtual void OnPress(Vector2 startPos, Vector2 pos, float delay, bool startedByMouse, bool isPressureSensitive = false, float pressure = 1f)
		{
			elapsedSincePress = delay;
			posStart = startPos;
			posPrevRaw = startPos;
			posPrevSmooth = posPrevRaw;
			posCurRaw = pos;
			posCurSmooth = pos;
			pressCur = true;
			controlledByMouse = startedByMouse;
			this.isPressureSensitive = isPressureSensitive;
			pressureCur = pressure;
			extremeDistCurSq = 0f;
			extremeDistPrevSq = 0f;
			extremeDistPerAxisCur = (extremeDistPerAxisPrev = Vector2.zero);
			elapsedSinceLastMove = 0f;
		}

		protected virtual void OnRelease(Vector2 pos, bool cancel)
		{
			elapsedSinceRelease = 0f;
			posCurRaw = pos;
			posCurSmooth = pos;
			pressCur = false;
			CheckMovement(itsFinalUpdate: true);
			relStartPos = posStart;
			relEndPos = pos;
			relDur = elapsedSincePress;
			relDistSq = (relEndPos - relStartPos).sqrMagnitude;
			relExtremeDistSq = extremeDistCurSq;
			relExtremeDistPerAxis = extremeDistPerAxisCur;
		}

		protected virtual void CheckMovement(bool itsFinalUpdate)
		{
			Vector2 vector = posCurRaw - posStart;
			extremeDistCurSq = Mathf.Max(extremeDistCurSq, vector.sqrMagnitude);
			extremeDistPerAxisCur = Vector2.Max(extremeDistPerAxisCur, new Vector2(Mathf.Abs(vector.x), Mathf.Abs(vector.y)));
		}
	}
}
