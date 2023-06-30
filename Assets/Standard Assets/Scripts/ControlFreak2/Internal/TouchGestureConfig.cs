using System;

namespace ControlFreak2.Internal
{
	[Serializable]
	public class TouchGestureConfig
	{
		public enum DirMode
		{
			FourWay,
			EightWay
		}

		public enum DirConstraint
		{
			None,
			Horizontal,
			Vertical,
			Auto
		}

		public int maxTapCount;

		public bool cleanTapsOnly;

		public bool detectLongPress;

		public bool detectLongTap;

		public bool endLongPressWhenMoved;

		public bool endLongPressWhenSwiped;

		public DirMode dirMode;

		public DirectionState.OriginalDirResetMode swipeOriginalDirResetMode;

		public DirConstraint swipeConstraint;

		public DirConstraint swipeDirConstraint;

		public DirConstraint scrollConstraint;

		public TouchGestureConfig()
		{
			maxTapCount = 1;
			cleanTapsOnly = true;
			swipeOriginalDirResetMode = DirectionState.OriginalDirResetMode.On180;
		}
	}
}
