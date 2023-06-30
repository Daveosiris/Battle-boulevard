using System;
using System.Collections.Generic;
using UnityEngine;

namespace ControlFreak2.Internal
{
	[Serializable]
	public class AxisBinding : InputBindingBase
	{
		[Serializable]
		public class TargetElem
		{
			public bool separateAxes;

			public string singleAxis;

			public bool reverseSingleAxis;

			public string positiveAxis;

			public string negativeAxis;

			public bool positiveAxisAsPositive;

			public bool negativeAxisAsPositive;

			private int singleAxisId;

			private int positiveAxisId;

			private int negativeAxisId;

			public TargetElem()
			{
				separateAxes = false;
				singleAxis = string.Empty;
				reverseSingleAxis = false;
				positiveAxis = string.Empty;
				negativeAxis = string.Empty;
				positiveAxisAsPositive = true;
				negativeAxisAsPositive = true;
				singleAxisId = 0;
				positiveAxisId = 0;
				negativeAxisId = 0;
			}

			public void SetSingleAxis(string name, bool flip)
			{
				separateAxes = false;
				singleAxis = name;
				reverseSingleAxis = flip;
			}

			public void SetSeparateAxis(string name, bool positiveSide, bool asPositive)
			{
				separateAxes = true;
				if (positiveSide)
				{
					positiveAxis = name;
					positiveAxisAsPositive = asPositive;
				}
				else
				{
					negativeAxis = name;
					negativeAxisAsPositive = asPositive;
				}
			}

			public void SyncFloat(float val, InputRig.InputSource source, InputRig rig)
			{
				if (separateAxes)
				{
					if (val >= 0f)
					{
						rig.SetAxis(positiveAxis, ref positiveAxisId, (!positiveAxisAsPositive) ? (0f - val) : val, source);
					}
					else
					{
						rig.SetAxis(negativeAxis, ref negativeAxisId, (!negativeAxisAsPositive) ? val : (0f - val), source);
					}
				}
				else
				{
					rig.SetAxis(singleAxis, ref singleAxisId, (!reverseSingleAxis) ? val : (0f - val), source);
				}
			}

			public void SyncScroll(int val, InputRig rig)
			{
				if (!separateAxes)
				{
					rig.SetAxisScroll(singleAxis, ref singleAxisId, (!reverseSingleAxis) ? val : (-val));
				}
			}

			public float GetAxis(InputRig rig)
			{
				if (rig == null)
				{
					return 0f;
				}
				if (separateAxes)
				{
					return ((!positiveAxisAsPositive) ? (-1f) : 1f) * rig.GetAxis(positiveAxis, ref positiveAxisId) - ((!negativeAxisAsPositive) ? (-1f) : 1f) * rig.GetAxis(negativeAxis, ref negativeAxisId);
				}
				return rig.GetAxis(singleAxis, ref singleAxisId);
			}

			public bool IsBoundToAxis(string axisName)
			{
				return (!separateAxes) ? (singleAxis == axisName) : (positiveAxis == axisName || negativeAxis == axisName);
			}

			public void CopyFrom(TargetElem elem)
			{
				separateAxes = elem.separateAxes;
				singleAxis = elem.singleAxis;
				reverseSingleAxis = elem.reverseSingleAxis;
				positiveAxis = elem.positiveAxis;
				positiveAxisAsPositive = elem.positiveAxisAsPositive;
				negativeAxis = elem.negativeAxis;
				negativeAxisAsPositive = elem.negativeAxisAsPositive;
			}
		}

		public List<TargetElem> targetList;

		public AxisBinding(InputBindingBase parent = null)
			: base(parent)
		{
			BasicConstructor();
		}

		public AxisBinding(string singleName, bool enabled, InputBindingBase parent = null)
			: base(parent)
		{
			BasicConstructor();
			AddTarget().SetSingleAxis(singleName, flip: false);
			if (enabled)
			{
				Enable();
			}
		}

		private void BasicConstructor()
		{
			enabled = false;
			targetList = new List<TargetElem>(1);
		}

		public void CopyFrom(AxisBinding b)
		{
			if (!(enabled = b.enabled))
			{
				return;
			}
			Enable();
			if (targetList.Count != b.targetList.Count)
			{
				targetList.Clear();
				for (int i = 0; i < b.targetList.Count; i++)
				{
					AddTarget();
				}
			}
			for (int j = 0; j < b.targetList.Count; j++)
			{
				targetList[j].CopyFrom(b.targetList[j]);
			}
		}

		public void Clear()
		{
			targetList.Clear();
		}

		public TargetElem AddTarget()
		{
			TargetElem targetElem = new TargetElem();
			targetList.Add(targetElem);
			return targetElem;
		}

		public void RemoveLastTarget()
		{
			if (targetList.Count > 0)
			{
				targetList.RemoveAt(targetList.Count - 1);
			}
		}

		public TargetElem GetTarget(int axisElemId)
		{
			return (axisElemId >= 0 && axisElemId < targetList.Count) ? targetList[axisElemId] : null;
		}

		public void SyncFloat(float val, InputRig.InputSource source, InputRig rig)
		{
			if (!(rig == null) && enabled)
			{
				for (int i = 0; i < targetList.Count; i++)
				{
					targetList[i].SyncFloat(val, source, rig);
				}
			}
		}

		public void SyncScroll(int val, InputRig rig)
		{
			if (!(rig == null) && enabled)
			{
				for (int i = 0; i < targetList.Count; i++)
				{
					targetList[i].SyncScroll(val, rig);
				}
			}
		}

		protected override bool OnIsBoundToAxis(string axisName, InputRig rig)
		{
			if (!enabled)
			{
				return false;
			}
			for (int i = 0; i < targetList.Count; i++)
			{
				if (targetList[i].IsBoundToAxis(axisName))
				{
					return true;
				}
			}
			return false;
		}

		protected override bool OnIsBoundToKey(KeyCode keycode, InputRig rig)
		{
			return false;
		}

		public float GetAxis(InputRig rig)
		{
			if (!enabled)
			{
				return 0f;
			}
			if (rig == null)
			{
				rig = CF2Input.activeRig;
			}
			if (rig == null)
			{
				return 0f;
			}
			if (targetList == null || targetList.Count == 0)
			{
				return 0f;
			}
			return targetList[0].GetAxis(rig);
		}
	}
}
