using System;
using System.Collections.Generic;
using UnityEngine;

namespace ControlFreak2.Internal
{
	[Serializable]
	public class DigitalBinding : InputBindingBase
	{
		[Serializable]
		public class AxisElem
		{
			public string axisName;

			public bool axisPositiveSide;

			[NonSerialized]
			private int cachedAxisId;

			public AxisElem()
			{
				axisPositiveSide = true;
				axisName = string.Empty;
			}

			private bool OnIsBoundToAxis(string axisName, InputRig rig)
			{
				return this.axisName == axisName;
			}

			public void SetAxis(string axisName, bool positiveSide)
			{
				this.axisName = axisName;
				axisPositiveSide = positiveSide;
			}

			public void CopyFrom(AxisElem b)
			{
				axisName = b.axisName;
				axisPositiveSide = b.axisPositiveSide;
			}

			public void ApplyToRig(InputRig rig, bool skipIfTargetIsMuted)
			{
				InputRig.AxisConfig axisConfig = rig.GetAxisConfig(axisName, ref cachedAxisId);
				if (axisConfig != null && (!skipIfTargetIsMuted || !axisConfig.IsMuted()))
				{
					axisConfig.SetSignedDigital(axisPositiveSide);
				}
			}
		}

		public List<AxisElem> axisList;

		public List<KeyCode> keyList;

		public DigitalBinding(InputBindingBase parent = null)
			: base(parent)
		{
			BasicConstructor();
		}

		public DigitalBinding(KeyCode key, bool bindToAxis, string axisName, bool axisNegSide, bool enabled, InputBindingBase parent = null)
			: base(parent)
		{
			BasicConstructor();
			if (enabled)
			{
				Enable();
			}
			if (key != 0)
			{
				AddKey(key);
			}
			if (!string.IsNullOrEmpty(axisName))
			{
				AddAxis().SetAxis(axisName, !axisNegSide);
			}
		}

		public DigitalBinding(KeyCode key, string axisName, bool enabled, InputBindingBase parent = null)
			: base(parent)
		{
			BasicConstructor();
			if (enabled)
			{
				Enable();
			}
			if (key != 0)
			{
				AddKey(key);
			}
			if (!string.IsNullOrEmpty(axisName))
			{
				AddAxis().SetAxis(axisName, positiveSide: true);
			}
		}

		public DigitalBinding(KeyCode key, bool enabled, InputBindingBase parent = null)
			: base(parent)
		{
			BasicConstructor();
			if (enabled)
			{
				Enable();
			}
			if (key != 0)
			{
				AddKey(key);
			}
		}

		private void BasicConstructor()
		{
			enabled = false;
			keyList = new List<KeyCode>(1);
			axisList = new List<AxisElem>(1);
		}

		public void CopyFrom(DigitalBinding b)
		{
			if (b == null || !(enabled = b.enabled))
			{
				return;
			}
			Enable();
			if (axisList.Count != b.axisList.Count)
			{
				axisList.Clear();
				for (int i = 0; i < b.axisList.Count; i++)
				{
					AddAxis();
				}
			}
			for (int j = 0; j < b.axisList.Count; j++)
			{
				axisList[j].CopyFrom(b.axisList[j]);
			}
			if (keyList.Count != b.keyList.Count)
			{
				keyList.Clear();
				for (int k = 0; k < b.keyList.Count; k++)
				{
					AddKey(b.keyList[k]);
				}
			}
			else
			{
				for (int l = 0; l < b.keyList.Count; l++)
				{
					keyList[l] = b.keyList[l];
				}
			}
		}

		public void Sync(bool state, InputRig rig, bool skipIfTargetIsMuted = false)
		{
			if (state && !(rig == null) && enabled)
			{
				for (int i = 0; i < keyList.Count; i++)
				{
					rig.SetKeyCode(keyList[i]);
				}
				for (int j = 0; j < axisList.Count; j++)
				{
					AxisElem axisElem = axisList[j];
					axisElem.ApplyToRig(rig, skipIfTargetIsMuted);
				}
			}
		}

		public void Clear()
		{
			ClearKeys();
			ClearAxes();
		}

		public void ClearKeys()
		{
			keyList.Clear();
		}

		public void ClearAxes()
		{
			axisList.Clear();
		}

		public void AddKey(KeyCode code)
		{
			keyList.Add(code);
		}

		public void RemoveLastKey()
		{
			if (keyList.Count > 0)
			{
				keyList.RemoveAt(keyList.Count - 1);
			}
		}

		public void ReplaceKey(int keyElemId, KeyCode key)
		{
			if (keyElemId >= 0 && keyElemId < keyList.Count)
			{
				keyList[keyElemId] = key;
			}
		}

		public AxisElem AddAxis()
		{
			AxisElem axisElem = new AxisElem();
			axisList.Add(axisElem);
			return axisElem;
		}

		public AxisElem GetAxisElem(int id)
		{
			return (id >= 0 && id < axisList.Count) ? axisList[id] : null;
		}

		public void RemoveLastAxis()
		{
			if (axisList.Count > 0)
			{
				axisList.RemoveAt(axisList.Count - 1);
			}
		}

		protected override bool OnIsBoundToKey(KeyCode keycode, InputRig rig)
		{
			return enabled && keyList.Count > 0 && keyList.IndexOf(keycode) >= 0;
		}

		protected override bool OnIsBoundToAxis(string axisName, InputRig rig)
		{
			return enabled && axisList.Count > 0 && axisList.FindIndex((AxisElem x) => x.axisName == axisName) >= 0;
		}
	}
}
