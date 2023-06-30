using System.Collections.Generic;

namespace ControlFreak2
{
	public class TouchSplitter : TouchControl
	{
		public List<TouchControl> targetControlList;

		public TouchSplitter()
		{
			ignoreFingerRadius = true;
			targetControlList = new List<TouchControl>(4);
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
			for (int i = 0; i < targetControlList.Count; i++)
			{
				TouchControl touchControl = targetControlList[i];
				if (touchControl != null)
				{
					touchControl.ReleaseAllTouches();
				}
			}
		}

		public override bool OnTouchStart(TouchObject touch, TouchControl sender, TouchStartType touchStartType)
		{
			bool result = false;
			for (int i = 0; i < targetControlList.Count; i++)
			{
				TouchControl touchControl = targetControlList[i];
				if (touchControl != null && touchControl.OnTouchStart(touch, this, TouchStartType.ProxyPress))
				{
					result = true;
				}
			}
			return result;
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
			if (!base.CanBeTouchedDirectly(touchObj))
			{
				return false;
			}
			for (int i = 0; i < targetControlList.Count; i++)
			{
				TouchControl touchControl = targetControlList[i];
				if (!(touchControl == null) && touchControl.CanBeActivatedByOtherControl(this, touchObj))
				{
					return true;
				}
			}
			return false;
		}

		public override bool CanSwipeOverOthers(TouchObject touchObj)
		{
			return false;
		}

		public override bool CanBeSwipedOverFromNothing(TouchObject touchObj)
		{
			return CanBeSwipedOverFromNothingDefault(touchObj);
		}

		public override bool CanBeSwipedOverFromRestrictedList(TouchObject touchObj)
		{
			return CanBeSwipedOverFromRestrictedListDefault(touchObj);
		}
	}
}
