using ControlFreak2.Internal;
using UnityEngine;

namespace ControlFreak2
{
	public abstract class DynamicTouchControl : TouchControl
	{
		public bool fadeOutWhenReleased = true;

		public bool startFadedOut;

		public float fadeOutTargetAlpha;

		public float fadeInDuration = 0.2f;

		public float fadeOutDelay = 0.5f;

		public float fadeOutDuration = 0.5f;

		public bool centerOnDirectTouch = true;

		public bool centerOnIndirectTouch = true;

		public bool centerWhenFollowing;

		public bool stickyMode;

		public bool clampInsideRegion = true;

		public bool clampInsideCanvas = true;

		public bool returnToStartingPosition;

		public Vector2 directInitialVector;

		public Vector2 indirectInitialVector;

		public float touchSmoothing;

		[Tooltip("Stick's origin smoothing - the higher the value, the slower the movement.")]
		[Range(0f, 1f)]
		public float originSmoothTime = 0.5f;

		private const float ORIGIN_ANIM_MAX_TIME = 0.2f;

		public DynamicRegion targetDynamicRegion;

		protected bool linkedToDynamicRegion;

		protected bool touchStartedByRegion;

		protected TouchStartType touchStartType;

		protected TouchGestureBasicState touchStateOriented;

		protected TouchGestureBasicState touchStateScreen;

		protected TouchGestureBasicState touchStateWorld;

		protected TouchObject touchObj;

		private RectTransform initialRectCopy;

		private Vector3 originPos;

		private Vector3 originStartPos;

		private bool originAnimOn;

		private float originAnimElapsed;

		private bool dynamicIsFadingOut;

		private bool dynamicAlphaAnimOn;

		private bool dynamicWaitingToFadeOut;

		private float dynamicAlphaAnimDur;

		private float dynamicAlphaAnimElapsed;

		private float dynamicFadeOutDelayElapsed;

		private float dynamicAlphaCur;

		private float dynamicAlphaStart;

		private float dynamicAlphaTarget;

		private GameObject initialRectCopyGo;

		private Vector2 initialAnchorMax;

		private Vector2 initialAnchorMin;

		private Vector2 initialOffsetMin;

		private Vector2 initialOffsetMax;

		private Vector2 initialPivot;

		private Vector3 initialAnchoredPosition3D;

		public DynamicTouchControl()
		{
			touchStateScreen = new TouchGestureBasicState();
			touchStateWorld = new TouchGestureBasicState();
			touchStateOriented = new TouchGestureBasicState();
			directInitialVector = Vector2.zero;
			indirectInitialVector = Vector2.zero;
			startFadedOut = true;
			fadeOutWhenReleased = true;
			touchSmoothing = 0.1f;
		}

		protected override void OnInitControl()
		{
			SetTargetDynamicRegion(targetDynamicRegion);
			SetTouchSmoothing(touchSmoothing);
			StoreDefaultPos();
		}

		public override void ResetControl()
		{
			if (CanFadeOut() && startFadedOut && !CFUtils.editorStopped)
			{
				DynamicFadeOut(animate: false);
			}
			else
			{
				DynamicWakeUp(animate: false);
			}
		}

		public override void InvalidateHierarchy()
		{
			base.InvalidateHierarchy();
			StoreDefaultPos();
		}

		public bool Pressed()
		{
			return touchStateWorld.PressedRaw();
		}

		public bool JustPressed()
		{
			return touchStateWorld.JustPressedRaw();
		}

		public bool JustReleased()
		{
			return touchStateWorld.JustReleasedRaw();
		}

		public bool IsTouchPressureSensitive()
		{
			return touchStateWorld.PressedRaw() && touchStateWorld.IsPressureSensitive();
		}

		public float GetTouchPressure()
		{
			return (!touchStateWorld.PressedRaw()) ? 0f : touchStateWorld.GetPressure();
		}

		public void SetTouchSmoothing(float smTime)
		{
			touchSmoothing = Mathf.Clamp01(smTime);
			touchStateWorld.SetSmoothingTime(touchSmoothing * 0.1f);
			touchStateOriented.SetSmoothingTime(touchSmoothing * 0.1f);
			touchStateScreen.SetSmoothingTime(touchSmoothing * 0.1f);
		}

		public void SetTargetDynamicRegion(DynamicRegion targetDynamicRegion)
		{
			this.targetDynamicRegion = targetDynamicRegion;
			if (targetDynamicRegion != null && targetDynamicRegion.CanBeUsed())
			{
				targetDynamicRegion.SetTargetControl(this);
			}
		}

		public void OnLinkToDynamicRegion(DynamicRegion dynRegion)
		{
			linkedToDynamicRegion = (dynRegion != null && dynRegion == targetDynamicRegion);
		}

		public DynamicRegion GetDynamicRegion()
		{
			return (!linkedToDynamicRegion) ? null : targetDynamicRegion;
		}

		public bool IsInDynamicMode()
		{
			return GetDynamicRegion() != null;
		}

		public float GetDynamicAlpha()
		{
			return dynamicAlphaCur;
		}

		public override float GetAlpha()
		{
			return GetDynamicAlpha() * base.GetAlpha();
		}

		private void SetDynamicAlpha(float alpha, float animDur)
		{
			if (animDur > 0.001f)
			{
				dynamicAlphaAnimDur = animDur;
				dynamicAlphaStart = dynamicAlphaCur;
				dynamicAlphaTarget = alpha;
				dynamicAlphaAnimElapsed = 0f;
				dynamicAlphaAnimOn = true;
			}
			else
			{
				dynamicAlphaAnimOn = false;
				dynamicAlphaAnimElapsed = 0f;
				dynamicAlphaCur = (dynamicAlphaTarget = (dynamicAlphaStart = alpha));
			}
		}

		private void DynamicWakeUp(bool animate)
		{
			dynamicIsFadingOut = false;
			dynamicWaitingToFadeOut = false;
			SetDynamicAlpha(1f, (!animate) ? 0f : fadeInDuration);
		}

		private void DynamicFadeOut(bool animate)
		{
			if (!animate)
			{
				dynamicIsFadingOut = true;
				dynamicWaitingToFadeOut = false;
				dynamicFadeOutDelayElapsed = 0f;
				SetDynamicAlpha(fadeOutTargetAlpha, 0f);
			}
			else if (!dynamicIsFadingOut)
			{
				dynamicIsFadingOut = true;
				dynamicWaitingToFadeOut = true;
				dynamicFadeOutDelayElapsed = 0f;
			}
		}

		private bool CanFadeOut()
		{
			return fadeOutWhenReleased && IsInDynamicMode();
		}

		private void UpdateDynamicAlpha()
		{
			if (dynamicAlphaAnimOn)
			{
				dynamicAlphaAnimElapsed += CFUtils.realDeltaTime;
				if (dynamicAlphaAnimElapsed > dynamicAlphaAnimDur)
				{
					dynamicAlphaAnimOn = false;
					dynamicAlphaCur = dynamicAlphaTarget;
				}
				else
				{
					dynamicAlphaCur = Mathf.Lerp(dynamicAlphaStart, dynamicAlphaTarget, dynamicAlphaAnimElapsed / dynamicAlphaAnimDur);
				}
			}
			if (dynamicIsFadingOut && dynamicWaitingToFadeOut)
			{
				dynamicFadeOutDelayElapsed += CFUtils.realDeltaTime;
				if (dynamicFadeOutDelayElapsed >= fadeOutDelay)
				{
					dynamicWaitingToFadeOut = false;
					SetDynamicAlpha(fadeOutTargetAlpha, fadeOutDuration);
				}
			}
		}

		public override void SetWorldPos(Vector2 pos2D)
		{
			SetOriginPos(pos2D, animate: false);
			StoreDefaultPos();
		}

		protected void SetOriginPos(Vector3 pos, bool animate)
		{
			originPos = pos;
			if (animate)
			{
				originStartPos = GetWorldPos();
				originAnimOn = true;
				originAnimElapsed = 0f;
			}
			else
			{
				SetWorldPosRaw(originPos);
				originStartPos = originPos;
				originAnimOn = false;
			}
		}

		protected void SetOriginPos(Vector3 pos)
		{
			SetOriginPos(pos, animate: true);
		}

		protected Vector2 GetOriginOffset()
		{
			return base.transform.position - originPos;
		}

		protected void UpdateOriginAnimation()
		{
			if (originAnimOn)
			{
				originAnimElapsed += CFUtils.realDeltaTime;
				if (originAnimElapsed >= originSmoothTime * 0.2f)
				{
					originAnimOn = false;
					SetWorldPosRaw(originPos);
				}
				else
				{
					SetWorldPosRaw(Vector3.Lerp(originStartPos, originPos, originAnimElapsed / (originSmoothTime * 0.2f)));
				}
			}
		}

		protected void StoreDefaultPos()
		{
			if (!CFUtils.editorStopped)
			{
				if (initialRectCopyGo == null)
				{
					initialRectCopyGo = new GameObject(base.name + "_INITIAL_POS", typeof(InitialPosPlaceholder));
				}
				RectTransform component = GetComponent<RectTransform>();
				initialRectCopyGo.transform.SetParent(component.parent, worldPositionStays: false);
				initialRectCopy = initialRectCopyGo.GetComponent<RectTransform>();
				initialRectCopyGo.hideFlags = HideFlags.DontSave;
				initialAnchorMin = component.anchorMin;
				initialAnchorMax = component.anchorMax;
				initialOffsetMin = component.offsetMin;
				initialOffsetMax = component.offsetMax;
				initialAnchoredPosition3D = component.anchoredPosition3D;
				initialPivot = component.pivot;
				SetupInitialRectPosition();
			}
		}

		private void SetupInitialRectPosition()
		{
			if (!(initialRectCopy == null))
			{
				initialRectCopy.anchoredPosition3D = initialAnchoredPosition3D;
				initialRectCopy.anchorMin = initialAnchorMin;
				initialRectCopy.anchorMax = initialAnchorMax;
				initialRectCopy.offsetMin = initialOffsetMin;
				initialRectCopy.offsetMax = initialOffsetMax;
				initialRectCopy.pivot = initialPivot;
			}
		}

		protected Vector3 GetDefaultPos()
		{
			return (!(initialRectCopy == null)) ? initialRectCopy.position : base.transform.position;
		}

		protected override void OnDestroyControl()
		{
			ResetControl();
			if (initialRectCopyGo != null)
			{
				UnityEngine.Object.Destroy(initialRectCopyGo);
			}
		}

		protected override void OnUpdateControl()
		{
			if (touchObj != null && base.rig != null)
			{
				base.rig.WakeTouchControlsUp();
			}
			UpdateDynamicAlpha();
			touchStateWorld.Update();
			touchStateScreen.Update();
			touchStateOriented.Update();
			if (touchStateScreen.JustPressedRaw())
			{
				DynamicWakeUp(animate: true);
				if (!IsInDynamicMode())
				{
					SetOriginPos(GetWorldPos(), animate: false);
				}
				else
				{
					bool flag = GetDynamicAlpha() < 0.001f;
					if (!centerOnDirectTouch && !touchStartedByRegion)
					{
						SetOriginPos(GetWorldPos(), animate: false);
					}
					else
					{
						Vector2 vector = touchStateWorld.GetStartPos();
						if (touchStartedByRegion)
						{
							if (indirectInitialVector != Vector2.zero)
							{
								vector -= NormalizedToWorldOffset(indirectInitialVector);
							}
						}
						else if (directInitialVector != Vector2.zero)
						{
							vector -= NormalizedToWorldOffset(directInitialVector);
						}
						if (!flag && !centerOnIndirectTouch)
						{
							vector = GetFollowPos(vector, Vector2.zero);
						}
						if (clampInsideRegion && GetDynamicRegion() != null)
						{
							vector = ClampInsideOther(vector, GetDynamicRegion());
						}
						if (clampInsideCanvas && base.canvas != null)
						{
							vector = ClampInsideCanvas(vector, base.canvas);
						}
						SetOriginPos(vector, !flag);
					}
				}
			}
			if (touchStateWorld.JustReleasedRaw() && (!IsInDynamicMode() || returnToStartingPosition))
			{
				SetOriginPos(GetDefaultPos(), animate: true);
			}
			if (IsInDynamicMode() && fadeOutWhenReleased && !touchStateWorld.PressedRaw())
			{
				DynamicFadeOut(animate: true);
			}
			touchStartedByRegion = false;
			if (touchStateWorld.PressedRaw() && stickyMode && (swipeOffMode == SwipeOffMode.Disabled || (swipeOffMode == SwipeOffMode.OnlyIfSwipedOver && touchStartType != TouchStartType.SwipeOver)))
			{
				bool posWasOutside = true;
				Vector3 targetWorldPos = touchStateWorld.GetCurPosSmooth();
				if (!centerWhenFollowing)
				{
					posWasOutside = false;
					targetWorldPos = GetFollowPos(targetWorldPos, GetOriginOffset(), out posWasOutside);
				}
				if (posWasOutside)
				{
					if (clampInsideRegion && GetDynamicRegion() != null)
					{
						targetWorldPos = ClampInsideOther(targetWorldPos, GetDynamicRegion());
					}
					if (clampInsideCanvas && base.canvas != null)
					{
						targetWorldPos = ClampInsideCanvas(targetWorldPos, base.canvas);
					}
					SetOriginPos(targetWorldPos);
				}
			}
			UpdateOriginAnimation();
		}

		public override bool OnTouchStart(TouchObject touch, TouchControl sender, TouchStartType touchStartType)
		{
			if (touchObj != null)
			{
				return false;
			}
			touchObj = touch;
			touchObj.AddControl(this);
			Vector3 v = (touchStartType != 0) ? touch.screenPosCur : touch.screenPosStart;
			Vector3 v2 = touch.screenPosCur;
			Vector3 v3 = ScreenToOrientedPos(v, touch.cam);
			Vector3 v4 = ScreenToOrientedPos(v2, touch.cam);
			Vector3 v5 = ScreenToWorldPos(v, touch.cam);
			Vector3 v6 = ScreenToWorldPos(v2, touch.cam);
			touchStateWorld.OnTouchStart(v5, v6, 0f, touchObj);
			touchStateScreen.OnTouchStart(v, v2, 0f, touchObj);
			touchStateOriented.OnTouchStart(v3, v4, 0f, touchObj);
			touchStartedByRegion = (sender != null && sender != this);
			this.touchStartType = touchStartType;
			return true;
		}

		public override bool OnTouchEnd(TouchObject touch, TouchEndType touchEndType)
		{
			if (touch != touchObj || touchObj == null)
			{
				return false;
			}
			touchObj = null;
			touchStateWorld.OnTouchEnd(touchEndType != TouchEndType.Release);
			touchStateScreen.OnTouchEnd(touchEndType != TouchEndType.Release);
			touchStateOriented.OnTouchEnd(touchEndType != TouchEndType.Release);
			return true;
		}

		public override bool OnTouchMove(TouchObject touch)
		{
			if (touch != touchObj || touchObj == null)
			{
				return false;
			}
			Vector3 v = touch.screenPosCur;
			Vector3 v2 = ScreenToWorldPos(touch.screenPosCur, touch.cam);
			Vector3 v3 = ScreenToOrientedPos(touch.screenPosCur, touch.cam);
			touchStateWorld.OnTouchMove(v2);
			touchStateScreen.OnTouchMove(v);
			touchStateOriented.OnTouchMove(v3);
			CheckSwipeOff(touch, touchStartType);
			return true;
		}

		public override bool OnTouchPressureChange(TouchObject touch)
		{
			if (touch != touchObj || touchObj == null)
			{
				return false;
			}
			touchStateWorld.OnTouchPressureChange(touch.GetPressure());
			touchStateScreen.OnTouchPressureChange(touch.GetPressure());
			touchStateOriented.OnTouchPressureChange(touch.GetPressure());
			return true;
		}

		public override void ReleaseAllTouches()
		{
			if (touchObj != null)
			{
				touchObj.ReleaseControl(this, TouchEndType.Cancel);
				touchObj = null;
			}
			touchStateWorld.OnTouchEnd(cancel: true);
			touchStateOriented.OnTouchEnd(cancel: true);
			touchStateScreen.OnTouchEnd(cancel: true);
		}

		public override bool CanBeTouchedDirectly(TouchObject touchObj)
		{
			return base.CanBeTouchedDirectly(touchObj) && this.touchObj == null;
		}

		public override bool CanBeSwipedOverFromNothing(TouchObject touchObj)
		{
			return CanBeSwipedOverFromNothingDefault(touchObj) && this.touchObj == null && IsActiveAndVisible();
		}

		public override bool CanBeSwipedOverFromRestrictedList(TouchObject touchObj)
		{
			return CanBeSwipedOverFromRestrictedListDefault(touchObj) && this.touchObj == null && IsActiveAndVisible();
		}

		public override bool CanSwipeOverOthers(TouchObject touchObj)
		{
			return CanSwipeOverOthersDefault(touchObj, this.touchObj, touchStartType);
		}

		public virtual bool CanBeActivatedByDynamicRegion()
		{
			return touchObj == null && IsActive();
		}

		public override bool CanBeActivatedByOtherControl(TouchControl c, TouchObject touchObj)
		{
			return base.CanBeActivatedByOtherControl(c, touchObj) && this.touchObj == null;
		}
	}
}
