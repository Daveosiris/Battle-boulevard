using ControlFreak2.Internal;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.EventSystems;

namespace ControlFreak2
{
	public class InputRig : ComponentBase, IBindingContainer
	{
		public struct Touch
		{
			public TouchPhase phase;

			public float deltaTime;

			public int fingerId;

			public int tapCount;

			public Vector2 position;

			public Vector2 rawPosition;

			public Vector2 deltaPosition;

			public float altitudeAngle;

			public float azimuthAngle;

			public float maximumPossiblePressure;

			public float pressure;

			public float radius;

			public float radiusVariance;

			public TouchType type;

			public static Touch Dummy;

			public static Touch[] EmptyArray;

			private static Touch[] mTranslatedArray;

			static Touch()
			{
				Dummy = default(Touch);
				Dummy.phase = TouchPhase.Canceled;
				EmptyArray = new Touch[0];
			}

			public Touch(UnityEngine.Touch t)
			{
				phase = t.phase;
				deltaTime = t.deltaTime;
				fingerId = t.fingerId;
				tapCount = t.tapCount;
				position = t.position;
				rawPosition = t.rawPosition;
				deltaPosition = t.deltaPosition;
				altitudeAngle = 0f;
				azimuthAngle = 0f;
				maximumPossiblePressure = 1f;
				pressure = 1f;
				radius = 1f;
				radiusVariance = 0f;
				type = TouchType.Direct;
			}

			public static Touch[] TranslateUnityTouches(UnityEngine.Touch[] tarr)
			{
				if (tarr == null || tarr.Length == 0)
				{
					return EmptyArray;
				}
				if (mTranslatedArray == null || tarr.Length != mTranslatedArray.Length)
				{
					mTranslatedArray = new Touch[tarr.Length];
				}
				for (int i = 0; i < tarr.Length; i++)
				{
					mTranslatedArray[i] = new Touch(tarr[i]);
				}
				return mTranslatedArray;
			}
		}

		private class EmulatedTouchState
		{
			private int fingerId;

			public TouchGestureBasicState touch;

			public Touch outputTouch;

			private bool isUsed;

			private bool updatedThisFrame;

			public EmulatedTouchState()
			{
				Reset();
			}

			public EmulatedTouchState(int fingerId)
			{
				this.fingerId = fingerId;
				touch = new TouchGestureBasicState();
				outputTouch = default(Touch);
			}

			public bool IsUsed()
			{
				return isUsed;
			}

			public bool IsActive()
			{
				return touch.PressedRaw() || touch.JustReleasedRaw();
			}

			public void Start(Vector2 pos)
			{
				isUsed = true;
				updatedThisFrame = true;
				touch.OnTouchStart(pos, pos, 0f, controlledByMouse: false, isPressureSensitive: false, 1f);
			}

			public void UpdatePos(Vector2 pos)
			{
				updatedThisFrame = true;
				touch.OnTouchMove(pos);
			}

			public void EndTouch(bool cancel)
			{
				updatedThisFrame = true;
				isUsed = false;
				touch.OnTouchEnd(cancel);
			}

			public void Reset()
			{
				isUsed = false;
				touch.Reset();
				SyncOutputTouch();
			}

			private void SyncOutputTouch()
			{
				outputTouch.fingerId = fingerId;
				outputTouch.deltaTime = Time.unscaledDeltaTime;
				outputTouch.tapCount = 0;
				if (touch.JustPressedRaw())
				{
					outputTouch.phase = TouchPhase.Began;
				}
				else if (touch.JustReleasedRaw())
				{
					outputTouch.phase = TouchPhase.Ended;
				}
				else if (touch.PressedRaw())
				{
					outputTouch.phase = ((touch.GetDeltaVecRaw().sqrMagnitude > 0.001f) ? TouchPhase.Moved : TouchPhase.Stationary);
				}
				else
				{
					outputTouch.phase = TouchPhase.Canceled;
				}
				outputTouch.position = (outputTouch.rawPosition = touch.GetCurPosRaw());
				outputTouch.deltaPosition = touch.GetDeltaVecRaw();
			}

			public void Update()
			{
				if (isUsed && !updatedThisFrame)
				{
					EndTouch(cancel: false);
				}
				touch.Update();
				SyncOutputTouch();
				updatedThisFrame = false;
			}
		}

		[Serializable]
		public class RigSwitchCollection : NamedConfigCollection<RigSwitch>
		{
			private bool _changed;

			public bool changed
			{
				get
				{
					return _changed;
				}
				set
				{
					_changed = value;
				}
			}

			public RigSwitchCollection(InputRig rig)
				: base(rig, 4)
			{
			}

			public RigSwitch Add(string name, bool undoable)
			{
				RigSwitch rigSwitch = Get(name);
				if (rigSwitch != null)
				{
					return rigSwitch;
				}
				rigSwitch = new RigSwitch();
				rigSwitch.name = name;
				list.Add(rigSwitch);
				return rigSwitch;
			}

			public void SetSwitchState(string name, ref int cachedId, bool state)
			{
				RigSwitch rigSwitch = Get(name, ref cachedId);
				if (rigSwitch != null)
				{
					if (rigSwitch.GetState() != state)
					{
						changed = true;
					}
					rigSwitch.SetState(state);
				}
			}

			public void SetSwitchState(string name, bool state)
			{
				int cachedId = 0;
				SetSwitchState(name, ref cachedId, state);
			}

			public bool GetSwitchState(string name, ref int cachedId, bool fallbackValue)
			{
				return Get(name, ref cachedId)?.GetState() ?? fallbackValue;
			}

			public bool GetSwitchState(string name, bool fallbackValue)
			{
				return Get(name)?.GetState() ?? fallbackValue;
			}

			public bool ToggleSwitchState(string name, ref int cachedId, bool fallbackValue)
			{
				RigSwitch rigSwitch = Get(name, ref cachedId);
				if (rigSwitch == null)
				{
					return fallbackValue;
				}
				changed = true;
				return rigSwitch.ToggleState();
			}

			public bool ToggleSwitchState(string name, bool fallbackValue)
			{
				int cachedId = 0;
				return ToggleSwitchState(name, ref cachedId, fallbackValue);
			}

			public void SetAll(bool state)
			{
				for (int i = 0; i < list.Count; i++)
				{
					RigSwitch rigSwitch = list[i];
					if (rigSwitch.GetState() != state)
					{
						changed = true;
						rigSwitch.SetState(state);
					}
				}
			}

			public void ResetSwitch(string name, ref int cachedId)
			{
				RigSwitch rigSwitch = Get(name, ref cachedId);
				if (rigSwitch != null && rigSwitch.GetState() != rigSwitch.defaultState)
				{
					changed = true;
					rigSwitch.SetState(rigSwitch.defaultState);
				}
			}

			public void ResetSwitch(string name)
			{
				int cachedId = 0;
				ResetSwitch(name, ref cachedId);
			}

			public void Reset()
			{
				for (int i = 0; i < list.Count; i++)
				{
					RigSwitch rigSwitch = list[i];
					if (rigSwitch.GetState() != rigSwitch.defaultState)
					{
						changed = true;
						rigSwitch.SetState(rigSwitch.defaultState);
					}
				}
			}

			public int GetSwitchId(string name)
			{
				int cachedId = -1;
				RigSwitch rigSwitch = Get(name, ref cachedId);
				return (rigSwitch != null) ? cachedId : (-1);
			}
		}

		[Serializable]
		public class RigSwitch : NamedConfigElement
		{
			public bool defaultState;

			private bool state;

			public RigSwitch()
			{
			}

			public RigSwitch(string name)
			{
				base.name = name;
			}

			public void SetState(bool state)
			{
				this.state = state;
			}

			public bool GetState()
			{
				return state;
			}

			public bool ToggleState()
			{
				return state = !state;
			}

			public override void Reset()
			{
				state = defaultState;
			}
		}

		[Serializable]
		public class GamepadConfig : IBindingContainer
		{
			public bool enabled;

			public DigitalBinding digiFaceDownBinding;

			public DigitalBinding digiFaceRightBinding;

			public DigitalBinding digiFaceLeftBinding;

			public DigitalBinding digiFaceUpBinding;

			public DigitalBinding digiStartBinding;

			public DigitalBinding digiSelectBinding;

			public DigitalBinding digiL1Binding;

			public DigitalBinding digiR1Binding;

			public DigitalBinding digiL2Binding;

			public DigitalBinding digiR2Binding;

			public DigitalBinding digiL3Binding;

			public DigitalBinding digiR3Binding;

			public AxisBinding analogL2Binding;

			public AxisBinding analogR2Binding;

			public JoystickStateBinding leftStickStateBinding;

			public JoystickStateBinding rightStickStateBinding;

			public JoystickStateBinding dpadStateBinding;

			[NonSerialized]
			private JoystickState leftStickState;

			[NonSerialized]
			private JoystickState rigthStickState;

			[NonSerialized]
			private JoystickState dpadState;

			public GamepadConfig(InputRig rig)
			{
				BasicConstructor(rig);
			}

			private void BasicConstructor(InputRig rig)
			{
				enabled = false;
				digiFaceDownBinding = new DigitalBinding();
				digiFaceRightBinding = new DigitalBinding();
				digiFaceLeftBinding = new DigitalBinding();
				digiFaceUpBinding = new DigitalBinding();
				digiStartBinding = new DigitalBinding();
				digiSelectBinding = new DigitalBinding();
				digiL1Binding = new DigitalBinding();
				digiR1Binding = new DigitalBinding();
				digiL2Binding = new DigitalBinding();
				digiR2Binding = new DigitalBinding();
				digiL3Binding = new DigitalBinding();
				digiR3Binding = new DigitalBinding();
				analogL2Binding = new AxisBinding();
				analogR2Binding = new AxisBinding();
				leftStickStateBinding = new JoystickStateBinding();
				rightStickStateBinding = new JoystickStateBinding();
				dpadStateBinding = new JoystickStateBinding();
			}

			public void SyncGamepad(GamepadManager.Gamepad gamepad, InputRig rig)
			{
				if (enabled && gamepad != null)
				{
					gamepad.leftStick.SyncJoyState(leftStickStateBinding, rig);
					gamepad.rightStick.SyncJoyState(rightStickStateBinding, rig);
					gamepad.dpad.SyncJoyState(dpadStateBinding, rig);
					gamepad.keys[0].SyncDigital(digiFaceDownBinding, rig);
					gamepad.keys[1].SyncDigital(digiFaceRightBinding, rig);
					gamepad.keys[3].SyncDigital(digiFaceLeftBinding, rig);
					gamepad.keys[2].SyncDigital(digiFaceUpBinding, rig);
					gamepad.keys[4].SyncDigital(digiStartBinding, rig);
					gamepad.keys[5].SyncDigital(digiSelectBinding, rig);
					gamepad.keys[6].SyncDigital(digiL1Binding, rig);
					gamepad.keys[7].SyncDigital(digiR1Binding, rig);
					gamepad.keys[8].SyncDigital(digiL2Binding, rig);
					gamepad.keys[9].SyncDigital(digiR2Binding, rig);
					gamepad.keys[10].SyncDigital(digiL3Binding, rig);
					gamepad.keys[11].SyncDigital(digiR3Binding, rig);
					gamepad.keys[8].SyncAnalog(analogL2Binding, rig);
					gamepad.keys[9].SyncAnalog(analogR2Binding, rig);
				}
			}

			public void GetSubBindingDescriptions(BindingDescriptionList descList, UnityEngine.Object undoObject, string parentMenuPath)
			{
				descList.Add(digiFaceDownBinding, "Bottom Face Button", parentMenuPath, undoObject);
				descList.Add(digiFaceRightBinding, "Right Face Button", parentMenuPath, undoObject);
				descList.Add(digiFaceUpBinding, "Top Face Button", parentMenuPath, undoObject);
				descList.Add(digiFaceLeftBinding, "Left Face Button", parentMenuPath, undoObject);
				descList.Add(digiL1Binding, "L1", parentMenuPath, undoObject);
				descList.Add(digiR1Binding, "R1", parentMenuPath, undoObject);
				descList.Add(digiL2Binding, "L2 (Digital)", parentMenuPath, undoObject);
				descList.Add(digiR2Binding, "R2 (Digital)", parentMenuPath, undoObject);
				descList.Add(analogL2Binding, "L2 (Analog)", parentMenuPath, undoObject);
				descList.Add(analogR2Binding, "R2 (Analog)", parentMenuPath, undoObject);
				descList.Add(digiL3Binding, "L3", parentMenuPath, undoObject);
				descList.Add(digiR3Binding, "R3", parentMenuPath, undoObject);
				descList.Add(digiStartBinding, "Start", parentMenuPath, undoObject);
				descList.Add(digiSelectBinding, "Select (Back)", parentMenuPath, undoObject);
				descList.Add(dpadStateBinding, "D-Pad State", parentMenuPath, undoObject);
				descList.Add(leftStickStateBinding, "Left Stick State", parentMenuPath, undoObject);
				descList.Add(rightStickStateBinding, "Right Stick State", parentMenuPath, undoObject);
			}

			public bool IsBoundToKey(KeyCode key, InputRig rig)
			{
				return digiFaceDownBinding.IsBoundToKey(key, rig) || digiFaceRightBinding.IsBoundToKey(key, rig) || digiFaceUpBinding.IsBoundToKey(key, rig) || digiFaceLeftBinding.IsBoundToKey(key, rig) || digiL1Binding.IsBoundToKey(key, rig) || digiR1Binding.IsBoundToKey(key, rig) || digiL2Binding.IsBoundToKey(key, rig) || digiR2Binding.IsBoundToKey(key, rig) || analogL2Binding.IsBoundToKey(key, rig) || analogR2Binding.IsBoundToKey(key, rig) || digiL3Binding.IsBoundToKey(key, rig) || digiR3Binding.IsBoundToKey(key, rig) || digiStartBinding.IsBoundToKey(key, rig) || digiSelectBinding.IsBoundToKey(key, rig) || dpadStateBinding.IsBoundToKey(key, rig) || leftStickStateBinding.IsBoundToKey(key, rig) || rightStickStateBinding.IsBoundToKey(key, rig);
			}

			public bool IsBoundToAxis(string axisName, InputRig rig)
			{
				return digiFaceDownBinding.IsBoundToAxis(axisName, rig) || digiFaceRightBinding.IsBoundToAxis(axisName, rig) || digiFaceUpBinding.IsBoundToAxis(axisName, rig) || digiFaceLeftBinding.IsBoundToAxis(axisName, rig) || digiL1Binding.IsBoundToAxis(axisName, rig) || digiR1Binding.IsBoundToAxis(axisName, rig) || digiL2Binding.IsBoundToAxis(axisName, rig) || digiR2Binding.IsBoundToAxis(axisName, rig) || analogL2Binding.IsBoundToAxis(axisName, rig) || analogR2Binding.IsBoundToAxis(axisName, rig) || digiL3Binding.IsBoundToAxis(axisName, rig) || digiR3Binding.IsBoundToAxis(axisName, rig) || digiStartBinding.IsBoundToAxis(axisName, rig) || digiSelectBinding.IsBoundToAxis(axisName, rig) || dpadStateBinding.IsBoundToAxis(axisName, rig) || leftStickStateBinding.IsBoundToAxis(axisName, rig) || rightStickStateBinding.IsBoundToAxis(axisName, rig);
			}

			public bool IsEmulatingTouches()
			{
				return digiFaceDownBinding.IsEmulatingTouches() || digiFaceRightBinding.IsEmulatingTouches() || digiFaceUpBinding.IsEmulatingTouches() || digiFaceLeftBinding.IsEmulatingTouches() || digiL1Binding.IsEmulatingTouches() || digiR1Binding.IsEmulatingTouches() || digiL2Binding.IsEmulatingTouches() || digiR2Binding.IsEmulatingTouches() || analogL2Binding.IsEmulatingTouches() || analogR2Binding.IsEmulatingTouches() || digiL3Binding.IsEmulatingTouches() || digiR3Binding.IsEmulatingTouches() || digiStartBinding.IsEmulatingTouches() || digiSelectBinding.IsEmulatingTouches() || dpadStateBinding.IsEmulatingTouches() || leftStickStateBinding.IsEmulatingTouches() || rightStickStateBinding.IsEmulatingTouches();
			}

			public bool IsEmulatingMousePosition()
			{
				return digiFaceDownBinding.IsEmulatingMousePosition() || digiFaceRightBinding.IsEmulatingMousePosition() || digiFaceUpBinding.IsEmulatingMousePosition() || digiFaceLeftBinding.IsEmulatingMousePosition() || digiL1Binding.IsEmulatingMousePosition() || digiR1Binding.IsEmulatingMousePosition() || digiL2Binding.IsEmulatingMousePosition() || digiR2Binding.IsEmulatingMousePosition() || analogL2Binding.IsEmulatingMousePosition() || analogR2Binding.IsEmulatingMousePosition() || digiL3Binding.IsEmulatingMousePosition() || digiR3Binding.IsEmulatingMousePosition() || digiStartBinding.IsEmulatingMousePosition() || digiSelectBinding.IsEmulatingMousePosition() || dpadStateBinding.IsEmulatingMousePosition() || leftStickStateBinding.IsEmulatingMousePosition() || rightStickStateBinding.IsEmulatingMousePosition();
			}
		}

		[Serializable]
		public class VirtualJoystickConfig : NamedConfigElement, IBindingContainer
		{
			public bool disableOnMobile;

			public JoystickConfig joystickConfig;

			public JoystickState joystickState;

			public KeyCode keyboardUp;

			public KeyCode keyboardRight;

			public KeyCode keyboardDown;

			public KeyCode keyboardLeft;

			public JoystickStateBinding joyStateBinding;

			public VirtualJoystickConfig()
			{
				BasicConstructor();
			}

			public VirtualJoystickConfig(string targetName, KeyCode keyUp, KeyCode keyRight, KeyCode keyDown, KeyCode keyLeft)
			{
				BasicConstructor();
				name = targetName;
				keyboardUp = keyUp;
				keyboardRight = keyRight;
				keyboardDown = keyDown;
				keyboardLeft = keyLeft;
			}

			private void BasicConstructor()
			{
				name = "Joystick";
				keyboardUp = KeyCode.W;
				keyboardRight = KeyCode.D;
				keyboardDown = KeyCode.S;
				keyboardLeft = KeyCode.A;
				if (joystickConfig == null)
				{
					joystickConfig = new JoystickConfig();
				}
				joystickState = new JoystickState(joystickConfig);
				joyStateBinding = new JoystickStateBinding();
			}

			public override void Reset()
			{
				joystickState.Reset();
			}

			public override void Update(InputRig rig)
			{
				joystickState.ApplyDigital(GetSourceKeyState(keyboardUp), GetSourceKeyState(keyboardRight), GetSourceKeyState(keyboardDown), GetSourceKeyState(keyboardLeft));
				joystickState.Update();
				joyStateBinding.SyncJoyState(joystickState, rig);
			}

			public void SetState(JoystickState state)
			{
				if (joystickState != null)
				{
					joystickState.ApplyState(state);
				}
			}

			public bool IsBoundToAxis(string axisName, InputRig rig)
			{
				return joyStateBinding.IsBoundToAxis(axisName, rig);
			}

			public bool IsBoundToKey(KeyCode key, InputRig rig)
			{
				return joyStateBinding.IsBoundToKey(key, rig);
			}

			public bool IsEmulatingTouches()
			{
				return false;
			}

			public bool IsEmulatingMousePosition()
			{
				return false;
			}

			public void GetSubBindingDescriptions(BindingDescriptionList descList, UnityEngine.Object undoObject, string parentMenuPath)
			{
				descList.Add(joyStateBinding, "Joystick State", parentMenuPath, undoObject);
			}
		}

		[Serializable]
		public class VirtualJoystickConfigCollection : NamedConfigCollection<VirtualJoystickConfig>
		{
			public VirtualJoystickConfigCollection(InputRig rig, int capacity)
				: base(rig, capacity)
			{
			}

			public VirtualJoystickConfig Add(string name, bool undoable)
			{
				VirtualJoystickConfig virtualJoystickConfig = Get(name);
				if (virtualJoystickConfig != null)
				{
					return virtualJoystickConfig;
				}
				virtualJoystickConfig = new VirtualJoystickConfig();
				virtualJoystickConfig.name = name;
				list.Add(virtualJoystickConfig);
				return virtualJoystickConfig;
			}
		}

		public enum AxisType
		{
			UnsignedAnalog,
			SignedAnalog,
			Digital,
			ScrollWheel,
			Delta
		}

		public enum InputSource
		{
			Digital,
			Analog,
			MouseDelta,
			TouchDelta,
			NormalizedDelta,
			Scroll
		}

		public enum DeltaTransformMode
		{
			Universal,
			EmulateMouse,
			Raw
		}

		[Serializable]
		public class AxisConfig : NamedConfigElement
		{
			public AxisType axisType;

			public DeltaTransformMode deltaMode;

			public bool snap;

			public float scale;

			public bool affectSourceKeys;

			public KeyCode affectedKeyPositive;

			public KeyCode affectedKeyNegative;

			public KeyCode keyboardPositive;

			public KeyCode keyboardPositiveAlt0;

			public KeyCode keyboardPositiveAlt1;

			public KeyCode keyboardPositiveAlt2;

			public KeyCode keyboardNegative;

			public KeyCode keyboardNegativeAlt0;

			public KeyCode keyboardNegativeAlt1;

			public KeyCode keyboardNegativeAlt2;

			public float analogToDigitalThresh;

			public float rawSmoothingTime;

			public float smoothingTime;

			public float digitalToAnalogAccelTime;

			public float digitalToAnalogDecelTime;

			public bool digitalToScrollAutoRepeat;

			public float digitalToScrollRepeatInterval;

			public float digitalToScrollDelay;

			public float scrollToAnalogSmoothingTime;

			public float scrollToAnalogStepDuration;

			public float scrollToDeltaSmoothingTime;

			private float valRaw;

			private float val;

			private int scrollPrev;

			private int scrollCur;

			private bool digiCur;

			private bool digiPrev;

			private bool muteUntilRelease;

			private float digitalToAnalogVal;

			private bool analogToDigitalNegCur;

			private bool analogToDigitalPosCur;

			private float valSmoothVel;

			private float valRawSmoothVel;

			private float scrollToAnalogTimeRemaining;

			private float scrollToAnalogValue;

			private int scrollToDigitalClicksRemaining;

			private bool scrollToDigitalClickOn;

			private float deltaAccumTarget;

			private float deltaAccumSmoothCur;

			private float deltaAccumSmoothPrev;

			private float deltaAccumRawCur;

			private float deltaAccumRawPrev;

			private float scrollElapsedSinceChange;

			private bool digiToScrollPositivePrev;

			private bool digiToScrollNegativePrev;

			private bool digiToScrollDelayPhase;

			public bool frDigitalPos
			{
				get;
				protected set;
			}

			public bool frDigitalNeg
			{
				get;
				protected set;
			}

			public float frAnalogPos
			{
				get;
				protected set;
			}

			public float frAnalogNeg
			{
				get;
				protected set;
			}

			public float frMouseDeltaPos
			{
				get;
				protected set;
			}

			public float frMouseDeltaNeg
			{
				get;
				protected set;
			}

			public float frTouchDeltaPos
			{
				get;
				protected set;
			}

			public float frTouchDeltaNeg
			{
				get;
				protected set;
			}

			public float frNormalizedDeltaPos
			{
				get;
				protected set;
			}

			public float frNormalizedDeltaNeg
			{
				get;
				protected set;
			}

			public int frScrollDelta
			{
				get;
				protected set;
			}

			public AxisConfig()
			{
				name = "Axis";
				axisType = AxisType.Digital;
				deltaMode = DeltaTransformMode.EmulateMouse;
				keyboardPositive = KeyCode.None;
				keyboardNegative = KeyCode.None;
				rawSmoothingTime = 0f;
				smoothingTime = 0f;
				analogToDigitalThresh = 0.125f;
				digitalToAnalogAccelTime = 0.25f;
				digitalToAnalogDecelTime = 0.25f;
				scrollToAnalogSmoothingTime = 0.1f;
				scale = 1f;
				affectSourceKeys = true;
				digitalToScrollRepeatInterval = 0.5f;
				digitalToScrollDelay = 1f;
				digitalToScrollAutoRepeat = true;
				scrollToAnalogStepDuration = 0.1f;
				scrollToAnalogSmoothingTime = 0.05f;
				scrollToDeltaSmoothingTime = 0.1f;
				Reset();
			}

			public static AxisConfig CreateDigital(string name, KeyCode key)
			{
				AxisConfig axisConfig = new AxisConfig();
				axisConfig.name = name;
				axisConfig.axisType = AxisType.Digital;
				axisConfig.keyboardPositive = key;
				axisConfig.keyboardNegative = KeyCode.None;
				return axisConfig;
			}

			public static AxisConfig CreateScrollWheel(string name, KeyCode keyPositive, KeyCode keyNegative)
			{
				AxisConfig axisConfig = new AxisConfig();
				axisConfig.name = name;
				axisConfig.axisType = AxisType.ScrollWheel;
				axisConfig.keyboardPositive = keyPositive;
				axisConfig.keyboardNegative = keyNegative;
				return axisConfig;
			}

			public static AxisConfig CreateAnalog(string name, KeyCode keyPositive)
			{
				AxisConfig axisConfig = new AxisConfig();
				axisConfig.name = name;
				axisConfig.axisType = AxisType.UnsignedAnalog;
				axisConfig.keyboardPositive = keyPositive;
				axisConfig.keyboardNegative = KeyCode.None;
				return axisConfig;
			}

			public static AxisConfig CreateSignedAnalog(string name, KeyCode keyPositive, KeyCode keyNegative)
			{
				AxisConfig axisConfig = new AxisConfig();
				axisConfig.name = name;
				axisConfig.axisType = AxisType.SignedAnalog;
				axisConfig.keyboardPositive = keyPositive;
				axisConfig.keyboardNegative = keyNegative;
				return axisConfig;
			}

			public static AxisConfig CreateDelta(string name, KeyCode keyPositive, KeyCode keyNegative)
			{
				AxisConfig axisConfig = new AxisConfig();
				axisConfig.name = name;
				axisConfig.axisType = AxisType.Delta;
				axisConfig.keyboardPositive = keyPositive;
				axisConfig.keyboardNegative = keyNegative;
				return axisConfig;
			}

			public override void Reset()
			{
				val = 0f;
				valRaw = 0f;
				valSmoothVel = 0f;
				valRawSmoothVel = 0f;
				frAnalogPos = 0f;
				frAnalogNeg = 0f;
				frDigitalNeg = false;
				frDigitalPos = false;
				frMouseDeltaPos = 0f;
				frMouseDeltaNeg = 0f;
				frTouchDeltaPos = 0f;
				frTouchDeltaNeg = 0f;
				frNormalizedDeltaPos = 0f;
				frNormalizedDeltaNeg = 0f;
				deltaAccumTarget = 0f;
				deltaAccumSmoothCur = 0f;
				deltaAccumSmoothPrev = 0f;
				deltaAccumRawCur = 0f;
				deltaAccumRawPrev = 0f;
				digiToScrollPositivePrev = false;
				digiToScrollNegativePrev = false;
				digiToScrollDelayPhase = true;
				scrollElapsedSinceChange = 0f;
			}

			public void MuteUntilRelease()
			{
				muteUntilRelease = true;
			}

			public bool IsMuted()
			{
				return muteUntilRelease;
			}

			public void ApplyKeyboardInput()
			{
				if (GetSourceKeyState(keyboardPositive) || GetSourceKeyState(keyboardPositiveAlt0) || GetSourceKeyState(keyboardPositiveAlt1) || GetSourceKeyState(keyboardPositiveAlt2))
				{
					SetSignedDigital(vpositive: true);
				}
				if (GetSourceKeyState(keyboardNegative) || GetSourceKeyState(keyboardNegativeAlt0) || GetSourceKeyState(keyboardNegativeAlt1) || GetSourceKeyState(keyboardNegativeAlt2))
				{
					SetSignedDigital(vpositive: false);
				}
			}

			public override void Update(InputRig rig)
			{
				scrollPrev = scrollCur;
				digiPrev = digiCur;
				analogToDigitalNegCur = (frAnalogPos + frAnalogNeg < 0f - analogToDigitalThresh);
				analogToDigitalPosCur = (frAnalogPos + frAnalogNeg > analogToDigitalThresh);
				float num = (frDigitalNeg ? (-1) : 0) + (frDigitalPos ? 1 : 0);
				float secondsPerUnit = (!frDigitalNeg && !frDigitalPos) ? digitalToAnalogDecelTime : digitalToAnalogAccelTime;
				if (snap && Mathf.Abs(num) > 0.1f && num >= 0f != digitalToAnalogVal >= 0f)
				{
					digitalToAnalogVal = 0f;
				}
				digitalToAnalogVal = CFUtils.MoveTowards(digitalToAnalogVal, num, secondsPerUnit, CFUtils.realDeltaTime, 0.01f);
				float value = CFUtils.ApplyDeltaInput(frAnalogNeg + frAnalogPos, digitalToAnalogVal);
				value = Mathf.Clamp(value, -1f, 1f);
				switch (axisType)
				{
				case AxisType.Digital:
					scrollToDigitalClicksRemaining += frScrollDelta;
					if (scrollToDigitalClickOn)
					{
						scrollToDigitalClickOn = false;
					}
					else if (scrollToDigitalClicksRemaining != 0)
					{
						scrollToDigitalClickOn = true;
						if (scrollToDigitalClicksRemaining > 0)
						{
							scrollToDigitalClicksRemaining--;
							frDigitalPos = true;
						}
						else
						{
							scrollToDigitalClicksRemaining++;
							frDigitalNeg = true;
						}
					}
					digiCur = (frDigitalNeg || frDigitalPos || analogToDigitalNegCur || analogToDigitalPosCur || scrollToDigitalClickOn);
					val = (valRaw = (digiCur ? 1 : 0));
					break;
				case AxisType.UnsignedAnalog:
				case AxisType.SignedAnalog:
					if (scrollToAnalogTimeRemaining != 0f)
					{
						scrollToAnalogTimeRemaining = Mathf.MoveTowards(scrollToAnalogTimeRemaining, 0f, CFUtils.realDeltaTimeClamped);
					}
					if (frScrollDelta != 0)
					{
						scrollToAnalogTimeRemaining += (float)frScrollDelta * scrollToAnalogStepDuration;
					}
					if (scrollToAnalogTimeRemaining != 0f || scrollToAnalogValue != 0f)
					{
						scrollToAnalogValue = CFUtils.SmoothTowards(scrollToAnalogValue, (scrollToAnalogTimeRemaining != 0f) ? ((!(scrollToAnalogTimeRemaining < 0f)) ? 1 : (-1)) : 0, scrollToAnalogSmoothingTime, CFUtils.realDeltaTimeClamped, 0.001f);
						value = CFUtils.ApplyDeltaInput(value, scrollToAnalogValue);
					}
					digiCur = (frDigitalNeg || frDigitalPos || analogToDigitalNegCur || analogToDigitalPosCur || scrollToAnalogTimeRemaining != 0f);
					if (axisType == AxisType.UnsignedAnalog && value < 0f)
					{
						value = 0f;
					}
					valRaw = CFUtils.SmoothDamp(valRaw, value, ref valRawSmoothVel, rawSmoothingTime, CFUtils.realDeltaTime, 0.001f);
					val = CFUtils.SmoothDamp(val, value, ref valSmoothVel, smoothingTime, CFUtils.realDeltaTime, 0.001f);
					break;
				case AxisType.ScrollWheel:
				{
					bool flag = frDigitalPos || analogToDigitalPosCur;
					bool flag2 = frDigitalNeg || analogToDigitalNegCur;
					int num2 = (flag ? 1 : 0) + (flag2 ? (-1) : 0);
					if (frScrollDelta != 0 || flag != digiToScrollPositivePrev || flag2 != digiToScrollNegativePrev)
					{
						digiToScrollDelayPhase = true;
						scrollElapsedSinceChange = 0f;
						if (num2 != 0 && !digiToScrollNegativePrev && !digiToScrollPositivePrev)
						{
							frScrollDelta = CFUtils.ApplyDeltaInputInt(frScrollDelta, num2);
						}
					}
					else if (digitalToScrollAutoRepeat && num2 != 0)
					{
						scrollElapsedSinceChange += CFUtils.realDeltaTimeClamped;
						if (scrollElapsedSinceChange > ((!digiToScrollDelayPhase) ? digitalToScrollRepeatInterval : digitalToScrollDelay))
						{
							digiToScrollDelayPhase = false;
							scrollElapsedSinceChange = 0f;
							frScrollDelta = CFUtils.ApplyDeltaInputInt(frScrollDelta, num2);
						}
					}
					digiToScrollPositivePrev = flag;
					digiToScrollNegativePrev = flag2;
					scrollCur += frScrollDelta;
					val = (valRaw = scrollCur - scrollPrev);
					digiCur = (frScrollDelta != 0);
					break;
				}
				case AxisType.Delta:
				{
					float accum = CFUtils.ApplyDeltaInput(rig.TransformTouchPixelDelta(frTouchDeltaPos + frTouchDeltaNeg, deltaMode), rig.TransformMousePointDelta(frMouseDeltaPos + frMouseDeltaNeg, deltaMode));
					accum = CFUtils.ApplyDeltaInput(accum, rig.TransformNormalizedDelta(frNormalizedDeltaPos + frNormalizedDeltaNeg, deltaMode));
					accum = CFUtils.ApplyDeltaInput(accum, rig.TransformScrollDelta(frScrollDelta, deltaMode));
					for (int i = 0; i < rig.fixedTimeStep.GetFrameSteps(); i++)
					{
						accum = CFUtils.ApplyDeltaInput(accum, rig.TransformAnalogDelta(value * rig.fixedTimeStep.GetDeltaTime(), deltaMode));
					}
					deltaAccumRawPrev = deltaAccumRawCur;
					deltaAccumTarget += accum;
					deltaAccumSmoothPrev = deltaAccumSmoothCur;
					deltaAccumSmoothCur = CFUtils.SmoothDamp(deltaAccumSmoothCur, deltaAccumTarget, ref valSmoothVel, smoothingTime, CFUtils.realDeltaTime, 1E-05f);
					deltaAccumRawPrev = deltaAccumRawCur;
					deltaAccumRawCur = CFUtils.SmoothDamp(deltaAccumRawCur, deltaAccumTarget, ref valRawSmoothVel, rawSmoothingTime, CFUtils.realDeltaTime, 1E-05f);
					val = deltaAccumSmoothCur - deltaAccumSmoothPrev;
					valRaw = deltaAccumRawCur - deltaAccumRawPrev;
					break;
				}
				}
				if (muteUntilRelease)
				{
					val = 0f;
					valRaw = 0f;
					valRawSmoothVel = 0f;
					valSmoothVel = 0f;
					digiCur = false;
					digiPrev = false;
					digitalToAnalogVal = 0f;
					if (!frDigitalNeg && !frDigitalPos && !analogToDigitalNegCur && !analogToDigitalPosCur && frScrollDelta == 0)
					{
						muteUntilRelease = false;
					}
					if (axisType == AxisType.Delta)
					{
						muteUntilRelease = false;
					}
				}
				if (analogToDigitalNegCur || frDigitalNeg)
				{
					if (affectSourceKeys)
					{
						rig.SetKeyCode(affectedKeyNegative);
					}
					else
					{
						rig.SetKeyCode(keyboardNegative);
						rig.SetKeyCode(keyboardNegativeAlt0);
						rig.SetKeyCode(keyboardNegativeAlt1);
						rig.SetKeyCode(keyboardNegativeAlt2);
					}
				}
				if (analogToDigitalPosCur || frDigitalPos)
				{
					if (affectSourceKeys)
					{
						rig.SetKeyCode(affectedKeyPositive);
					}
					else
					{
						rig.SetKeyCode(keyboardPositive);
						rig.SetKeyCode(keyboardPositiveAlt0);
						rig.SetKeyCode(keyboardPositiveAlt1);
						rig.SetKeyCode(keyboardPositiveAlt2);
					}
				}
				frAnalogPos = 0f;
				frAnalogNeg = 0f;
				frMouseDeltaPos = 0f;
				frMouseDeltaNeg = 0f;
				frTouchDeltaPos = 0f;
				frTouchDeltaNeg = 0f;
				frNormalizedDeltaPos = 0f;
				frNormalizedDeltaNeg = 0f;
				frScrollDelta = 0;
				frDigitalPos = false;
				frDigitalNeg = false;
			}

			public void Set(float v, InputSource source)
			{
				switch (source)
				{
				case InputSource.Digital:
					if (Mathf.Abs(v) > 0.5f)
					{
						SetSignedDigital(v >= 0f);
					}
					break;
				case InputSource.Analog:
					SetAnalog(v);
					break;
				case InputSource.MouseDelta:
					SetMouseDelta(v);
					break;
				case InputSource.TouchDelta:
					SetTouchDelta(v);
					break;
				case InputSource.NormalizedDelta:
					SetNormalizedDelta(v);
					break;
				case InputSource.Scroll:
					SetScrollDelta(Mathf.RoundToInt(v));
					break;
				}
			}

			public void SetAnalog(float v)
			{
				frAnalogPos = CFUtils.ApplyPositveDeltaInput(frAnalogPos, v);
				frAnalogNeg = CFUtils.ApplyNegativeDeltaInput(frAnalogNeg, v);
			}

			public void SetDigital()
			{
				frDigitalPos = true;
			}

			public void SetSignedDigital(bool vpositive)
			{
				if (vpositive)
				{
					frDigitalPos = true;
				}
				else
				{
					frDigitalNeg = true;
				}
			}

			public void SetScrollDelta(int scrollDelta)
			{
				frScrollDelta = CFUtils.ApplyDeltaInputInt(frScrollDelta, scrollDelta);
			}

			public void SetTouchDelta(float touchDelta)
			{
				frTouchDeltaPos = CFUtils.ApplyPositveDeltaInput(frTouchDeltaPos, touchDelta);
				frTouchDeltaNeg = CFUtils.ApplyNegativeDeltaInput(frTouchDeltaNeg, touchDelta);
			}

			public void SetMouseDelta(float mouseDelta)
			{
				frMouseDeltaPos = CFUtils.ApplyPositveDeltaInput(frMouseDeltaPos, mouseDelta);
				frMouseDeltaNeg = CFUtils.ApplyNegativeDeltaInput(frMouseDeltaNeg, mouseDelta);
			}

			public void SetNormalizedDelta(float normalizedDelta)
			{
				frNormalizedDeltaPos = CFUtils.ApplyPositveDeltaInput(frNormalizedDeltaPos, normalizedDelta);
				frNormalizedDeltaNeg = CFUtils.ApplyNegativeDeltaInput(frNormalizedDeltaNeg, normalizedDelta);
			}

			public float GetAnalog()
			{
				if (axisType != AxisType.ScrollWheel)
				{
					return val * scale;
				}
				return val;
			}

			public float GetAnalogRaw()
			{
				if (axisType != AxisType.ScrollWheel)
				{
					return valRaw * scale;
				}
				return valRaw;
			}

			public bool IsControlledByInput()
			{
				return frDigitalNeg || frDigitalPos || Mathf.Abs(frAnalogNeg) > 0.001f || Mathf.Abs(frAnalogPos) > 0.001f || Mathf.Abs(frMouseDeltaNeg) > 0.001f || Mathf.Abs(frMouseDeltaPos) > 0.001f || Mathf.Abs(frNormalizedDeltaNeg) > 0.001f || Mathf.Abs(frNormalizedDeltaPos) > 0.001f || Mathf.Abs(frTouchDeltaNeg) > 0.001f || Mathf.Abs(frTouchDeltaPos) > 0.001f || frScrollDelta != 0;
			}

			public bool GetButton()
			{
				return digiCur;
			}

			public bool GetButtonDown()
			{
				return !digiPrev && digiCur;
			}

			public bool GetButtonUp()
			{
				return digiPrev && !digiCur;
			}

			public int GetSupportedInputSourceMask()
			{
				int result = 0;
				switch (axisType)
				{
				case AxisType.Digital:
					result = 35;
					break;
				case AxisType.UnsignedAnalog:
				case AxisType.SignedAnalog:
					result = 35;
					break;
				case AxisType.ScrollWheel:
					result = 35;
					break;
				case AxisType.Delta:
					result = 63;
					break;
				}
				return result;
			}

			public bool DoesSupportInputSource(InputSource source)
			{
				return (GetSupportedInputSourceMask() & (1 << (int)source)) != 0;
			}

			private bool IsSignedAxis()
			{
				return axisType == AxisType.Delta || axisType == AxisType.ScrollWheel || axisType == AxisType.SignedAnalog;
			}

			public bool DoesAffectKeyCode(KeyCode key)
			{
				if (!affectSourceKeys)
				{
					if (affectedKeyPositive == key || (IsSignedAxis() && affectedKeyNegative == key))
					{
						return true;
					}
				}
				else
				{
					if (key == keyboardPositive || key == keyboardPositiveAlt0 || key == keyboardPositiveAlt1 || key == keyboardPositiveAlt2)
					{
						return true;
					}
					if (IsSignedAxis() && (key == keyboardNegative || key == keyboardNegativeAlt0 || key == keyboardNegativeAlt1 || key == keyboardNegativeAlt2))
					{
						return true;
					}
				}
				return false;
			}
		}

		[Serializable]
		public class AxisConfigCollection : NamedConfigCollection<AxisConfig>
		{
			public AxisConfigCollection(InputRig rig, int capacity)
				: base(rig, capacity)
			{
			}

			public AxisConfig Add(string name, AxisType axisType, bool undoable)
			{
				AxisConfig axisConfig = Get(name);
				if (axisConfig != null)
				{
					return axisConfig;
				}
				axisConfig = new AxisConfig();
				axisConfig.name = name;
				axisConfig.axisType = axisType;
				list.Add(axisConfig);
				return axisConfig;
			}

			public void ApplyKeyboardInput()
			{
				for (int i = 0; i < list.Count; i++)
				{
					list[i].ApplyKeyboardInput();
				}
			}

			public void MuteAllUntilRelease()
			{
				for (int i = 0; i < list.Count; i++)
				{
					list[i].MuteUntilRelease();
				}
			}
		}

		[Serializable]
		public class AutomaticInputConfig : NamedConfigElement
		{
			[Serializable]
			public class RelatedAxis
			{
				public string axisName;

				public bool mustBeControlledByInput;

				private int axisId;

				public RelatedAxis()
				{
					axisName = string.Empty;
				}

				public bool IsEnabling(InputRig rig)
				{
					AxisConfig axisConfig = rig.GetAxisConfig(axisName, ref axisId);
					if (axisConfig == null)
					{
						return true;
					}
					return axisConfig.IsControlledByInput() == mustBeControlledByInput;
				}
			}

			[Serializable]
			public class RelatedKey
			{
				public KeyCode key;

				public bool mustBeControlledByInput;

				public bool IsEnabling(InputRig rig)
				{
					return key == KeyCode.None || rig.GetKey(key) == mustBeControlledByInput;
				}
			}

			public DigitalBinding targetBinding;

			public DisablingConditionSet disablingConditions;

			[NonSerialized]
			private bool disabledByConditions;

			public List<RelatedAxis> relatedAxisList;

			public List<RelatedKey> relatedKeyList;

			public AutomaticInputConfig()
			{
				targetBinding = new DigitalBinding();
				disabledByConditions = false;
				relatedAxisList = new List<RelatedAxis>();
				relatedKeyList = new List<RelatedKey>();
				disablingConditions = new DisablingConditionSet(null);
				disablingConditions.mobileModeRelation = DisablingConditionSet.MobileModeRelation.EnabledOnlyInMobileMode;
			}

			public void SetRig(InputRig rig)
			{
				disablingConditions.SetRig(rig);
				OnDisablingConditionsChange();
			}

			public void OnDisablingConditionsChange()
			{
				disabledByConditions = disablingConditions.IsInEffect();
			}

			public override void Update(InputRig rig)
			{
				if (disabledByConditions)
				{
					return;
				}
				for (int i = 0; i < relatedKeyList.Count; i++)
				{
					if (!relatedKeyList[i].IsEnabling(rig))
					{
						return;
					}
				}
				for (int j = 0; j < relatedAxisList.Count; j++)
				{
					if (!relatedAxisList[j].IsEnabling(rig))
					{
						return;
					}
				}
				targetBinding.Sync(state: true, rig, skipIfTargetIsMuted: true);
			}
		}

		[Serializable]
		public class AutomaticInputConfigCollection : NamedConfigCollection<AutomaticInputConfig>
		{
			public AutomaticInputConfigCollection(InputRig rig)
				: base(rig, 0)
			{
			}

			public void SetRig(InputRig rig)
			{
				for (int i = 0; i < list.Count; i++)
				{
					list[i].SetRig(rig);
				}
			}

			public AutomaticInputConfig Add(string name, bool createUndo)
			{
				AutomaticInputConfig automaticInputConfig = new AutomaticInputConfig();
				automaticInputConfig.name = name;
				automaticInputConfig.SetRig(rig);
				list.Add(automaticInputConfig);
				return automaticInputConfig;
			}

			public void OnDisablingConditionsChange()
			{
				for (int i = 0; i < list.Count; i++)
				{
					list[i].OnDisablingConditionsChange();
				}
			}
		}

		public class NamedConfigElement
		{
			public string name;

			public virtual void Reset()
			{
			}

			public virtual void Update(InputRig rig)
			{
			}
		}

		[Serializable]
		public class NamedConfigCollection<T> where T : NamedConfigElement, new()
		{
			public List<T> list;

			[NonSerialized]
			protected InputRig rig;

			public NamedConfigCollection(InputRig rig, int capacity)
			{
				this.rig = rig;
				list = new List<T>(capacity);
			}

			public void ResetAll()
			{
				int count = list.Count;
				for (int i = 0; i < count; i++)
				{
					T val = list[i];
					val.Reset();
				}
			}

			public void Update(InputRig rig)
			{
				int count = list.Count;
				for (int i = 0; i < count; i++)
				{
					T val = list[i];
					val.Update(rig);
				}
			}

			public T Get(string name, ref int cachedId)
			{
				if (cachedId >= 0 && cachedId < list.Count && list[cachedId].name == name)
				{
					return list[cachedId];
				}
				int count = list.Count;
				for (int i = 0; i < count; i++)
				{
					if (list[i].name == name)
					{
						cachedId = i;
						return list[i];
					}
				}
				return (T)null;
			}

			public T Get(string name)
			{
				int count = list.Count;
				for (int i = 0; i < count; i++)
				{
					if (list[i].name == name)
					{
						return list[i];
					}
				}
				return (T)null;
			}
		}

		[Serializable]
		public class TiltConfig : IBindingContainer
		{
			[NonSerialized]
			private InputRig rig;

			public TiltState tiltState;

			private int digitalRollDirection;

			private int digitalPitchDirection;

			private bool disabledByRigSwitches;

			public DisablingConditionSet disablingConditions;

			public AnalogConfig rollAnalogConfig;

			public AnalogConfig pitchAnalogConfig;

			public AxisBinding rollBinding;

			public AxisBinding pitchBinding;

			public DigitalBinding rollLeftBinding;

			public DigitalBinding rollRightBinding;

			public DigitalBinding pitchForwardBinding;

			public DigitalBinding pitchBackwardBinding;

			public TiltConfig(InputRig rig)
			{
				this.rig = rig;
				tiltState = new TiltState();
				rollAnalogConfig = new AnalogConfig();
				pitchAnalogConfig = new AnalogConfig();
				rollAnalogConfig.analogDeadZone = 0.3f;
				pitchAnalogConfig.analogDeadZone = 0.3f;
				disablingConditions = new DisablingConditionSet(rig);
				disablingConditions.disableWhenCursorIsUnlocked = false;
				disablingConditions.disableWhenTouchScreenInactive = false;
				disablingConditions.mobileModeRelation = DisablingConditionSet.MobileModeRelation.EnabledOnlyInMobileMode;
				rollBinding = new AxisBinding("Horizontal", enabled: false);
				pitchBinding = new AxisBinding("Vertical", enabled: false);
				rollLeftBinding = new DigitalBinding();
				rollRightBinding = new DigitalBinding();
				pitchForwardBinding = new DigitalBinding();
				pitchBackwardBinding = new DigitalBinding();
			}

			public void Reset()
			{
				digitalPitchDirection = 0;
				digitalRollDirection = 0;
				tiltState.Reset();
				OnDisablingConditionsChange();
			}

			public void OnDisablingConditionsChange()
			{
				disabledByRigSwitches = disablingConditions.IsInEffect();
			}

			public bool IsEnabled()
			{
				return !disabledByRigSwitches;
			}

			public void Update()
			{
				tiltState.InternalApplyVector(Input.acceleration);
				tiltState.Update();
				Vector2 analog = tiltState.GetAnalog();
				digitalRollDirection = rollAnalogConfig.GetSignedDigitalVal(analog.x, digitalRollDirection);
				digitalPitchDirection = pitchAnalogConfig.GetSignedDigitalVal(analog.y, digitalPitchDirection);
				if (IsEnabled())
				{
					if (rollBinding.enabled)
					{
						rollBinding.SyncFloat(rollAnalogConfig.GetAnalogVal(analog.x), InputSource.Analog, rig);
					}
					if (pitchBinding.enabled)
					{
						pitchBinding.SyncFloat(pitchAnalogConfig.GetAnalogVal(analog.y), InputSource.Analog, rig);
					}
					if (digitalRollDirection != 0)
					{
						((digitalRollDirection >= 0) ? rollRightBinding : rollLeftBinding).Sync(state: true, rig);
					}
					if (digitalPitchDirection != 0)
					{
						((digitalPitchDirection >= 0) ? pitchForwardBinding : pitchBackwardBinding).Sync(state: true, rig);
					}
				}
			}

			public bool IsBoundToAxis(string axisName, InputRig rig)
			{
				return rollBinding.IsBoundToAxis(axisName, rig) || pitchBinding.IsBoundToAxis(axisName, rig) || rollLeftBinding.IsBoundToAxis(axisName, rig) || rollRightBinding.IsBoundToAxis(axisName, rig) || pitchForwardBinding.IsBoundToAxis(axisName, rig) || pitchBackwardBinding.IsBoundToAxis(axisName, rig);
			}

			public bool IsBoundToKey(KeyCode key, InputRig rig)
			{
				return rollBinding.IsBoundToKey(key, rig) || pitchBinding.IsBoundToKey(key, rig) || rollLeftBinding.IsBoundToKey(key, rig) || rollRightBinding.IsBoundToKey(key, rig) || pitchForwardBinding.IsBoundToKey(key, rig) || pitchBackwardBinding.IsBoundToKey(key, rig);
			}

			public bool IsEmulatingTouches()
			{
				return false;
			}

			public bool IsEmulatingMousePosition()
			{
				return false;
			}

			public void GetSubBindingDescriptions(BindingDescriptionList descList, UnityEngine.Object undoObject, string parentMenuPath)
			{
				descList.Add(rollBinding, InputSource.Analog, "Roll Angle (Analog)", parentMenuPath, undoObject);
				descList.Add(pitchBinding, InputSource.Analog, "Pitch Angle (Analog)", parentMenuPath, undoObject);
				descList.Add(rollLeftBinding, "Roll Left (Digital)", parentMenuPath, undoObject);
				descList.Add(rollRightBinding, "Roll Right (Digital)", parentMenuPath, undoObject);
				descList.Add(pitchForwardBinding, "Pitch Forward (Digital)", parentMenuPath, undoObject);
				descList.Add(pitchBackwardBinding, "Pitch Backward (Digital)", parentMenuPath, undoObject);
			}
		}

		[Serializable]
		public class ScrollWheelState : IBindingContainer
		{
			private InputRig rig;

			public ScrollDeltaBinding horzScrollDeltaBinding;

			public ScrollDeltaBinding vertScrollDeltaBinding;

			public ScrollWheelState(InputRig rig)
			{
				this.rig = rig;
				vertScrollDeltaBinding = new ScrollDeltaBinding("Mouse ScrollWheel", enabled: true);
				horzScrollDeltaBinding = new ScrollDeltaBinding("Mouse ScrollWheel Secondary", enabled: true);
			}

			public void Reset()
			{
			}

			public void Update()
			{
				Vector2 mouseScrollDelta = Input.mouseScrollDelta;
				horzScrollDeltaBinding.SyncScrollDelta(Mathf.RoundToInt(mouseScrollDelta.x), rig);
				vertScrollDeltaBinding.SyncScrollDelta(Mathf.RoundToInt(mouseScrollDelta.y), rig);
			}

			public Vector2 GetDelta()
			{
				Vector2 mouseScrollDelta = Input.mouseScrollDelta;
				if (rig == null)
				{
					return mouseScrollDelta;
				}
				if (horzScrollDeltaBinding.deltaBinding.enabled)
				{
					mouseScrollDelta.x = horzScrollDeltaBinding.deltaBinding.GetAxis(rig);
				}
				if (vertScrollDeltaBinding.deltaBinding.enabled)
				{
					mouseScrollDelta.y = vertScrollDeltaBinding.deltaBinding.GetAxis(rig);
				}
				return mouseScrollDelta;
			}

			public bool IsBoundToAxis(string axisName, InputRig rig)
			{
				return horzScrollDeltaBinding.IsBoundToAxis(axisName, rig) || vertScrollDeltaBinding.IsBoundToAxis(axisName, rig);
			}

			public bool IsBoundToKey(KeyCode key, InputRig rig)
			{
				return horzScrollDeltaBinding.IsBoundToKey(key, rig) || vertScrollDeltaBinding.IsBoundToKey(key, rig);
			}

			public bool IsEmulatingTouches()
			{
				return false;
			}

			public bool IsEmulatingMousePosition()
			{
				return false;
			}

			public void GetSubBindingDescriptions(BindingDescriptionList descList, UnityEngine.Object undoObject, string parentMenuPath)
			{
				descList.Add(vertScrollDeltaBinding, "Vertical/Primary Scroll Wheel Delta", parentMenuPath, undoObject);
				descList.Add(horzScrollDeltaBinding, "Horizontal/Secondary Scroll Wheel Delta", parentMenuPath, undoObject);
			}
		}

		[Serializable]
		public class MouseConfig : IBindingContainer
		{
			private InputRig rig;

			public AxisBinding horzDeltaBinding;

			public AxisBinding vertDeltaBinding;

			private Vector2 mousePos;

			private Vector2 frMousePos;

			private int frMousePosPrio;

			public MouseConfig(InputRig rig)
			{
				this.rig = rig;
				horzDeltaBinding = new AxisBinding("Mouse X", enabled: false);
				vertDeltaBinding = new AxisBinding("Mouse Y", enabled: false);
			}

			public void Reset()
			{
			}

			public void SetPosition(Vector2 pos, int prio)
			{
				if (prio > frMousePosPrio)
				{
					frMousePos = pos;
					frMousePosPrio = prio;
				}
			}

			public Vector3 GetPosition()
			{
				return mousePos;
			}

			private bool IsMouseDeltaUsable()
			{
				return !CF2Input.IsInMobileMode() || (Input.touchSupported && Input.mousePresent && !Input.simulateMouseWithTouches && CFCursor.lockState == CursorLockMode.Locked);
			}

			private bool IsMousePositionUsable()
			{
				return !CF2Input.IsInMobileMode() || (Input.touchSupported && Input.mousePresent && !Input.simulateMouseWithTouches && CFCursor.lockState != CursorLockMode.Locked);
			}

			public void Update()
			{
				if (IsMouseDeltaUsable())
				{
					horzDeltaBinding.SyncFloat(UnityEngine.Input.GetAxisRaw("cfMouseX"), InputSource.MouseDelta, rig);
					vertDeltaBinding.SyncFloat(UnityEngine.Input.GetAxisRaw("cfMouseY"), InputSource.MouseDelta, rig);
				}
				if (IsMousePositionUsable())
				{
					SetPosition(UnityEngine.Input.mousePosition, -1);
				}
				mousePos = frMousePos;
				frMousePosPrio = -10;
			}

			public bool IsBoundToAxis(string axisName, InputRig rig)
			{
				return horzDeltaBinding.IsBoundToAxis(axisName, rig) || vertDeltaBinding.IsBoundToAxis(axisName, rig);
			}

			public bool IsBoundToKey(KeyCode key, InputRig rig)
			{
				return horzDeltaBinding.IsBoundToKey(key, rig) || vertDeltaBinding.IsBoundToKey(key, rig);
			}

			public bool IsEmulatingTouches()
			{
				return false;
			}

			public bool IsEmulatingMousePosition()
			{
				return false;
			}

			public void GetSubBindingDescriptions(BindingDescriptionList descList, UnityEngine.Object undoObject, string parentMenuPath)
			{
				descList.Add(horzDeltaBinding, InputSource.MouseDelta, "Horz. Mouse Delta", parentMenuPath, undoObject);
				descList.Add(vertDeltaBinding, InputSource.MouseDelta, "Vert. Mouse Delta", parentMenuPath, undoObject);
			}
		}

		public const string CF_EMPTY_AXIS = "cfEmpty";

		public const string CF_SCROLL_WHEEL_X_AXIS = "cfScroll0";

		public const string CF_SCROLL_WHEEL_Y_AXIS = "cfScroll1";

		public const string CF_MOUSE_DELTA_X_AXIS = "cfMouseX";

		public const string CF_MOUSE_DELTA_Y_AXIS = "cfMouseY";

		public const string DEFAULT_LEFT_STICK_NAME = "LeftStick";

		public const string DEFAULT_RIGHT_STICK_NAME = "RightStick";

		public const string DEFAULT_VERT_SCROLL_WHEEL_NAME = "Mouse ScrollWheel";

		public const string DEFAULT_HORZ_SCROLL_WHEEL_NAME = "Mouse ScrollWheel Secondary";

		public bool autoActivate = true;

		public bool overrideActiveRig = true;

		public bool hideWhenDisactivated = true;

		public bool disableWhenDisactivated;

		public bool hideWhenTouchScreenIsUnused = true;

		public float hideWhenTouchScreenIsUnusedDelay;

		public bool hideWhenGamepadIsActivated = true;

		public float hideWhenGamepadIsActivatedDelay;

		public float fingerRadiusInCm = 0.25f;

		public bool swipeOverFromNothing;

		public float controlBaseAlphaAnimDuration = 0.5f;

		public float animatorMaxAnimDuration = 0.2f;

		public float ninetyDegTurnMouseDelta = 500f;

		public float ninetyDegTurnTouchSwipeInCm = 4f;

		public float ninetyDegTurnAnalogDuration = 0.75f;

		public float virtualScreenDiameterInches = 4f;

		public int scrollStepsPerNinetyDegTurn = 10;

		public const float TOUCH_SMOOTHING_MAX_TIME = 0.1f;

		[NonSerialized]
		private float elapsedSinceLastTouch;

		[NonSerialized]
		private bool touchControlsSleeping;

		public VirtualJoystickConfigCollection joysticks;

		public AxisConfigCollection axes;

		public List<KeyCode> keyboardBlockedCodes;

		[NonSerialized]
		private BitArray keysPrev;

		[NonSerialized]
		private BitArray keysCur;

		[NonSerialized]
		private BitArray keysNext;

		[NonSerialized]
		private BitArray keysMuted;

		[NonSerialized]
		private BitArray keysBlocked;

		[NonSerialized]
		private bool keysNextSomeDown;

		[NonSerialized]
		private bool keysNextSomeOn;

		[NonSerialized]
		private bool keysCurSomeOn;

		[NonSerialized]
		private bool keysCurSomeDown;

		public JoystickConfig dpadConfig;

		public JoystickConfig leftStickConfig;

		public JoystickConfig rightStickConfig;

		public AnalogConfig leftTriggerAnalogConfig;

		public AnalogConfig rightTriggerAnalogConfig;

		public GamepadConfig[] gamepads;

		public GamepadConfig anyGamepad;

		public RigSwitchCollection rigSwitches;

		public AutomaticInputConfigCollection autoInputList;

		[NonSerialized]
		private List<TouchControl> touchControls;

		public TiltConfig tilt;

		public MouseConfig mouseConfig;

		public ScrollWheelState scrollWheel;

		private FixedTimeStepController fixedTimeStep;

		private const float MAX_DELTA_TIME = 0.2f;

		private const float DELTA_TIME_SMOOTHING_FACTOR = 0.1f;

		private const float DELTA_TIME_SMOOTHING_TIME = 1f;

		private const int FIXED_FPS = 120;

		private bool touchControlsHidden;

		private float analogToEmuMouseScale;

		private float analogToUniversalScale;

		private float analogToRawDeltaScale;

		private float scrollToEmuMouseScale;

		private float scrollToUniversalScale;

		private float mousePointsToUniversalScale;

		private float touchPixelsToEmuMouseScale;

		private float touchPixelsToUniversalScale;

		private int storedHorzRes;

		private int storedVertRes;

		private float storedDPCM;

		private const int MAX_EMU_TOUCHES = 8;

		private List<EmulatedTouchState> emuTouches;

		private List<EmulatedTouchState> emuTouchesOrdered;

		private Touch[] emuOutputTouches;

		private bool emuOutputTouchesDirty;

		public int axisConfigCount => axes.list.Count;

		public event Action onSwitchesChanged;

		public event Action onAddExtraInput;

		public static event Action onAddExtraInputToActiveRig;

		public InputRig()
		{
			BasicConstructor();
		}

		public void OnActivateRig()
		{
			if (hideWhenDisactivated)
			{
				ShowOrHideTouchControls(show: true, skipAnim: false);
			}
		}

		public void OnDisactivateRig()
		{
			if (disableWhenDisactivated)
			{
				base.gameObject.SetActive(value: false);
			}
			else if (hideWhenDisactivated)
			{
				ShowOrHideTouchControls(show: false, skipAnim: false);
			}
		}

		private void BasicConstructor()
		{
			fixedTimeStep = new FixedTimeStepController(120);
			ninetyDegTurnMouseDelta = 500f;
			ninetyDegTurnTouchSwipeInCm = 4f;
			ninetyDegTurnAnalogDuration = 0.75f;
			virtualScreenDiameterInches = 4f;
			hideWhenDisactivated = true;
			hideWhenTouchScreenIsUnused = true;
			hideWhenTouchScreenIsUnusedDelay = 10f;
			hideWhenGamepadIsActivated = true;
			hideWhenGamepadIsActivatedDelay = 5f;
			rigSwitches = new RigSwitchCollection(this);
			InitEmuTouches();
			touchControls = new List<TouchControl>(32);
			int length = CFUtils.GetEnumMaxValue(typeof(KeyCode)) + 1;
			keysCur = new BitArray(length);
			keysPrev = new BitArray(length);
			keysNext = new BitArray(length);
			keysBlocked = new BitArray(length);
			keysMuted = new BitArray(length);
			joysticks = new VirtualJoystickConfigCollection(this, 1);
			axes = new AxisConfigCollection(this, 16);
			axes.list.Clear();
			axes.list.Add(AxisConfig.CreateSignedAnalog("Horizontal", KeyCode.D, KeyCode.A));
			axes.list.Add(AxisConfig.CreateSignedAnalog("Vertical", KeyCode.W, KeyCode.S));
			axes.list.Add(AxisConfig.CreateDelta("Mouse X", KeyCode.None, KeyCode.None));
			axes.list.Add(AxisConfig.CreateDelta("Mouse Y", KeyCode.None, KeyCode.None));
			axes.list.Add(AxisConfig.CreateScrollWheel("Mouse ScrollWheel", KeyCode.None, KeyCode.None));
			axes.list.Add(AxisConfig.CreateScrollWheel("Mouse ScrollWheel Secondary", KeyCode.None, KeyCode.None));
			axes.list.Add(AxisConfig.CreateDigital("Fire1", KeyCode.LeftShift));
			axes.list.Add(AxisConfig.CreateDigital("Fire2", KeyCode.LeftControl));
			axes.list.Add(AxisConfig.CreateDigital("Jump", KeyCode.Space));
			keyboardBlockedCodes = new List<KeyCode>(16);
			dpadConfig = new JoystickConfig();
			dpadConfig.stickMode = JoystickConfig.StickMode.Digital8;
			dpadConfig.analogDeadZone = 0.3f;
			dpadConfig.angularMagnet = 0f;
			dpadConfig.digitalDetectionMode = JoystickConfig.DigitalDetectionMode.Joystick;
			leftStickConfig = new JoystickConfig();
			leftStickConfig.stickMode = JoystickConfig.StickMode.Analog;
			leftStickConfig.analogDeadZone = 0.3f;
			leftStickConfig.analogEndZone = 0.7f;
			leftStickConfig.angularMagnet = 0.5f;
			leftStickConfig.digitalDetectionMode = JoystickConfig.DigitalDetectionMode.Joystick;
			rightStickConfig = new JoystickConfig();
			rightStickConfig.stickMode = JoystickConfig.StickMode.Analog;
			rightStickConfig.analogDeadZone = 0.3f;
			rightStickConfig.analogEndZone = 0.7f;
			rightStickConfig.angularMagnet = 0.5f;
			rightStickConfig.digitalDetectionMode = JoystickConfig.DigitalDetectionMode.Joystick;
			leftTriggerAnalogConfig = new AnalogConfig();
			leftTriggerAnalogConfig.analogDeadZone = 0.2f;
			rightTriggerAnalogConfig = new AnalogConfig();
			rightTriggerAnalogConfig.analogDeadZone = 0.2f;
			anyGamepad = new GamepadConfig(this);
			gamepads = new GamepadConfig[4];
			for (int i = 0; i < gamepads.Length; i++)
			{
				gamepads[i] = new GamepadConfig(this);
			}
			anyGamepad.enabled = true;
			anyGamepad.leftStickStateBinding.horzAxisBinding.AddTarget().SetSingleAxis("Horizontal", flip: false);
			anyGamepad.leftStickStateBinding.vertAxisBinding.AddTarget().SetSingleAxis("Vertical", flip: false);
			anyGamepad.leftStickStateBinding.enabled = true;
			anyGamepad.leftStickStateBinding.horzAxisBinding.enabled = true;
			anyGamepad.leftStickStateBinding.vertAxisBinding.enabled = true;
			anyGamepad.rightStickStateBinding.horzAxisBinding.AddTarget().SetSingleAxis("Mouse X", flip: false);
			anyGamepad.rightStickStateBinding.vertAxisBinding.AddTarget().SetSingleAxis("Mouse Y", flip: false);
			anyGamepad.rightStickStateBinding.enabled = true;
			anyGamepad.rightStickStateBinding.horzAxisBinding.enabled = true;
			anyGamepad.rightStickStateBinding.vertAxisBinding.enabled = true;
			anyGamepad.dpadStateBinding.horzAxisBinding.AddTarget().SetSingleAxis("Horizontal", flip: false);
			anyGamepad.dpadStateBinding.vertAxisBinding.AddTarget().SetSingleAxis("Vertical", flip: false);
			anyGamepad.dpadStateBinding.enabled = true;
			anyGamepad.dpadStateBinding.horzAxisBinding.enabled = true;
			anyGamepad.dpadStateBinding.vertAxisBinding.enabled = true;
			anyGamepad.digiFaceDownBinding.enabled = true;
			anyGamepad.digiFaceDownBinding.AddAxis().SetAxis("Fire1", positiveSide: true);
			anyGamepad.digiFaceRightBinding.enabled = true;
			anyGamepad.digiFaceRightBinding.AddAxis().SetAxis("Jump", positiveSide: true);
			anyGamepad.digiFaceLeftBinding.enabled = true;
			anyGamepad.digiFaceLeftBinding.AddAxis().SetAxis("Fire2", positiveSide: true);
			anyGamepad.digiFaceUpBinding.enabled = true;
			anyGamepad.digiFaceUpBinding.AddAxis().SetAxis("Fire3", positiveSide: true);
			anyGamepad.digiR1Binding.enabled = true;
			anyGamepad.digiR1Binding.AddAxis().SetAxis("Fire1", positiveSide: true);
			anyGamepad.digiR2Binding.enabled = true;
			anyGamepad.digiR2Binding.AddAxis().SetAxis("Fire1", positiveSide: true);
			anyGamepad.digiL1Binding.enabled = true;
			anyGamepad.digiL1Binding.AddAxis().SetAxis("Fire2", positiveSide: true);
			anyGamepad.digiL2Binding.enabled = true;
			anyGamepad.digiL2Binding.AddAxis().SetAxis("Fire2", positiveSide: true);
			tilt = new TiltConfig(this);
			mouseConfig = new MouseConfig(this);
			scrollWheel = new ScrollWheelState(this);
			autoInputList = new AutomaticInputConfigCollection(this);
		}

		protected override void OnInitComponent()
		{
			autoInputList.SetRig(this);
			ResetState();
			ResetAllSwitches(skipAnim: true);
			InvalidateBlockedKeys();
		}

		protected override void OnDestroyComponent()
		{
			if (touchControls == null)
			{
				return;
			}
			int count = touchControls.Count;
			for (int i = 0; i < count; i++)
			{
				TouchControl touchControl = touchControls[i];
				if (touchControl != null)
				{
					touchControl.SetRig(null);
				}
			}
		}

		protected override void OnEnableComponent()
		{
			if (autoActivate && (CF2Input.activeRig == null || overrideActiveRig))
			{
				CF2Input.activeRig = this;
			}
			EventSystem eventSystem = EventSystem.current;
			if (eventSystem == null)
			{
				eventSystem = UnityEngine.Object.FindObjectOfType<EventSystem>();
			}
			if (eventSystem != null)
			{
				MonoBehaviour monoBehaviour = eventSystem.GetComponent("TouchInputModule") as MonoBehaviour;
				if (monoBehaviour != null && monoBehaviour.enabled)
				{
					monoBehaviour.enabled = false;
				}
			}
			fixedTimeStep.Reset();
			ResetState();
			MuteUntilRelease();
			if (!CFUtils.editorStopped)
			{
				CF2Input.onMobileModeChange += OnMobileModeChange;
				CFCursor.onLockStateChange += OnCursorLockStateChange;
			}
		}

		protected override void OnDisableComponent()
		{
			if (CF2Input.activeRig == this)
			{
				CF2Input.activeRig = null;
			}
			ResetState();
			if (!CFUtils.editorStopped)
			{
				CF2Input.onMobileModeChange -= OnMobileModeChange;
				CFCursor.onLockStateChange -= OnCursorLockStateChange;
			}
		}

		private void OnMobileModeChange()
		{
			SyncDisablingConditions(noAnim: false);
		}

		private void OnCursorLockStateChange()
		{
			SyncDisablingConditions(noAnim: false);
		}

		private void OnApplicationPause(bool paused)
		{
			int count = touchControls.Count;
			for (int i = 0; i < count; i++)
			{
				TouchControl touchControl = touchControls[i];
				if (touchControl != null)
				{
					touchControl.ReleaseAllTouches();
				}
			}
		}

		private void Update()
		{
			fixedTimeStep.Update(CFUtils.realDeltaTimeClamped);
			if (base.IsInitialized)
			{
				UpdateConfigs();
			}
		}

		public void ResetState()
		{
			if (!base.IsInitialized)
			{
				return;
			}
			tilt.Reset();
			mouseConfig.Reset();
			scrollWheel.Reset();
			ResetEmuTouches();
			joysticks.ResetAll();
			axes.ResetAll();
			MuteUntilRelease();
			keysCur.SetAll(value: false);
			keysPrev.SetAll(value: false);
			keysNext.SetAll(value: false);
			keysMuted.SetAll(value: true);
			keysCurSomeDown = false;
			keysCurSomeOn = false;
			keysNextSomeDown = false;
			keysNextSomeOn = false;
			InvalidateBlockedKeys();
			for (int i = 0; i < touchControls.Count; i++)
			{
				TouchControl touchControl = touchControls[i];
				if (touchControl != null)
				{
					touchControl.ResetControl();
				}
			}
		}

		public void MuteUntilRelease()
		{
			axes.MuteAllUntilRelease();
			keysPrev.SetAll(value: false);
			keysCur.SetAll(value: false);
			keysMuted.SetAll(value: true);
		}

		private void UpdateConfigs()
		{
			CheckResolution();
			if (!touchControlsSleeping)
			{
				float num = -1f;
				if (hideWhenTouchScreenIsUnused)
				{
					num = hideWhenTouchScreenIsUnusedDelay;
				}
				if (hideWhenGamepadIsActivated && GamepadManager.activeManager != null && GamepadManager.activeManager.GetActiveGamepadCount() > 0)
				{
					if (num < 0f)
					{
						num = hideWhenGamepadIsActivatedDelay;
					}
					else if (hideWhenGamepadIsActivatedDelay < num)
					{
						num = hideWhenGamepadIsActivatedDelay;
					}
				}
				if (num > 0f)
				{
					elapsedSinceLastTouch += CFUtils.realDeltaTimeClamped;
					if (elapsedSinceLastTouch > num)
					{
						PutTouchControlsToSleep();
					}
				}
			}
			else if (!hideWhenTouchScreenIsUnused && hideWhenGamepadIsActivated && GamepadManager.activeManager != null && GamepadManager.activeManager.GetActiveGamepadCount() == 0)
			{
				WakeTouchControlsUp();
			}
			ApplySwitches(skipAnim: false);
			for (int i = 0; i < touchControls.Count; i++)
			{
				touchControls[i].UpdateControl();
			}
			tilt.Update();
			mouseConfig.Update();
			scrollWheel.Update();
			joysticks.Update(this);
			axes.ApplyKeyboardInput();
			if (this == CF2Input.activeRig && InputRig.onAddExtraInputToActiveRig != null)
			{
				InputRig.onAddExtraInputToActiveRig();
			}
			if (this.onAddExtraInput != null)
			{
				this.onAddExtraInput();
			}
			autoInputList.Update(this);
			axes.Update(this);
			UpdateKeyCodes();
			UpdateEmuTouches();
		}

		private void UpdateKeyCodes()
		{
			BitArray bitArray = keysPrev;
			keysPrev = keysCur;
			keysCur = keysNext;
			keysNext = bitArray;
			keysNext.SetAll(value: false);
			keysMuted.And(keysCur);
			keysMuted.Not();
			keysCur.And(keysMuted);
			keysMuted.Not();
			keysCurSomeDown = keysNextSomeDown;
			keysCurSomeOn = keysNextSomeOn;
			keysNextSomeDown = false;
			keysNextSomeOn = false;
		}

		private void InvalidateBlockedKeys()
		{
			keysBlocked.SetAll(value: false);
			for (int i = 0; i < keyboardBlockedCodes.Count; i++)
			{
				int num = (int)keyboardBlockedCodes[i];
				if (num >= 0 && num < keysBlocked.Length)
				{
					keysBlocked[num] = true;
				}
			}
		}

		public static bool GetSourceKeyState(KeyCode key)
		{
			if (key == KeyCode.None || (CF2Input.IsInMobileMode() && key >= KeyCode.Mouse0 && key <= KeyCode.Mouse6))
			{
				return false;
			}
			return UnityEngine.Input.GetKey(key);
		}

		public static bool IsMouseKeyCode(KeyCode key)
		{
			return key >= KeyCode.Mouse0 && key <= KeyCode.Mouse6;
		}

		public void AddControl(TouchControl c)
		{
			if (CanBeUsed())
			{
				touchControls.Add(c);
			}
		}

		public void RemoveControl(TouchControl c)
		{
			if (CanBeUsed() && touchControls != null)
			{
				touchControls.Remove(c);
			}
		}

		public List<TouchControl> GetTouchControls()
		{
			return touchControls;
		}

		public TouchControl FindTouchControl(string name)
		{
			return touchControls.Find((TouchControl x) => x.name.Equals(name, StringComparison.OrdinalIgnoreCase));
		}

		public void ShowOrHideTouchControls(bool show, bool skipAnim)
		{
			if (touchControlsHidden != !show)
			{
				touchControlsHidden = !show;
				SyncDisablingConditions(skipAnim);
				if ((GamepadManager.activeManager == null || GamepadManager.activeManager.GetActiveGamepadCount() == 0) && show && AreTouchControlsSleeping())
				{
					WakeTouchControlsUp();
				}
			}
		}

		public bool AreTouchControlsHiddenManually()
		{
			return touchControlsHidden;
		}

		public bool AreTouchControlsSleeping()
		{
			return touchControlsSleeping;
		}

		public void WakeTouchControlsUp()
		{
			elapsedSinceLastTouch = 0f;
			if (touchControlsSleeping)
			{
				touchControlsSleeping = false;
				SyncDisablingConditions(noAnim: false);
			}
		}

		public void PutTouchControlsToSleep()
		{
			touchControlsSleeping = true;
			SyncDisablingConditions(noAnim: false);
		}

		public bool AreTouchControlsVisible()
		{
			return !touchControlsSleeping && !touchControlsHidden;
		}

		private void SyncDisablingConditions(bool noAnim)
		{
			int count = touchControls.Count;
			for (int i = 0; i < count; i++)
			{
				TouchControl touchControl = touchControls[i];
				if (touchControl != null)
				{
					touchControl.SyncDisablingConditions(noAnim);
				}
			}
			tilt.OnDisablingConditionsChange();
			autoInputList.OnDisablingConditionsChange();
		}

		public static bool IsTiltAvailable()
		{
			return TiltState.IsAvailable();
		}

		public bool IsTiltCalibrated()
		{
			return tilt.tiltState.IsCalibrated();
		}

		public void CalibrateTilt()
		{
			tilt.tiltState.Calibate();
		}

		public void ResetTilt()
		{
			tilt.Reset();
		}

		public bool GetSwitchState(string name, ref int cachedId, bool fallbackVal)
		{
			return rigSwitches.GetSwitchState(name, ref cachedId, fallbackVal);
		}

		public bool GetSwitchState(string name, bool fallbackVal)
		{
			return rigSwitches.GetSwitchState(name, fallbackVal);
		}

		public void SetSwitchState(string name, ref int cachedId, bool state)
		{
			rigSwitches.SetSwitchState(name, ref cachedId, state);
		}

		public void SetSwitchState(string name, bool state)
		{
			rigSwitches.SetSwitchState(name, state);
		}

		public void SetAllSwitches(bool state)
		{
			rigSwitches.SetAll(state);
		}

		public void ResetSwitch(string name, ref int cachedId)
		{
			rigSwitches.ResetSwitch(name, ref cachedId);
		}

		public void ResetSwitch(string name)
		{
			rigSwitches.ResetSwitch(name);
		}

		public void ResetAllSwitches(bool skipAnim)
		{
			rigSwitches.Reset();
			if (skipAnim)
			{
				ApplySwitches(skipAnim: true);
			}
		}

		public bool IsSwitchDefined(string name, ref int cachedId)
		{
			return rigSwitches.Get(name, ref cachedId) != null;
		}

		public bool IsSwitchDefined(string name)
		{
			return rigSwitches.Get(name) != null;
		}

		public void ApplySwitches(bool skipAnim)
		{
			if (rigSwitches.changed)
			{
				SyncDisablingConditions(skipAnim);
				rigSwitches.changed = false;
				if (this.onSwitchesChanged != null)
				{
					this.onSwitchesChanged();
				}
			}
		}

		public void ResetInputAxes()
		{
			MuteUntilRelease();
		}

		public bool IsAxisDefined(string name, ref int cachedId)
		{
			return axes.Get(name, ref cachedId) != null;
		}

		public bool IsAxisDefined(string name)
		{
			return axes.Get(name) != null;
		}

		public float GetAxis(string axisName)
		{
			return axes.Get(axisName)?.GetAnalog() ?? 0f;
		}

		public float GetAxis(string axisName, ref int cachedId)
		{
			return axes.Get(axisName, ref cachedId)?.GetAnalog() ?? 0f;
		}

		public float GetAxisRaw(string axisName)
		{
			return axes.Get(axisName)?.GetAnalogRaw() ?? 0f;
		}

		public float GetAxisRaw(string axisName, ref int cachedId)
		{
			return axes.Get(axisName, ref cachedId)?.GetAnalogRaw() ?? 0f;
		}

		public bool GetButton(string axisName)
		{
			return axes.Get(axisName)?.GetButton() ?? false;
		}

		public bool GetButton(string axisName, ref int cachedId)
		{
			return axes.Get(axisName, ref cachedId)?.GetButton() ?? false;
		}

		public bool GetButtonDown(string axisName)
		{
			return axes.Get(axisName)?.GetButtonDown() ?? false;
		}

		public bool GetButtonDown(string axisName, ref int cachedId)
		{
			return axes.Get(axisName, ref cachedId)?.GetButtonDown() ?? false;
		}

		public bool GetButtonUp(string axisName)
		{
			return axes.Get(axisName)?.GetButtonUp() ?? false;
		}

		public bool GetButtonUp(string axisName, ref int cachedId)
		{
			return axes.Get(axisName, ref cachedId)?.GetButtonUp() ?? false;
		}

		public bool GetKey(KeyCode keyCode)
		{
			if (keyCode < KeyCode.None || (int)keyCode >= keysCur.Length)
			{
				return false;
			}
			if (keysCur.Get((int)keyCode))
			{
				return true;
			}
			if (keysBlocked[(int)keyCode])
			{
				return false;
			}
			return (!CF2Input.IsInMobileMode() || !IsMouseKeyCode(keyCode)) && UnityEngine.Input.GetKey(keyCode);
		}

		public bool GetKeyDown(KeyCode keyCode)
		{
			if (keyCode < KeyCode.None || (int)keyCode >= keysCur.Length)
			{
				return false;
			}
			if (!keysPrev.Get((int)keyCode) && keysCur.Get((int)keyCode))
			{
				return true;
			}
			if (keysBlocked[(int)keyCode])
			{
				return false;
			}
			return (!CF2Input.IsInMobileMode() || !IsMouseKeyCode(keyCode)) && UnityEngine.Input.GetKeyDown(keyCode);
		}

		public bool GetKeyUp(KeyCode keyCode)
		{
			if (keyCode < KeyCode.None || (int)keyCode >= keysCur.Length)
			{
				return false;
			}
			if (keysPrev.Get((int)keyCode) && !keysCur.Get((int)keyCode))
			{
				return true;
			}
			if (keysBlocked[(int)keyCode])
			{
				return false;
			}
			return (!CF2Input.IsInMobileMode() || !IsMouseKeyCode(keyCode)) && UnityEngine.Input.GetKeyUp(keyCode);
		}

		public bool GetKey(string keyName)
		{
			return GetKey(NameToKeyCode(keyName));
		}

		public bool GetKeyDown(string keyName)
		{
			return GetKeyDown(NameToKeyCode(keyName));
		}

		public bool GetKeyUp(string keyName)
		{
			return GetKeyUp(NameToKeyCode(keyName));
		}

		public bool AnyKey()
		{
			return keysCurSomeOn || Input.anyKey;
		}

		public bool AnyKeyDown()
		{
			return keysCurSomeDown || Input.anyKeyDown;
		}

		public bool GetMouseButton(int mouseButton)
		{
			return mouseButton >= 0 && mouseButton <= 6 && GetKey((KeyCode)(323 + mouseButton));
		}

		public bool GetMouseButtonDown(int mouseButton)
		{
			return mouseButton >= 0 && mouseButton <= 6 && GetKeyDown((KeyCode)(323 + mouseButton));
		}

		public bool GetMouseButtonUp(int mouseButton)
		{
			return mouseButton >= 0 && mouseButton <= 6 && GetKeyUp((KeyCode)(323 + mouseButton));
		}

		public bool IsAxisAvailableOnMobile(string axisName)
		{
			for (int i = 0; i < touchControls.Count; i++)
			{
				if (touchControls[i].IsBoundToAxis(axisName, this))
				{
					return true;
				}
			}
			return false;
		}

		public bool IsKeyAvailableOnMobile(KeyCode keyCode)
		{
			for (int i = 0; i < touchControls.Count; i++)
			{
				if (touchControls[i].IsBoundToKey(keyCode, this))
				{
					return true;
				}
			}
			for (int j = 0; j < axes.list.Count; j++)
			{
				AxisConfig axisConfig = axes.list[j];
				if (axisConfig.DoesAffectKeyCode(keyCode) && IsAxisAvailableOnMobile(axisConfig.name))
				{
					return true;
				}
			}
			return false;
		}

		public bool IsTouchEmulatedOnMobile()
		{
			for (int i = 0; i < touchControls.Count; i++)
			{
				if (touchControls[i].IsEmulatingTouches())
				{
					return true;
				}
			}
			return false;
		}

		public bool IsMousePositionEmulatedOnMobile()
		{
			for (int i = 0; i < touchControls.Count; i++)
			{
				if (touchControls[i].IsEmulatingMousePosition())
				{
					return true;
				}
			}
			return false;
		}

		public bool IsScrollWheelEmulatedOnMobile()
		{
			if (!scrollWheel.vertScrollDeltaBinding.deltaBinding.enabled)
			{
				return false;
			}
			AxisBinding.TargetElem target = scrollWheel.vertScrollDeltaBinding.deltaBinding.GetTarget(0);
			if (target == null)
			{
				return false;
			}
			return !target.separateAxes && IsAxisAvailableOnMobile(target.singleAxis);
		}

		public bool IsJoystickDefined(string name, ref int cachedId)
		{
			return joysticks.Get(name, ref cachedId) != null;
		}

		public bool IsJoystickDefined(string name)
		{
			return joysticks.Get(name) != null;
		}

		public JoystickState GetJoystickState(string name, ref int cachedId)
		{
			return joysticks.Get(name, ref cachedId)?.joystickState;
		}

		public static KeyCode MouseButtonToKey(int mouseButton)
		{
			return (KeyCode)((mouseButton < 0 || mouseButton > 6) ? 323 : (323 + mouseButton));
		}

		public AxisConfig GetAxisConfig(int id)
		{
			if (id < 0 || id >= axes.list.Count)
			{
				return null;
			}
			return axes.list[id];
		}

		public AxisConfig GetAxisConfig(string name, ref int cachedId)
		{
			return axes.Get(name, ref cachedId);
		}

		public AxisConfig GetAxisConfig(string name)
		{
			return axes.Get(name);
		}

		public void SetAxis(string name, ref int cachedId, float v, InputSource source)
		{
			axes.Get(name, ref cachedId)?.Set(v, source);
		}

		public void SetAxisScroll(string name, ref int cachedId, int v)
		{
			axes.Get(name, ref cachedId)?.SetScrollDelta(v);
		}

		public void SetAxisDigital(string name, ref int cachedId, bool negSide)
		{
			axes.Get(name, ref cachedId)?.SetSignedDigital(!negSide);
		}

		public void SetJoystickState(string name, ref int joyId, JoystickState state)
		{
			joysticks.Get(name, ref joyId)?.SetState(state);
		}

		public void SetKeyCode(KeyCode keyCode)
		{
			if (keyCode != 0)
			{
				keysNext.Set((int)keyCode, value: true);
				if (!keysCur[(int)keyCode])
				{
					keysNextSomeDown = true;
				}
				keysNextSomeOn = true;
			}
		}

		public bool GetNextFrameKeyState(KeyCode key)
		{
			return keysNext[(int)key];
		}

		public void RecalcPixelConversionParams()
		{
			CheckResolution(forceRecalc: true);
		}

		private void CheckResolution(bool forceRecalc = false)
		{
			float num = CFScreen.dpcm;
			if (forceRecalc || storedHorzRes == 0 || storedHorzRes != Screen.width || storedVertRes != Screen.height || storedDPCM < 1f || storedDPCM != num)
			{
				storedHorzRes = Screen.width;
				storedVertRes = Screen.height;
				storedDPCM = num;
				if (!Application.isMobilePlatform || num <= 1f)
				{
					num = Mathf.Sqrt(storedHorzRes * storedHorzRes + storedVertRes * storedVertRes) / (virtualScreenDiameterInches * 2.54f);
				}
				touchPixelsToEmuMouseScale = ((!(num < 1f)) ? (ninetyDegTurnMouseDelta / (ninetyDegTurnTouchSwipeInCm * num)) : 0f);
				mousePointsToUniversalScale = ((ninetyDegTurnMouseDelta != 0f) ? (1f / ninetyDegTurnMouseDelta) : 0f);
				touchPixelsToUniversalScale = ((!(num < 1f)) ? (1f / (ninetyDegTurnTouchSwipeInCm * num)) : 0f);
				analogToEmuMouseScale = ninetyDegTurnMouseDelta / ninetyDegTurnAnalogDuration;
				analogToUniversalScale = 1f / ninetyDegTurnAnalogDuration;
				analogToRawDeltaScale = ninetyDegTurnMouseDelta / ninetyDegTurnAnalogDuration;
				scrollToEmuMouseScale = ninetyDegTurnMouseDelta / (float)scrollStepsPerNinetyDegTurn;
				scrollToUniversalScale = 1f / (float)scrollStepsPerNinetyDegTurn;
			}
		}

		public float TransformMousePointDelta(float mousePoints, DeltaTransformMode mode)
		{
			switch (mode)
			{
			case DeltaTransformMode.EmulateMouse:
				return mousePoints;
			case DeltaTransformMode.Universal:
				return mousePoints * mousePointsToUniversalScale;
			default:
				return mousePoints;
			}
		}

		public float TransformTouchPixelDelta(float touchPix, DeltaTransformMode mode)
		{
			switch (mode)
			{
			case DeltaTransformMode.EmulateMouse:
				return touchPix * touchPixelsToEmuMouseScale;
			case DeltaTransformMode.Universal:
				return touchPix * touchPixelsToUniversalScale;
			default:
				return touchPix;
			}
		}

		public float TransformNormalizedDelta(float normDelta, DeltaTransformMode mode)
		{
			switch (mode)
			{
			case DeltaTransformMode.EmulateMouse:
				return normDelta * ninetyDegTurnMouseDelta;
			case DeltaTransformMode.Universal:
				return normDelta;
			case DeltaTransformMode.Raw:
				return normDelta * ninetyDegTurnMouseDelta;
			default:
				return normDelta;
			}
		}

		public float TransformScrollDelta(float scrollDelta, DeltaTransformMode mode)
		{
			switch (mode)
			{
			case DeltaTransformMode.EmulateMouse:
				return scrollDelta * scrollToEmuMouseScale;
			case DeltaTransformMode.Raw:
				return scrollDelta * scrollToEmuMouseScale;
			case DeltaTransformMode.Universal:
				return scrollDelta * scrollToUniversalScale;
			default:
				return scrollDelta;
			}
		}

		public float TransformAnalogDelta(float analogVal, DeltaTransformMode mode)
		{
			switch (mode)
			{
			case DeltaTransformMode.EmulateMouse:
				return analogVal * analogToEmuMouseScale;
			case DeltaTransformMode.Universal:
				return analogVal * analogToUniversalScale;
			case DeltaTransformMode.Raw:
				return analogVal * analogToRawDeltaScale;
			default:
				return analogVal;
			}
		}

		public void GetSubBindingDescriptions(BindingDescriptionList descList, UnityEngine.Object undoObject, string parentMenuPath)
		{
			mouseConfig.GetSubBindingDescriptions(descList, this, parentMenuPath + "Mouse/");
			tilt.GetSubBindingDescriptions(descList, this, parentMenuPath + "Tilt/");
			scrollWheel.GetSubBindingDescriptions(descList, this, parentMenuPath + "Mouse Scroll Wheel/");
			anyGamepad.GetSubBindingDescriptions(descList, this, parentMenuPath + "Gamepads/Combined Gamepad/");
			for (int i = 0; i < gamepads.Length; i++)
			{
				gamepads[i].GetSubBindingDescriptions(descList, this, parentMenuPath + "Gamepads/Gamepad " + (i + 1).ToString() + "/");
			}
			for (int j = 0; j < joysticks.list.Count; j++)
			{
				joysticks.list[j].GetSubBindingDescriptions(descList, this, parentMenuPath + "Virtual Joysticks/Joystick [" + joysticks.list[j].name + "]/");
			}
		}

		public bool IsBoundToKey(KeyCode key, InputRig rig)
		{
			if (mouseConfig.IsBoundToKey(key, rig) || tilt.IsBoundToKey(key, rig) || scrollWheel.IsBoundToKey(key, rig) || anyGamepad.IsBoundToKey(key, rig))
			{
				return true;
			}
			for (int i = 0; i < gamepads.Length; i++)
			{
				if (gamepads[i].IsBoundToKey(key, rig))
				{
					return true;
				}
			}
			for (int j = 0; j < joysticks.list.Count; j++)
			{
				if (joysticks.list[j].IsBoundToKey(key, rig))
				{
					return true;
				}
			}
			return false;
		}

		public bool IsBoundToAxis(string axisName, InputRig rig)
		{
			if (mouseConfig.IsBoundToAxis(axisName, rig) || tilt.IsBoundToAxis(axisName, rig) || scrollWheel.IsBoundToAxis(axisName, rig) || anyGamepad.IsBoundToAxis(axisName, rig))
			{
				return true;
			}
			for (int i = 0; i < gamepads.Length; i++)
			{
				if (gamepads[i].IsBoundToAxis(axisName, rig))
				{
					return true;
				}
			}
			for (int j = 0; j < joysticks.list.Count; j++)
			{
				if (joysticks.list[j].IsBoundToAxis(axisName, rig))
				{
					return true;
				}
			}
			return false;
		}

		public bool IsEmulatingTouches()
		{
			if (mouseConfig.IsEmulatingTouches() || tilt.IsEmulatingTouches() || scrollWheel.IsEmulatingTouches() || anyGamepad.IsEmulatingTouches())
			{
				return true;
			}
			for (int i = 0; i < gamepads.Length; i++)
			{
				if (gamepads[i].IsEmulatingTouches())
				{
					return true;
				}
			}
			for (int j = 0; j < joysticks.list.Count; j++)
			{
				if (joysticks.list[j].IsEmulatingTouches())
				{
					return true;
				}
			}
			return false;
		}

		public bool IsEmulatingMousePosition()
		{
			if (mouseConfig.IsEmulatingMousePosition() || tilt.IsEmulatingMousePosition() || scrollWheel.IsEmulatingMousePosition() || anyGamepad.IsEmulatingMousePosition())
			{
				return true;
			}
			for (int i = 0; i < gamepads.Length; i++)
			{
				if (gamepads[i].IsEmulatingMousePosition())
				{
					return true;
				}
			}
			for (int j = 0; j < joysticks.list.Count; j++)
			{
				if (joysticks.list[j].IsEmulatingMousePosition())
				{
					return true;
				}
			}
			return false;
		}

		private void InitEmuTouches()
		{
			emuTouches = new List<EmulatedTouchState>(8);
			emuTouchesOrdered = new List<EmulatedTouchState>(8);
			for (int i = 0; i < 8; i++)
			{
				emuTouches.Add(new EmulatedTouchState(i));
			}
		}

		public int InternalStartEmuTouch(Vector2 pos)
		{
			for (int i = 0; i < emuTouches.Count; i++)
			{
				EmulatedTouchState emulatedTouchState = emuTouches[i];
				if (!emulatedTouchState.IsUsed())
				{
					emulatedTouchState.Start(pos);
					return i;
				}
			}
			return -1;
		}

		public void InternalEndEmuTouch(int emuTouchId, bool cancel)
		{
			if (emuTouchId >= 0 && emuTouchId < emuTouches.Count)
			{
				emuTouches[emuTouchId].EndTouch(cancel);
			}
		}

		public void InternalUpdateEmuTouch(int emuTouchId, Vector2 pos)
		{
			if (emuTouchId >= 0 && emuTouchId < emuTouches.Count)
			{
				emuTouches[emuTouchId].UpdatePos(pos);
			}
		}

		public Touch[] GetEmuTouchArray()
		{
			if (emuOutputTouchesDirty || emuOutputTouches == null)
			{
				if (emuOutputTouches == null || emuTouchesOrdered.Count != emuOutputTouches.Length)
				{
					emuOutputTouches = new Touch[emuTouchesOrdered.Count];
				}
				for (int i = 0; i < emuTouchesOrdered.Count; i++)
				{
					emuOutputTouches[i] = emuTouchesOrdered[i].outputTouch;
				}
				emuOutputTouchesDirty = false;
			}
			return emuOutputTouches;
		}

		public int GetEmuTouchCount()
		{
			return emuTouchesOrdered.Count;
		}

		public Touch GetEmuTouch(int i)
		{
			if (i < 0 || i >= emuTouchesOrdered.Count)
			{
				return Touch.Dummy;
			}
			return emuTouchesOrdered[i].outputTouch;
		}

		private void ResetEmuTouches()
		{
			for (int i = 0; i < emuTouches.Count; i++)
			{
				emuTouches[i].Reset();
			}
		}

		private void UpdateEmuTouches()
		{
			emuTouchesOrdered.Clear();
			for (int i = 0; i < emuTouches.Count; i++)
			{
				EmulatedTouchState emulatedTouchState = emuTouches[i];
				emulatedTouchState.Update();
				if (emulatedTouchState.IsActive())
				{
					emuTouchesOrdered.Add(emulatedTouchState);
				}
			}
			emuOutputTouchesDirty = true;
		}

		public void SyncEmuTouch(TouchGestureBasicState touch, ref int emuTouchId)
		{
			if (touch != null)
			{
				if (touch.JustPressedRaw())
				{
					emuTouchId = InternalStartEmuTouch(touch.GetStartPos());
				}
				else if (touch.JustReleasedRaw())
				{
					InternalEndEmuTouch(emuTouchId, cancel: false);
					emuTouchId = -1;
				}
				if (touch.PressedRaw())
				{
					InternalUpdateEmuTouch(emuTouchId, touch.GetCurPosRaw());
				}
			}
		}

		public static KeyCode NameToKeyCode(string keyName)
		{
			if (keyName.Length == 1)
			{
				char c = keyName[0];
				if (c >= 'a' && c <= 'z')
				{
					return (KeyCode)(97 + (c - 97));
				}
				if (c >= 'A' && c <= 'Z')
				{
					return (KeyCode)(97 + (c - 65));
				}
				if (c >= '0' && c <= '9')
				{
					return (KeyCode)(48 + (c - 48));
				}
			}
			StringComparison comparisonType = StringComparison.OrdinalIgnoreCase;
			if (keyName.Equals("enter", comparisonType))
			{
				return KeyCode.Return;
			}
			if (keyName.Equals("return", comparisonType))
			{
				return KeyCode.Return;
			}
			if (keyName.Equals("space", comparisonType))
			{
				return KeyCode.Space;
			}
			if (keyName.Equals("spacebar", comparisonType))
			{
				return KeyCode.Space;
			}
			if (keyName.Equals(" ", comparisonType))
			{
				return KeyCode.Space;
			}
			if (keyName.Equals("esc", comparisonType))
			{
				return KeyCode.Escape;
			}
			if (keyName.Equals("escape", comparisonType))
			{
				return KeyCode.Escape;
			}
			if (keyName.Equals("left", comparisonType))
			{
				return KeyCode.LeftArrow;
			}
			if (keyName.Equals("right", comparisonType))
			{
				return KeyCode.RightArrow;
			}
			if (keyName.Equals("up", comparisonType))
			{
				return KeyCode.UpArrow;
			}
			if (keyName.Equals("down", comparisonType))
			{
				return KeyCode.DownArrow;
			}
			if (keyName.Equals("Left Arrow", comparisonType))
			{
				return KeyCode.LeftArrow;
			}
			if (keyName.Equals("Right Arrow", comparisonType))
			{
				return KeyCode.RightArrow;
			}
			if (keyName.Equals("Up Arrow", comparisonType))
			{
				return KeyCode.UpArrow;
			}
			if (keyName.Equals("Down Arrow", comparisonType))
			{
				return KeyCode.DownArrow;
			}
			if (keyName.Equals("Arrow Left", comparisonType))
			{
				return KeyCode.LeftArrow;
			}
			if (keyName.Equals("Arrow Right", comparisonType))
			{
				return KeyCode.RightArrow;
			}
			if (keyName.Equals("Arrow Up", comparisonType))
			{
				return KeyCode.UpArrow;
			}
			if (keyName.Equals("Arrow Down", comparisonType))
			{
				return KeyCode.DownArrow;
			}
			if (keyName.Equals("Page Down", comparisonType))
			{
				return KeyCode.PageDown;
			}
			if (keyName.Equals("PageDown", comparisonType))
			{
				return KeyCode.PageDown;
			}
			if (keyName.Equals("PgDwn", comparisonType))
			{
				return KeyCode.PageDown;
			}
			if (keyName.Equals("Page Up", comparisonType))
			{
				return KeyCode.PageUp;
			}
			if (keyName.Equals("PageUp", comparisonType))
			{
				return KeyCode.PageUp;
			}
			if (keyName.Equals("PgUp", comparisonType))
			{
				return KeyCode.PageUp;
			}
			if (keyName.Equals("alt", comparisonType))
			{
				return KeyCode.LeftAlt;
			}
			if (keyName.Equals("L alt", comparisonType))
			{
				return KeyCode.LeftAlt;
			}
			if (keyName.Equals("Left alt", comparisonType))
			{
				return KeyCode.LeftAlt;
			}
			if (keyName.Equals("R alt", comparisonType))
			{
				return KeyCode.RightAlt;
			}
			if (keyName.Equals("Right alt", comparisonType))
			{
				return KeyCode.RightAlt;
			}
			if (keyName.Equals("control", comparisonType))
			{
				return KeyCode.LeftControl;
			}
			if (keyName.Equals("L control", comparisonType))
			{
				return KeyCode.LeftControl;
			}
			if (keyName.Equals("Left control", comparisonType))
			{
				return KeyCode.LeftControl;
			}
			if (keyName.Equals("R control", comparisonType))
			{
				return KeyCode.RightControl;
			}
			if (keyName.Equals("Right control", comparisonType))
			{
				return KeyCode.RightControl;
			}
			if (keyName.Equals("ctrl", comparisonType))
			{
				return KeyCode.LeftControl;
			}
			if (keyName.Equals("L ctrl", comparisonType))
			{
				return KeyCode.LeftControl;
			}
			if (keyName.Equals("Left ctrl", comparisonType))
			{
				return KeyCode.LeftControl;
			}
			if (keyName.Equals("R ctrl", comparisonType))
			{
				return KeyCode.RightControl;
			}
			if (keyName.Equals("Right ctrl", comparisonType))
			{
				return KeyCode.RightControl;
			}
			if (keyName.Equals("shift", comparisonType))
			{
				return KeyCode.LeftShift;
			}
			if (keyName.Equals("L shift", comparisonType))
			{
				return KeyCode.LeftShift;
			}
			if (keyName.Equals("Left shift", comparisonType))
			{
				return KeyCode.LeftShift;
			}
			if (keyName.Equals("R shift", comparisonType))
			{
				return KeyCode.RightShift;
			}
			if (keyName.Equals("Right shift", comparisonType))
			{
				return KeyCode.RightShift;
			}
			if (keyName.Equals("Caps Lock", comparisonType))
			{
				return KeyCode.CapsLock;
			}
			if (keyName.Equals("CapsLock", comparisonType))
			{
				return KeyCode.CapsLock;
			}
			if (keyName.Equals("Caps", comparisonType))
			{
				return KeyCode.CapsLock;
			}
			if (keyName.Equals("tab", comparisonType))
			{
				return KeyCode.Tab;
			}
			if (keyName.Equals("/", comparisonType))
			{
				return KeyCode.Backslash;
			}
			if (keyName.Equals("backslash", comparisonType))
			{
				return KeyCode.Backslash;
			}
			if (keyName.Equals("\\", comparisonType))
			{
				return KeyCode.Slash;
			}
			if (keyName.Equals("slash", comparisonType))
			{
				return KeyCode.Slash;
			}
			if (keyName.Equals("[", comparisonType))
			{
				return KeyCode.LeftBracket;
			}
			if (keyName.Equals("]", comparisonType))
			{
				return KeyCode.RightBracket;
			}
			if (keyName.Equals(".", comparisonType))
			{
				return KeyCode.Comma;
			}
			if (keyName.Equals(",", comparisonType))
			{
				return KeyCode.Colon;
			}
			if (keyName.Equals("'", comparisonType))
			{
				return KeyCode.Quote;
			}
			if (keyName.Equals(";", comparisonType))
			{
				return KeyCode.Semicolon;
			}
			if (keyName.Equals("mouse 0", comparisonType))
			{
				return KeyCode.Mouse0;
			}
			if (keyName.Equals("mouse 1", comparisonType))
			{
				return KeyCode.Mouse1;
			}
			if (keyName.Equals("mouse 2", comparisonType))
			{
				return KeyCode.Mouse2;
			}
			if (keyName.Equals("mouse 3", comparisonType))
			{
				return KeyCode.Mouse3;
			}
			if (keyName.Equals("mouse 4", comparisonType))
			{
				return KeyCode.Mouse4;
			}
			if (keyName.Equals("left mouse", comparisonType))
			{
				return KeyCode.Mouse0;
			}
			if (keyName.Equals("right mouse", comparisonType))
			{
				return KeyCode.Mouse1;
			}
			if (keyName.Equals("LMB", comparisonType))
			{
				return KeyCode.Mouse0;
			}
			if (keyName.Equals("RMB", comparisonType))
			{
				return KeyCode.Mouse1;
			}
			if (keyName.Equals("MMB", comparisonType))
			{
				return KeyCode.Mouse2;
			}
			if (keyName.Equals("F1", comparisonType))
			{
				return KeyCode.F1;
			}
			if (keyName.Equals("F2", comparisonType))
			{
				return KeyCode.F2;
			}
			if (keyName.Equals("F3", comparisonType))
			{
				return KeyCode.F3;
			}
			if (keyName.Equals("F4", comparisonType))
			{
				return KeyCode.F4;
			}
			if (keyName.Equals("F5", comparisonType))
			{
				return KeyCode.F5;
			}
			if (keyName.Equals("F6", comparisonType))
			{
				return KeyCode.F6;
			}
			if (keyName.Equals("F7", comparisonType))
			{
				return KeyCode.F7;
			}
			if (keyName.Equals("F8", comparisonType))
			{
				return KeyCode.F8;
			}
			if (keyName.Equals("F9", comparisonType))
			{
				return KeyCode.F9;
			}
			if (keyName.Equals("F10", comparisonType))
			{
				return KeyCode.F10;
			}
			if (keyName.Equals("F11", comparisonType))
			{
				return KeyCode.F11;
			}
			if (keyName.Equals("F12", comparisonType))
			{
				return KeyCode.F12;
			}
			return KeyCode.None;
		}
	}
}
