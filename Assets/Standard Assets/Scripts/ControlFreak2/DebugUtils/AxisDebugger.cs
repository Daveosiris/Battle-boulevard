using System;
using System.Collections.Generic;
using UnityEngine;

namespace ControlFreak2.DebugUtils
{
	public class AxisDebugger : MonoBehaviour
	{
		public bool drawGUI = true;

		public bool drawMouseStats;

		public bool drawUnityAxes;

		[Tooltip("When pressed, delta accumulators will be reset.")]
		public KeyCode deltaResetKey = KeyCode.F5;

		private Vector2 cfScroll;

		private Vector2 unScroll;

		private Vector2 cfMouseDelta;

		private Vector2 unMouseDelta;

		public GUISkin skin;

		private void Update()
		{
			if (UnityEngine.Input.GetKeyDown(deltaResetKey))
			{
				unScroll = Vector2.zero;
				cfScroll = Vector2.zero;
				cfMouseDelta = Vector2.zero;
				unMouseDelta = Vector2.zero;
			}
			cfScroll += CF2Input.mouseScrollDelta;
			ref Vector2 reference = ref unScroll;
			float x = reference.x;
			Vector2 mouseScrollDelta = Input.mouseScrollDelta;
			reference.x = x + mouseScrollDelta.x;
			ref Vector2 reference2 = ref unScroll;
			float y = reference2.y;
			Vector2 mouseScrollDelta2 = Input.mouseScrollDelta;
			reference2.y = y + mouseScrollDelta2.y;
			if (CF2Input.activeRig != null)
			{
				cfMouseDelta.x += CF2Input.GetAxis("Mouse X");
				cfMouseDelta.y += CF2Input.GetAxis("Mouse Y");
			}
			unMouseDelta.x += GetUnityAxis("Mouse X");
			unMouseDelta.y += GetUnityAxis("Mouse Y");
		}

		private void OnGUI()
		{
			if (CF2Input.activeRig == null)
			{
				return;
			}
			GUI.skin = skin;
			GUILayout.BeginVertical((!(GUI.skin != null)) ? GUIStyle.none : GUI.skin.box);
			GUILayout.Box("Active RIG: " + CF2Input.activeRig.name);
			if (drawMouseStats)
			{
				GUILayout.Box("Scroll: cf:" + cfScroll + " un:" + unScroll + "\nMouse Delta: cf: " + cfMouseDelta + " un:" + unMouseDelta);
			}
			List<InputRig.AxisConfig> list = CF2Input.activeRig.axes.list;
			for (int i = 0; i < list.Count; i++)
			{
				InputRig.AxisConfig axisConfig = list[i];
				float num = 0f;
				float num2 = 0f;
				bool flag = false;
				num = axisConfig.GetAnalog();
				try
				{
					num2 = UnityEngine.Input.GetAxis(axisConfig.name);
					flag = true;
				}
				catch (Exception)
				{
				}
				if (num != 0f || (drawUnityAxes && num2 != 0f))
				{
					GUILayout.BeginVertical();
					GUILayout.Label(axisConfig.name);
					GUI.color = ((num != 0f && num != 1f && num != -1f) ? Color.gray : Color.green);
					GUILayout.Label("CF : " + num.ToString("0.00000"));
					GUILayout.Box(string.Empty, GUILayout.Width(Mathf.Abs(num) * 100f));
					if (drawUnityAxes)
					{
						GUI.color = ((num2 != 0f && num2 != 1f && num2 != -1f) ? Color.gray : Color.green);
						GUILayout.Label("UN : " + ((!flag) ? "UNAVAILABLE!" : num2.ToString("0.00000")));
						GUILayout.Box(string.Empty, GUILayout.Width(Mathf.Abs(num2) * 100f));
					}
					GUILayout.EndVertical();
					GUI.color = Color.white;
					if (axisConfig.axisType != 0)
					{
					}
				}
			}
			GUILayout.EndVertical();
		}

		private float GetUnityAxis(string name, float defaultVal = 0f)
		{
			float result = defaultVal;
			try
			{
				result = UnityEngine.Input.GetAxis(name);
				return result;
			}
			catch (Exception)
			{
				return result;
			}
		}
	}
}
