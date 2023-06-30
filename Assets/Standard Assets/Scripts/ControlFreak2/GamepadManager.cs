using ControlFreak2.Internal;
using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace ControlFreak2
{
	public class GamepadManager : ComponentBase
	{
		public delegate void OnConnectionChnageCallback(Gamepad gamepad);

		public delegate void OnDisconnectionCallback(Gamepad gamepad, DisconnectionReason reason);

		public enum GamepadStick
		{
			LeftAnalog,
			RightAnalog,
			Dpad
		}

		public enum GamepadKey
		{
			FaceBottom,
			FaceRight,
			FaceTop,
			FaceLeft,
			Start,
			Select,
			L1,
			R1,
			L2,
			R2,
			L3,
			R3
		}

		public enum DisconnectionReason
		{
			MassDisactivation,
			Disactivation,
			Disconnection,
			ManagerDisabled
		}

		public class Gamepad
		{
			public class JoyState
			{
				private Gamepad gamepad;

				public JoystickState state;

				private bool isConnectedToHardware;

				private int srcAxisIdL;

				private int srcAxisIdR;

				private int srcAxisIdU;

				private int srcAxisIdD;

				private bool srcAxisFlipL;

				private bool srcAxisFlipR;

				private bool srcAxisFlipU;

				private bool srcAxisFlipD;

				private int srcKeyU;

				private int srcKeyR;

				private int srcKeyD;

				private int srcKeyL;

				public JoyState(Gamepad gamepad, JoystickConfig config)
				{
					this.gamepad = gamepad;
					state = new JoystickState(config);
					isConnectedToHardware = false;
				}

				public void SetConfig(JoystickConfig config)
				{
					state.SetConfig(config);
				}

				public void Reset()
				{
					state.Reset();
				}

				public void Update()
				{
					ReadHardwareState();
					state.Update();
				}

				private float GetCompositeAnalogVal(int posAxisId, bool posFlip, int negAxisId, bool negFlip)
				{
					if (posAxisId >= 0 && posAxisId == negAxisId)
					{
						float internalAxisAnalog = gamepad.GetInternalAxisAnalog(posAxisId);
						return (!posFlip) ? internalAxisAnalog : (0f - internalAxisAnalog);
					}
					float num = gamepad.GetInternalAxisAnalog(posAxisId);
					if (posFlip)
					{
						num = 0f - num;
					}
					float num2 = gamepad.GetInternalAxisAnalog(negAxisId);
					if (negFlip)
					{
						num2 = 0f - num2;
					}
					return Mathf.Clamp(num, 0f, 1f) + Mathf.Clamp(num2, -1f, 0f);
				}

				private void ReadHardwareState()
				{
					if (isConnectedToHardware)
					{
						Vector2 zero = Vector2.zero;
						zero.x = GetCompositeAnalogVal(srcAxisIdR, srcAxisFlipR, srcAxisIdL, srcAxisFlipL);
						zero.y = GetCompositeAnalogVal(srcAxisIdU, srcAxisFlipU, srcAxisIdD, srcAxisFlipD);
						bool digiU = srcKeyU >= 0 && gamepad.GetInternalKeyState(srcKeyU);
						bool digiD = srcKeyD >= 0 && gamepad.GetInternalKeyState(srcKeyD);
						bool digiR = srcKeyR >= 0 && gamepad.GetInternalKeyState(srcKeyR);
						bool digiL = srcKeyL >= 0 && gamepad.GetInternalKeyState(srcKeyL);
						state.ApplyClampedVec(zero, JoystickConfig.ClampMode.Square);
						state.ApplyDigital(digiU, digiR, digiD, digiL);
					}
				}

				public void ConnectToJoy(GamepadProfile.JoystickSource joySrc)
				{
					isConnectedToHardware = (joySrc != null);
					if (joySrc != null)
					{
						srcAxisIdL = joySrc.keyL.axisId;
						srcAxisFlipL = !joySrc.keyL.axisSign;
						srcAxisIdR = joySrc.keyR.axisId;
						srcAxisFlipR = !joySrc.keyR.axisSign;
						srcAxisIdU = joySrc.keyU.axisId;
						srcAxisFlipU = !joySrc.keyU.axisSign;
						srcAxisIdD = joySrc.keyD.axisId;
						srcAxisFlipD = !joySrc.keyD.axisSign;
						srcKeyL = joySrc.keyL.keyId;
						srcKeyR = joySrc.keyR.keyId;
						srcKeyU = joySrc.keyU.keyId;
						srcKeyD = joySrc.keyD.keyId;
					}
					Reset();
				}

				public void SyncJoyState(JoystickStateBinding bind, InputRig rig)
				{
					bind.SyncJoyState(state, rig);
				}
			}

			public class KeyState
			{
				private Gamepad gamepad;

				private bool isTrigger;

				private AnalogConfig analogConfig;

				public bool isConnectedToHardware;

				private float frAnalog;

				private bool frDigital;

				private float analogCur;

				private float analogRawCur;

				private bool digiCurRaw;

				private bool digiCur;

				private bool digiPrev;

				private int srcAxisId;

				private int srcKeyId;

				private bool srcAxisSign;

				public KeyState(Gamepad gamepad, bool isTrigger, AnalogConfig analogConfig = null)
				{
					this.gamepad = gamepad;
					this.isTrigger = isTrigger;
					this.analogConfig = analogConfig;
					isConnectedToHardware = false;
					Reset();
				}

				public void SetConfig(AnalogConfig config)
				{
					analogConfig = config;
				}

				public void SetDigital(bool state)
				{
					if (state)
					{
						frDigital = true;
					}
				}

				public void SetAnalog(float val)
				{
					if (val > frAnalog)
					{
						frAnalog = Mathf.Clamp01(val);
					}
				}

				public bool GetDigitalRaw()
				{
					return digiCurRaw;
				}

				public bool GetDigital()
				{
					return digiCur;
				}

				public bool GetDigitalDown()
				{
					return digiCur && !digiPrev;
				}

				public bool GetDigitalUp()
				{
					return !digiCur && digiPrev;
				}

				public float GetAnalog()
				{
					return analogCur;
				}

				public float GetAnalogRaw()
				{
					return analogRawCur;
				}

				public bool IsTrigger()
				{
					return isTrigger;
				}

				public void Reset()
				{
					analogCur = 0f;
					analogRawCur = 0f;
					digiCur = false;
					digiPrev = false;
					digiCurRaw = false;
					frAnalog = 0f;
					frDigital = false;
				}

				public void Update()
				{
					ReadHardwareState();
					digiPrev = digiCur;
					digiCurRaw = frDigital;
					analogRawCur = frAnalog;
					frAnalog = 0f;
					frDigital = false;
					if (analogConfig != null)
					{
						analogCur = analogConfig.GetAnalogVal((!digiCurRaw) ? analogRawCur : 1f);
						digiCur = (digiCurRaw || analogConfig.GetDigitalVal(analogRawCur, digiPrev));
					}
					else
					{
						analogCur = ((!digiCurRaw) ? analogRawCur : 1f);
						digiCur = (digiCurRaw || analogRawCur > 0.5f);
					}
				}

				private void ReadHardwareState()
				{
					if (isConnectedToHardware)
					{
						bool digital = srcKeyId >= 0 && gamepad.GetInternalKeyState(srcKeyId);
						float analog = 0f;
						if (srcAxisId >= 0)
						{
							analog = gamepad.GetInternalAxisAnalog(srcAxisId);
							analog = Mathf.Clamp01((!srcAxisSign) ? (0f - analog) : analog);
						}
						SetAnalog(analog);
						SetDigital(digital);
					}
				}

				public void ConnectToJoy(GamepadProfile.KeySource src)
				{
					isConnectedToHardware = (src != null);
					srcAxisId = -1;
					srcKeyId = -1;
					if (src != null)
					{
						srcKeyId = src.keyId;
						srcAxisId = src.axisId;
						srcAxisSign = src.axisSign;
					}
					Reset();
				}

				public void SyncDigital(DigitalBinding bind, InputRig rig)
				{
					if (GetDigital())
					{
						bind.Sync(state: true, rig);
					}
				}

				public void SyncAnalog(AxisBinding bind, InputRig rig)
				{
					bind.SyncFloat(GetAnalog(), InputRig.InputSource.Analog, rig);
					bind.SyncFloat(GetDigitalRaw() ? 1 : 0, InputRig.InputSource.Digital, rig);
				}
			}

			private class InternalKeyState
			{
				private KeyCode key;

				public bool stateCur;

				public bool statePrev;

				public bool isReadyToUse;

				public InternalKeyState()
				{
					isReadyToUse = false;
					stateCur = false;
					stateCur = false;
					key = KeyCode.None;
				}

				public bool GetState()
				{
					return isReadyToUse && stateCur;
				}

				public void Reset()
				{
					stateCur = (statePrev = false);
					isReadyToUse = false;
				}

				public void Update()
				{
					statePrev = stateCur;
					ReadHardwareState();
					if (!isReadyToUse && !stateCur)
					{
						isReadyToUse = true;
					}
				}

				private void ReadHardwareState()
				{
					stateCur = (key != 0 && UnityEngine.Input.GetKey(key));
				}

				public void ConnectToHardwareKey(int joyId, int keyId)
				{
					key = GetJoyKeyCode(joyId, keyId);
					Reset();
				}
			}

			private class InternalAxisState
			{
				private string axisName;

				public float valCur;

				public float valPrev;

				public bool isReadyToUse;

				public bool trueAnalogRange;

				public InternalAxisState()
				{
					isReadyToUse = false;
					valCur = (valPrev = 0f);
					axisName = string.Empty;
					trueAnalogRange = false;
				}

				public float GetVal()
				{
					return (!isReadyToUse) ? 0f : valCur;
				}

				public int GetDigital()
				{
					return (isReadyToUse && Mathf.Abs(valCur) > 0.5f) ? ((valCur > 0f) ? 1 : (-1)) : 0;
				}

				public void Reset()
				{
					valCur = (valPrev = 0f);
					isReadyToUse = false;
					trueAnalogRange = false;
				}

				public void Update()
				{
					valPrev = valCur;
					ReadHardwareState();
					if (!isReadyToUse && Mathf.Abs(valCur) < 0.1f)
					{
						isReadyToUse = true;
					}
				}

				public void SetVal(float v)
				{
					valCur = v;
					if (!trueAnalogRange && Mathf.Abs(v) > 0.01f && Mathf.Abs(v) < 0.99f)
					{
						trueAnalogRange = true;
					}
				}

				private void ReadHardwareState()
				{
					SetVal((!string.IsNullOrEmpty(axisName)) ? UnityEngine.Input.GetAxisRaw(axisName) : 0f);
				}

				public void ConnectToHardwareAxis(int joyId, int axisId)
				{
					axisName = GetJoyAxisName(joyId, axisId);
					Reset();
				}

				public bool IsFullyAnalog()
				{
					return trueAnalogRange;
				}
			}

			private GamepadManager manager;

			private GamepadProfile profile;

			private bool isActivated;

			private bool isConnected;

			private bool isBlocked;

			private string internalJoyName;

			private int internalJoyId;

			private int slot;

			public JoyState leftStick;

			public JoyState rightStick;

			public JoyState dpad;

			public KeyState[] keys;

			private InternalAxisState[] internalAxes;

			private InternalKeyState[] internalKeys;

			public Gamepad(GamepadManager manager)
			{
				this.manager = manager;
				internalAxes = new InternalAxisState[10];
				internalKeys = new InternalKeyState[20];
				for (int i = 0; i < internalAxes.Length; i++)
				{
					internalAxes[i] = new InternalAxisState();
				}
				for (int j = 0; j < internalKeys.Length; j++)
				{
					internalKeys[j] = new InternalKeyState();
				}
				leftStick = new JoyState(this, manager.fallbackLeftStickConfig);
				rightStick = new JoyState(this, manager.fallbackRightStickConfig);
				dpad = new JoyState(this, manager.fallbackDpadConfig);
				keys = new KeyState[12];
				for (int k = 0; k < keys.Length; k++)
				{
					switch (k)
					{
					case 6:
					case 8:
						keys[k] = new KeyState(this, isTrigger: true, manager.fallbackLeftTriggerAnalogConfig);
						break;
					case 7:
					case 9:
						keys[k] = new KeyState(this, isTrigger: true, manager.fallbackRightTriggerAnalogConfig);
						break;
					default:
						keys[k] = new KeyState(this, isTrigger: false);
						break;
					}
				}
				OnDisconnect();
			}

			public bool IsConnected()
			{
				return isConnected;
			}

			public bool IsActivated()
			{
				return isActivated;
			}

			public bool IsSupported()
			{
				return profile != null;
			}

			public int GetSlot()
			{
				return slot;
			}

			public string GetInternalJoyName()
			{
				return internalJoyName;
			}

			public int GetInternalJoyId()
			{
				return internalJoyId;
			}

			public string GetProfileName()
			{
				return (profile == null) ? string.Empty : profile.name;
			}

			public void Block(bool block)
			{
				isBlocked = block;
			}

			public bool IsBlocked()
			{
				return isBlocked;
			}

			public void SetRig(InputRig rig)
			{
				leftStick.SetConfig((!(rig != null)) ? manager.fallbackLeftStickConfig : rig.leftStickConfig);
				rightStick.SetConfig((!(rig != null)) ? manager.fallbackRightStickConfig : rig.rightStickConfig);
				dpad.SetConfig((!(rig != null)) ? manager.fallbackDpadConfig : rig.dpadConfig);
				keys[6].SetConfig((!(rig != null)) ? manager.fallbackLeftTriggerAnalogConfig : rig.leftTriggerAnalogConfig);
				keys[8].SetConfig((!(rig != null)) ? manager.fallbackLeftTriggerAnalogConfig : rig.leftTriggerAnalogConfig);
				keys[7].SetConfig((!(rig != null)) ? manager.fallbackRightTriggerAnalogConfig : rig.rightTriggerAnalogConfig);
				keys[9].SetConfig((!(rig != null)) ? manager.fallbackRightTriggerAnalogConfig : rig.rightTriggerAnalogConfig);
			}

			public void ConnectToJoy(int internalJoyId, string joyName)
			{
				profile = null;
				isActivated = false;
				profile = null;
				isBlocked = false;
				if (internalJoyId >= 0)
				{
					this.internalJoyId = internalJoyId;
					internalJoyName = joyName;
					isConnected = true;
				}
				else
				{
					isConnected = false;
				}
				for (int i = 0; i < internalAxes.Length; i++)
				{
					internalAxes[i].ConnectToHardwareAxis(internalJoyId, i);
				}
				for (int j = 0; j < internalKeys.Length; j++)
				{
					internalKeys[j].ConnectToHardwareKey(internalJoyId, j);
				}
				Reset();
			}

			public void OnDisconnect()
			{
				ConnectToJoy(-1, string.Empty);
			}

			public void OnActivate(int slot)
			{
				this.slot = slot;
				isActivated = true;
				Reset();
			}

			public void OnDisactivate()
			{
				isActivated = false;
				Reset();
			}

			public GamepadProfile GetProfile()
			{
				return profile;
			}

			public void SetProfile(GamepadProfile profile)
			{
				this.profile = profile;
				leftStick.ConnectToJoy(profile?.leftStick);
				rightStick.ConnectToJoy(profile?.rightStick);
				dpad.ConnectToJoy(profile?.dpad);
				for (int i = 0; i < keys.Length; i++)
				{
					keys[i].ConnectToJoy(profile?.GetKeySource(i));
				}
				Reset();
			}

			public void Reset()
			{
				leftStick.Reset();
				rightStick.Reset();
				dpad.Reset();
				for (int i = 0; i < keys.Length; i++)
				{
					keys[i].Reset();
				}
				for (int j = 0; j < internalAxes.Length; j++)
				{
					internalAxes[j].Reset();
				}
				for (int k = 0; k < internalKeys.Length; k++)
				{
					internalKeys[k].Reset();
				}
			}

			public bool GetKey(GamepadKey key)
			{
				return keys[(int)key].GetDigital();
			}

			public bool GetKeyDown(GamepadKey key)
			{
				return keys[(int)key].GetDigitalDown();
			}

			public bool GetKeyUp(GamepadKey key)
			{
				return keys[(int)key].GetDigitalUp();
			}

			public float GetKeyAnalog(GamepadKey key)
			{
				return keys[(int)key].GetAnalog();
			}

			public Vector2 GetStickVec(GamepadStick s)
			{
				return GetStick(s).GetVector();
			}

			public Dir GetStickDir8(GamepadStick s)
			{
				return GetStick(s).GetDir8();
			}

			public Dir GetStickDir4(GamepadStick s)
			{
				return GetStick(s).GetDir4();
			}

			public Dir GetStickDir(GamepadStick s)
			{
				return GetStick(s).GetDir();
			}

			public JoystickState GetStick(GamepadStick s)
			{
				switch (s)
				{
				case GamepadStick.LeftAnalog:
					return leftStick.state;
				case GamepadStick.RightAnalog:
					return rightStick.state;
				default:
					return dpad.state;
				}
			}

			public bool GetInternalKeyState(int keyId)
			{
				return keyId >= 0 && keyId < internalKeys.Length && internalKeys[keyId].GetState();
			}

			public float GetInternalAxisAnalog(int axisId)
			{
				return (axisId < 0 || axisId >= internalAxes.Length) ? 0f : internalAxes[axisId].GetVal();
			}

			public int GetInternalAxisDigital(int axisId)
			{
				return (axisId >= 0 && axisId < internalAxes.Length) ? internalAxes[axisId].GetDigital() : 0;
			}

			public bool IsInternalAxisFullyAnalog(int axisId)
			{
				return axisId >= 0 && axisId < internalAxes.Length && internalAxes[axisId].IsFullyAnalog();
			}

			public bool AnyInternalKeyOrAxisPressed()
			{
				return AnyInternalKeyPressed() || AnyInternalAxisPressed();
			}

			public bool AnyInternalKeyPressed()
			{
				for (int i = 0; i < internalKeys.Length; i++)
				{
					if (internalKeys[i].GetState())
					{
						return true;
					}
				}
				return false;
			}

			public bool AnyInternalAxisPressed()
			{
				for (int i = 0; i < internalAxes.Length; i++)
				{
					if (internalAxes[i].GetDigital() != 0)
					{
						return true;
					}
				}
				return false;
			}

			public int GetPressedInternalKey(int start = 0)
			{
				for (int i = start; i < internalKeys.Length; i++)
				{
					if (internalKeys[i].GetState())
					{
						return i;
					}
				}
				return -1;
			}

			public int GetPressedInternalAxis(out bool positiveSide, int start = 0)
			{
				for (int i = start; i < internalAxes.Length; i++)
				{
					int digital = internalAxes[i].GetDigital();
					if (digital != 0)
					{
						positiveSide = (digital > 0);
						return i;
					}
				}
				positiveSide = false;
				return -1;
			}

			public void Update()
			{
				if (IsConnected())
				{
					for (int i = 0; i < internalKeys.Length; i++)
					{
						internalKeys[i].Update();
					}
					for (int j = 0; j < internalAxes.Length; j++)
					{
						internalAxes[j].Update();
					}
				}
				leftStick.Update();
				rightStick.Update();
				dpad.Update();
				for (int k = 0; k < keys.Length; k++)
				{
					keys[k].Update();
				}
			}

			public void ApplyGamepadState(Gamepad g)
			{
				if (g != null)
				{
					leftStick.state.ApplyState(g.leftStick.state);
					rightStick.state.ApplyState(g.rightStick.state);
					dpad.state.ApplyState(g.dpad.state);
					for (int i = 0; i < keys.Length; i++)
					{
						keys[i].SetDigital(g.keys[i].GetDigitalRaw());
						keys[i].SetAnalog(g.keys[i].GetAnalogRaw());
					}
				}
			}
		}

		public const int MAX_JOYSTICKS = 4;

		public const int MAX_INTERNAL_AXES = 10;

		public const int MAX_INTERNAL_KEYS = 20;

		public const int GamepadStickCount = 3;

		public const int GamepadKeyCount = 12;

		private CustomGamepadProfileBank customProfileBank;

		public bool dontDestroyOnLoad;

		public float connectionCheckInterval;

		private float elapsedSinceLastConnectionCheck;

		[NonSerialized]
		private JoystickConfig fallbackDpadConfig;

		[NonSerialized]
		private JoystickConfig fallbackLeftStickConfig;

		[NonSerialized]
		private JoystickConfig fallbackRightStickConfig;

		[NonSerialized]
		private AnalogConfig fallbackLeftTriggerAnalogConfig;

		[NonSerialized]
		private AnalogConfig fallbackRightTriggerAnalogConfig;

		private List<Gamepad> activeGamepads;

		private List<Gamepad> freeGamepads;

		private List<Gamepad> connectedGamepads;

		private Gamepad gamepadsCombined;

		private int activeGamepadNum;

		private string[] deviceConnections;

		public static GamepadManager activeManager
		{
			get;
			private set;
		}

		public static event OnDisconnectionCallback onGamepadDisconnected;

		public static event OnDisconnectionCallback onGamepadDisactivated;

		public static event OnConnectionChnageCallback onGamepadConnected;

		public static event OnConnectionChnageCallback onGamepadActivated;

		public static event Action onChange;

		public GamepadManager()
		{
			customProfileBank = new CustomGamepadProfileBank();
			dontDestroyOnLoad = false;
			connectionCheckInterval = 1f;
			elapsedSinceLastConnectionCheck = 10000f;
			fallbackDpadConfig = new JoystickConfig();
			fallbackDpadConfig.stickMode = JoystickConfig.StickMode.Digital8;
			fallbackDpadConfig.analogDeadZone = 0.3f;
			fallbackLeftStickConfig = new JoystickConfig();
			fallbackLeftStickConfig.stickMode = JoystickConfig.StickMode.Analog;
			fallbackLeftStickConfig.analogDeadZone = 0.3f;
			fallbackRightStickConfig = new JoystickConfig();
			fallbackRightStickConfig.stickMode = JoystickConfig.StickMode.Analog;
			fallbackRightStickConfig.analogDeadZone = 0.3f;
			fallbackLeftTriggerAnalogConfig = new AnalogConfig();
			fallbackLeftTriggerAnalogConfig.analogDeadZone = 0.2f;
			fallbackRightTriggerAnalogConfig = new AnalogConfig();
			fallbackRightTriggerAnalogConfig.analogDeadZone = 0.2f;
			gamepadsCombined = new Gamepad(this);
			freeGamepads = new List<Gamepad>(4);
			activeGamepads = new List<Gamepad>(4);
			connectedGamepads = new List<Gamepad>(4);
			activeGamepadNum = 0;
			for (int i = 0; i < 4; i++)
			{
				freeGamepads.Add(new Gamepad(this));
			}
		}

		public static string GetJoyAxisName(int joyId, int axisId)
		{
			if (joyId < 0 || joyId >= 4 || axisId < 0 || axisId >= 10)
			{
				return string.Empty;
			}
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

		protected override void OnInitComponent()
		{
			customProfileBank.Load();
		}

		protected override void OnDestroyComponent()
		{
		}

		protected override void OnEnableComponent()
		{
			if (activeManager == null)
			{
				SetAsMain();
				if (dontDestroyOnLoad && !CFUtils.editorStopped)
				{
					UnityEngine.Object.DontDestroyOnLoad(this);
				}
			}
			else if (activeManager != this && !CFUtils.editorStopped)
			{
				base.enabled = false;
			}
		}

		public bool IsMain()
		{
			return activeManager == this;
		}

		public void SetAsMain()
		{
			base.enabled = true;
			if (!IsMain())
			{
				if (activeManager != null)
				{
					activeManager.RemoveAsMain();
				}
				activeManager = this;
				CF2Input.onActiveRigChange += OnActiveRigChange;
				OnActiveRigChange();
			}
		}

		public void RemoveAsMain()
		{
			if (activeManager == this)
			{
				activeManager = null;
			}
			CF2Input.onActiveRigChange -= OnActiveRigChange;
		}

		public CustomGamepadProfileBank GetCustomProfileBank()
		{
			return customProfileBank;
		}

		protected override void OnDisableComponent()
		{
			for (int i = 0; i < activeGamepads.Count; i++)
			{
				Gamepad gamepad = activeGamepads[i];
				if (gamepad != null)
				{
					DisconnectGamepad(gamepad, DisconnectionReason.ManagerDisabled);
				}
			}
			if (IsMain())
			{
				if (GamepadManager.onChange != null)
				{
					GamepadManager.onChange();
				}
				RemoveAsMain();
			}
		}

		private void OnActiveRigChange()
		{
			gamepadsCombined.SetRig(CF2Input.activeRig);
			for (int i = 0; i < freeGamepads.Count; i++)
			{
				freeGamepads[i].SetRig(CF2Input.activeRig);
			}
			for (int j = 0; j < connectedGamepads.Count; j++)
			{
				connectedGamepads[j].SetRig(CF2Input.activeRig);
			}
		}

		private void Update()
		{
			CheckJoystickConnections();
			for (int i = 0; i < connectedGamepads.Count; i++)
			{
				connectedGamepads[i].Update();
			}
			for (int j = 0; j < activeGamepads.Count; j++)
			{
				Gamepad gamepad = activeGamepads[j];
				if (gamepad != null && !gamepad.IsBlocked())
				{
					gamepadsCombined.ApplyGamepadState(gamepad);
				}
			}
			gamepadsCombined.Update();
			InputRig activeRig = CF2Input.activeRig;
			if (activeRig != null)
			{
				ApplyToRig(activeRig);
			}
		}

		private void ApplyToRig(InputRig rig)
		{
			if (rig == null)
			{
				return;
			}
			for (int i = 0; i < activeGamepads.Count; i++)
			{
				Gamepad gamepad = activeGamepads[i];
				if (gamepad != null && !gamepad.IsBlocked() && rig.gamepads.Length > i)
				{
					rig.gamepads[i].SyncGamepad(gamepad, rig);
				}
			}
			rig.anyGamepad.SyncGamepad(gamepadsCombined, rig);
		}

		private void CheckJoystickConnections()
		{
			CheckUnactivatedGamepads();
			if (!((elapsedSinceLastConnectionCheck += Time.unscaledDeltaTime) < connectionCheckInterval))
			{
				ConnectJoysticks();
			}
		}

		private string GetConnectedDeviceName(int internalJoyId)
		{
			if (deviceConnections == null || internalJoyId < 0 || internalJoyId >= deviceConnections.Length || deviceConnections[internalJoyId] == null || deviceConnections[internalJoyId].Length == 0)
			{
				return string.Empty;
			}
			return deviceConnections[internalJoyId];
		}

		private Gamepad AddConnectedGamepad()
		{
			if (freeGamepads.Count <= 0)
			{
				return null;
			}
			Gamepad gamepad = freeGamepads[freeGamepads.Count - 1];
			freeGamepads.RemoveAt(freeGamepads.Count - 1);
			connectedGamepads.Add(gamepad);
			return gamepad;
		}

		private Gamepad FindDisconnectedGamepad(int internalJoyId, string deviceName)
		{
			Gamepad gamepad = null;
			Gamepad gamepad2 = null;
			for (int i = 0; i < connectedGamepads.Count; i++)
			{
				Gamepad gamepad3 = connectedGamepads[i];
				if (gamepad3.IsConnected())
				{
					continue;
				}
				if (gamepad == null)
				{
					gamepad = gamepad3;
				}
				if (gamepad3.GetInternalJoyName() == deviceName)
				{
					if (internalJoyId == gamepad3.GetInternalJoyId())
					{
						return gamepad3;
					}
					if (gamepad2 == null)
					{
						gamepad2 = gamepad3;
					}
				}
			}
			return (gamepad2 == null) ? gamepad : gamepad2;
		}

		private void ActivateGamepad(Gamepad g)
		{
			if (g == null)
			{
				return;
			}
			int num = -1;
			int slot = g.GetSlot();
			if (slot >= 0 && slot < activeGamepads.Count && activeGamepads[slot] == null)
			{
				activeGamepads[slot] = g;
				num = slot;
			}
			if (num < 0)
			{
				for (int i = 0; i < activeGamepads.Count; i++)
				{
					if (activeGamepads[i] == null)
					{
						activeGamepads[i] = g;
						num = i;
						break;
					}
				}
				if (num < 0)
				{
					num = activeGamepads.Count;
					activeGamepads.Add(g);
				}
			}
			if (num >= 0)
			{
				g.OnActivate(num);
				CountActiveGamepads();
				if (IsMain() && GamepadManager.onGamepadActivated != null)
				{
					GamepadManager.onGamepadActivated(g);
				}
			}
		}

		private void DisactivateGamepad(Gamepad g, DisconnectionReason reason)
		{
			if (g != null && g.IsActivated())
			{
				if (IsMain() && GamepadManager.onGamepadDisactivated != null)
				{
					GamepadManager.onGamepadDisactivated(g, reason);
				}
				g.OnDisactivate();
				int num = activeGamepads.IndexOf(g);
				if (num >= 0)
				{
					activeGamepads[num] = null;
					CountActiveGamepads();
				}
			}
		}

		private void CountActiveGamepads()
		{
			activeGamepadNum = 0;
			for (int i = 0; i < activeGamepads.Count; i++)
			{
				if (activeGamepads[i] != null)
				{
					activeGamepadNum++;
				}
			}
		}

		private void DisconnectGamepad(Gamepad g, DisconnectionReason reason)
		{
			if (g.IsActivated())
			{
				DisactivateGamepad(g, reason);
			}
			if (IsMain() && GamepadManager.onGamepadDisconnected != null)
			{
				GamepadManager.onGamepadDisconnected(g, reason);
			}
			g.OnDisconnect();
		}

		private void ConnectJoysticks()
		{
			elapsedSinceLastConnectionCheck = 0f;
			deviceConnections = Input.GetJoystickNames();
			bool flag = false;
			int num = 0;
			int num2 = 0;
			for (int i = 0; i < connectedGamepads.Count; i++)
			{
				Gamepad gamepad = connectedGamepads[i];
				if (!gamepad.IsConnected())
				{
					num2 |= 1 << i;
					continue;
				}
				string connectedDeviceName = GetConnectedDeviceName(gamepad.GetInternalJoyId());
				if (connectedDeviceName == gamepad.GetInternalJoyName())
				{
					num |= 1 << gamepad.GetInternalJoyId();
					continue;
				}
				flag = true;
				DisconnectGamepad(gamepad, DisconnectionReason.Disconnection);
			}
			for (int j = 0; j < 4; j++)
			{
				string connectedDeviceName2 = GetConnectedDeviceName(j);
				if (string.IsNullOrEmpty(connectedDeviceName2) || (num & (1 << j)) != 0)
				{
					continue;
				}
				Gamepad gamepad2 = FindDisconnectedGamepad(j, connectedDeviceName2);
				if (gamepad2 == null)
				{
					gamepad2 = AddConnectedGamepad();
				}
				if (gamepad2 != null)
				{
					flag = true;
					gamepad2.ConnectToJoy(j, connectedDeviceName2);
					GamepadProfile profileForDevice = GetProfileForDevice(connectedDeviceName2);
					if (profileForDevice != null)
					{
						gamepad2.SetProfile(profileForDevice);
					}
					if (IsMain() && GamepadManager.onGamepadConnected != null)
					{
						GamepadManager.onGamepadConnected(gamepad2);
					}
				}
			}
			if (flag && IsMain() && GamepadManager.onChange != null)
			{
				GamepadManager.onChange();
			}
		}

		private void CheckUnactivatedGamepads()
		{
			bool flag = false;
			for (int i = 0; i < connectedGamepads.Count; i++)
			{
				Gamepad gamepad = connectedGamepads[i];
				if (gamepad.IsConnected() && gamepad.IsSupported() && !gamepad.IsBlocked() && !gamepad.IsActivated() && gamepad.AnyInternalKeyOrAxisPressed())
				{
					flag = true;
					ActivateGamepad(gamepad);
				}
			}
			if (IsMain() && flag && GamepadManager.onChange != null)
			{
				GamepadManager.onChange();
			}
		}

		public int GetGamepadSlotCount()
		{
			return activeGamepads.Count;
		}

		public int GetActiveGamepadCount()
		{
			return activeGamepadNum;
		}

		public Gamepad GetGamepadAtSlot(int slot)
		{
			return (slot >= 0 && slot < activeGamepads.Count) ? activeGamepads[slot] : null;
		}

		public Gamepad GetCombinedGamepad()
		{
			return gamepadsCombined;
		}

		public int GetConnectedGamepadCount()
		{
			return connectedGamepads.Count;
		}

		public Gamepad GetConnectedGamepad(int index)
		{
			return (index >= 0 && index < connectedGamepads.Count) ? connectedGamepads[index] : null;
		}

		public void DisactivateGamepads()
		{
			for (int i = 0; i < activeGamepads.Count; i++)
			{
				Gamepad gamepad = activeGamepads[i];
				if (gamepad != null)
				{
					DisactivateGamepad(gamepad, DisconnectionReason.MassDisactivation);
				}
			}
			if (IsMain() && GamepadManager.onChange != null)
			{
				GamepadManager.onChange();
			}
		}

		private GamepadProfile GetProfileForDevice(string joyDevice)
		{
			GamepadProfile gamepadProfile = null;
			if (customProfileBank != null)
			{
				gamepadProfile = customProfileBank.GetProfile(joyDevice);
			}
			if (gamepadProfile == null)
			{
				gamepadProfile = BuiltInGamepadProfileBank.GetProfile(joyDevice);
			}
			if (gamepadProfile == null)
			{
				gamepadProfile = BuiltInGamepadProfileBank.GetGenericProfile();
			}
			return gamepadProfile;
		}
	}
}
