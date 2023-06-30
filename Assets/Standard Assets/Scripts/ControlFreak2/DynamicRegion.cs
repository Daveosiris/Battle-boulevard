using System;

namespace ControlFreak2
{
	public class DynamicRegion : TouchControl
	{
		[NonSerialized]
		private DynamicTouchControl targetControl;

		public DynamicRegion()
		{
			ignoreFingerRadius = true;
		}

		public DynamicTouchControl GetTargetControl()
		{
			return targetControl;
		}

		public void SetTargetControl(DynamicTouchControl targetControl)
		{
			if (this.targetControl == targetControl)
			{
				return;
			}
			if (this.targetControl != null)
			{
				if (this.targetControl.GetDynamicRegion() == this)
				{
					return;
				}
				this.targetControl.OnLinkToDynamicRegion(null);
			}
			this.targetControl = targetControl;
			this.targetControl.OnLinkToDynamicRegion(this);
		}

		protected override void OnInitControl()
		{
			ResetControl();
		}

		protected override void OnUpdateControl()
		{
		}

		protected override void OnDestroyControl()
		{
		}

		public override void ResetControl()
		{
			ReleaseAllTouches();
		}

		public override void ReleaseAllTouches()
		{
		}

		public override bool OnTouchStart(TouchObject touch, TouchControl sender, TouchStartType touchStartType)
		{
			if (targetControl != null)
			{
				return targetControl.OnTouchStart(touch, this, TouchStartType.ProxyPress);
			}
			return false;
		}

		public override bool OnTouchEnd(TouchObject touch, TouchEndType touchEndType)
		{
			return false;
		}

		public override bool OnTouchMove(TouchObject touch)
		{
			return false;
		}

		public override bool OnTouchPressureChange(TouchObject touch)
		{
			return false;
		}

		public override bool CanBeTouchedDirectly(TouchObject touchObj)
		{
			return base.CanBeTouchedDirectly(touchObj) && targetControl != null && targetControl.CanBeActivatedByDynamicRegion();
		}

		public override bool CanBeSwipedOverFromNothing(TouchObject touchObj)
		{
			return false;
		}

		public override bool CanBeSwipedOverFromRestrictedList(TouchObject touchObj)
		{
			return CanBeSwipedOverFromRestrictedListDefault(touchObj) && targetControl != null && targetControl.CanBeActivatedByDynamicRegion();
		}

		public override bool CanSwipeOverOthers(TouchObject touchObj)
		{
			return false;
		}

		public override bool CanBeActivatedByOtherControl(TouchControl c, TouchObject touchObj)
		{
			return base.CanBeActivatedByOtherControl(c, touchObj) && targetControl != null && targetControl.CanBeActivatedByDynamicRegion();
		}
	}
}
