using System;
using UnityEngine;

namespace ControlFreak2.Internal
{
	[Serializable]
	public class ScrollDeltaBinding : InputBindingBase
	{
		public AxisBinding deltaBinding;

		public DigitalBinding positiveDigitalBinding;

		public DigitalBinding negativeDigitalBinding;

		public ScrollDeltaBinding(InputBindingBase parent = null)
			: base(parent)
		{
			deltaBinding = new AxisBinding("Mouse ScrollWheel", enabled: false, this);
			positiveDigitalBinding = new DigitalBinding(this);
			negativeDigitalBinding = new DigitalBinding(this);
		}

		public ScrollDeltaBinding(string axisName, bool enabled = false, InputBindingBase parent = null)
			: base(parent)
		{
			base.enabled = enabled;
			deltaBinding = new AxisBinding(axisName, enabled, this);
			positiveDigitalBinding = new DigitalBinding(this);
			negativeDigitalBinding = new DigitalBinding(this);
		}

		public void CopyFrom(ScrollDeltaBinding b)
		{
			if (enabled = b.enabled)
			{
				Enable();
				deltaBinding.CopyFrom(b.deltaBinding);
				positiveDigitalBinding.CopyFrom(b.positiveDigitalBinding);
				negativeDigitalBinding.CopyFrom(b.negativeDigitalBinding);
			}
		}

		protected override void OnGetSubBindingDescriptions(BindingDescriptionList descList, UnityEngine.Object undoObject, string parentMenuPath)
		{
			descList.Add(deltaBinding, InputRig.InputSource.Scroll, "Delta Binding", parentMenuPath, undoObject);
			descList.Add(positiveDigitalBinding, "Positive Digital", parentMenuPath, undoObject);
			descList.Add(negativeDigitalBinding, "Negative Digital", parentMenuPath, undoObject);
		}

		public void SyncScrollDelta(int delta, InputRig rig)
		{
			if (rig == null || !enabled)
			{
				return;
			}
			deltaBinding.SyncScroll(delta, rig);
			if (delta != 0)
			{
				if (delta > 0)
				{
					positiveDigitalBinding.Sync(state: true, rig);
				}
				else
				{
					negativeDigitalBinding.Sync(state: true, rig);
				}
			}
		}

		protected override bool OnIsBoundToAxis(string axisName, InputRig rig)
		{
			if (!enabled)
			{
				return false;
			}
			return deltaBinding.IsBoundToAxis(axisName, rig) || positiveDigitalBinding.IsBoundToAxis(axisName, rig) || negativeDigitalBinding.IsBoundToAxis(axisName, rig);
		}

		protected override bool OnIsBoundToKey(KeyCode keyCode, InputRig rig)
		{
			if (!enabled)
			{
				return false;
			}
			return deltaBinding.IsBoundToKey(keyCode, rig) || positiveDigitalBinding.IsBoundToKey(keyCode, rig) || negativeDigitalBinding.IsBoundToKey(keyCode, rig);
		}
	}
}
