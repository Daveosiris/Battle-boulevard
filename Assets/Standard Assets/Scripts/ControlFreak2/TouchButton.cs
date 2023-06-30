using ControlFreak2.Internal;
using UnityEngine;

namespace ControlFreak2
{
	public class TouchButton : DynamicTouchControl, IBindingContainer
	{
		public enum ToggleOnAction
		{
			OnPress,
			OnRelease
		}

		public enum ToggleOffAction
		{
			OnPress,
			OnRelease,
			OnTimeout
		}

		public bool toggle;

		public ToggleOnAction toggleOnAction;

		public ToggleOffAction toggleOffAction;

		public bool toggleOffWhenHiding;

		public bool autoToggleOff;

		public float autoToggleOffTimeOut;

		public bool linkToggleToRigSwitch;

		public string toggleRigSwitchName;

		private int toggleRigSwitchId;

		public DigitalBinding pressBinding;

		public DigitalBinding toggleOnlyBinding;

		public bool emulateTouchPressure;

		public AxisBinding touchPressureBinding;

		private bool toggledCur;

		private bool toggledPrev;

		private bool curTouchToggledOn;

		private float elapsedSinceToggled;

		public TouchButton()
		{
			pressBinding = new DigitalBinding();
			toggleOnlyBinding = new DigitalBinding();
			emulateTouchPressure = true;
			touchPressureBinding = new AxisBinding();
			centerWhenFollowing = true;
			toggleOnAction = ToggleOnAction.OnPress;
			toggleOffAction = ToggleOffAction.OnRelease;
			autoToggleOff = false;
			autoToggleOffTimeOut = 1f;
		}

		public bool Toggled()
		{
			return toggledCur;
		}

		public bool JustToggled()
		{
			return toggledCur && !toggledPrev;
		}

		public bool JustUntoggled()
		{
			return !toggledCur && toggledPrev;
		}

		public bool PressedOrToggled()
		{
			return Pressed() || Toggled();
		}

		protected override void OnInitControl()
		{
			base.OnInitControl();
			if (toggle && linkToggleToRigSwitch && base.rig != null)
			{
				ChangeToggleState(base.rig.GetSwitchState(toggleRigSwitchName, ref toggleRigSwitchId, fallbackVal: false), syncRigSwitch: false);
			}
			ResetControl();
		}

		public override void ResetControl()
		{
			base.ResetControl();
			ReleaseAllTouches();
			touchStateWorld.Reset();
			touchStateScreen.Reset();
			touchStateOriented.Reset();
		}

		protected override void OnUpdateControl()
		{
			base.OnUpdateControl();
			toggledPrev = toggledCur;
			if (!toggle)
			{
				toggledCur = false;
			}
			else
			{
				bool flag = false;
				if (!Toggled())
				{
					if (toggleOnAction == ToggleOnAction.OnPress)
					{
						if (touchStateWorld.JustPressedRaw())
						{
							flag = true;
							curTouchToggledOn = true;
						}
					}
					else if (toggleOnAction == ToggleOnAction.OnRelease && !curTouchToggledOn && touchStateWorld.JustReleasedRaw())
					{
						flag = true;
						curTouchToggledOn = false;
					}
				}
				else if (toggleOffAction == ToggleOffAction.OnPress)
				{
					if (touchStateWorld.JustPressedRaw())
					{
						flag = true;
						curTouchToggledOn = true;
					}
				}
				else if (toggleOffAction == ToggleOffAction.OnRelease && !curTouchToggledOn && touchStateWorld.JustReleasedRaw())
				{
					flag = true;
					curTouchToggledOn = false;
				}
				if (!touchStateWorld.PressedRaw())
				{
					curTouchToggledOn = false;
				}
				if (flag)
				{
					ChangeToggleState(!toggledCur, syncRigSwitch: true);
				}
				else if (toggle && linkToggleToRigSwitch)
				{
					ChangeToggleState(base.rig.rigSwitches.GetSwitchState(toggleRigSwitchName, ref toggleRigSwitchId, toggledCur), syncRigSwitch: false);
				}
				if (Toggled() && (autoToggleOff || toggleOffAction == ToggleOffAction.OnTimeout))
				{
					if (Pressed())
					{
						elapsedSinceToggled = 0f;
					}
					else if ((elapsedSinceToggled += CFUtils.realDeltaTime) > autoToggleOffTimeOut)
					{
						ChangeToggleState(toggleState: false, syncRigSwitch: true);
					}
				}
			}
			if (IsActive())
			{
				SyncRigState();
			}
		}

		private void ChangeToggleState(bool toggleState, bool syncRigSwitch)
		{
			toggledCur = toggleState;
			elapsedSinceToggled = 0f;
			if (syncRigSwitch && linkToggleToRigSwitch && base.rig != null)
			{
				base.rig.rigSwitches.SetSwitchState(toggleRigSwitchName, ref toggleRigSwitchId, toggleState);
			}
		}

		private void SyncRigState()
		{
			if (PressedOrToggled())
			{
				pressBinding.Sync(state: true, base.rig);
			}
			toggleOnlyBinding.Sync(Toggled(), base.rig);
			if (Pressed())
			{
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

		protected override void OnGetSubBindingDescriptions(BindingDescriptionList descList, Object undoObject, string parentMenuPath)
		{
			descList.Add(pressBinding, "Press", parentMenuPath, this);
			descList.Add(touchPressureBinding, InputRig.InputSource.Analog, "Touch Pressure", parentMenuPath, this);
			if (toggle || descList.addUnusedBindings)
			{
				descList.Add(toggleOnlyBinding, "Toggle", parentMenuPath, this);
			}
		}

		protected override bool OnIsBoundToAxis(string axisName, InputRig rig)
		{
			return pressBinding.IsBoundToAxis(axisName, rig) || touchPressureBinding.IsBoundToAxis(axisName, rig) || toggleOnlyBinding.IsBoundToAxis(axisName, rig);
		}

		protected override bool OnIsBoundToKey(KeyCode key, InputRig rig)
		{
			return pressBinding.IsBoundToKey(key, rig) || touchPressureBinding.IsBoundToKey(key, rig) || toggleOnlyBinding.IsBoundToKey(key, rig);
		}

		public override void ReleaseAllTouches()
		{
			base.ReleaseAllTouches();
			ChangeToggleState(toggleState: false, toggleOffWhenHiding);
		}
	}
}
