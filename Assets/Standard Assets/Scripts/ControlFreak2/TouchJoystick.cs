using ControlFreak2.Internal;
using UnityEngine;

namespace ControlFreak2
{
	public class TouchJoystick : DynamicTouchControl
	{
		public JoystickConfig config;

		private JoystickState state;

		public DigitalBinding pressBinding;

		public JoystickStateBinding joyStateBinding;

		public bool emulateTouchPressure;

		public AxisBinding touchPressureBinding;

		public TouchJoystick()
		{
			joyStateBinding = new JoystickStateBinding();
			pressBinding = new DigitalBinding();
			emulateTouchPressure = true;
			touchPressureBinding = new AxisBinding();
			centerWhenFollowing = false;
			config = new JoystickConfig();
			state = new JoystickState(config);
		}

		protected override void OnInitControl()
		{
			base.OnInitControl();
			ResetControl();
		}

		public Vector2 GetVector()
		{
			return state.GetVector();
		}

		public Vector2 GetVectorRaw()
		{
			return state.GetVectorRaw();
		}

		public Dir GetDir()
		{
			return state.GetDir();
		}

		public JoystickState GetState()
		{
			return state;
		}

		public override void ResetControl()
		{
			base.ResetControl();
			ReleaseAllTouches();
			touchStateWorld.Reset();
			touchStateScreen.Reset();
			touchStateOriented.Reset();
			state.Reset();
		}

		protected override void OnUpdateControl()
		{
			base.OnUpdateControl();
			if (touchStateWorld.PressedRaw())
			{
				state.ApplyUnclampedVec(WorldToNormalizedPos(touchStateWorld.GetCurPosSmooth(), GetOriginOffset()));
			}
			state.Update();
			if (IsActive())
			{
				SyncRigState();
			}
		}

		protected override bool OnIsBoundToAxis(string axisName, InputRig rig)
		{
			return pressBinding.IsBoundToAxis(axisName, rig) || touchPressureBinding.IsBoundToAxis(axisName, rig) || joyStateBinding.IsBoundToAxis(axisName, rig);
		}

		protected override bool OnIsBoundToKey(KeyCode key, InputRig rig)
		{
			return pressBinding.IsBoundToKey(key, rig) || touchPressureBinding.IsBoundToKey(key, rig) || joyStateBinding.IsBoundToKey(key, rig);
		}

		protected override void OnGetSubBindingDescriptions(BindingDescriptionList descList, Object undoObject, string parentMenuPath)
		{
			descList.Add(pressBinding, "Press", parentMenuPath, this);
			descList.Add(touchPressureBinding, InputRig.InputSource.Analog, "Touch Pressure", parentMenuPath, this);
			descList.Add(joyStateBinding, "Joy State", parentMenuPath, this);
		}

		public override bool IsUsingKeyForEmulation(KeyCode key)
		{
			return false;
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
			joyStateBinding.SyncJoyState(state, base.rig);
		}
	}
}
