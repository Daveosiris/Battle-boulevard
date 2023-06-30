using ControlFreak2.Internal;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace ControlFreak2
{
	[ExecuteInEditMode]
	public class TouchControlPanel : ComponentBase
	{
		public class SystemTouchEventData
		{
			public Vector2 pos;

			public Camera cam;

			public int id;

			public bool isMouseEvent;

			public int touchId;
		}

		private class SystemTouch
		{
			public TouchObject touch;

			public int hwId;

			public float elapsedSinceLastAction;

			public int startFrame;

			public SystemTouch(TouchControlPanel panel)
			{
				touch = new TouchObject();
				hwId = 0;
				elapsedSinceLastAction = 0f;
			}

			public void WakeUp()
			{
				elapsedSinceLastAction = 0f;
			}

			public void Update()
			{
				if (touch.IsOn() && (elapsedSinceLastAction += Time.unscaledDeltaTime) > 2f && !Input.GetMouseButton(0) && !Input.GetMouseButton(1) && !Input.GetMouseButton(2) && UnityEngine.Input.touchCount == 0)
				{
					touch.End(cancel: true);
				}
			}
		}

		private const float staticFingerTimeout = 2f;

		public bool autoConnectToRig = true;

		public InputRig rig;

		[NonSerialized]
		protected List<TouchControl> controls;

		[NonSerialized]
		private List<SystemTouch> touchList;

		[NonSerialized]
		private TouchControl.HitPool hitPool;

		private const int MAX_SYSTEM_TOUCHES = 16;

		private const int MAX_RAYCAST_HITS = 8;

		private float fingerRadInPx;

		public TouchControlPanel()
		{
			controls = new List<TouchControl>(16);
			hitPool = new TouchControl.HitPool();
			hitPool.EnsureCapacity(8);
			touchList = new List<SystemTouch>(16);
			for (int i = 0; i < 16; i++)
			{
				SystemTouch item = new SystemTouch(this);
				touchList.Add(item);
			}
		}

		protected override void OnInitComponent()
		{
			InvalidateHierarchy();
		}

		protected override void OnDestroyComponent()
		{
			ReleaseAll(cancel: true);
			if (controls != null)
			{
				foreach (TouchControl control in controls)
				{
					if (control != null)
					{
						control.SetTouchControlPanel(null);
					}
				}
			}
		}

		protected override void OnEnableComponent()
		{
		}

		protected override void OnDisableComponent()
		{
			ReleaseAll(cancel: true);
		}

		private void Update()
		{
			UpdatePanel();
		}

		public void UpdatePanel()
		{
			UpdateTouches();
		}

		public void InvalidateHierarchy()
		{
			if (autoConnectToRig || rig == null)
			{
				rig = GetComponent<InputRig>();
				if (rig == null)
				{
					rig = GetComponentInParent<InputRig>();
				}
			}
		}

		public void AddControl(TouchControl c)
		{
			if (CanBeUsed() && !controls.Contains(c))
			{
				controls.Add(c);
			}
		}

		public void RemoveControl(TouchControl c)
		{
			if (CanBeUsed() && controls != null)
			{
				controls.Remove(c);
			}
		}

		public List<TouchControl> GetControlList()
		{
			return controls;
		}

		private void Prepare()
		{
			fingerRadInPx = ((!(rig != null)) ? 0.1f : rig.fingerRadiusInCm) * CFScreen.dpcm;
		}

		public bool Raycast(Vector2 sp, Camera eventCamera)
		{
			if (rig != null)
			{
				if (rig.AreTouchControlsHiddenManually())
				{
					return false;
				}
				if (rig.AreTouchControlsSleeping())
				{
					return true;
				}
				if (rig.swipeOverFromNothing)
				{
					return true;
				}
			}
			Prepare();
			return hitPool.HitTestAny(controls, sp, eventCamera, fingerRadInPx, RaycastControlFilter);
		}

		private bool RaycastControlFilter(TouchControl c)
		{
			return c != null && c.CanBeTouchedDirectly(null);
		}

		public void OnSystemTouchStart(SystemTouchEventData data)
		{
			if (!base.IsInitialized)
			{
				return;
			}
			if (rig != null)
			{
				rig.WakeTouchControlsUp();
			}
			SystemTouch systemTouch = StartNewTouch(data);
			if (systemTouch == null)
			{
				return;
			}
			Prepare();
			if (hitPool.HitTest(controls, data.pos, data.cam, 8, fingerRadInPx, systemTouch.touch.DirectTouchControlFilter) <= 0)
			{
				return;
			}
			for (int i = 0; i < hitPool.GetList().Count; i++)
			{
				TouchControl.Hit hit = hitPool.GetList()[i];
				if (hit != null && !(hit.c == null) && (!hit.c.dontAcceptSharedTouches || systemTouch.touch.GetControlCount() <= 0) && hit.c.OnTouchStart(systemTouch.touch, null, TouchControl.TouchStartType.DirectPress) && !hit.c.shareTouch)
				{
					break;
				}
			}
		}

		public void OnSystemTouchEnd(SystemTouchEventData data)
		{
			if (base.IsInitialized)
			{
				if (rig != null)
				{
					rig.WakeTouchControlsUp();
				}
				FindTouch(data.id)?.touch.End(cancel: false);
			}
		}

		public void OnSystemTouchMove(SystemTouchEventData data)
		{
			if (!base.IsInitialized)
			{
				return;
			}
			if (rig != null)
			{
				rig.WakeTouchControlsUp();
			}
			SystemTouch systemTouch = FindTouch(data.id);
			if (systemTouch == null)
			{
				return;
			}
			systemTouch.WakeUp();
			Vector2 pos = data.pos;
			systemTouch.touch.Move(pos, data.cam);
			List<TouchControl> restrictedSwipeOverTargetList = systemTouch.touch.GetRestrictedSwipeOverTargetList();
			List<TouchControl> list = (restrictedSwipeOverTargetList == null) ? controls : restrictedSwipeOverTargetList;
			if (list.Count <= 0 || hitPool.HitTest(list, systemTouch.touch.screenPosCur, systemTouch.touch.cam) <= 0)
			{
				return;
			}
			for (int i = 0; i < hitPool.GetList().Count; i++)
			{
				TouchControl c = hitPool.GetList()[i].c;
				if (c.IsActive() && ((restrictedSwipeOverTargetList != null) ? c.CanBeSwipedOverFromRestrictedList(systemTouch.touch) : c.CanBeSwipedOverFromNothing(systemTouch.touch)) && c.OnTouchStart(systemTouch.touch, null, TouchControl.TouchStartType.SwipeOver) && !c.shareTouch)
				{
					break;
				}
			}
		}

		public void UpdateTouches()
		{
			for (int i = 0; i < touchList.Count; i++)
			{
				touchList[i].Update();
			}
			UpdateTouchPressure();
		}

		private bool IsTouchPressureSensitive(int touchId, out float pressureOut)
		{
			pressureOut = 1f;
			if (Input.touchPressureSupported)
			{
				for (int i = 0; i < UnityEngine.Input.touchCount; i++)
				{
					Touch touch = UnityEngine.Input.GetTouch(i);
					if (touch.phase != TouchPhase.Canceled && touch.phase != TouchPhase.Ended && touch.fingerId == touchId)
					{
						pressureOut = touch.pressure / touch.maximumPossiblePressure;
						return true;
					}
				}
			}
			return false;
		}

		private void UpdateTouchPressure()
		{
			if (!Input.touchPressureSupported)
			{
				return;
			}
			for (int i = 0; i < UnityEngine.Input.touchCount; i++)
			{
				Touch touch = UnityEngine.Input.GetTouch(i);
				if (touch.phase != TouchPhase.Canceled && touch.phase != TouchPhase.Ended)
				{
					FindTouch(touch.fingerId)?.touch.SetPressure(touch.pressure, touch.maximumPossiblePressure);
				}
			}
		}

		private void ReleaseAll(bool cancel)
		{
			for (int i = 0; i < touchList.Count; i++)
			{
				touchList[i].touch.End(cancel);
			}
		}

		private SystemTouch FindTouch(int hwId)
		{
			for (int i = 0; i < touchList.Count; i++)
			{
				SystemTouch systemTouch = touchList[i];
				if (systemTouch.touch.IsOn() && systemTouch.hwId == hwId)
				{
					return systemTouch;
				}
			}
			return null;
		}

		private SystemTouch StartNewTouch(SystemTouchEventData data)
		{
			SystemTouch systemTouch = null;
			for (int i = 0; i < touchList.Count; i++)
			{
				SystemTouch systemTouch2 = touchList[i];
				if (!systemTouch2.touch.IsOn())
				{
					systemTouch = systemTouch2;
				}
				else if (systemTouch2.hwId == data.id)
				{
					systemTouch2.touch.End(cancel: true);
				}
			}
			if (systemTouch != null)
			{
				systemTouch.elapsedSinceLastAction = 0f;
				systemTouch.hwId = data.id;
				systemTouch.startFrame = Time.frameCount;
				float pressureOut = 1f;
				bool isPressureSensitive = !data.isMouseEvent && IsTouchPressureSensitive(systemTouch.hwId, out pressureOut);
				systemTouch.touch.Start(data.pos, data.pos, data.cam, data.isMouseEvent, isPressureSensitive, pressureOut);
				return systemTouch;
			}
			return null;
		}

		public int GetActiveTouchCount()
		{
			int num = 0;
			for (int i = 0; i < touchList.Count; i++)
			{
				if (touchList[i].touch.IsOn())
				{
					num++;
				}
			}
			return num;
		}
	}
}
