using UnityEngine;

namespace ControlFreak2.DebugUtils
{
	[ExecuteInEditMode]
	public class GamepadHardwareTester : MonoBehaviour
	{
		public GUISkin skin;

		public const int MAX_JOYSTICKS = 4;

		public const int MAX_JOYSTICK_AXES = 10;

		public const int MAX_JOYSTICK_KEYS = 20;

		private const float AXIS_DEADZONE = 0.05f;

		private const int AXIS_COUNT = 8;

		private const int KEY_COUNT = 20;

		public static string GetJoyAxisName(int joyId, int axisId)
		{
			return "cfJ" + joyId + string.Empty + axisId;
		}

		public static KeyCode GetJoyKeyCode(int joyId, int keyId)
		{
			KeyCode keyCode = KeyCode.Joystick1Button0;
			switch (joyId)
			{
			case 0:
				keyCode = KeyCode.Joystick1Button0;
				break;
			case 1:
				keyCode = KeyCode.Joystick2Button0;
				break;
			case 2:
				keyCode = KeyCode.Joystick3Button0;
				break;
			case 3:
				keyCode = KeyCode.Joystick4Button0;
				break;
			default:
				return KeyCode.None;
			}
			if (keyId < 0 || keyId >= 20)
			{
				return KeyCode.None;
			}
			return keyCode + keyId;
		}

		private string GetAxisCode(int axis, bool positiveSide)
		{
			return "Axis" + axis + ((!positiveSide) ? "-" : "+");
		}

		private string GetAxisName(int axis, bool positiveSide)
		{
			return "Axis[" + axis + "] " + ((!positiveSide) ? "-" : "+");
		}

		private float GetAxisState(int axis)
		{
			return UnityEngine.Input.GetAxis("JoyAxis" + axis);
		}

		private string GetKeyCode(int key)
		{
			return "Key" + key;
		}

		private bool GetKeyState(int key)
		{
			return UnityEngine.Input.GetKey((KeyCode)(350 + key));
		}

		private void OnGUI()
		{
			GUI.skin = skin;
			string[] joystickNames = Input.GetJoystickNames();
			for (int i = 0; i < 4; i++)
			{
				string text = string.Empty;
				if (joystickNames == null || joystickNames.Length <= i)
				{
					text = "No joystick connected!";
				}
				else
				{
					for (int j = 0; j < 10; j++)
					{
						float axisRaw = UnityEngine.Input.GetAxisRaw(GetJoyAxisName(i, j));
						if (!(Mathf.Abs(axisRaw) <= 0.05f))
						{
							string text2 = text;
							text = text2 + GetAxisName(j, axisRaw > 0f) + "   [ " + axisRaw.ToString("0.00") + "]\n";
						}
					}
					for (int k = 0; k < 20; k++)
					{
						if (UnityEngine.Input.GetKey(GetJoyKeyCode(i, k)))
						{
							string text2 = text;
							text = text2 + "Key" + k + "\n";
						}
					}
					if (text == string.Empty)
					{
						text = "Nothing pressed";
					}
					text = i.ToString() + ":[" + joystickNames[i] + "]\n\n" + text;
				}
				float num = (Screen.width - 20) / 4;
				GUI.Box(new Rect(10f + (float)i * num + 5f, 10f, num - 5f, Screen.height - 130), text);
			}
			string text3 = string.Empty;
			if (UnityEngine.Input.GetKey(KeyCode.Escape))
			{
				text3 += "Escape ";
			}
			if (UnityEngine.Input.GetKey(KeyCode.Return))
			{
				text3 += "Enter ";
			}
			if (UnityEngine.Input.GetKey(KeyCode.LeftShift))
			{
				text3 += "LShift ";
			}
			if (UnityEngine.Input.GetKey(KeyCode.RightShift))
			{
				text3 += "RShift ";
			}
			if (UnityEngine.Input.GetKey(KeyCode.LeftControl))
			{
				text3 += "LCtrl ";
			}
			if (UnityEngine.Input.GetKey(KeyCode.RightControl))
			{
				text3 += "RCtrl ";
			}
			if (UnityEngine.Input.GetKey(KeyCode.Space))
			{
				text3 += "Space ";
			}
			GUI.Box(new Rect(10f, Screen.height - 100, Screen.width - 20, 50f), text3);
			if (GUI.Button(new Rect(10f, Screen.height - 40, 100f, 30f), "Reset Axes"))
			{
				Input.ResetInputAxes();
			}
			if (GUI.Button(new Rect(115f, Screen.height - 40, 100f, 30f), "Exit"))
			{
				Application.Quit();
			}
			GUI.Box(new Rect(220f, Screen.height - 40, Screen.width - 230, 30f), "Unity Ver [" + Application.unityVersion + "]");
		}
	}
}
