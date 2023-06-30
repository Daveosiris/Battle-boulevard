using System;
using System.Threading;
using UnityEngine;

namespace ControlFreak2
{
	public static class CFCursor
	{
		private static CursorLockMode mLockState;

		private static bool mVisible;

		public static CursorLockMode lockState
		{
			get
			{
				if (IsCursorLockingAllowed() && mLockState != InternalGetLockMode())
				{
					mLockState = InternalGetLockMode();
					if (CFCursor.onLockStateChange != null)
					{
						CFCursor.onLockStateChange();
					}
				}
				return mLockState;
			}
			set
			{
				CursorLockMode cursorLockMode = mLockState;
				mLockState = value;
				if (IsCursorLockingAllowed())
				{
					InternalSetLockMode(value);
					mLockState = InternalGetLockMode();
				}
				else
				{
					InternalSetLockMode(CursorLockMode.None);
				}
				if (cursorLockMode != mLockState && CFCursor.onLockStateChange != null)
				{
					CFCursor.onLockStateChange();
				}
			}
		}

		public static bool visible
		{
			get
			{
				return (!IsCursorLockingAllowed()) ? mVisible : (mVisible = InternalIsVisible());
			}
			set
			{
				mVisible = value;
				if (IsCursorLockingAllowed())
				{
					InternalSetVisible(value);
				}
				else
				{
					InternalSetVisible(visible: true);
				}
			}
		}

		public static event Action onLockStateChange;

		static CFCursor()
		{
			mLockState = CursorLockMode.None;
			mVisible = true;
		}

		private static bool IsCursorLockingAllowed()
		{
			return !(CF2Input.activeRig != null) || !CF2Input.IsInMobileMode();
		}

		public static void InternalRefresh()
		{
			if (!IsCursorLockingAllowed())
			{
				InternalSetLockMode(CursorLockMode.None);
				InternalSetVisible(visible: true);
				return;
			}
			CursorLockMode cursorLockMode = InternalGetLockMode();
			bool flag = InternalIsVisible();
			InternalSetLockMode(mLockState);
			InternalSetVisible(mVisible);
			mLockState = InternalGetLockMode();
			mVisible = InternalIsVisible();
			if ((mLockState != cursorLockMode || mVisible != flag) && CFCursor.onLockStateChange != null)
			{
				CFCursor.onLockStateChange();
			}
		}

		public static void SetCursor(Texture2D tex, Vector2 hotSpot, CursorMode mode)
		{
			Cursor.SetCursor(tex, hotSpot, mode);
		}

		private static CursorLockMode InternalGetLockMode()
		{
			return Cursor.lockState;
		}

		private static void InternalSetLockMode(CursorLockMode mode)
		{
			Cursor.lockState = mode;
		}

		private static bool InternalIsVisible()
		{
			return Cursor.visible;
		}

		private static void InternalSetVisible(bool visible)
		{
			Cursor.visible = visible;
		}
	}
}
