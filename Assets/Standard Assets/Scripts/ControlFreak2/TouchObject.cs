using System.Collections.Generic;
using UnityEngine;

namespace ControlFreak2
{
	public class TouchObject
	{
		private bool isOn;

		private bool isMouse;

		private bool isPressureSensitive;

		private float normalizedPressure;

		private List<TouchControl> controls;

		public Vector2 screenPosCur;

		public Vector2 screenPosStart;

		public Camera cam;

		private bool isSwipeOverRestricted;

		private List<TouchControl> swipeOverTargetList;

		public TouchObject()
		{
			controls = new List<TouchControl>(8);
			swipeOverTargetList = new List<TouchControl>(16);
			isSwipeOverRestricted = false;
			isOn = false;
			isPressureSensitive = false;
			normalizedPressure = 1f;
		}

		public bool IsOn()
		{
			return isOn;
		}

		public bool IsMouse()
		{
			return isMouse;
		}

		public bool IsPressureSensitive()
		{
			return isPressureSensitive;
		}

		public float GetPressure()
		{
			return normalizedPressure;
		}

		public int GetControlCount()
		{
			return controls.Count;
		}

		public void Start(Vector2 screenPosStart, Vector2 screenPosCur, Camera cam, bool isMouse, bool isPressureSensitive, float pressure)
		{
			this.cam = cam;
			this.screenPosStart = screenPosStart;
			this.screenPosCur = screenPosCur;
			isSwipeOverRestricted = false;
			swipeOverTargetList.Clear();
			this.isMouse = isMouse;
			this.isPressureSensitive = isPressureSensitive;
			normalizedPressure = pressure;
			isOn = true;
			OnControlListChange();
		}

		public void MoveIfNeeded(Vector2 screenPos, Camera cam)
		{
			if (!object.ReferenceEquals(cam, this.cam) || screenPos != screenPosCur)
			{
				Move(screenPos, cam);
			}
		}

		public void Move(Vector2 screenPos, Camera cam)
		{
			this.cam = cam;
			screenPosCur = screenPos;
			for (int i = 0; i < controls.Count; i++)
			{
				TouchControl touchControl = controls[i];
				if (touchControl != null)
				{
					touchControl.OnTouchMove(this);
				}
			}
		}

		public void End(bool cancel)
		{
			isOn = false;
			for (int i = 0; i < controls.Count; i++)
			{
				controls[i].OnTouchEnd(this, cancel ? TouchControl.TouchEndType.Cancel : TouchControl.TouchEndType.Release);
			}
			controls.Clear();
			swipeOverTargetList.Clear();
			OnControlListChange();
		}

		public void SetPressure(float rawPressure, float maxPressure)
		{
			isPressureSensitive = true;
			normalizedPressure = ((!(maxPressure < 0.001f)) ? (rawPressure / maxPressure) : 1f);
			for (int i = 0; i < controls.Count; i++)
			{
				TouchControl touchControl = controls[i];
				if (touchControl != null)
				{
					touchControl.OnTouchPressureChange(this);
				}
			}
		}

		public void ReleaseControl(TouchControl c, TouchControl.TouchEndType touchEndType)
		{
			int num = controls.IndexOf(c);
			if (num >= 0)
			{
				c.OnTouchEnd(this, touchEndType);
				controls.RemoveAt(num);
				OnControlListChange();
			}
		}

		public void AddControl(TouchControl c)
		{
			if (!(c == null) && !controls.Contains(c))
			{
				controls.Add(c);
				OnControlListChange();
			}
		}

		protected void OnControlListChange()
		{
			isSwipeOverRestricted = false;
			swipeOverTargetList.Clear();
			for (int i = 0; i < controls.Count; i++)
			{
				TouchControl touchControl = controls[i];
				if (touchControl == null)
				{
					continue;
				}
				if (!touchControl.CanSwipeOverOthers(this))
				{
					isSwipeOverRestricted = true;
				}
				else
				{
					if (!touchControl.restictSwipeOverTargets)
					{
						continue;
					}
					isSwipeOverRestricted = true;
					for (int j = 0; j < touchControl.swipeOverTargetList.Count; j++)
					{
						TouchControl item = touchControl.swipeOverTargetList[j];
						if (touchControl != null && !controls.Contains(item) && !swipeOverTargetList.Contains(item))
						{
							swipeOverTargetList.Add(item);
						}
					}
				}
			}
		}

		public bool CanAcceptControl(TouchControl c)
		{
			for (int i = 0; i < controls.Count; i++)
			{
				TouchControl touchControl = controls[i];
				if (touchControl != null && !touchControl.CanShareTouchWith(c))
				{
					return false;
				}
			}
			return true;
		}

		public List<TouchControl> GetRestrictedSwipeOverTargetList()
		{
			return (!isSwipeOverRestricted) ? null : swipeOverTargetList;
		}

		public bool SwipeOverFromNothingControlFilter(TouchControl c)
		{
			return c != null && c.CanBeSwipedOverFromNothing(this);
		}

		public bool DirectTouchControlFilter(TouchControl c)
		{
			return c != null && c.CanBeTouchedDirectly(this);
		}
	}
}
