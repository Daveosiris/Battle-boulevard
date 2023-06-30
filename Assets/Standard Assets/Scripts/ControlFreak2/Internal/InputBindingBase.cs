using System;
using UnityEngine;

namespace ControlFreak2.Internal
{
	[Serializable]
	public abstract class InputBindingBase : IBindingContainer
	{
		public bool enabled;

		[NonSerialized]
		private InputBindingBase parent;

		public InputBindingBase(InputBindingBase parent)
		{
			enabled = false;
			this.parent = parent;
		}

		public InputBindingBase GetParent()
		{
			return parent;
		}

		public void Enable()
		{
			for (InputBindingBase inputBindingBase = this; inputBindingBase != null; inputBindingBase = inputBindingBase.parent)
			{
				inputBindingBase.enabled = true;
			}
		}

		public bool IsEnabledInHierarchy()
		{
			InputBindingBase inputBindingBase = this;
			do
			{
				if (!inputBindingBase.enabled)
				{
					return false;
				}
			}
			while ((inputBindingBase = inputBindingBase.parent) != null);
			return true;
		}

		public void GetSubBindingDescriptions(BindingDescriptionList descList, UnityEngine.Object undoObject, string parentMenuPath)
		{
			OnGetSubBindingDescriptions(descList, undoObject, parentMenuPath);
		}

		protected virtual void OnGetSubBindingDescriptions(BindingDescriptionList descList, UnityEngine.Object undoObject, string parentMenuPath)
		{
		}

		public bool IsBoundToKey(KeyCode key, InputRig rig)
		{
			return OnIsBoundToKey(key, rig);
		}

		public bool IsBoundToAxis(string axisName, InputRig rig)
		{
			return OnIsBoundToAxis(axisName, rig);
		}

		public bool IsEmulatingTouches()
		{
			return OnIsEmulatingTouches();
		}

		public bool IsEmulatingMousePosition()
		{
			return OnIsEmulatingMousePosition();
		}

		protected virtual bool OnIsBoundToKey(KeyCode key, InputRig rig)
		{
			return false;
		}

		protected virtual bool OnIsBoundToAxis(string axisName, InputRig rig)
		{
			return false;
		}

		protected virtual bool OnIsEmulatingTouches()
		{
			return false;
		}

		protected virtual bool OnIsEmulatingMousePosition()
		{
			return false;
		}
	}
}
