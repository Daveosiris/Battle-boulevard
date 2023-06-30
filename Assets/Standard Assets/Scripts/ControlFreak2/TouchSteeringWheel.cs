using ControlFreak2.Internal;
using UnityEngine;

namespace ControlFreak2
{
	public class TouchSteeringWheel : DynamicTouchControl
	{
		public enum WheelMode
		{
			Swipe,
			Turn
		}

		public WheelMode wheelMode;

		public bool limitTurnSpeed;

		public float minTurnTime = 0.05f;

		public float maxReturnTime = 0.25f;

		public float maxTurnAngle = 60f;

		public float turnModeDeadZone = 0.05f;

		public bool physicalMode;

		public float physicalMoveRangeCm = 2f;

		public bool sendInputWhileReturning;

		public AnalogConfig analogConfig;

		public DigitalBinding pressBinding;

		public AxisBinding analogTurnBinding;

		public DigitalBinding turnRightBinding;

		public DigitalBinding turnLeftBinding;

		public bool emulateTouchPressure;

		public AxisBinding touchPressureBinding;

		private Vector2 pressOrigin;

		private float rawValCur;

		private float valCur;

		private float valPrev;

		private Vector2 startVec;

		private float startRawVal;

		private float startAngle;

		private float curAngle;

		private float angleDelta;

		private bool angleIsSafe;

		public TouchSteeringWheel()
		{
			analogConfig = new AnalogConfig();
			analogConfig.analogDeadZone = 0f;
			touchSmoothing = 0.1f;
			centerOnDirectTouch = false;
			centerWhenFollowing = false;
			wheelMode = WheelMode.Swipe;
			pressBinding = new DigitalBinding();
			analogTurnBinding = new AxisBinding("Horizontal", enabled: false);
			turnLeftBinding = new DigitalBinding(KeyCode.None, bindToAxis: true, "Horizontal", axisNegSide: true, enabled: false);
			turnRightBinding = new DigitalBinding(KeyCode.None, bindToAxis: true, "Horizontal", axisNegSide: false, enabled: false);
			emulateTouchPressure = true;
			touchPressureBinding = new AxisBinding();
		}

		public float GetValue()
		{
			return valCur;
		}

		public float GetValueDelta()
		{
			return valCur - valPrev;
		}

		protected override void OnInitControl()
		{
			base.OnInitControl();
			ResetControl();
		}

		public override void ResetControl()
		{
			base.ResetControl();
			ReleaseAllTouches();
			touchStateWorld.Reset();
			touchStateScreen.Reset();
			touchStateOriented.Reset();
			valCur = 0f;
			valPrev = 0f;
			rawValCur = 0f;
			startRawVal = 0f;
			startVec = Vector2.zero;
			angleIsSafe = false;
			startAngle = 0f;
			curAngle = 0f;
			angleDelta = 0f;
		}

		protected override void OnUpdateControl()
		{
			base.OnUpdateControl();
			valPrev = valCur;
			if (touchStateWorld.JustPressedRaw())
			{
				startRawVal = rawValCur;
				if (wheelMode == WheelMode.Swipe)
				{
					if (physicalMode)
					{
						startVec = touchStateOriented.GetCurPosSmooth();
					}
					else
					{
						startVec = WorldToNormalizedPos(touchStateWorld.GetStartPos(), GetOriginOffset());
					}
				}
				else
				{
					startVec = WorldToNormalizedPos(touchStateWorld.GetStartPos(), GetOriginOffset());
					angleIsSafe = false;
					curAngle = 0f;
					startAngle = 0f;
					angleDelta = 0f;
				}
			}
			if (touchStateWorld.PressedRaw())
			{
				float num = 0f;
				if (wheelMode == WheelMode.Swipe)
				{
					if (physicalMode)
					{
						Vector2 curPosSmooth = touchStateOriented.GetCurPosSmooth();
						num = (curPosSmooth.x - startVec.x) / (physicalMoveRangeCm * CFScreen.dpcm * 0.5f);
					}
					else
					{
						Vector3 vector = WorldToNormalizedPos(touchStateWorld.GetCurPosSmooth(), GetOriginOffset());
						num = vector.x - startVec.x;
					}
				}
				else
				{
					Vector3 v = WorldToNormalizedPos(touchStateWorld.GetCurPosSmooth(), GetOriginOffset());
					if (v.sqrMagnitude < turnModeDeadZone * turnModeDeadZone)
					{
						angleIsSafe = false;
					}
					else
					{
						curAngle = GetWheelAngle(v, curAngle);
						if (!angleIsSafe)
						{
							startAngle = curAngle;
							startRawVal = rawValCur;
							angleIsSafe = true;
						}
					}
					angleDelta = CFUtils.SmartDeltaAngle(startAngle, curAngle, angleDelta);
					angleDelta = Mathf.Clamp(angleDelta, 0f - maxTurnAngle - 360f, maxTurnAngle + 360f);
					num = angleDelta / maxTurnAngle;
				}
				float b = startRawVal + num;
				rawValCur = CFUtils.MoveTowards(rawValCur, b, (!limitTurnSpeed) ? 0f : minTurnTime, CFUtils.realDeltaTime, 0.001f);
			}
			else
			{
				rawValCur = CFUtils.MoveTowards(rawValCur, 0f, (!limitTurnSpeed) ? 0f : maxReturnTime, CFUtils.realDeltaTime, 0.001f);
			}
			rawValCur = Mathf.Clamp(rawValCur, -1f, 1f);
			valCur = analogConfig.GetAnalogVal(rawValCur);
			if (IsActive())
			{
				SyncRigState();
			}
		}

		private void SyncRigState()
		{
			if (Pressed() || sendInputWhileReturning)
			{
				analogTurnBinding.SyncFloat(GetValue(), InputRig.InputSource.Analog, base.rig);
			}
			if (Pressed())
			{
				pressBinding.Sync(Pressed(), base.rig);
				if (GetValue() <= 0f - analogConfig.digitalEnterThresh)
				{
					turnLeftBinding.Sync(state: true, base.rig);
				}
				else if (GetValue() >= analogConfig.digitalEnterThresh)
				{
					turnRightBinding.Sync(state: true, base.rig);
				}
				if (IsTouchPressureSensitive())
				{
					touchPressureBinding.SyncFloat(GetTouchPressure(), InputRig.InputSource.Analog, base.rig);
				}
				else if (emulateTouchPressure)
				{
					touchPressureBinding.SyncFloat(1f, InputRig.InputSource.Digital, base.rig);
				}
			}
		}

		private float GetWheelAngle(Vector2 np, float fallbackAngle)
		{
			if (np.sqrMagnitude < turnModeDeadZone * turnModeDeadZone)
			{
				return fallbackAngle;
			}
			np.Normalize();
			return Mathf.Atan2(np.x, np.y) * 57.29578f;
		}

		protected override bool OnIsBoundToAxis(string axisName, InputRig rig)
		{
			return analogTurnBinding.IsBoundToAxis(axisName, rig) || pressBinding.IsBoundToAxis(axisName, rig) || touchPressureBinding.IsBoundToAxis(axisName, rig) || turnLeftBinding.IsBoundToAxis(axisName, rig) || turnRightBinding.IsBoundToAxis(axisName, rig);
		}

		protected override bool OnIsBoundToKey(KeyCode key, InputRig rig)
		{
			return analogTurnBinding.IsBoundToKey(key, rig) || pressBinding.IsBoundToKey(key, rig) || touchPressureBinding.IsBoundToKey(key, rig) || turnLeftBinding.IsBoundToKey(key, rig) || turnRightBinding.IsBoundToKey(key, rig);
		}

		protected override void OnGetSubBindingDescriptions(BindingDescriptionList descList, Object undoObject, string parentMenuPath)
		{
			descList.Add(pressBinding, "Press", parentMenuPath, this);
			descList.Add(touchPressureBinding, InputRig.InputSource.Analog, "Touch Pressure", parentMenuPath, this);
			descList.Add(analogTurnBinding, InputRig.InputSource.Analog, "Analog Turn", parentMenuPath, this);
			descList.Add(turnLeftBinding, "Turn Left", parentMenuPath, this);
			descList.Add(turnRightBinding, "Turn Right", parentMenuPath, this);
		}
	}
}
