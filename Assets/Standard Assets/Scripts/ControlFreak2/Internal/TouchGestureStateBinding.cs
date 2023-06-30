using System;
using UnityEngine;

namespace ControlFreak2.Internal
{
	[Serializable]
	public class TouchGestureStateBinding : InputBindingBase
	{
		public DigitalBinding rawPressBinding;

		public DigitalBinding normalPressBinding;

		public DigitalBinding longPressBinding;

		public DigitalBinding tapBinding;

		public DigitalBinding doubleTapBinding;

		public DigitalBinding longTapBinding;

		public DirectionBinding normalPressSwipeDirBinding;

		public DirectionBinding longPressSwipeDirBinding;

		public AxisBinding normalPressSwipeHorzAxisBinding;

		public AxisBinding normalPressSwipeVertAxisBinding;

		public AxisBinding longPressSwipeHorzAxisBinding;

		public AxisBinding longPressSwipeVertAxisBinding;

		public ScrollDeltaBinding normalPressScrollHorzBinding;

		public ScrollDeltaBinding normalPressScrollVertBinding;

		public ScrollDeltaBinding longPressScrollHorzBinding;

		public ScrollDeltaBinding longPressScrollVertBinding;

		public MousePositionBinding rawPressMousePosBinding;

		public MousePositionBinding normalPressMousePosBinding;

		public MousePositionBinding longPressMousePosBinding;

		public MousePositionBinding tapMousePosBinding;

		public MousePositionBinding doubleTapMousePosBinding;

		public MousePositionBinding longTapMousePosBinding;

		public MousePositionBinding normalPressSwipeMousePosBinding;

		public MousePositionBinding longPressSwipeMousePosBinding;

		public EmuTouchBinding rawPressEmuTouchBinding;

		public EmuTouchBinding normalPressEmuTouchBinding;

		public EmuTouchBinding longPressEmuTouchBinding;

		public JoystickStateBinding normalPressSwipeJoyBinding;

		public JoystickStateBinding longPressSwipeJoyBinding;

		public TouchGestureStateBinding(InputBindingBase parent = null)
			: base(parent)
		{
			enabled = false;
			rawPressBinding = new DigitalBinding(this);
			longPressBinding = new DigitalBinding(this);
			normalPressBinding = new DigitalBinding(this);
			tapBinding = new DigitalBinding(this);
			doubleTapBinding = new DigitalBinding(this);
			longTapBinding = new DigitalBinding(this);
			normalPressSwipeHorzAxisBinding = new AxisBinding(this);
			normalPressSwipeVertAxisBinding = new AxisBinding(this);
			longPressSwipeHorzAxisBinding = new AxisBinding(this);
			longPressSwipeVertAxisBinding = new AxisBinding(this);
			normalPressScrollHorzBinding = new ScrollDeltaBinding(this);
			normalPressScrollVertBinding = new ScrollDeltaBinding(this);
			longPressScrollHorzBinding = new ScrollDeltaBinding(this);
			longPressScrollVertBinding = new ScrollDeltaBinding(this);
			rawPressEmuTouchBinding = new EmuTouchBinding(this);
			normalPressEmuTouchBinding = new EmuTouchBinding(this);
			longPressEmuTouchBinding = new EmuTouchBinding(this);
			rawPressMousePosBinding = new MousePositionBinding(10, enabled: false, this);
			normalPressMousePosBinding = new MousePositionBinding(20, enabled: false, this);
			longPressMousePosBinding = new MousePositionBinding(20, enabled: false, this);
			tapMousePosBinding = new MousePositionBinding(30, enabled: false, this);
			doubleTapMousePosBinding = new MousePositionBinding(30, enabled: false, this);
			longTapMousePosBinding = new MousePositionBinding(30, enabled: false, this);
			normalPressSwipeMousePosBinding = new MousePositionBinding(20, enabled: false, this);
			longPressSwipeMousePosBinding = new MousePositionBinding(20, enabled: false, this);
			normalPressSwipeDirBinding = new DirectionBinding(this);
			longPressSwipeDirBinding = new DirectionBinding(this);
			normalPressSwipeJoyBinding = new JoystickStateBinding(this);
			longPressSwipeJoyBinding = new JoystickStateBinding(this);
		}

		public void CopyFrom(TouchGestureStateBinding b)
		{
			if (enabled = b.enabled)
			{
				Enable();
				rawPressBinding.CopyFrom(b.rawPressBinding);
				normalPressBinding.CopyFrom(b.normalPressBinding);
				longPressBinding.CopyFrom(b.longPressBinding);
				tapBinding.CopyFrom(b.tapBinding);
				doubleTapBinding.CopyFrom(b.doubleTapBinding);
				longTapBinding.CopyFrom(b.longTapBinding);
				normalPressSwipeDirBinding.CopyFrom(b.normalPressSwipeDirBinding);
				longPressSwipeDirBinding.CopyFrom(b.longPressSwipeDirBinding);
				normalPressSwipeHorzAxisBinding.CopyFrom(b.normalPressSwipeHorzAxisBinding);
				normalPressSwipeVertAxisBinding.CopyFrom(b.normalPressSwipeVertAxisBinding);
				longPressSwipeHorzAxisBinding.CopyFrom(b.longPressSwipeHorzAxisBinding);
				longPressSwipeVertAxisBinding.CopyFrom(b.longPressSwipeVertAxisBinding);
				normalPressScrollHorzBinding.CopyFrom(b.normalPressScrollHorzBinding);
				normalPressScrollVertBinding.CopyFrom(b.normalPressScrollVertBinding);
				longPressScrollHorzBinding.CopyFrom(b.longPressScrollHorzBinding);
				longPressScrollVertBinding.CopyFrom(b.longPressScrollVertBinding);
				rawPressMousePosBinding.CopyFrom(b.rawPressMousePosBinding);
				normalPressMousePosBinding.CopyFrom(b.normalPressMousePosBinding);
				longPressMousePosBinding.CopyFrom(b.longPressMousePosBinding);
				tapMousePosBinding.CopyFrom(b.tapMousePosBinding);
				doubleTapMousePosBinding.CopyFrom(b.doubleTapMousePosBinding);
				longTapMousePosBinding.CopyFrom(b.longTapMousePosBinding);
				normalPressSwipeMousePosBinding.CopyFrom(b.normalPressSwipeMousePosBinding);
				longPressSwipeMousePosBinding.CopyFrom(b.longPressSwipeMousePosBinding);
				rawPressEmuTouchBinding.CopyFrom(b.rawPressEmuTouchBinding);
				normalPressEmuTouchBinding.CopyFrom(b.normalPressEmuTouchBinding);
				longPressEmuTouchBinding.CopyFrom(b.longPressEmuTouchBinding);
				normalPressSwipeJoyBinding.CopyFrom(b.normalPressSwipeJoyBinding);
				longPressSwipeJoyBinding.CopyFrom(b.longPressSwipeJoyBinding);
			}
		}

		public void SyncTouchState(TouchGestureState screenState, TouchGestureState orientedState, JoystickState swipeJoyState, InputRig rig)
		{
			if (screenState == null || !enabled || rig == null)
			{
				return;
			}
			if (orientedState == null)
			{
				orientedState = screenState;
			}
			if (screenState.JustTapped(1))
			{
				tapBinding.Sync(state: true, rig);
				tapMousePosBinding.SyncPos(screenState.GetTapPos(), rig);
			}
			if (screenState.JustTapped(2))
			{
				doubleTapBinding.Sync(state: true, rig);
				doubleTapMousePosBinding.SyncPos(screenState.GetTapPos(), rig);
			}
			if (screenState.JustLongTapped())
			{
				longTapBinding.Sync(state: true, rig);
				longTapMousePosBinding.SyncPos(screenState.GetTapPos(), rig);
			}
			if (orientedState.PressedLong())
			{
				longPressSwipeDirBinding.SyncDirectionState(orientedState.GetSwipeDirState(), rig);
			}
			else if (orientedState.PressedNormal())
			{
				normalPressSwipeDirBinding.SyncDirectionState(orientedState.GetSwipeDirState(), rig);
			}
			if (orientedState.PressedRaw())
			{
				rawPressBinding.Sync(state: true, rig);
				rawPressMousePosBinding.SyncPos(screenState.GetCurPosSmooth(), rig);
				if (screenState.PressedNormal())
				{
					normalPressBinding.Sync(state: true, rig);
					normalPressMousePosBinding.SyncPos(screenState.GetCurPosSmooth(), rig);
				}
				if (screenState.PressedLong())
				{
					longPressBinding.Sync(state: true, rig);
					longPressMousePosBinding.SyncPos(screenState.GetCurPosSmooth(), rig);
				}
				Vector2 constrainedDeltaVec = orientedState.GetConstrainedDeltaVec();
				Vector2 scrollDelta = orientedState.GetScrollDelta();
				if (orientedState.PressedLong())
				{
					longPressScrollHorzBinding.SyncScrollDelta(Mathf.RoundToInt(scrollDelta.x), rig);
					longPressScrollVertBinding.SyncScrollDelta(Mathf.RoundToInt(scrollDelta.y), rig);
					if (screenState.Swiped())
					{
						longPressSwipeMousePosBinding.SyncPos(screenState.GetCurPosSmooth(), rig);
						longPressSwipeHorzAxisBinding.SyncFloat(constrainedDeltaVec.x, InputRig.InputSource.TouchDelta, rig);
						longPressSwipeVertAxisBinding.SyncFloat(constrainedDeltaVec.y, InputRig.InputSource.TouchDelta, rig);
					}
					if (swipeJoyState != null)
					{
						longPressSwipeJoyBinding.SyncJoyState(swipeJoyState, rig);
					}
				}
				else if (orientedState.PressedNormal())
				{
					normalPressScrollHorzBinding.SyncScrollDelta(Mathf.RoundToInt(scrollDelta.x), rig);
					normalPressScrollVertBinding.SyncScrollDelta(Mathf.RoundToInt(scrollDelta.y), rig);
					if (screenState.Swiped())
					{
						normalPressSwipeMousePosBinding.SyncPos(screenState.GetCurPosSmooth(), rig);
						normalPressSwipeHorzAxisBinding.SyncFloat(constrainedDeltaVec.x, InputRig.InputSource.TouchDelta, rig);
						normalPressSwipeVertAxisBinding.SyncFloat(constrainedDeltaVec.y, InputRig.InputSource.TouchDelta, rig);
					}
					if (swipeJoyState != null)
					{
						normalPressSwipeJoyBinding.SyncJoyState(swipeJoyState, rig);
					}
				}
			}
			rawPressEmuTouchBinding.SyncState(screenState, rig);
			if (orientedState.PressedLong())
			{
				longPressEmuTouchBinding.SyncState(screenState, rig);
			}
			else if (orientedState.PressedNormal())
			{
				normalPressEmuTouchBinding.SyncState(screenState, rig);
			}
		}

		protected override bool OnIsBoundToAxis(string axisName, InputRig rig)
		{
			if (!enabled)
			{
				return false;
			}
			return rawPressBinding.IsBoundToAxis(axisName, rig) || normalPressBinding.IsBoundToAxis(axisName, rig) || longPressBinding.IsBoundToAxis(axisName, rig) || tapBinding.IsBoundToAxis(axisName, rig) || doubleTapBinding.IsBoundToAxis(axisName, rig) || longTapBinding.IsBoundToAxis(axisName, rig) || normalPressSwipeDirBinding.IsBoundToAxis(axisName, rig) || longPressSwipeDirBinding.IsBoundToAxis(axisName, rig) || normalPressSwipeHorzAxisBinding.IsBoundToAxis(axisName, rig) || normalPressSwipeVertAxisBinding.IsBoundToAxis(axisName, rig) || longPressSwipeHorzAxisBinding.IsBoundToAxis(axisName, rig) || longPressSwipeVertAxisBinding.IsBoundToAxis(axisName, rig) || normalPressScrollHorzBinding.IsBoundToAxis(axisName, rig) || normalPressScrollVertBinding.IsBoundToAxis(axisName, rig) || longPressScrollHorzBinding.IsBoundToAxis(axisName, rig) || longPressScrollVertBinding.IsBoundToAxis(axisName, rig) || rawPressMousePosBinding.IsBoundToAxis(axisName, rig) || normalPressMousePosBinding.IsBoundToAxis(axisName, rig) || longPressMousePosBinding.IsBoundToAxis(axisName, rig) || tapMousePosBinding.IsBoundToAxis(axisName, rig) || doubleTapMousePosBinding.IsBoundToAxis(axisName, rig) || longTapMousePosBinding.IsBoundToAxis(axisName, rig) || normalPressSwipeMousePosBinding.IsBoundToAxis(axisName, rig) || longPressSwipeMousePosBinding.IsBoundToAxis(axisName, rig) || longPressSwipeJoyBinding.IsBoundToAxis(axisName, rig) || normalPressSwipeJoyBinding.IsBoundToAxis(axisName, rig) || rawPressEmuTouchBinding.IsBoundToAxis(axisName, rig) || normalPressEmuTouchBinding.IsBoundToAxis(axisName, rig) || longPressEmuTouchBinding.IsBoundToAxis(axisName, rig);
		}

		protected override bool OnIsBoundToKey(KeyCode keyCode, InputRig rig)
		{
			if (!enabled)
			{
				return false;
			}
			return rawPressBinding.IsBoundToKey(keyCode, rig) || normalPressBinding.IsBoundToKey(keyCode, rig) || longPressBinding.IsBoundToKey(keyCode, rig) || tapBinding.IsBoundToKey(keyCode, rig) || doubleTapBinding.IsBoundToKey(keyCode, rig) || longTapBinding.IsBoundToKey(keyCode, rig) || normalPressSwipeDirBinding.IsBoundToKey(keyCode, rig) || longPressSwipeDirBinding.IsBoundToKey(keyCode, rig) || normalPressSwipeHorzAxisBinding.IsBoundToKey(keyCode, rig) || normalPressSwipeVertAxisBinding.IsBoundToKey(keyCode, rig) || longPressSwipeHorzAxisBinding.IsBoundToKey(keyCode, rig) || longPressSwipeVertAxisBinding.IsBoundToKey(keyCode, rig) || longPressSwipeJoyBinding.IsBoundToKey(keyCode, rig) || normalPressSwipeJoyBinding.IsBoundToKey(keyCode, rig) || normalPressScrollHorzBinding.IsBoundToKey(keyCode, rig) || normalPressScrollVertBinding.IsBoundToKey(keyCode, rig) || longPressScrollHorzBinding.IsBoundToKey(keyCode, rig) || longPressScrollVertBinding.IsBoundToKey(keyCode, rig);
		}

		protected override bool OnIsEmulatingMousePosition()
		{
			return rawPressMousePosBinding.IsEmulatingMousePosition() || normalPressMousePosBinding.IsEmulatingMousePosition() || longPressMousePosBinding.IsEmulatingMousePosition() || tapMousePosBinding.IsEmulatingMousePosition() || doubleTapMousePosBinding.IsEmulatingMousePosition() || longTapMousePosBinding.IsEmulatingMousePosition() || normalPressSwipeMousePosBinding.IsEmulatingMousePosition() || longPressSwipeMousePosBinding.IsEmulatingMousePosition() || longPressSwipeJoyBinding.IsEmulatingMousePosition() || normalPressSwipeJoyBinding.IsEmulatingMousePosition() || rawPressEmuTouchBinding.IsEmulatingMousePosition() || normalPressEmuTouchBinding.IsEmulatingMousePosition() || longPressEmuTouchBinding.IsEmulatingMousePosition();
		}

		protected override bool OnIsEmulatingTouches()
		{
			return rawPressMousePosBinding.IsEmulatingTouches() || normalPressMousePosBinding.IsEmulatingTouches() || longPressMousePosBinding.IsEmulatingTouches() || tapMousePosBinding.IsEmulatingTouches() || doubleTapMousePosBinding.IsEmulatingTouches() || longTapMousePosBinding.IsEmulatingTouches() || normalPressSwipeMousePosBinding.IsEmulatingTouches() || longPressSwipeMousePosBinding.IsEmulatingTouches() || longPressSwipeJoyBinding.IsEmulatingTouches() || normalPressSwipeJoyBinding.IsEmulatingTouches() || rawPressEmuTouchBinding.IsEmulatingTouches() || normalPressEmuTouchBinding.IsEmulatingTouches() || longPressEmuTouchBinding.IsEmulatingTouches();
		}

		protected override void OnGetSubBindingDescriptions(BindingDescriptionList descList, UnityEngine.Object undoObject, string parentMenuPath)
		{
			descList.Add(rawPressBinding, "Press (Raw)", parentMenuPath, undoObject);
			descList.Add(normalPressBinding, "Press (Normal)", parentMenuPath, undoObject);
			descList.Add(longPressBinding, "Long Press", parentMenuPath, undoObject);
			descList.Add(tapBinding, "Tap", parentMenuPath, undoObject);
			descList.Add(doubleTapBinding, "Double Tap", parentMenuPath, undoObject);
			descList.Add(longTapBinding, "Long Tap", parentMenuPath, undoObject);
			descList.Add(normalPressScrollHorzBinding, "Horizontal Scroll (Normal Press)", parentMenuPath, undoObject);
			descList.Add(normalPressScrollVertBinding, "Vertical Scroll (Normal Press)", parentMenuPath, undoObject);
			descList.Add(longPressScrollHorzBinding, "Horizontal Scroll (Long Press)", parentMenuPath, undoObject);
			descList.Add(longPressScrollVertBinding, "Vertical Scroll (Long Press)", parentMenuPath, undoObject);
			descList.Add(normalPressSwipeHorzAxisBinding, InputRig.InputSource.TouchDelta, "Horizontal Swipe Delta (Normal Press)", parentMenuPath, undoObject);
			descList.Add(normalPressSwipeVertAxisBinding, InputRig.InputSource.TouchDelta, "Vertical Swipe Delta (Normal Press)", parentMenuPath, undoObject);
			descList.Add(longPressSwipeHorzAxisBinding, InputRig.InputSource.TouchDelta, "Horizontal Swipe Delta (Long Press)", parentMenuPath, undoObject);
			descList.Add(longPressSwipeVertAxisBinding, InputRig.InputSource.TouchDelta, "Vertical Swipe Delta (Long Press)", parentMenuPath, undoObject);
			descList.Add(normalPressSwipeDirBinding, "Swipe Direction (Normal Press)", parentMenuPath, undoObject);
			descList.Add(longPressSwipeDirBinding, "Swipe Direction (Long Press)", parentMenuPath, undoObject);
			descList.Add(normalPressSwipeJoyBinding, "Swipe Joystick (Normal Press)", parentMenuPath, undoObject);
			descList.Add(longPressSwipeJoyBinding, "Swipe Joystick (Long Press)", parentMenuPath, undoObject);
			descList.Add(rawPressMousePosBinding, "Raw Press Position", parentMenuPath, undoObject);
			descList.Add(normalPressMousePosBinding, "Normal Press Position", parentMenuPath, undoObject);
			descList.Add(longPressMousePosBinding, "Long Press Position", parentMenuPath, undoObject);
			descList.Add(tapMousePosBinding, "Tap Position", parentMenuPath, undoObject);
			descList.Add(doubleTapMousePosBinding, "Double Tap Position", parentMenuPath, undoObject);
			descList.Add(longTapMousePosBinding, "Long Tap Position", parentMenuPath, undoObject);
			descList.Add(normalPressSwipeMousePosBinding, "Swipe Position (Normal Press)", parentMenuPath, undoObject);
			descList.Add(longPressSwipeMousePosBinding, "Swipe Position (Long Press)", parentMenuPath, undoObject);
			descList.Add(normalPressEmuTouchBinding, "Emulated Touch (Normal Press)", parentMenuPath, undoObject);
			descList.Add(longPressEmuTouchBinding, "Emulated Touch (Long Press)", parentMenuPath, undoObject);
		}
	}
}
