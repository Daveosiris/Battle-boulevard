using System;
using UnityEngine;

namespace ControlFreak2.Internal
{
	[ExecuteInEditMode]
	public abstract class TouchControlAnimatorBase : ComponentBase
	{
		public bool autoConnectToSource;

		public TouchControl sourceControl;

		protected Type sourceType;

		public TouchControlAnimatorBase(Type sourceType)
		{
			this.sourceType = sourceType;
		}

		protected abstract void OnUpdateAnimator(bool skipAnim);

		public void UpdateAnimator(bool skipAnim)
		{
			if (!(sourceControl == null))
			{
				OnUpdateAnimator(skipAnim);
			}
		}

		public void SetSourceControl(TouchControl c)
		{
			if (sourceControl != null)
			{
				sourceControl.RemoveAnimator(this);
			}
			if (c != null && !c.CanBeUsed())
			{
				c = null;
			}
			sourceControl = c;
			if (sourceControl != null)
			{
				sourceControl.AddAnimator(this);
			}
		}

		public Type GetSourceControlType()
		{
			return sourceType;
		}

		public TouchControl FindAutoSource()
		{
			return (TouchControl)GetComponentInParent(sourceType);
		}

		public void AutoConnectToSource()
		{
			TouchControl x = FindAutoSource();
			if (x != null)
			{
				SetSourceControl(x);
			}
		}

		public bool IsIllegallyAttachedToSource()
		{
			return sourceControl != null && sourceControl.gameObject == base.gameObject;
		}

		public void InvalidateHierarchy()
		{
			if (autoConnectToSource && sourceControl == null)
			{
				SetSourceControl(FindAutoSource());
			}
		}

		protected override void OnInitComponent()
		{
			if (autoConnectToSource || sourceControl == null)
			{
				SetSourceControl(FindAutoSource());
			}
		}

		protected override void OnDestroyComponent()
		{
		}

		protected override void OnEnableComponent()
		{
		}

		protected override void OnDisableComponent()
		{
		}
	}
}
