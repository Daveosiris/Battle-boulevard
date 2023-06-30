using System;
using System.Threading;
using UnityEngine;

namespace ControlFreak2
{
	public static class CF2Input
	{
		public enum MobileMode
		{
			Auto,
			Enabled,
			Disabled
		}

		private const bool simulateMouseWithTouchesWhenRigIsActive = false;

		private static MobileMode mMobileMode = MobileMode.Enabled;

		private static bool mSimulateMouseWithTouches;

		private static InputRig mRig;

		public static InputRig activeRig
		{
			get
			{
				return mRig;
			}
			set
			{
				if (!(value == mRig))
				{
					if (mRig != null)
					{
						mRig.OnDisactivateRig();
					}
					else
					{
						mSimulateMouseWithTouches = Input.simulateMouseWithTouches;
					}
					mRig = value;
					if (mRig != null)
					{
						mRig.OnActivateRig();
						Input.simulateMouseWithTouches = false;
					}
					else
					{
						Input.simulateMouseWithTouches = mSimulateMouseWithTouches;
					}
					if (CF2Input.onActiveRigChange != null)
					{
						CF2Input.onActiveRigChange();
					}
					CFCursor.InternalRefresh();
				}
			}
		}

		public static bool anyKey => (!(mRig != null)) ? Input.anyKey : mRig.AnyKey();

		public static bool anyKeyDown => (!(mRig != null)) ? Input.anyKeyDown : mRig.AnyKeyDown();

		public static int touchCount => (!(mRig != null)) ? UnityEngine.Input.touchCount : mRig.GetEmuTouchCount();

		public static InputRig.Touch[] touches => (!(mRig != null)) ? InputRig.Touch.TranslateUnityTouches(Input.touches) : mRig.GetEmuTouchArray();

		public static Vector3 mousePosition => (!(mRig == null)) ? mRig.mouseConfig.GetPosition() : UnityEngine.Input.mousePosition;

		public static Vector2 mouseScrollDelta
		{
			get
			{
				Vector2 result;
				if (mRig == null)
				{
					Vector2 mouseScrollDelta = Input.mouseScrollDelta;
					float x = mouseScrollDelta.x;
					Vector2 mouseScrollDelta2 = Input.mouseScrollDelta;
					result = new Vector2(x, mouseScrollDelta2.y);
				}
				else
				{
					result = mRig.scrollWheel.GetDelta();
				}
				return result;
			}
		}

		public static bool simulateMouseWithTouches
		{
			get
			{
				return (!(mRig == null)) ? mSimulateMouseWithTouches : Input.simulateMouseWithTouches;
			}
			set
			{
				mSimulateMouseWithTouches = value;
				if (mRig == null)
				{
					Input.simulateMouseWithTouches = mSimulateMouseWithTouches;
				}
				else
				{
					Input.simulateMouseWithTouches = false;
				}
			}
		}

		public static event Action onMobileModeChange;

		public static event Action onActiveRigChange;

		public static MobileMode GetMobileMode()
		{
			return mMobileMode;
		}

		public static void SetMobileMode(MobileMode mode)
		{
			mMobileMode = mode;
			if (CF2Input.onMobileModeChange != null)
			{
				CF2Input.onMobileModeChange();
			}
			CFCursor.InternalRefresh();
		}

		public static bool IsInMobileMode()
		{
			return mMobileMode == MobileMode.Enabled || (mMobileMode == MobileMode.Auto && IsMobilePlatform());
		}

		public static bool IsMobilePlatform()
		{
			return Application.isMobilePlatform;
		}

		[Obsolete("Use .IsInMobileMode()")]
		public static bool ControllerActive()
		{
			return IsInMobileMode();
		}

		public static void ResetInputAxes()
		{
			if (mRig != null)
			{
				mRig.ResetInputAxes();
			}
			Input.ResetInputAxes();
		}

		public static float GetAxis(string axisName)
		{
			return (!(mRig != null)) ? UnityEngine.Input.GetAxis(axisName) : mRig.GetAxis(axisName);
		}

		public static float GetAxis(string axisName, ref int cachedId)
		{
			return (!(mRig != null)) ? UnityEngine.Input.GetAxis(axisName) : mRig.GetAxis(axisName, ref cachedId);
		}

		public static float GetAxisRaw(string axisName)
		{
			return (!(mRig != null)) ? UnityEngine.Input.GetAxisRaw(axisName) : mRig.GetAxisRaw(axisName);
		}

		public static float GetAxisRaw(string axisName, ref int cachedId)
		{
			return (!(mRig != null)) ? UnityEngine.Input.GetAxisRaw(axisName) : mRig.GetAxisRaw(axisName, ref cachedId);
		}

		public static bool GetButton(string axisName)
		{
			return (!(mRig != null)) ? Input.GetButton(axisName) : mRig.GetButton(axisName);
		}

		public static bool GetButton(string axisName, ref int cachedId)
		{
			return (!(mRig != null)) ? Input.GetButton(axisName) : mRig.GetButton(axisName, ref cachedId);
		}

		public static bool GetButtonDown(string axisName)
		{
			return (!(mRig != null)) ? Input.GetButtonDown(axisName) : mRig.GetButtonDown(axisName);
		}

		public static bool GetButtonDown(string axisName, ref int cachedId)
		{
			return (!(mRig != null)) ? Input.GetButtonDown(axisName) : mRig.GetButtonDown(axisName, ref cachedId);
		}

		public static bool GetButtonUp(string axisName)
		{
			return (!(mRig != null)) ? Input.GetButtonUp(axisName) : mRig.GetButtonUp(axisName);
		}

		public static bool GetButtonUp(string axisName, ref int cachedId)
		{
			return (!(mRig != null)) ? Input.GetButtonUp(axisName) : mRig.GetButtonUp(axisName, ref cachedId);
		}

		public static bool GetKey(KeyCode keyCode)
		{
			return (!(mRig != null)) ? UnityEngine.Input.GetKey(keyCode) : mRig.GetKey(keyCode);
		}

		public static bool GetKeyDown(KeyCode keyCode)
		{
			return (!(mRig != null)) ? UnityEngine.Input.GetKeyDown(keyCode) : mRig.GetKeyDown(keyCode);
		}

		public static bool GetKeyUp(KeyCode keyCode)
		{
			return (!(mRig != null)) ? UnityEngine.Input.GetKeyUp(keyCode) : mRig.GetKeyUp(keyCode);
		}

		[Obsolete("Please, use GetKey(KeyCode) version instead!")]
		public static bool GetKey(string keyName)
		{
			return (!(mRig != null)) ? UnityEngine.Input.GetKey(keyName) : mRig.GetKey(keyName);
		}

		[Obsolete("Please, use GetKeyDown(KeyCode) version instead!")]
		public static bool GetKeyDown(string keyName)
		{
			return (!(mRig != null)) ? UnityEngine.Input.GetKeyDown(keyName) : mRig.GetKeyDown(keyName);
		}

		[Obsolete("Please, use GetKeyUp(KeyCode) version instead!")]
		public static bool GetKeyUp(string keyName)
		{
			return (!(mRig != null)) ? UnityEngine.Input.GetKeyUp(keyName) : mRig.GetKeyUp(keyName);
		}

		public static InputRig.Touch GetTouch(int i)
		{
			return (!(mRig != null)) ? new InputRig.Touch(UnityEngine.Input.GetTouch(i)) : mRig.GetEmuTouch(i);
		}

		public static bool GetMouseButton(int mouseButton)
		{
			return (!(mRig != null)) ? Input.GetMouseButton(mouseButton) : mRig.GetMouseButton(mouseButton);
		}

		public static bool GetMouseButtonDown(int mouseButton)
		{
			return (!(mRig != null)) ? Input.GetMouseButtonDown(mouseButton) : mRig.GetMouseButtonDown(mouseButton);
		}

		public static bool GetMouseButtonUp(int mouseButton)
		{
			return (!(mRig != null)) ? Input.GetMouseButtonUp(mouseButton) : mRig.GetMouseButtonUp(mouseButton);
		}

		public static void CalibrateTilt()
		{
			if (activeRig != null)
			{
				activeRig.CalibrateTilt();
			}
		}
	}
}
