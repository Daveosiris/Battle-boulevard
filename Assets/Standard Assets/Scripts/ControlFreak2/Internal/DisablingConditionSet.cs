using System;
using System.Collections.Generic;

namespace ControlFreak2.Internal
{
	[Serializable]
	public class DisablingConditionSet
	{
		public enum MobileModeRelation
		{
			EnabledOnlyInMobileMode,
			DisabledInMobileMode,
			AlwaysEnabled
		}

		[Serializable]
		public class DisablingRigSwitch
		{
			public string name;

			public bool disableWhenSwitchIsOff;

			private int cachedId;

			public DisablingRigSwitch()
			{
				name = string.Empty;
			}

			public DisablingRigSwitch(string name)
			{
				this.name = name;
			}

			public bool IsInEffect(InputRig rig)
			{
				return rig.rigSwitches.GetSwitchState(name, ref cachedId, disableWhenSwitchIsOff) != disableWhenSwitchIsOff;
			}
		}

		public MobileModeRelation mobileModeRelation;

		public bool disableWhenTouchScreenInactive;

		public bool disableWhenCursorIsUnlocked;

		public List<DisablingRigSwitch> switchList;

		[NonSerialized]
		private InputRig rig;

		public DisablingConditionSet(InputRig rig)
		{
			this.rig = rig;
			switchList = new List<DisablingRigSwitch>(32);
			mobileModeRelation = MobileModeRelation.EnabledOnlyInMobileMode;
			disableWhenTouchScreenInactive = true;
			disableWhenCursorIsUnlocked = false;
		}

		public void SetRig(InputRig rig)
		{
			this.rig = rig;
		}

		public bool IsInEffect()
		{
			if (mobileModeRelation != MobileModeRelation.AlwaysEnabled && ((!CF2Input.IsInMobileMode()) ? (mobileModeRelation == MobileModeRelation.EnabledOnlyInMobileMode) : (mobileModeRelation == MobileModeRelation.DisabledInMobileMode)))
			{
				return true;
			}
			if (rig == null)
			{
				return false;
			}
			if (disableWhenTouchScreenInactive && rig.AreTouchControlsSleeping())
			{
				return true;
			}
			if (disableWhenCursorIsUnlocked && !CFScreen.lockCursor)
			{
				return true;
			}
			if (IsDisabledByRigSwitches())
			{
				return true;
			}
			return false;
		}

		public bool IsDisabledByRigSwitches()
		{
			if (rig == null)
			{
				return false;
			}
			for (int i = 0; i < switchList.Count; i++)
			{
				if (switchList[i].IsInEffect(rig))
				{
					return true;
				}
			}
			return false;
		}
	}
}
