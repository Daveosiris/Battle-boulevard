using UnityEngine;

namespace ControlFreak2
{
	public abstract class BuiltInGamepadProfileBank
	{
		private static BuiltInGamepadProfileBank mInst;

		protected GamepadProfile[] profiles;

		protected GamepadProfile genericProfile;

		public static GamepadProfile GetProfile(string deviceName)
		{
			return Inst()?.FindProfile(deviceName);
		}

		public static GamepadProfile GetGenericProfile()
		{
			return Inst()?.GetInternalGenericProfile();
		}

		protected virtual GamepadProfile GetInternalGenericProfile()
		{
			if (genericProfile == null)
			{
				genericProfile = new GamepadProfile.GenericProfile();
			}
			return genericProfile;
		}

		protected virtual GamepadProfile FindProfile(string deviceName)
		{
			return FindInternalProfile(deviceName);
		}

		protected GamepadProfile FindInternalProfile(string deviceName)
		{
			if (profiles == null || profiles.Length == 0)
			{
				return null;
			}
			for (int i = 0; i < profiles.Length; i++)
			{
				if (profiles[i] != null && profiles[i].IsCompatible(deviceName))
				{
					return profiles[i];
				}
			}
			return null;
		}

		private static BuiltInGamepadProfileBank Inst()
		{
			if (mInst != null)
			{
				return mInst;
			}
			switch (Application.platform)
			{
			case RuntimePlatform.Android:
				mInst = new BuiltInGamepadProfileBankAndroid();
				break;
			case RuntimePlatform.OSXEditor:
			case RuntimePlatform.OSXPlayer:
				mInst = new BuiltInGamepadProfileBankOSX();
				break;
			case RuntimePlatform.WindowsPlayer:
			case RuntimePlatform.WindowsEditor:
			case RuntimePlatform.MetroPlayerX86:
			case RuntimePlatform.MetroPlayerX64:
			case RuntimePlatform.MetroPlayerARM:
				mInst = new BuiltInGamepadProfileBankWin();
				break;
			case RuntimePlatform.IPhonePlayer:
			case RuntimePlatform.tvOS:
				mInst = new BuiltInGamepadProfileBankIOS();
				break;
			case RuntimePlatform.LinuxPlayer:
				mInst = new BuiltInGamepadProfileBankLinux();
				break;
			case RuntimePlatform.WebGLPlayer:
				mInst = new BuiltInGamepadProfileBankWebGL();
				break;
			}
			return mInst;
		}
	}
}
