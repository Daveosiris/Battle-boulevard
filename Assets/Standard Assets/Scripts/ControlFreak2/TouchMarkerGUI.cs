using System.Collections.Generic;
using UnityEngine;

namespace ControlFreak2
{
	public class TouchMarkerGUI : MonoBehaviour
	{
		public Texture2D fingerMarker;

		public Texture2D pinchHintMarker;

		public Texture2D twistHintMarker;

		public static TouchMarkerGUI mInst;

		private void OnEnable()
		{
			mInst = this;
		}

		private void OnDisable()
		{
			if (mInst == this)
			{
				mInst = null;
			}
		}

		private void OnGUI()
		{
			if (CF2Input.activeRig == null)
			{
				return;
			}
			List<TouchControl> touchControls = CF2Input.activeRig.GetTouchControls();
			for (int i = 0; i < touchControls.Count; i++)
			{
				SuperTouchZone superTouchZone = touchControls[i] as SuperTouchZone;
				if (superTouchZone != null)
				{
					superTouchZone.DrawMarkerGUI();
				}
			}
		}
	}
}
