using UnityEngine;

namespace ControlFreak2.Internal
{
	public class TouchGestureState : TouchGestureBasicState
	{
		private float elapsedSinceLastTapCandidate;

		private Vector2 segmentOrigin;

		private Vector2 constrainedVecCur;

		private Vector2 constrainedVecPrev;

		private Vector2 scrollVecCur;

		private Vector2 scrollVecPrev;

		private TouchGestureConfig.DirConstraint swipeConstraint;

		private TouchGestureConfig.DirConstraint swipeDirConstraint;

		private TouchGestureConfig.DirConstraint scrollConstraint;

		private bool cleanPressCur;

		private bool cleanPressPrev;

		private bool holdPressCur;

		private bool holdPressPrev;

		private Vector2 lastDirOrigin;

		public TouchGestureThresholds thresh;

		public TouchGestureConfig config;

		private bool moved;

		private bool justMoved;

		private bool nonStaticFlag;

		private bool cancelTapFlag;

		private bool disableTapUntilReleaseFlag;

		private bool flushTapsFlag;

		private bool holdDelayedEvents;

		private bool justTapped;

		private bool justLongTapped;

		private Vector2 tapPos;

		private int tapCount;

		private DirectionState swipeDirState;

		private DirectionState swipeDirState4;

		private DirectionState swipeDirState8;

		private bool blockDrag;

		private Vector2 tapFirstPos;

		private int tapCandidateCount;

		private int lastReportedTapCount;

		private bool tapCandidateReleased;

		public TouchGestureState(TouchGestureThresholds thresholds, TouchGestureConfig config)
		{
			BasicConstructor(thresholds, config);
		}

		public TouchGestureState()
		{
			BasicConstructor(null, null);
		}

		private void BasicConstructor(TouchGestureThresholds thresholds, TouchGestureConfig config)
		{
			this.config = config;
			thresh = thresholds;
			swipeDirState = new DirectionState();
			swipeDirState4 = new DirectionState();
			swipeDirState8 = new DirectionState();
			Reset();
		}

		public void SetThresholds(TouchGestureThresholds thresh)
		{
			this.thresh = thresh;
		}

		public void SetConfig(TouchGestureConfig config)
		{
			this.config = config;
		}

		public override void Reset()
		{
			base.Reset();
			swipeConstraint = TouchGestureConfig.DirConstraint.None;
			scrollConstraint = TouchGestureConfig.DirConstraint.None;
			swipeDirConstraint = TouchGestureConfig.DirConstraint.None;
			constrainedVecCur = Vector2.zero;
			constrainedVecPrev = Vector2.zero;
			tapCount = 0;
			tapCandidateCount = 0;
			elapsedSincePress = 0f;
			elapsedSinceRelease = 0f;
			tapCandidateReleased = false;
			moved = false;
			justMoved = false;
			justTapped = false;
			justLongTapped = false;
			cleanPressCur = false;
			cleanPressPrev = false;
			holdPressCur = false;
			holdPressPrev = false;
			flushTapsFlag = false;
			nonStaticFlag = false;
			cancelTapFlag = false;
			disableTapUntilReleaseFlag = false;
			swipeDirState.Reset();
			swipeDirState4.Reset();
			swipeDirState8.Reset();
		}

		public bool PressedNormal()
		{
			return cleanPressCur;
		}

		public bool JustPressedNormal()
		{
			return cleanPressCur && !cleanPressPrev;
		}

		public bool JustReleasedNormal()
		{
			return !cleanPressCur && cleanPressPrev;
		}

		public bool PressedLong()
		{
			return holdPressCur;
		}

		public bool JustPressedLong()
		{
			return holdPressCur && !holdPressPrev;
		}

		public bool JustReleasedLong()
		{
			return !holdPressCur && holdPressPrev;
		}

		public bool JustTapped(int howManyTimes)
		{
			return justTapped && tapCount == howManyTimes;
		}

		public Vector2 GetTapPos()
		{
			return tapPos;
		}

		public bool JustLongTapped()
		{
			return justLongTapped;
		}

		public DirectionState GetSwipeDirState()
		{
			return swipeDirState;
		}

		public DirectionState GetSwipeDirState4()
		{
			return swipeDirState4;
		}

		public DirectionState GetSwipeDirState8()
		{
			return swipeDirState8;
		}

		public Vector2 GetSegmentOrigin()
		{
			return segmentOrigin;
		}

		public bool JustSwiped()
		{
			return justMoved;
		}

		public bool Swiped()
		{
			return moved;
		}

		public Vector2 GetConstrainedSwipeVec()
		{
			return constrainedVecCur;
		}

		public Vector2 GetConstrainedDeltaVec()
		{
			return constrainedVecCur - constrainedVecPrev;
		}

		public Vector2 GetScroll()
		{
			return scrollVecCur;
		}

		public Vector2 GetScrollDelta()
		{
			return scrollVecCur - scrollVecPrev;
		}

		public float GetReleasedDurationRaw()
		{
			return relDur;
		}

		public void ForceSwipe()
		{
			if (!moved)
			{
				OnSwipeStart();
			}
		}

		public void BlockSwipe()
		{
			blockDrag = true;
			if (moved)
			{
				moved = false;
				justMoved = false;
			}
		}

		public void HoldDelayedEvents(bool holdThemForNow)
		{
			holdDelayedEvents = holdThemForNow;
		}

		public void CancelTap()
		{
			if (PressedRaw())
			{
				cancelTapFlag = true;
			}
			ResetTapState();
		}

		public void DisableTapUntilRelease()
		{
			disableTapUntilReleaseFlag = true;
			ResetTapState();
		}

		public void FlushRegisteredTaps()
		{
			flushTapsFlag = true;
		}

		public void MarkAsNonStatic()
		{
			nonStaticFlag = true;
		}

		public override void Update()
		{
			if (thresh != null)
			{
				thresh.Recalc(CFScreen.dpi);
			}
			justTapped = false;
			justMoved = false;
			justLongTapped = false;
			cleanPressPrev = cleanPressCur;
			holdPressPrev = holdPressCur;
			scrollVecPrev = scrollVecCur;
			constrainedVecPrev = constrainedVecCur;
			swipeDirState.BeginFrame();
			swipeDirState4.BeginFrame();
			swipeDirState8.BeginFrame();
			InternalUpdate();
			CheckTap(lastUpdate: false);
			InternalPostUpdate();
			if (tapCandidateReleased)
			{
				elapsedSinceLastTapCandidate += CFUtils.realDeltaTime;
			}
		}

		protected override void OnRelease(Vector2 pos, bool cancel)
		{
			if (cancel)
			{
				CancelTap();
			}
			base.OnRelease(pos, cancel);
		}

		protected override void OnPress(Vector2 startPos, Vector2 pos, float delay, bool startedByMouse, bool isPressureSensitive, float pressure)
		{
			base.OnPress(startPos, pos, delay, startedByMouse, isPressureSensitive, pressure);
			blockDrag = false;
			moved = false;
			justMoved = false;
			nonStaticFlag = false;
			constrainedVecCur = (constrainedVecPrev = Vector2.zero);
			scrollVecCur = (scrollVecPrev = Vector2.zero);
			segmentOrigin = posCurSmooth;
			scrollConstraint = TouchGestureConfig.DirConstraint.None;
			swipeConstraint = TouchGestureConfig.DirConstraint.None;
			swipeDirConstraint = TouchGestureConfig.DirConstraint.None;
			if (config != null)
			{
				scrollConstraint = config.scrollConstraint;
				swipeConstraint = config.swipeConstraint;
				swipeDirConstraint = config.swipeDirConstraint;
			}
			swipeDirState.Reset();
			swipeDirState4.Reset();
			swipeDirState8.Reset();
		}

		private void OnSwipeStart()
		{
			if (!moved)
			{
				justMoved = true;
				moved = true;
			}
		}

		protected override void CheckMovement(bool itsFinalUpdate)
		{
			base.CheckMovement(itsFinalUpdate);
			if (thresh == null || config == null)
			{
				return;
			}
			if (!PressedRaw())
			{
				cleanPressCur = false;
				holdPressCur = false;
			}
			else
			{
				bool flag = false;
				if (holdPressCur)
				{
					if (config.endLongPressWhenMoved && (nonStaticFlag || Moved(thresh.tapMoveThreshPxSq)))
					{
						holdPressCur = false;
					}
					else if (config.endLongPressWhenSwiped && Swiped())
					{
						holdPressCur = false;
					}
				}
				else if (config.detectLongPress && !nonStaticFlag && !Moved(thresh.tapMoveThreshPxSq))
				{
					if (elapsedSincePress > thresh.longPressMinTime)
					{
						holdPressCur = true;
					}
					else
					{
						flag = true;
					}
				}
				if (!cleanPressCur && !holdPressCur && !flag && !IsPotentialTap())
				{
					cleanPressCur = true;
				}
			}
			Vector2 swipeVecRaw = GetSwipeVecRaw();
			if (scrollConstraint != 0)
			{
				if (scrollConstraint == TouchGestureConfig.DirConstraint.Auto)
				{
					if (Mathf.Abs(swipeVecRaw.x) > thresh.scrollThreshPx)
					{
						scrollConstraint = TouchGestureConfig.DirConstraint.Horizontal;
					}
					if (Mathf.Abs(swipeVecRaw.y) > thresh.scrollThreshPx && Mathf.Abs(swipeVecRaw.y) > Mathf.Abs(swipeVecRaw.x))
					{
						scrollConstraint = TouchGestureConfig.DirConstraint.Vertical;
					}
				}
				if (scrollConstraint == TouchGestureConfig.DirConstraint.Horizontal)
				{
					swipeVecRaw.y = 0f;
				}
				else if (scrollConstraint == TouchGestureConfig.DirConstraint.Vertical)
				{
					swipeVecRaw.x = 0f;
				}
			}
			for (int i = 0; i < 2; i++)
			{
				scrollVecCur[i] = CFUtils.GetScrollValue(swipeVecRaw[i], (int)scrollVecCur[i], thresh.scrollThreshPx, thresh.scrollMagnetFactor);
			}
			constrainedVecCur = GetSwipeVecSmooth();
			if (swipeConstraint != 0)
			{
				if (swipeConstraint == TouchGestureConfig.DirConstraint.Auto)
				{
					if (Mathf.Abs(extremeDistPerAxisCur.x) > thresh.dragThreshPx)
					{
						swipeConstraint = TouchGestureConfig.DirConstraint.Horizontal;
					}
					if (Mathf.Abs(extremeDistPerAxisCur.y) > thresh.dragThreshPx && Mathf.Abs(extremeDistPerAxisCur.y) > Mathf.Abs(extremeDistPerAxisCur.x))
					{
						swipeConstraint = TouchGestureConfig.DirConstraint.Vertical;
					}
				}
				if (swipeConstraint == TouchGestureConfig.DirConstraint.Horizontal)
				{
					constrainedVecCur.y = 0f;
				}
				else if (swipeConstraint == TouchGestureConfig.DirConstraint.Vertical)
				{
					constrainedVecCur.x = 0f;
				}
				else
				{
					constrainedVecCur = Vector2.zero;
				}
			}
			if (!moved && !blockDrag && constrainedVecCur.sqrMagnitude > thresh.dragThreshPxSq)
			{
				OnSwipeStart();
			}
			Vector2 vec = posCurSmooth - segmentOrigin;
			if (swipeDirConstraint != 0)
			{
				if (swipeDirConstraint == TouchGestureConfig.DirConstraint.Auto)
				{
					if (Mathf.Abs(vec.x) > thresh.swipeSegLenPx)
					{
						swipeDirConstraint = TouchGestureConfig.DirConstraint.Horizontal;
					}
					if (Mathf.Abs(vec.y) > thresh.swipeSegLenPx && Mathf.Abs(vec.y) > Mathf.Abs(vec.x))
					{
						swipeDirConstraint = TouchGestureConfig.DirConstraint.Vertical;
					}
				}
				if (swipeDirConstraint == TouchGestureConfig.DirConstraint.Horizontal)
				{
					vec.y = 0f;
				}
				else if (swipeDirConstraint == TouchGestureConfig.DirConstraint.Vertical)
				{
					vec.x = 0f;
				}
				else
				{
					vec = Vector2.zero;
				}
			}
			float sqrMagnitude = vec.sqrMagnitude;
			if (sqrMagnitude > thresh.swipeSegLenPxSq)
			{
				vec.Normalize();
				swipeDirState4.SetDir(CFUtils.VecToDir(vec, swipeDirState4.GetCur(), 0.1f, as8way: false), config.swipeOriginalDirResetMode);
				swipeDirState8.SetDir(CFUtils.VecToDir(vec, swipeDirState8.GetCur(), 0.1f, as8way: true), config.swipeOriginalDirResetMode);
				swipeDirState.SetDir(((config.dirMode != TouchGestureConfig.DirMode.EightWay) ? swipeDirState4 : swipeDirState8).GetCur(), config.swipeOriginalDirResetMode);
				segmentOrigin = posCurSmooth;
			}
		}

		private void ResetTapState()
		{
			tapCandidateCount = 0;
			tapCandidateReleased = false;
			elapsedSinceLastTapCandidate = 0f;
			lastReportedTapCount = 0;
		}

		public bool IsPotentialTap()
		{
			if (thresh == null || config == null)
			{
				return false;
			}
			return PressedRaw() && !cancelTapFlag && !disableTapUntilReleaseFlag && config.maxTapCount > 0 && !nonStaticFlag && elapsedSincePress < thresh.tapMaxDur && extremeDistCurSq < thresh.tapMoveThreshPxSq;
		}

		public bool IsPotentialLongPress()
		{
			if (thresh == null || config == null)
			{
				return false;
			}
			return PressedRaw() && !nonStaticFlag && config.detectLongPress && elapsedSincePress < thresh.longPressMinTime && extremeDistCurSq < thresh.tapMoveThreshPxSq;
		}

		private void CheckTap(bool lastUpdate)
		{
			if (thresh == null || config == null || config.maxTapCount <= 0)
			{
				return;
			}
			if (flushTapsFlag)
			{
				ReportTap(reset: true);
				flushTapsFlag = false;
			}
			if (!PressedRaw())
			{
				if (tapCandidateReleased && !holdDelayedEvents && elapsedSinceLastTapCandidate > thresh.multiTapMaxTimeGap)
				{
					ReportTap(reset: true);
				}
			}
			else if (tapCandidateCount > 0 && ((!holdDelayedEvents && elapsedSincePress > thresh.tapMaxDur) || nonStaticFlag || extremeDistCurSq > thresh.tapMoveThreshPxSq))
			{
				if (config.cleanTapsOnly)
				{
					ResetTapState();
				}
				else
				{
					ReportTap(reset: true);
				}
			}
			if (JustPressedRaw())
			{
				if (tapCandidateCount > 0 && (posCurRaw - tapFirstPos).sqrMagnitude > thresh.tapPosThreshPxSq)
				{
					if (config.cleanTapsOnly)
					{
						ResetTapState();
					}
					else
					{
						ReportTap(reset: true);
					}
				}
				if (tapCandidateCount == 0)
				{
					tapFirstPos = posStart;
				}
			}
			if (!JustReleasedRaw())
			{
				return;
			}
			if (cancelTapFlag || disableTapUntilReleaseFlag)
			{
				ResetTapState();
			}
			else
			{
				if (config.detectLongTap && relDur > thresh.longPressMinTime && !nonStaticFlag && relDur <= thresh.longPressMinTime + thresh.longTapMaxDuration && relExtremeDistSq <= thresh.tapMoveThreshPxSq)
				{
					justLongTapped = true;
					tapPos = relStartPos;
				}
				if (relDur <= thresh.tapMaxDur && !nonStaticFlag && relExtremeDistSq <= thresh.tapMoveThreshPxSq)
				{
					tapCandidateCount++;
					if (tapCandidateCount >= config.maxTapCount)
					{
						ReportTap(reset: true);
					}
					else
					{
						if (!config.cleanTapsOnly)
						{
							ReportTap(reset: false);
						}
						tapCandidateReleased = true;
						elapsedSinceLastTapCandidate = 0f;
					}
				}
				else if (config.cleanTapsOnly)
				{
					ResetTapState();
				}
				else
				{
					ReportTap(reset: true);
				}
			}
			disableTapUntilReleaseFlag = false;
			cancelTapFlag = false;
		}

		private void ReportTap()
		{
			ReportTap(reset: true);
		}

		private void ReportTap(bool reset)
		{
			if (tapCandidateCount > 0 && tapCandidateCount > lastReportedTapCount)
			{
				lastReportedTapCount = tapCandidateCount;
				justTapped = true;
				tapPos = tapFirstPos;
				tapCount = tapCandidateCount;
			}
			if (reset)
			{
				ResetTapState();
			}
		}
	}
}
