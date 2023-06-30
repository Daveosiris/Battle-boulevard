using System;

namespace ControlFreak2.Internal
{
	[Serializable]
	public class EmuTouchBinding : InputBindingBase
	{
		private int emuTouchId;

		public EmuTouchBinding(InputBindingBase parent = null)
			: base(parent)
		{
			enabled = false;
			emuTouchId = -1;
		}

		public void CopyFrom(EmuTouchBinding b)
		{
			if (b != null && (enabled = b.enabled))
			{
				Enable();
			}
		}

		public void SyncState(TouchGestureBasicState touchState, InputRig rig)
		{
			if (enabled && !(rig == null))
			{
				rig.SyncEmuTouch(touchState, ref emuTouchId);
			}
		}

		protected override bool OnIsEmulatingTouches()
		{
			return enabled;
		}
	}
}
