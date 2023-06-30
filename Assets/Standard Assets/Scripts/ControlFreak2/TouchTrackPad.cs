using ControlFreak2.Internal;
using UnityEngine;

namespace ControlFreak2
{
	public class TouchTrackPad : TouchControl
	{
		public float touchSmoothing;

		public DigitalBinding pressBinding;

		public AxisBinding horzSwipeBinding;

		public AxisBinding vertSwipeBinding;

		public bool emulateTouchPressure;

		public AxisBinding touchPressureBinding;

		private TouchObject touchObj;

		private TouchStartType touchStartType;

		private TouchGestureBasicState touchState;

		public TouchTrackPad()
		{
			touchSmoothing = 0.5f;
			touchState = new TouchGestureBasicState();
			pressBinding = new DigitalBinding();
			horzSwipeBinding = new AxisBinding("Mouse X", enabled: false);
			vertSwipeBinding = new AxisBinding("Mouse Y", enabled: false);
			emulateTouchPressure = true;
			touchPressureBinding = new AxisBinding();
		}

		public bool Pressed()
		{
			return touchState.PressedRaw();
		}

		public bool JustPressed()
		{
			return touchState.JustPressedRaw();
		}

		public bool JustReleased()
		{
			return touchState.JustReleasedRaw();
		}

		public bool IsTouchPressureSensitive()
		{
			return touchState.PressedRaw() && touchState.IsPressureSensitive();
		}

		public float GetTouchPressure()
		{
			return (!touchState.PressedRaw()) ? 0f : touchState.GetPressure();
		}

		public Vector2 GetSwipeDelta()
		{
			return touchState.GetDeltaVecSmooth();
		}

		public void SetTouchSmoothing(float smTime)
		{
			touchSmoothing = Mathf.Clamp01(smTime);
			touchState.SetSmoothingTime(touchSmoothing * 0.1f);
		}

		protected override void OnInitControl()
		{
			ResetControl();
			SetTouchSmoothing(touchSmoothing);
		}

		protected override void OnDestroyControl()
		{
			ResetControl();
		}

		public override void ResetControl()
		{
			ReleaseAllTouches();
			touchState.Reset();
		}

		protected override void OnUpdateControl()
		{
			if (touchObj != null && base.rig != null)
			{
				base.rig.WakeTouchControlsUp();
			}
			touchState.Update();
			if (IsActive())
			{
				SyncRigState();
			}
		}

		private void SyncRigState()
		{
			if (Pressed())
			{
				pressBinding.Sync(state: true, base.rig);
				if (IsTouchPressureSensitive())
				{
					touchPressureBinding.SyncFloat(GetTouchPressure(), InputRig.InputSource.Analog, base.rig);
				}
				else if (emulateTouchPressure)
				{
					touchPressureBinding.SyncFloat(1f, InputRig.InputSource.Digital, base.rig);
				}
			}
			Vector2 swipeDelta = GetSwipeDelta();
			horzSwipeBinding.SyncFloat(swipeDelta.x, InputRig.InputSource.TouchDelta, base.rig);
			vertSwipeBinding.SyncFloat(swipeDelta.y, InputRig.InputSource.TouchDelta, base.rig);
		}

		protected override bool OnIsBoundToAxis(string axisName, InputRig rig)
		{
			return pressBinding.IsBoundToAxis(axisName, rig) || touchPressureBinding.IsBoundToAxis(axisName, rig) || horzSwipeBinding.IsBoundToAxis(axisName, rig) || vertSwipeBinding.IsBoundToAxis(axisName, rig);
		}

		protected override bool OnIsBoundToKey(KeyCode key, InputRig rig)
		{
			return pressBinding.IsBoundToKey(key, rig) || touchPressureBinding.IsBoundToKey(key, rig) || horzSwipeBinding.IsBoundToKey(key, rig) || vertSwipeBinding.IsBoundToKey(key, rig);
		}

		protected override void OnGetSubBindingDescriptions(BindingDescriptionList descList, Object undoObject, string parentMenuPath)
		{
			descList.Add(pressBinding, "Press", parentMenuPath, this);
			descList.Add(touchPressureBinding, InputRig.InputSource.Analog, "Touch Pressure", parentMenuPath, this);
			descList.Add(horzSwipeBinding, InputRig.InputSource.TouchDelta, "Horz. Swipe Delta", parentMenuPath, this);
			descList.Add(vertSwipeBinding, InputRig.InputSource.TouchDelta, "Vert. Swipe Delta", parentMenuPath, this);
		}

		public override bool CanBeTouchedDirectly(TouchObject touchObj)
		{
			return base.CanBeTouchedDirectly(touchObj) && this.touchObj == null;
		}

		public override bool CanBeActivatedByOtherControl(TouchControl c, TouchObject touchObj)
		{
			return base.CanBeActivatedByOtherControl(c, touchObj) && this.touchObj == null;
		}

		public override bool CanBeSwipedOverFromNothing(TouchObject touchObj)
		{
			return CanBeSwipedOverFromNothingDefault(touchObj) && this.touchObj == null;
		}

		public override bool CanBeSwipedOverFromRestrictedList(TouchObject touchObj)
		{
			return CanBeSwipedOverFromRestrictedListDefault(touchObj) && this.touchObj == null;
		}

		public override bool CanSwipeOverOthers(TouchObject touchObj)
		{
			return CanSwipeOverOthersDefault(touchObj, this.touchObj, touchStartType);
		}

		public override void ReleaseAllTouches()
		{
			if (touchObj != null)
			{
				touchObj.ReleaseControl(this, TouchEndType.Cancel);
				touchObj = null;
			}
		}

		public override bool OnTouchStart(TouchObject touchObj, TouchControl sender, TouchStartType touchStartType)
		{
			if (this.touchObj != null)
			{
				return false;
			}
			this.touchObj = touchObj;
			this.touchStartType = touchStartType;
			this.touchObj.AddControl(this);
			Vector2 vector = ScreenToOrientedPos(touchObj.screenPosStart, touchObj.cam);
			touchState.OnTouchStart(vector, vector, 0f, touchObj);
			return true;
		}

		public override bool OnTouchEnd(TouchObject touchObj, TouchEndType touchEndType)
		{
			if (this.touchObj == null || this.touchObj != touchObj)
			{
				return false;
			}
			this.touchObj = null;
			touchState.OnTouchEnd(touchEndType != TouchEndType.Release);
			return true;
		}

		public override bool OnTouchMove(TouchObject touchObj)
		{
			if (this.touchObj == null || this.touchObj != touchObj)
			{
				return false;
			}
			Vector2 pos = ScreenToOrientedPos(touchObj.screenPosCur, touchObj.cam);
			touchState.OnTouchMove(pos);
			CheckSwipeOff(touchObj, touchStartType);
			return true;
		}

		public override bool OnTouchPressureChange(TouchObject touchObj)
		{
			if (this.touchObj == null || this.touchObj != touchObj)
			{
				return false;
			}
			touchState.OnTouchPressureChange(touchObj.GetPressure());
			return true;
		}
	}
}
