using System;
using UnityEngine;

namespace ControlFreak2.Internal
{
	[Serializable]
	public class DirectionBinding : InputBindingBase
	{
		public enum BindMode
		{
			Normal,
			OnChange,
			OnRelease,
			OriginalOnStart,
			OriginalUntilRelease,
			OriginalUntilChange
		}

		public bool bindDiagonals;

		public BindMode bindMode;

		public DigitalBinding dirBindingU;

		public DigitalBinding dirBindingUR;

		public DigitalBinding dirBindingR;

		public DigitalBinding dirBindingDR;

		public DigitalBinding dirBindingD;

		public DigitalBinding dirBindingDL;

		public DigitalBinding dirBindingL;

		public DigitalBinding dirBindingUL;

		public DigitalBinding dirBindingN;

		public DigitalBinding dirBindingAny;

		public DirectionBinding(InputBindingBase parent = null)
			: base(parent)
		{
			bindDiagonals = true;
			dirBindingN = new DigitalBinding(this);
			dirBindingAny = new DigitalBinding(this);
			dirBindingU = new DigitalBinding(this);
			dirBindingUR = new DigitalBinding(this);
			dirBindingR = new DigitalBinding(this);
			dirBindingDR = new DigitalBinding(this);
			dirBindingD = new DigitalBinding(this);
			dirBindingDL = new DigitalBinding(this);
			dirBindingL = new DigitalBinding(this);
			dirBindingUL = new DigitalBinding(this);
		}

		public void CopyFrom(DirectionBinding b)
		{
			if (enabled = b.enabled)
			{
				Enable();
				bindDiagonals = b.bindDiagonals;
				bindMode = b.bindMode;
				dirBindingN.CopyFrom(b.dirBindingN);
				dirBindingAny.CopyFrom(b.dirBindingAny);
				dirBindingU.CopyFrom(b.dirBindingU);
				dirBindingUR.CopyFrom(b.dirBindingUR);
				dirBindingR.CopyFrom(b.dirBindingR);
				dirBindingDR.CopyFrom(b.dirBindingDR);
				dirBindingD.CopyFrom(b.dirBindingD);
				dirBindingDL.CopyFrom(b.dirBindingDL);
				dirBindingD.CopyFrom(b.dirBindingL);
				dirBindingUL.CopyFrom(b.dirBindingUL);
			}
		}

		protected override void OnGetSubBindingDescriptions(BindingDescriptionList descList, UnityEngine.Object undoObject, string parentMenuPath)
		{
			descList.Add(dirBindingN, "Neutral", parentMenuPath, undoObject);
			descList.Add(dirBindingAny, "Any Non-Neutral", parentMenuPath, undoObject);
			descList.Add(dirBindingU, "Up", parentMenuPath, undoObject);
			if (descList.addUnusedBindings || bindDiagonals)
			{
				descList.Add(dirBindingUR, "Up-Right", parentMenuPath, undoObject);
			}
			descList.Add(dirBindingR, "Right", parentMenuPath, undoObject);
			if (descList.addUnusedBindings || bindDiagonals)
			{
				descList.Add(dirBindingDR, "Down-Right", parentMenuPath, undoObject);
			}
			descList.Add(dirBindingD, "Down", parentMenuPath, undoObject);
			if (descList.addUnusedBindings || bindDiagonals)
			{
				descList.Add(dirBindingDL, "Down-Left", parentMenuPath, undoObject);
			}
			descList.Add(dirBindingL, "Left", parentMenuPath, undoObject);
			if (descList.addUnusedBindings || bindDiagonals)
			{
				descList.Add(dirBindingUL, "Up-Left", parentMenuPath, undoObject);
			}
		}

		public void SyncDirectionState(DirectionState dirState, InputRig rig)
		{
			switch (bindMode)
			{
			case BindMode.Normal:
				SyncDirRaw(dirState.GetCur(), rig);
				break;
			case BindMode.OnChange:
				if (dirState.GetCur() != dirState.GetPrev())
				{
					SyncDirRaw(dirState.GetCur(), rig);
				}
				break;
			case BindMode.OnRelease:
				if (dirState.GetCur() == Dir.N && dirState.GetPrev() != 0)
				{
					SyncDirRaw(dirState.GetPrev(), rig);
				}
				break;
			case BindMode.OriginalOnStart:
				if (dirState.GetOriginal() != 0 && dirState.GetPrevOriginal() != dirState.GetOriginal())
				{
					SyncDirRaw(dirState.GetOriginal(), rig);
				}
				break;
			case BindMode.OriginalUntilChange:
				if (dirState.GetOriginal() != 0 && dirState.GetOriginal() == dirState.GetCur())
				{
					SyncDirRaw(dirState.GetOriginal(), rig);
				}
				break;
			case BindMode.OriginalUntilRelease:
				if (dirState.GetOriginal() != 0)
				{
					SyncDirRaw(dirState.GetOriginal(), rig);
				}
				break;
			}
		}

		public void SyncDirRaw(Dir dir, InputRig rig)
		{
			if (rig == null || !enabled)
			{
				return;
			}
			if (dir != 0)
			{
				dirBindingAny.Sync(state: true, rig);
			}
			if (bindDiagonals)
			{
				switch (dir)
				{
				case Dir.N:
					dirBindingN.Sync(state: true, rig);
					break;
				case Dir.U:
					dirBindingU.Sync(state: true, rig);
					break;
				case Dir.UR:
					dirBindingUR.Sync(state: true, rig);
					break;
				case Dir.R:
					dirBindingR.Sync(state: true, rig);
					break;
				case Dir.DR:
					dirBindingDR.Sync(state: true, rig);
					break;
				case Dir.D:
					dirBindingD.Sync(state: true, rig);
					break;
				case Dir.DL:
					dirBindingDL.Sync(state: true, rig);
					break;
				case Dir.L:
					dirBindingL.Sync(state: true, rig);
					break;
				case Dir.UL:
					dirBindingUL.Sync(state: true, rig);
					break;
				}
			}
			else
			{
				if (dir == Dir.U || dir == Dir.UL || dir == Dir.UR)
				{
					dirBindingU.Sync(state: true, rig);
				}
				if (dir == Dir.R || dir == Dir.UR || dir == Dir.DR)
				{
					dirBindingR.Sync(state: true, rig);
				}
				if (dir == Dir.D || dir == Dir.DL || dir == Dir.DR)
				{
					dirBindingD.Sync(state: true, rig);
				}
				if (dir == Dir.L || dir == Dir.UL || dir == Dir.DL)
				{
					dirBindingL.Sync(state: true, rig);
				}
			}
		}

		protected override bool OnIsBoundToAxis(string axisName, InputRig rig)
		{
			if (!enabled)
			{
				return false;
			}
			return dirBindingN.IsBoundToAxis(axisName, rig) || dirBindingAny.IsBoundToAxis(axisName, rig) || dirBindingU.IsBoundToAxis(axisName, rig) || dirBindingR.IsBoundToAxis(axisName, rig) || dirBindingD.IsBoundToAxis(axisName, rig) || dirBindingL.IsBoundToAxis(axisName, rig) || (bindDiagonals && (dirBindingUR.IsBoundToAxis(axisName, rig) || dirBindingDR.IsBoundToAxis(axisName, rig) || dirBindingDL.IsBoundToAxis(axisName, rig) || dirBindingUL.IsBoundToAxis(axisName, rig)));
		}

		protected override bool OnIsBoundToKey(KeyCode keyCode, InputRig rig)
		{
			if (!enabled)
			{
				return false;
			}
			return dirBindingN.IsBoundToKey(keyCode, rig) || dirBindingAny.IsBoundToKey(keyCode, rig) || dirBindingU.IsBoundToKey(keyCode, rig) || dirBindingR.IsBoundToKey(keyCode, rig) || dirBindingD.IsBoundToKey(keyCode, rig) || dirBindingL.IsBoundToKey(keyCode, rig) || (bindDiagonals && (dirBindingUR.IsBoundToKey(keyCode, rig) || dirBindingDR.IsBoundToKey(keyCode, rig) || dirBindingDL.IsBoundToKey(keyCode, rig) || dirBindingUL.IsBoundToKey(keyCode, rig)));
		}
	}
}
