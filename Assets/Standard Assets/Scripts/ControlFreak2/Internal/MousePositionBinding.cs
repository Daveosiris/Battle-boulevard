using System;
using UnityEngine;

namespace ControlFreak2.Internal
{
	[Serializable]
	public class MousePositionBinding : InputBindingBase
	{
		public int priority;

		public MousePositionBinding(InputBindingBase parent = null)
			: base(parent)
		{
			enabled = false;
			priority = 0;
		}

		public MousePositionBinding(int prio, bool enabled, InputBindingBase parent = null)
			: base(parent)
		{
			base.enabled = enabled;
			priority = prio;
		}

		public void CopyFrom(MousePositionBinding b)
		{
			if (b != null && (enabled = b.enabled))
			{
				Enable();
				priority = b.priority;
			}
		}

		public void SyncPos(Vector2 pos, InputRig rig)
		{
			if (enabled && !(rig == null))
			{
				rig.mouseConfig.SetPosition(pos, priority);
			}
		}

		protected override bool OnIsEmulatingMousePosition()
		{
			return enabled;
		}
	}
}
