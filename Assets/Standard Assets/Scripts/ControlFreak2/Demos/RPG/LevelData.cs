using System.Collections.Generic;
using UnityEngine;

namespace ControlFreak2.Demos.RPG
{
	public class LevelData : MonoBehaviour
	{
		public List<InteractiveObjectBase> interactiveObjectList;

		public LevelData()
		{
			interactiveObjectList = new List<InteractiveObjectBase>();
		}

		public InteractiveObjectBase FindInteractiveObjectFor(CharacterAction chara)
		{
			InteractiveObjectBase interactiveObjectBase = null;
			float num = 0f;
			for (int i = 0; i < interactiveObjectList.Count; i++)
			{
				InteractiveObjectBase interactiveObjectBase2 = interactiveObjectList[i];
				if (!(interactiveObjectBase2 == null) && interactiveObjectBase2.IsNear(chara))
				{
					float sqrMagnitude = (chara.transform.position - interactiveObjectBase2.transform.position).sqrMagnitude;
					if (interactiveObjectBase == null || sqrMagnitude < num)
					{
						interactiveObjectBase = interactiveObjectBase2;
						num = sqrMagnitude;
					}
				}
			}
			return interactiveObjectBase;
		}
	}
}
