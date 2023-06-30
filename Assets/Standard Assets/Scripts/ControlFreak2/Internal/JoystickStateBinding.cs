using System;
using UnityEngine;

namespace ControlFreak2.Internal
{
	[Serializable]
	public class JoystickStateBinding : InputBindingBase
	{
		public AxisBinding horzAxisBinding;

		public AxisBinding vertAxisBinding;

		public DirectionBinding dirBinding;

		public JoystickStateBinding(InputBindingBase parent = null)
			: base(parent)
		{
			enabled = false;
			horzAxisBinding = new AxisBinding(this);
			vertAxisBinding = new AxisBinding(this);
			dirBinding = new DirectionBinding(this);
		}

		public void CopyFrom(JoystickStateBinding b)
		{
			if (enabled = b.enabled)
			{
				Enable();
				dirBinding.CopyFrom(b.dirBinding);
				horzAxisBinding.CopyFrom(b.horzAxisBinding);
				vertAxisBinding.CopyFrom(b.vertAxisBinding);
			}
		}

		public void SyncJoyState(JoystickState state, InputRig rig)
		{
			if (state != null && enabled && !(rig == null))
			{
				Vector2 vector = state.GetVector();
				horzAxisBinding.SyncFloat(vector.x, InputRig.InputSource.Analog, rig);
				vertAxisBinding.SyncFloat(vector.y, InputRig.InputSource.Analog, rig);
				dirBinding.SyncDirectionState(state.GetDirState(), rig);
			}
		}

		protected override bool OnIsBoundToAxis(string axisName, InputRig rig)
		{
			if (!enabled)
			{
				return false;
			}
			return horzAxisBinding.IsBoundToAxis(axisName, rig) || vertAxisBinding.IsBoundToAxis(axisName, rig) || dirBinding.IsBoundToAxis(axisName, rig);
		}

		protected override bool OnIsBoundToKey(KeyCode keyCode, InputRig rig)
		{
			if (!enabled)
			{
				return false;
			}
			return horzAxisBinding.IsBoundToKey(keyCode, rig) || vertAxisBinding.IsBoundToKey(keyCode, rig) || dirBinding.IsBoundToKey(keyCode, rig);
		}

		protected override void OnGetSubBindingDescriptions(BindingDescriptionList descList, UnityEngine.Object undoObject, string parentMenuPath)
		{
			descList.Add(horzAxisBinding, InputRig.InputSource.Analog, "Horizontal", parentMenuPath, undoObject);
			descList.Add(vertAxisBinding, InputRig.InputSource.Analog, "Vertical", parentMenuPath, undoObject);
			descList.Add(dirBinding, "Direction", parentMenuPath, undoObject);
		}
	}
}
