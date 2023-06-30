using ControlFreak2.Internal;
using System;
using UnityEngine;

namespace ControlFreak2
{
	[ExecuteInEditMode]
	public class SuperTouchZone : TouchControl
	{
		public enum GestureDetectionOrder
		{
			TwistPinchSwipe,
			TwistSwipePinch,
			PinchTwistSwipe,
			PinchSwipeTwist,
			SwipeTwistPinch,
			SwipePinchTwist
		}

		public enum EmuMouseAxis
		{
			X,
			Y
		}

		protected class FingerState
		{
			public SuperTouchZone zone;

			public TouchObject touchObj;

			public TouchStartType touchStartType;

			public int emuTouchId;

			public TouchGestureBasicState touchScreen;

			public TouchGestureBasicState touchOriented;

			public bool IsActive => touchObj != null;

			public FingerState(SuperTouchZone zone)
			{
				this.zone = zone;
				touchObj = null;
				touchScreen = new TouchGestureBasicState();
				touchOriented = new TouchGestureBasicState();
				emuTouchId = -1;
			}

			public bool IsControlledByMouse()
			{
				return touchObj != null && touchObj.IsMouse();
			}

			public void Reset()
			{
				touchScreen.Reset();
				touchOriented.Reset();
			}

			public void Update()
			{
				touchScreen.Update();
				touchOriented.Update();
			}

			public bool OnTouchStart(TouchObject touchObj, float delay, TouchStartType touchStartType)
			{
				if (this.touchObj != null)
				{
					return false;
				}
				this.touchObj = touchObj;
				this.touchStartType = touchStartType;
				Vector2 vector = (touchStartType != 0) ? touchObj.screenPosCur : touchObj.screenPosStart;
				touchScreen.OnTouchStart(vector, touchObj.screenPosCur, delay, this.touchObj);
				touchOriented.OnTouchStart(zone.ScreenToOrientedPos(vector, touchObj.cam), zone.ScreenToOrientedPos(touchObj.screenPosCur, touchObj.cam), delay, this.touchObj);
				return true;
			}

			public bool OnTouchEnd(TouchObject touchObj, TouchEndType touchEndType)
			{
				if (this.touchObj != touchObj || this.touchObj == null)
				{
					return false;
				}
				touchScreen.OnTouchEnd(touchObj.screenPosCur, touchEndType != TouchEndType.Release);
				touchOriented.OnTouchEnd(zone.ScreenToOrientedPos(touchObj.screenPosCur, touchObj.cam), touchEndType != TouchEndType.Release);
				this.touchObj = null;
				return true;
			}

			public bool OnTouchMove(TouchObject touchObj)
			{
				if (this.touchObj != touchObj || this.touchObj == null)
				{
					return false;
				}
				touchScreen.OnTouchMove(touchObj.screenPosCur);
				touchOriented.OnTouchMove(zone.ScreenToOrientedPos(touchObj.screenPosCur, touchObj.cam));
				zone.CheckSwipeOff(touchObj, touchStartType);
				return true;
			}

			public bool OnTouchPressureChange(TouchObject touchObj)
			{
				if (this.touchObj != touchObj || this.touchObj == null)
				{
					return false;
				}
				touchScreen.OnTouchPressureChange(touchObj.GetPressure());
				touchOriented.OnTouchPressureChange(touchObj.GetPressure());
				return true;
			}

			public void ReleaseTouch(bool cancel)
			{
				if (touchObj != null)
				{
					touchObj.ReleaseControl(zone, cancel ? TouchEndType.Cancel : TouchEndType.Release);
					touchObj = null;
				}
				touchScreen.OnTouchEnd(cancel);
				touchOriented.OnTouchEnd(cancel);
			}
		}

		[Serializable]
		public class MultiFingerTouchConfig
		{
			public TouchGestureStateBinding binding;

			public bool driveOtherControl;

			public TouchControl controlToDriveOnRawPress;

			public TouchControl controlToDriveOnNormalPress;

			public TouchControl controlToDriveOnLongPress;

			public TouchControl controlToDriveOnNormalPressSwipe;

			public TouchControl controlToDriveOnNormalPressSwipeU;

			public TouchControl controlToDriveOnNormalPressSwipeR;

			public TouchControl controlToDriveOnNormalPressSwipeD;

			public TouchControl controlToDriveOnNormalPressSwipeL;

			public TouchControl controlToDriveOnLongPressSwipe;

			public TouchControl controlToDriveOnLongPressSwipeU;

			public TouchControl controlToDriveOnLongPressSwipeR;

			public TouchControl controlToDriveOnLongPressSwipeD;

			public TouchControl controlToDriveOnLongPressSwipeL;

			public TouchGestureConfig touchConfig;

			public JoystickConfig swipeJoyConfig;

			public MultiFingerTouchConfig()
			{
				binding = new TouchGestureStateBinding();
				driveOtherControl = false;
				touchConfig = new TouchGestureConfig();
				swipeJoyConfig = new JoystickConfig();
			}
		}

		protected class MultiFingerTouch
		{
			private SuperTouchZone zone;

			public TouchGestureState touchScreen;

			public TouchGestureState touchOriented;

			public float quietModeElapsed;

			private FingerState[] fingers;

			public bool active;

			public bool quietMode;

			private Vector2 posStartScreen;

			private Vector2 posStartOriented;

			private Vector2 posCurScreen;

			private Vector2 posCurOriented;

			private bool controlledByMouse;

			private JoystickState swipeJoyState;

			private MultiFingerTouchConfig config;

			private TouchObject drivingTouch;

			public MultiFingerTouch(int fingerCount, SuperTouchZone zone)
			{
				this.zone = zone;
				touchScreen = new TouchGestureState();
				touchOriented = new TouchGestureState();
				fingers = new FingerState[fingerCount];
				drivingTouch = new TouchObject();
				swipeJoyState = new JoystickState(null);
				Reset();
			}

			public void Init(MultiFingerTouchConfig config)
			{
				this.config = config;
				touchScreen.SetConfig(config?.touchConfig);
				touchOriented.SetConfig(config?.touchConfig);
				touchScreen.SetThresholds(zone.GetThresholds());
				touchOriented.SetThresholds(zone.GetThresholds());
				swipeJoyState.config = config.swipeJoyConfig;
			}

			public int GetFingerCount()
			{
				return fingers.Length;
			}

			public FingerState GetFinger(int i)
			{
				return fingers[i];
			}

			public bool IsActive()
			{
				return active;
			}

			public bool IsQuiet()
			{
				return quietMode;
			}

			public void Reset()
			{
				touchScreen.Reset();
				touchOriented.Reset();
				swipeJoyState.Reset();
				active = false;
				quietMode = false;
			}

			public void EndDrivingTouch(bool cancel)
			{
				drivingTouch.End(cancel);
			}

			public void SyncToRig()
			{
				if (config != null)
				{
					config.binding.SyncTouchState(touchScreen, touchOriented, swipeJoyState, zone.rig);
				}
			}

			public void Start(FingerState[] fingers, Vector2 startScreenPos, Vector2 startOrientedPos, bool quietMode, float quietAlreadyElapsed)
			{
				controlledByMouse = true;
				for (int i = 0; i < this.fingers.Length; i++)
				{
					this.fingers[i] = fingers[i];
					if (!this.fingers[i].IsControlledByMouse())
					{
						controlledByMouse = false;
					}
				}
				active = true;
				this.quietMode = quietMode;
				quietModeElapsed = quietAlreadyElapsed;
				posStartScreen = startScreenPos;
				posStartOriented = startOrientedPos;
				posCurScreen = startScreenPos;
				posCurOriented = startOrientedPos;
				if (!quietMode)
				{
					touchScreen.OnTouchStart(startScreenPos, startScreenPos, 0f, controlledByMouse, isPressureSensitive: false, 1f);
					touchOriented.OnTouchStart(startOrientedPos, startOrientedPos, 0f, controlledByMouse, isPressureSensitive: false, 1f);
				}
			}

			public void End(bool cancel)
			{
				if (active)
				{
					active = false;
					touchScreen.OnTouchEnd(cancel || quietMode);
					touchOriented.OnTouchEnd(cancel || quietMode);
				}
			}

			public void EndAndReport()
			{
				if (active)
				{
					if (quietMode)
					{
						StartOfficially();
					}
					active = false;
					touchScreen.OnTouchEnd(cancel: false);
					touchOriented.OnTouchEnd(cancel: false);
				}
			}

			public void SetPos(Vector2 screenPos, Vector2 orientedPos)
			{
				posCurScreen = screenPos;
				posCurOriented = orientedPos;
			}

			private bool IsPotentialTapPossible()
			{
				return active && quietMode;
			}

			public void CancelTap()
			{
				touchOriented.CancelTap();
				touchScreen.CancelTap();
			}

			public void DisableTapUntilRelease()
			{
				touchOriented.DisableTapUntilRelease();
				touchScreen.DisableTapUntilRelease();
			}

			public void StartOfficially()
			{
				if (active && quietMode)
				{
					quietMode = false;
					touchScreen.OnTouchStart(posStartScreen, posCurScreen, quietModeElapsed, controlledByMouse, isPressureSensitive: false, 1f);
					touchOriented.OnTouchStart(posStartOriented, posCurOriented, quietModeElapsed, controlledByMouse, isPressureSensitive: false, 1f);
				}
			}

			public void Update()
			{
				touchScreen.HoldDelayedEvents(IsPotentialTapPossible());
				touchOriented.HoldDelayedEvents(IsPotentialTapPossible());
				if (active)
				{
					for (int i = 0; i < fingers.Length; i++)
					{
						if (!fingers[i].touchScreen.PressedRaw())
						{
							EndAndReport();
							break;
						}
						if (fingers[i].touchScreen.Moved(zone.GetThresholds().tapMoveThreshPxSq))
						{
							touchScreen.MarkAsNonStatic();
							touchOriented.MarkAsNonStatic();
							if (quietMode)
							{
								StartOfficially();
							}
						}
					}
					if (active)
					{
						if (quietMode)
						{
							quietModeElapsed += CFUtils.realDeltaTime;
							if (quietModeElapsed > zone.strictMultiTouchTime)
							{
								StartOfficially();
							}
						}
						else
						{
							touchScreen.OnTouchMove(posCurScreen);
							touchOriented.OnTouchMove(posCurOriented);
						}
					}
				}
				touchScreen.Update();
				touchOriented.Update();
				if (config == null)
				{
					return;
				}
				if (touchOriented.PressedRaw() && touchScreen.Swiped())
				{
					swipeJoyState.ApplyUnclampedVec(touchOriented.GetSwipeVecSmooth() / zone.GetThresholds().swipeJoystickRadPx);
				}
				swipeJoyState.Update();
				if (drivingTouch.IsOn())
				{
					if (!touchScreen.PressedRaw())
					{
						drivingTouch.End(cancel: false);
					}
					else
					{
						drivingTouch.MoveIfNeeded(touchScreen.GetCurPosRaw(), zone.GetCamera());
					}
				}
				else
				{
					if (!config.driveOtherControl)
					{
						return;
					}
					if (config.controlToDriveOnRawPress != null)
					{
						if (touchScreen.JustPressedRaw())
						{
							StartDrivingControl(config.controlToDriveOnRawPress);
						}
					}
					else if (touchOriented.PressedLong())
					{
						if (config.controlToDriveOnLongPressSwipeU != null || config.controlToDriveOnLongPressSwipeD != null || config.controlToDriveOnLongPressSwipeR != null || config.controlToDriveOnLongPressSwipeL != null)
						{
							if (touchOriented.GetSwipeDirState4().GetCur() != 0)
							{
								switch (touchOriented.GetSwipeDirState4().GetCur())
								{
								case Dir.U:
									StartDrivingControl(config.controlToDriveOnLongPressSwipeU);
									break;
								case Dir.R:
									StartDrivingControl(config.controlToDriveOnLongPressSwipeR);
									break;
								case Dir.D:
									StartDrivingControl(config.controlToDriveOnLongPressSwipeD);
									break;
								case Dir.L:
									StartDrivingControl(config.controlToDriveOnLongPressSwipeL);
									break;
								}
							}
						}
						else if (config.controlToDriveOnLongPressSwipe != null && touchOriented.Swiped())
						{
							StartDrivingControl(config.controlToDriveOnLongPressSwipe);
						}
						if (config.controlToDriveOnLongPress != null)
						{
							StartDrivingControl(config.controlToDriveOnLongPress);
						}
					}
					else
					{
						if (!touchOriented.PressedNormal())
						{
							return;
						}
						if (config.controlToDriveOnNormalPressSwipeU != null || config.controlToDriveOnNormalPressSwipeD != null || config.controlToDriveOnNormalPressSwipeR != null || config.controlToDriveOnNormalPressSwipeL != null)
						{
							if (touchOriented.PressedNormal() && touchOriented.GetSwipeDirState4().GetCur() != 0)
							{
								switch (touchOriented.GetSwipeDirState4().GetCur())
								{
								case Dir.U:
									StartDrivingControl(config.controlToDriveOnNormalPressSwipeU);
									break;
								case Dir.R:
									StartDrivingControl(config.controlToDriveOnNormalPressSwipeR);
									break;
								case Dir.D:
									StartDrivingControl(config.controlToDriveOnNormalPressSwipeD);
									break;
								case Dir.L:
									StartDrivingControl(config.controlToDriveOnNormalPressSwipeL);
									break;
								}
							}
						}
						else if (config.controlToDriveOnNormalPressSwipe != null && touchOriented.PressedNormal() && touchOriented.Swiped())
						{
							StartDrivingControl(config.controlToDriveOnNormalPressSwipe);
						}
						if (config.controlToDriveOnNormalPress != null && touchScreen.PressedNormal())
						{
							StartDrivingControl(config.controlToDriveOnNormalPress);
						}
					}
				}
			}

			private void StartDrivingControl(TouchControl c)
			{
				if (!(c == null) && c.IsActive() && !drivingTouch.IsOn())
				{
					drivingTouch.Start(touchScreen.GetStartPos(), touchScreen.GetCurPosRaw(), zone.GetCamera(), isMouse: false, isPressureSensitive: false, 1f);
					if (!c.OnTouchStart(drivingTouch, zone, TouchStartType.ProxyPress))
					{
						drivingTouch.End(cancel: true);
					}
				}
			}
		}

		private class EmulatedFingerObject : TouchObject
		{
			public Vector2 emuPos;

			public SuperTouchZone parentZone;

			public EmulatedFingerObject(SuperTouchZone parentZone)
			{
				this.parentZone = parentZone;
			}
		}

		private class EmulatedFingerSystem
		{
			public EmulatedFingerObject[] fingers;

			private SuperTouchZone zone;

			private TouchObject mouseTouchObj;

			private Vector2 lastMousePos;

			private int curFingerNum;

			private Vector2 centerPos;

			private float twistCur;

			private float pinchDistCur;

			public EmulatedFingerSystem(SuperTouchZone zone)
			{
				this.zone = zone;
				fingers = new EmulatedFingerObject[3];
				for (int i = 0; i < fingers.Length; i++)
				{
					fingers[i] = new EmulatedFingerObject(zone);
				}
			}

			public void EndMouseAndTouches(bool cancel)
			{
				if (mouseTouchObj != null)
				{
					mouseTouchObj.ReleaseControl(zone, cancel ? TouchEndType.Cancel : TouchEndType.Release);
					mouseTouchObj = null;
				}
				EndTouches(cancel);
			}

			public void EndTouches(bool cancel)
			{
				curFingerNum = 0;
				for (int i = 0; i < fingers.Length; i++)
				{
					fingers[i].End(cancel);
				}
			}

			public bool OnSystemTouchStart(TouchObject touchObj, TouchControl sender, TouchStartType touchStartType)
			{
				if (sender != null && sender != zone)
				{
					return false;
				}
				if (mouseTouchObj != null)
				{
					return false;
				}
				EmulatedFingerObject emulatedFingerObject = touchObj as EmulatedFingerObject;
				if (emulatedFingerObject != null && emulatedFingerObject.parentZone == zone)
				{
					return false;
				}
				EndTouches(cancel: false);
				mouseTouchObj = touchObj;
				mouseTouchObj.AddControl(zone);
				int num = (UnityEngine.Input.GetKey(zone.emuMouseTwoFingersKey) || UnityEngine.Input.GetKey(zone.emuMouseTwistKey) || UnityEngine.Input.GetKey(zone.emuMousePinchKey)) ? 2 : ((!Input.GetKey(zone.emuMouseThreeFingersKey)) ? 1 : 3);
				lastMousePos = touchObj.screenPosCur;
				twistCur = 0f;
				pinchDistCur = 0.25f * (float)Mathf.Min(Screen.width, Screen.height);
				StartTouches(num, lastMousePos, touchObj.IsMouse());
				return true;
			}

			public bool OnSystemTouchEnd(TouchObject touchObj, TouchEndType touchEndType)
			{
				if (mouseTouchObj == null || mouseTouchObj != touchObj)
				{
					return false;
				}
				mouseTouchObj = null;
				EndTouches(touchEndType != TouchEndType.Release);
				return true;
			}

			public bool OnSystemTouchMove(TouchObject touchObj)
			{
				if (mouseTouchObj == null || mouseTouchObj != touchObj)
				{
					return false;
				}
				return true;
			}

			public void StartTouches(int num, Vector2 center, bool isMouse)
			{
				EndTouches(cancel: false);
				if (num > fingers.Length)
				{
					num = fingers.Length;
				}
				curFingerNum = num;
				centerPos = center;
				for (int i = 0; i < num; i++)
				{
					EmulatedFingerObject emulatedFingerObject = fingers[i];
					emulatedFingerObject.emuPos = GetFingerPos(i);
					emulatedFingerObject.Start(emulatedFingerObject.emuPos, emulatedFingerObject.emuPos, zone.GetCamera(), isMouse, isPressureSensitive: false, 1f);
					zone.OnTouchStart(emulatedFingerObject, null, TouchStartType.DirectPress);
				}
			}

			public void Update()
			{
				float num = Mathf.Min(Screen.width, Screen.height);
				if (mouseTouchObj != null)
				{
					Vector2 vector = mouseTouchObj.screenPosCur - lastMousePos;
					lastMousePos = mouseTouchObj.screenPosCur;
					bool key = UnityEngine.Input.GetKey(zone.emuMouseTwistKey);
					bool flag = UnityEngine.Input.GetKey(zone.emuMousePinchKey);
					if (zone.emuMousePinchAxis == zone.emuMouseTwistAxis)
					{
						flag = false;
					}
					bool flag2 = UnityEngine.Input.GetKey(zone.emuMouseTwoFingersKey) || UnityEngine.Input.GetKey(zone.emuMouseThreeFingersKey) || (!key && !flag);
					if (key)
					{
						twistCur += vector[(int)zone.emuMouseTwistAxis] * (90f / Mathf.Max(30f, zone.mouseEmuTwistScreenFactor * num));
					}
					if (flag)
					{
						pinchDistCur += vector[(int)zone.emuMousePinchAxis];
					}
					if (flag2)
					{
						centerPos += vector;
					}
				}
				else
				{
					Vector2 a = new Vector2((UnityEngine.Input.GetKey(zone.emuKeySwipeR) ? 1 : 0) + (UnityEngine.Input.GetKey(zone.emuKeySwipeL) ? (-1) : 0), (UnityEngine.Input.GetKey(zone.emuKeySwipeU) ? 1 : 0) + (UnityEngine.Input.GetKey(zone.emuKeySwipeD) ? (-1) : 0));
					float num2 = (UnityEngine.Input.GetKey(zone.emuKeyPinch) ? (-1) : 0) + (UnityEngine.Input.GetKey(zone.emuKeySpread) ? 1 : 0);
					float num3 = (UnityEngine.Input.GetKey(zone.emuKeyTwistL) ? (-1) : 0) + (UnityEngine.Input.GetKey(zone.emuKeyTwistR) ? 1 : 0);
					if (curFingerNum == 0)
					{
						int num4 = 0;
						if (a.sqrMagnitude > 0.0001f || UnityEngine.Input.GetKey(zone.emuKeyOneFinger))
						{
							num4 = 1;
						}
						else if (Mathf.Abs(num2) > 0.0001f || Mathf.Abs(num3) > 0.001f || UnityEngine.Input.GetKey(zone.emuKeyTwoFingers))
						{
							num4 = 2;
						}
						else if (UnityEngine.Input.GetKey(zone.emuKeyThreeFingers))
						{
							num4 = 3;
						}
						StartTouches(num4, zone.GetScreenSpaceCenter(zone.GetCamera()), isMouse: false);
					}
					else if (a.sqrMagnitude < 0.0001f && Mathf.Abs(num2) < 0.0001f && Mathf.Abs(num3) < 0.001f && ((curFingerNum == 1) ? (!Input.GetKey(zone.emuKeyOneFinger)) : ((curFingerNum == 2) ? (!Input.GetKey(zone.emuKeyTwoFingers)) : (curFingerNum != 3 || !Input.GetKey(zone.emuKeyThreeFingers)))))
					{
						EndTouches(cancel: false);
					}
					else
					{
						centerPos += a * (num * zone.emuKeySwipeSpeed * CFUtils.realDeltaTime);
						pinchDistCur += num2 * (num * zone.emuKeyPinchSpeed * CFUtils.realDeltaTime);
						pinchDistCur = Mathf.Clamp(pinchDistCur, num * 0.1f, num * 0.9f);
						twistCur += num3 * zone.emuKeyTwistSpeed * CFUtils.realDeltaTime;
					}
				}
				pinchDistCur = Mathf.Clamp(pinchDistCur, num * 0.1f, num * 0.9f);
				for (int i = 0; i < fingers.Length; i++)
				{
					EmulatedFingerObject emulatedFingerObject = fingers[i];
					emulatedFingerObject.emuPos = GetFingerPos(i);
					if (emulatedFingerObject.IsOn())
					{
						emulatedFingerObject.MoveIfNeeded(emulatedFingerObject.emuPos, zone.GetCamera());
					}
				}
			}

			public void DrawMarkers()
			{
				Texture2D texture2D = null;
				Texture2D texture2D2 = null;
				Texture2D texture2D3 = null;
				if (TouchMarkerGUI.mInst != null)
				{
					texture2D = TouchMarkerGUI.mInst.fingerMarker;
					texture2D2 = TouchMarkerGUI.mInst.pinchHintMarker;
					texture2D3 = TouchMarkerGUI.mInst.twistHintMarker;
				}
				if (texture2D == null || texture2D2 == null || texture2D3 == null)
				{
					return;
				}
				float num = 32f;
				Matrix4x4 matrix = GUI.matrix;
				Color color = GUI.color;
				if (curFingerNum > 1)
				{
					bool key = UnityEngine.Input.GetKey(zone.emuMouseTwistKey);
					bool flag = UnityEngine.Input.GetKey(zone.emuMousePinchKey);
					if (zone.emuMousePinchAxis == zone.emuMouseTwistAxis)
					{
						flag = false;
					}
					float num2 = 0.6f;
					float num3 = 0.6f;
					if (key && flag)
					{
						float num4 = Time.unscaledTime / 2f;
						num2 *= Mathf.Clamp01(Mathf.Sin(num4 * 2f * 3.14159274f));
						num3 *= Mathf.Clamp01(Mathf.Sin((num4 + 0.5f) * 2f * 3.14159274f));
					}
					float num5 = 128f;
					Rect position = new Rect((0f - num5) * 0.5f, (0f - num5) * 0.5f, num5, num5);
					Vector2 v = centerPos;
					v.y = (float)Screen.height - v.y;
					if (key)
					{
						GUI.color = new Color(1f, 1f, 1f, num2);
						GUI.matrix = Matrix4x4.TRS(v, Quaternion.Euler(0f, 0f, (zone.emuMouseTwistAxis == EmuMouseAxis.Y) ? (-90) : 0), Vector3.one);
						GUI.DrawTexture(position, texture2D3, ScaleMode.ScaleToFit);
					}
					if (flag)
					{
						GUI.color = new Color(1f, 1f, 1f, num3);
						GUI.matrix = Matrix4x4.TRS(v, Quaternion.Euler(0f, 0f, (zone.emuMousePinchAxis == EmuMouseAxis.Y) ? (-90) : 0), Vector3.one);
						GUI.DrawTexture(position, texture2D2, ScaleMode.ScaleToFit);
					}
				}
				GUI.color = new Color(1f, 1f, 1f, 0.5f);
				for (int i = 0; i < fingers.Length; i++)
				{
					if (fingers[i].IsOn())
					{
						Vector2 emuPos = fingers[i].emuPos;
						emuPos.y = (float)Screen.height - emuPos.y;
						GUI.matrix = Matrix4x4.TRS(emuPos, Quaternion.Euler(0f, 0f, GetFingerAngle(i)), Vector3.one);
						GUI.DrawTexture(new Rect(0f - num * 0.5f, 0f - num, num, num), texture2D);
					}
				}
				GUI.color = color;
				GUI.matrix = matrix;
			}

			private Vector2 GetFingerPos(int fingerId)
			{
				if (curFingerNum <= 1)
				{
					return centerPos;
				}
				float d = pinchDistCur * 0.5f;
				float f = GetFingerAngle(fingerId) * 0.0174532924f;
				return centerPos + new Vector2(Mathf.Sin(f), Mathf.Cos(f)) * d;
			}

			private float GetFingerAngle(int fingerId)
			{
				if (curFingerNum <= 1)
				{
					return 0f;
				}
				return 360f * ((float)fingerId / (float)curFingerNum) + twistCur;
			}
		}

		public MultiTouchGestureThresholds customThresh;

		[Tooltip("How many fingers can be used on this zone at the same time?")]
		[Range(1f, 3f)]
		public int maxFingers = 3;

		public float touchSmoothing = 0.5f;

		public bool strictMultiTouch;

		public float strictMultiTouchTime;

		public bool freezeTwistWhenTooClose;

		public bool noPinchAfterDrag;

		public bool noPinchAfterTwist;

		public bool noTwistAfterDrag;

		public bool noTwistAfterPinch;

		public bool noDragAfterPinch;

		public bool noDragAfterTwist;

		public bool startPinchWhenTwisting;

		public bool startPinchWhenDragging;

		public bool startDragWhenPinching;

		public bool startDragWhenTwisting;

		public bool startTwistWhenDragging;

		public bool startTwistWhenPinching;

		public GestureDetectionOrder gestureDetectionOrder;

		private MultiFingerTouch[] multiFingerTouch;

		public MultiFingerTouchConfig[] multiFingerConfigs;

		private FingerState[] fingers;

		private FingerState[] fingersOrdered;

		private int rawFingersPressedCur;

		private int rawFingersPressedPrev;

		private bool dontAllowNewFingers;

		private bool waitingForMoreFingers;

		private float elapsedSinceFirstTouch;

		private bool pinchMoved;

		private bool pinchJustMoved;

		private bool twistMoved;

		private bool twistJustMoved;

		private float pinchDistQuietInitial;

		private float pinchDistInitial;

		private float pinchDistCur;

		private float pinchDistPrev;

		private float twistAngleQuietInitial;

		private float twistAngleInitial;

		private float twistAngleCur;

		private float twistAnglePrev;

		private int twistScrollPrev;

		private int twistScrollCur;

		private int pinchScrollPrev;

		private int pinchScrollCur;

		public const int MAX_FINGERS = 3;

		private const float MIN_TWIST_FINGER_DIST_SQ = 0.1f;

		private const float MIN_PINCH_DIST = 0.1f;

		public EmuTouchBinding separateFingersAsEmuTouchesBinding;

		public AnalogConfig twistAnalogConfig;

		public AnalogConfig pinchAnalogConfig;

		public AxisBinding twistAnalogBinding;

		public AxisBinding twistDeltaBinding;

		public DigitalBinding twistRightDigitalBinding;

		public DigitalBinding twistLeftDigitalBinding;

		public ScrollDeltaBinding pinchScrollDeltaBinding;

		public ScrollDeltaBinding twistScrollDeltaBinding;

		public AxisBinding pinchAnalogBinding;

		public AxisBinding pinchDeltaBinding;

		public DigitalBinding pinchDigitalBinding;

		public DigitalBinding spreadDigitalBinding;

		public bool emuWithKeys;

		public KeyCode emuKeyOneFinger;

		public KeyCode emuKeyTwoFingers;

		public KeyCode emuKeyThreeFingers;

		public KeyCode emuKeySwipeU;

		public KeyCode emuKeySwipeR;

		public KeyCode emuKeySwipeD;

		public KeyCode emuKeySwipeL;

		public KeyCode emuKeyTwistR;

		public KeyCode emuKeyTwistL;

		public KeyCode emuKeyPinch;

		public KeyCode emuKeySpread;

		public KeyCode emuMouseTwoFingersKey = KeyCode.LeftControl;

		public KeyCode emuMouseTwistKey = KeyCode.LeftShift;

		public KeyCode emuMousePinchKey = KeyCode.LeftShift;

		public KeyCode emuMouseThreeFingersKey;

		public EmuMouseAxis emuMousePinchAxis;

		public EmuMouseAxis emuMouseTwistAxis = EmuMouseAxis.Y;

		public float emuKeySwipeSpeed = 0.25f;

		public float emuKeyPinchSpeed = 0.25f;

		public float emuKeyTwistSpeed = 45f;

		public float mouseEmuTwistScreenFactor = 0.3f;

		private int multiFingerCountCur;

		private int multiFingerCountPrev;

		private const float MIN_EMU_FINGER_DIST_FACTOR = 0.1f;

		private const float MAX_EMU_FINGER_DIST_FACTOR = 0.9f;

		private EmulatedFingerSystem emulatedFingers;

		public SuperTouchZone()
		{
			strictMultiTouchTime = 0.5f;
			customThresh = new MultiTouchGestureThresholds();
			fingers = new FingerState[3];
			fingersOrdered = new FingerState[3];
			for (int i = 0; i < 3; i++)
			{
				fingers[i] = new FingerState(this);
				fingersOrdered[i] = fingers[i];
			}
			multiFingerTouch = new MultiFingerTouch[3];
			for (int j = 0; j < multiFingerTouch.Length; j++)
			{
				multiFingerTouch[j] = new MultiFingerTouch(j + 1, this);
			}
			multiFingerConfigs = new MultiFingerTouchConfig[3];
			for (int k = 0; k < multiFingerConfigs.Length; k++)
			{
				multiFingerConfigs[k] = new MultiFingerTouchConfig();
			}
			separateFingersAsEmuTouchesBinding = new EmuTouchBinding();
			twistAnalogBinding = new AxisBinding();
			twistDeltaBinding = new AxisBinding();
			twistLeftDigitalBinding = new DigitalBinding();
			twistRightDigitalBinding = new DigitalBinding();
			pinchAnalogBinding = new AxisBinding();
			pinchDeltaBinding = new AxisBinding();
			pinchDigitalBinding = new DigitalBinding();
			spreadDigitalBinding = new DigitalBinding();
			pinchScrollDeltaBinding = new ScrollDeltaBinding();
			twistScrollDeltaBinding = new ScrollDeltaBinding();
			pinchAnalogConfig = new AnalogConfig();
			twistAnalogConfig = new AnalogConfig();
			pinchDistCur = 0.1f;
			pinchDistPrev = 0.1f;
			pinchDistInitial = 0.1f;
			pinchDistQuietInitial = 0.1f;
			InitEmulatedFingers();
		}

		protected override void OnDestroyControl()
		{
			ResetControl();
		}

		protected override void OnInitControl()
		{
			for (int i = 0; i < multiFingerTouch.Length; i++)
			{
				multiFingerTouch[i].Init((i >= multiFingerConfigs.Length) ? null : multiFingerConfigs[i]);
			}
			SetTouchSmoothing(touchSmoothing);
			ResetControl();
		}

		public override void ResetControl()
		{
			ReleaseAllTouches();
			for (int i = 0; i < multiFingerTouch.Length; i++)
			{
				multiFingerTouch[i].Reset();
			}
			for (int j = 0; j < fingers.Length; j++)
			{
				fingers[j].Reset();
			}
			twistMoved = false;
			twistJustMoved = false;
			twistAngleCur = 0f;
			twistAngleInitial = 0f;
			twistAnglePrev = 0f;
			twistAngleQuietInitial = 0f;
			pinchMoved = false;
			pinchJustMoved = false;
			pinchDistCur = 0.1f;
			pinchDistPrev = 0.1f;
			pinchDistInitial = 0.1f;
			pinchDistQuietInitial = 0.1f;
			pinchScrollCur = 0;
			pinchScrollPrev = 0;
			twistScrollCur = 0;
			twistScrollPrev = 0;
		}

		public bool PressedRaw(int fingerCount)
		{
			return GetMultiFingerTouch(fingerCount)?.touchScreen.PressedRaw() ?? false;
		}

		public bool JustPressedRaw(int fingerCount)
		{
			return GetMultiFingerTouch(fingerCount)?.touchScreen.JustPressedRaw() ?? false;
		}

		public bool JustReleasedRaw(int fingerCount)
		{
			return GetMultiFingerTouch(fingerCount)?.touchScreen.JustReleasedRaw() ?? false;
		}

		public bool PressedNormal(int fingerCount)
		{
			return GetMultiFingerTouch(fingerCount)?.touchScreen.PressedNormal() ?? false;
		}

		public bool JustPressedNormal(int fingerCount)
		{
			return GetMultiFingerTouch(fingerCount)?.touchScreen.JustPressedNormal() ?? false;
		}

		public bool JustReleasedNormal(int fingerCount)
		{
			return GetMultiFingerTouch(fingerCount)?.touchScreen.JustReleasedNormal() ?? false;
		}

		public bool PressedLong(int fingerCount)
		{
			return GetMultiFingerTouch(fingerCount)?.touchScreen.PressedLong() ?? false;
		}

		public bool JustPressedLong(int fingerCount)
		{
			return GetMultiFingerTouch(fingerCount)?.touchScreen.JustPressedLong() ?? false;
		}

		public bool JustReleasedLong(int fingerCount)
		{
			return GetMultiFingerTouch(fingerCount)?.touchScreen.JustReleasedLong() ?? false;
		}

		public bool JustTapped(int fingerCount, int howManyTimes)
		{
			return GetMultiFingerTouch(fingerCount)?.touchScreen.JustTapped(howManyTimes) ?? false;
		}

		public bool JustLongTapped(int fingerCount)
		{
			return GetMultiFingerTouch(fingerCount)?.touchScreen.JustLongTapped() ?? false;
		}

		public bool SwipeActive(int fingerCount)
		{
			return GetMultiFingerTouch(fingerCount)?.touchOriented.Swiped() ?? false;
		}

		public bool SwipeJustActivated(int fingerCount)
		{
			return GetMultiFingerTouch(fingerCount)?.touchOriented.JustSwiped() ?? false;
		}

		public Vector2 GetCurPos(int fingerCount)
		{
			return GetMultiFingerTouch(fingerCount)?.touchScreen.GetCurPosRaw() ?? Vector2.zero;
		}

		public Vector2 GetStartPos(int fingerCount)
		{
			return GetMultiFingerTouch(fingerCount)?.touchScreen.GetStartPos() ?? Vector2.zero;
		}

		public Vector2 GetTapPos(int fingerCount)
		{
			return GetMultiFingerTouch(fingerCount)?.touchScreen.GetTapPos() ?? Vector2.zero;
		}

		public Vector2 GetRawSwipeVector(int fingerCount)
		{
			return GetMultiFingerTouch(fingerCount)?.touchOriented.GetSwipeVecRaw() ?? Vector2.zero;
		}

		public Vector2 GetRawSwipeDelta(int fingerCount)
		{
			return GetMultiFingerTouch(fingerCount)?.touchOriented.GetDeltaVecRaw() ?? Vector2.zero;
		}

		public Vector2 GetUnconstrainedSwipeVector(int fingerCount)
		{
			return GetMultiFingerTouch(fingerCount)?.touchOriented.GetSwipeVecSmooth() ?? Vector2.zero;
		}

		public Vector2 GetUnconstrainedSwipeDelta(int fingerCount)
		{
			return GetMultiFingerTouch(fingerCount)?.touchOriented.GetDeltaVecSmooth() ?? Vector2.zero;
		}

		public Vector2 GetSwipeVector(int fingerCount)
		{
			return GetMultiFingerTouch(fingerCount)?.touchOriented.GetConstrainedSwipeVec() ?? Vector2.zero;
		}

		public Vector2 GetSwipeDelta(int fingerCount)
		{
			return GetMultiFingerTouch(fingerCount)?.touchOriented.GetConstrainedDeltaVec() ?? Vector2.zero;
		}

		public Dir GetSwipeDir(int fingerCount)
		{
			return GetMultiFingerTouch(fingerCount)?.touchOriented.GetSwipeDirState().GetCur() ?? Dir.N;
		}

		public Vector2 GetScroll(int fingerCount)
		{
			return GetMultiFingerTouch(fingerCount)?.touchOriented.GetScroll() ?? Vector2.zero;
		}

		public Vector2 GetScrollDelta(int fingerCount)
		{
			return GetMultiFingerTouch(fingerCount)?.touchOriented.GetScrollDelta() ?? Vector2.zero;
		}

		public bool PinchActive()
		{
			return pinchMoved;
		}

		public bool PinchJustActivated()
		{
			return pinchJustMoved;
		}

		public float GetPinchScale()
		{
			return pinchDistCur / pinchDistInitial;
		}

		public float GetPinchScaleDelta()
		{
			return pinchDistCur / pinchDistPrev;
		}

		public float GetPinchDist()
		{
			return pinchDistCur - pinchDistInitial;
		}

		public float GetPinchDistDelta()
		{
			return pinchDistCur - pinchDistPrev;
		}

		public bool TwistActive()
		{
			return twistMoved;
		}

		public bool TwistJustActivated()
		{
			return twistJustMoved;
		}

		public float GetTwist()
		{
			return twistAngleCur;
		}

		public float GetTwistDelta()
		{
			return twistAngleCur - twistAnglePrev;
		}

		public int GetTwistScrollDelta()
		{
			return twistScrollCur - twistScrollPrev;
		}

		public int GetPinchScrollDelta()
		{
			return pinchScrollCur - pinchScrollPrev;
		}

		public float GetReleasedDuration(int fingerCount)
		{
			return GetMultiFingerTouch(fingerCount)?.touchScreen.GetReleasedDurationRaw() ?? 0f;
		}

		public void SetTouchSmoothing(float smPow)
		{
			touchSmoothing = Mathf.Clamp01(smPow);
			for (int i = 0; i < fingers.Length; i++)
			{
				fingers[i].touchScreen.SetSmoothingTime(touchSmoothing * 0.1f);
				fingers[i].touchOriented.SetSmoothingTime(touchSmoothing * 0.1f);
			}
			for (int j = 0; j < multiFingerTouch.Length; j++)
			{
				multiFingerTouch[j].touchScreen.SetSmoothingTime(touchSmoothing * 0.1f);
				multiFingerTouch[j].touchOriented.SetSmoothingTime(touchSmoothing * 0.1f);
			}
		}

		protected MultiFingerTouch GetMultiFingerTouch(int fingerCount)
		{
			if (fingerCount <= 0 || fingerCount > multiFingerTouch.Length)
			{
				return null;
			}
			return multiFingerTouch[fingerCount - 1];
		}

		public MultiTouchGestureThresholds GetThresholds()
		{
			return customThresh;
		}

		private Vector2 GetAveragedFingerPos(FingerState[] fingers, int count, bool orientedSpace, bool useSmoothPos)
		{
			count = Mathf.Clamp(count, 0, fingers.Length);
			Vector2 vector = Vector2.zero;
			Vector2 vector2 = Vector2.zero;
			for (int i = 0; i < count; i++)
			{
				TouchGestureBasicState touchGestureBasicState = (!orientedSpace) ? fingers[i].touchScreen : fingers[i].touchOriented;
				Vector2 vector3 = (!useSmoothPos) ? touchGestureBasicState.GetCurPosRaw() : touchGestureBasicState.GetCurPosSmooth();
				if (i == 0)
				{
					vector = (vector2 = vector3);
					continue;
				}
				vector = Vector2.Min(vector, vector3);
				vector2 = Vector2.Max(vector2, vector3);
			}
			return vector + (vector2 - vector) * 0.5f;
		}

		private void StartMultiFingerTouch(int fingerNum, FingerState[] fingers, Vector2 startScreenPos, Vector2 startOrientedPos, bool quietMode)
		{
			for (int i = 0; i < this.multiFingerTouch.Length; i++)
			{
				MultiFingerTouch multiFingerTouch = this.multiFingerTouch[i];
				if (multiFingerTouch.GetFingerCount() == fingerNum)
				{
					multiFingerTouch.Start(fingers, startScreenPos, startOrientedPos, quietMode, elapsedSinceFirstTouch);
					if (multiFingerTouch.GetFingerCount() == 2)
					{
						pinchDistQuietInitial = GetTwoFingerDist();
						twistAngleQuietInitial = GetTwoFingerAngle(0f);
					}
					continue;
				}
				if (fingerNum > multiFingerTouch.GetFingerCount())
				{
					multiFingerTouch.End(cancel: true);
				}
				else
				{
					multiFingerTouch.EndAndReport();
				}
				if (fingerNum > 0 && multiFingerTouch.GetFingerCount() < fingerNum)
				{
					multiFingerTouch.touchScreen.CancelTap();
					multiFingerTouch.touchOriented.CancelTap();
				}
			}
		}

		private void CancelTapsOnMultiFingersOtherThan(int howManyFingers)
		{
			for (int i = 0; i < this.multiFingerTouch.Length; i++)
			{
				MultiFingerTouch multiFingerTouch = this.multiFingerTouch[i];
				if (multiFingerTouch.GetFingerCount() != howManyFingers)
				{
					multiFingerTouch.touchOriented.CancelTap();
					multiFingerTouch.touchScreen.CancelTap();
				}
			}
		}

		protected override void OnUpdateControl()
		{
			UpdateEmulatedFingers();
			rawFingersPressedPrev = rawFingersPressedCur;
			rawFingersPressedCur = 0;
			bool flag = false;
			for (int i = 0; i < fingers.Length; i++)
			{
				FingerState fingerState = fingers[i];
				fingerState.Update();
				if (fingerState.touchScreen.PressedRaw())
				{
					fingersOrdered[rawFingersPressedCur] = fingerState;
					rawFingersPressedCur++;
				}
				if (fingerState.touchScreen.JustReleasedRaw())
				{
					flag = true;
				}
			}
			Vector2 averagedFingerPos = GetAveragedFingerPos(fingersOrdered, rawFingersPressedCur, orientedSpace: false, useSmoothPos: false);
			Vector2 averagedFingerPos2 = GetAveragedFingerPos(fingersOrdered, rawFingersPressedCur, orientedSpace: true, useSmoothPos: false);
			if (rawFingersPressedCur > 0 && base.rig != null)
			{
				base.rig.WakeTouchControlsUp();
			}
			if (rawFingersPressedCur == 0)
			{
				elapsedSinceFirstTouch = 0f;
				dontAllowNewFingers = false;
			}
			if (!strictMultiTouch || maxFingers <= 1)
			{
				if (rawFingersPressedCur != rawFingersPressedPrev)
				{
					elapsedSinceFirstTouch = 0f;
					StartMultiFingerTouch(rawFingersPressedCur, fingersOrdered, averagedFingerPos, averagedFingerPos2, quietMode: false);
					dontAllowNewFingers = false;
					waitingForMoreFingers = false;
					if (rawFingersPressedCur < rawFingersPressedPrev)
					{
						GetMultiFingerTouch(rawFingersPressedCur)?.DisableTapUntilRelease();
					}
				}
			}
			else if (rawFingersPressedCur != rawFingersPressedPrev || flag)
			{
				if (rawFingersPressedCur < rawFingersPressedPrev || flag)
				{
					dontAllowNewFingers = (rawFingersPressedCur > 0);
					StartMultiFingerTouch(0, fingersOrdered, averagedFingerPos, averagedFingerPos2, quietMode: false);
				}
				else
				{
					if (rawFingersPressedPrev == 0)
					{
						waitingForMoreFingers = true;
						elapsedSinceFirstTouch = 0f;
					}
					StartMultiFingerTouch(rawFingersPressedCur, fingersOrdered, averagedFingerPos, averagedFingerPos2, strictMultiTouch);
				}
			}
			else if (strictMultiTouch && waitingForMoreFingers && rawFingersPressedCur > 0)
			{
				elapsedSinceFirstTouch += CFUtils.realDeltaTime;
				if (elapsedSinceFirstTouch > strictMultiTouchTime)
				{
					waitingForMoreFingers = false;
					dontAllowNewFingers = true;
				}
			}
			for (int j = 0; j < this.multiFingerTouch.Length; j++)
			{
				MultiFingerTouch multiFingerTouch = this.multiFingerTouch[j];
				if (multiFingerTouch.GetFingerCount() == rawFingersPressedCur)
				{
					multiFingerTouch.SetPos(averagedFingerPos, averagedFingerPos2);
				}
				multiFingerTouch.Update();
				if (multiFingerTouch.touchScreen.JustPressedRaw())
				{
					CancelTapsOnMultiFingersOtherThan(multiFingerTouch.GetFingerCount());
				}
			}
			UpdateTwistAndPinch(lastUpdateMode: false);
			if (separateFingersAsEmuTouchesBinding.enabled)
			{
				for (int k = 0; k < fingers.Length; k++)
				{
					base.rig.SyncEmuTouch(fingers[k].touchScreen, ref fingers[k].emuTouchId);
				}
			}
			for (int l = 0; l < this.multiFingerTouch.Length; l++)
			{
				this.multiFingerTouch[l].SyncToRig();
			}
			MultiTouchGestureThresholds thresholds = GetThresholds();
			if (thresholds == null)
			{
				return;
			}
			pinchScrollDeltaBinding.SyncScrollDelta(GetPinchScrollDelta(), base.rig);
			twistScrollDeltaBinding.SyncScrollDelta(GetTwistScrollDelta(), base.rig);
			if (PinchActive())
			{
				if (pinchAnalogBinding.enabled)
				{
					pinchAnalogBinding.SyncFloat(pinchAnalogConfig.GetAnalogVal(GetPinchDist() / thresholds.pinchAnalogRangePx), InputRig.InputSource.Analog, base.rig);
				}
				if (pinchDeltaBinding.enabled)
				{
					pinchDeltaBinding.SyncFloat(GetPinchDistDelta() / thresholds.pinchDeltaRangePx, InputRig.InputSource.NormalizedDelta, base.rig);
				}
				if (GetPinchDist() > thresholds.pinchDigitalThreshPx)
				{
					spreadDigitalBinding.Sync(state: true, base.rig);
				}
				else if (GetPinchDist() < thresholds.pinchDigitalThreshPx)
				{
					pinchDigitalBinding.Sync(state: true, base.rig);
				}
			}
			if (TwistActive())
			{
				if (twistAnalogBinding.enabled)
				{
					twistAnalogBinding.SyncFloat(twistAnalogConfig.GetAnalogVal(GetTwist() / thresholds.twistAnalogRange), InputRig.InputSource.Analog, base.rig);
				}
				if (twistDeltaBinding.enabled)
				{
					twistDeltaBinding.SyncFloat(GetTwistDelta() / thresholds.twistDeltaRange, InputRig.InputSource.NormalizedDelta, base.rig);
				}
				if (GetTwist() > thresholds.twistDigitalThresh)
				{
					twistRightDigitalBinding.Sync(state: true, base.rig);
				}
				if (GetTwist() < thresholds.twistDigitalThresh)
				{
					twistLeftDigitalBinding.Sync(state: true, base.rig);
				}
			}
		}

		private float GetTwoFingerDist()
		{
			MultiFingerTouch multiFingerTouch = GetMultiFingerTouch(2);
			FingerState finger2;
			FingerState finger;
			if (multiFingerTouch == null || (finger = multiFingerTouch.GetFinger(0)) == null || (finger2 = multiFingerTouch.GetFinger(1)) == null)
			{
				return 0f;
			}
			return Mathf.Max(0.1f, (finger.touchScreen.GetCurPosSmooth() - finger2.touchScreen.GetCurPosSmooth()).magnitude);
		}

		private float GetTwoFingerAngle(float defaultAngle)
		{
			MultiFingerTouch multiFingerTouch = GetMultiFingerTouch(2);
			FingerState finger2;
			FingerState finger;
			if (multiFingerTouch == null || (finger = multiFingerTouch.GetFinger(0)) == null || (finger2 = multiFingerTouch.GetFinger(1)) == null)
			{
				return defaultAngle;
			}
			Vector2 vector = finger.touchScreen.GetCurPosSmooth() - finger2.touchScreen.GetCurPosSmooth();
			float sqrMagnitude = vector.sqrMagnitude;
			if (sqrMagnitude <= 0.1f)
			{
				return defaultAngle;
			}
			float target = CFUtils.VecToAngle(vector.normalized);
			return defaultAngle + Mathf.DeltaAngle(defaultAngle, target);
		}

		private void UpdateTwistAndPinch()
		{
			UpdateTwistAndPinch(lastUpdateMode: false);
		}

		private void UpdateTwistAndPinch(bool lastUpdateMode)
		{
			if (lastUpdateMode)
			{
				return;
			}
			pinchDistPrev = pinchDistCur;
			twistAnglePrev = twistAngleCur;
			twistScrollPrev = twistScrollCur;
			pinchScrollPrev = pinchScrollCur;
			pinchJustMoved = false;
			twistJustMoved = false;
			MultiTouchGestureThresholds thresholds = GetThresholds();
			MultiFingerTouch multiFingerTouch = GetMultiFingerTouch(2);
			if (multiFingerTouch == null)
			{
				return;
			}
			if (multiFingerTouch.touchScreen.JustPressedRaw())
			{
				pinchDistInitial = pinchDistQuietInitial;
				pinchDistPrev = (pinchDistCur = pinchDistInitial);
				pinchJustMoved = false;
				pinchMoved = false;
				twistAngleInitial = twistAngleQuietInitial;
				twistAngleCur = 0f;
				twistAnglePrev = 0f;
				twistJustMoved = false;
				twistMoved = false;
				twistScrollCur = (twistScrollPrev = 0);
				pinchScrollCur = (pinchScrollPrev = 0);
			}
			else if (!multiFingerTouch.touchScreen.PressedRaw())
			{
				pinchDistInitial = pinchDistCur;
				twistAngleInitial = twistAngleCur;
				twistJustMoved = false;
				twistMoved = false;
				pinchMoved = false;
				pinchJustMoved = false;
			}
			else
			{
				if (!multiFingerTouch.touchScreen.PressedRaw())
				{
					return;
				}
				bool flag = false;
				bool flag2 = false;
				bool flag3 = multiFingerTouch.touchScreen.JustSwiped();
				pinchDistCur = GetTwoFingerDist();
				if (!pinchMoved && Mathf.Abs(pinchDistInitial - pinchDistCur) > thresholds.pinchDistThreshPx)
				{
					flag2 = true;
				}
				twistAngleCur = GetTwoFingerAngle(twistAngleCur) - twistAngleInitial;
				if (!twistMoved && pinchDistCur > thresholds.twistMinDistPx && Mathf.Abs(twistAngleCur) > thresholds.twistAngleThresh)
				{
					flag = true;
				}
				pinchScrollCur = CFUtils.GetScrollValue(pinchDistInitial - pinchDistCur, pinchScrollCur, thresholds.pinchScrollStepPx, thresholds.pinchScrollMagnet);
				twistScrollCur = CFUtils.GetScrollValue(twistAngleCur, twistScrollCur, thresholds.twistScrollStep, thresholds.twistScrollMagnet);
				bool flag4 = false;
				int num = 0;
				switch (gestureDetectionOrder)
				{
				case GestureDetectionOrder.TwistPinchSwipe:
					num = 136;
					break;
				case GestureDetectionOrder.TwistSwipePinch:
					num = 80;
					break;
				case GestureDetectionOrder.PinchTwistSwipe:
					num = 129;
					break;
				case GestureDetectionOrder.PinchSwipeTwist:
					num = 17;
					break;
				case GestureDetectionOrder.SwipeTwistPinch:
					num = 66;
					break;
				case GestureDetectionOrder.SwipePinchTwist:
					num = 10;
					break;
				}
				for (int i = 0; i < 3; i++)
				{
					switch ((num >> ((i * 3) & 0x1F)) & 7)
					{
					case 0:
						if (twistMoved || flag)
						{
							if (noDragAfterTwist)
							{
								flag4 = true;
							}
							if (noPinchAfterTwist)
							{
								flag2 = false;
							}
						}
						break;
					case 1:
						if (pinchMoved || flag2)
						{
							if (noDragAfterPinch)
							{
								flag4 = true;
							}
							if (noTwistAfterPinch)
							{
								flag = false;
							}
						}
						break;
					case 2:
						if (multiFingerTouch.touchScreen.Swiped() || flag3)
						{
							if (noTwistAfterDrag)
							{
								flag = false;
							}
							if (noPinchAfterDrag)
							{
								flag2 = false;
							}
						}
						break;
					}
				}
				if (flag4)
				{
					multiFingerTouch.touchScreen.BlockSwipe();
					multiFingerTouch.touchOriented.BlockSwipe();
					flag3 = false;
				}
				if (flag3)
				{
					OnTwoFingerDragStart();
				}
				if (flag2)
				{
					OnPinchStart();
				}
				if (flag)
				{
					OnTwistStart();
				}
			}
		}

		private void OnPinchStart()
		{
			if (!pinchMoved)
			{
				pinchMoved = true;
				pinchJustMoved = true;
				if (startDragWhenPinching)
				{
					OnTwoFingerDragStart();
				}
				if (startTwistWhenPinching)
				{
					OnTwistStart();
				}
			}
		}

		private void OnTwistStart()
		{
			if (!twistMoved)
			{
				twistMoved = true;
				twistJustMoved = true;
				if (startDragWhenTwisting)
				{
					OnTwoFingerDragStart();
				}
				if (startPinchWhenTwisting)
				{
					OnPinchStart();
				}
			}
		}

		private void OnTwoFingerDragStart()
		{
			MultiFingerTouch multiFingerTouch = GetMultiFingerTouch(2);
			if (multiFingerTouch != null && !multiFingerTouch.touchScreen.Swiped())
			{
				multiFingerTouch.touchScreen.ForceSwipe();
				multiFingerTouch.touchOriented.ForceSwipe();
				if (startTwistWhenDragging)
				{
					OnTwistStart();
				}
				if (startPinchWhenDragging)
				{
					OnPinchStart();
				}
			}
		}

		private FingerState GetFirstUnusedFinger(TouchObject newTouchObj)
		{
			if (strictMultiTouch && dontAllowNewFingers)
			{
				return null;
			}
			int num = Mathf.Min(maxFingers, fingers.Length);
			for (int i = 0; i < num; i++)
			{
				FingerState fingerState = fingers[i];
				if (fingerState.touchObj == null)
				{
					return fingerState;
				}
			}
			return null;
		}

		private FingerState GetFingerByTouch(TouchObject touchObj)
		{
			if (touchObj == null)
			{
				return null;
			}
			for (int i = 0; i < fingers.Length; i++)
			{
				if (touchObj == fingers[i].touchObj)
				{
					return fingers[i];
				}
			}
			return null;
		}

		public override bool CanBeActivatedByOtherControl(TouchControl c, TouchObject touchObj)
		{
			return base.CanBeActivatedByOtherControl(c, touchObj) && GetFirstUnusedFinger(touchObj) != null;
		}

		public override bool CanBeTouchedDirectly(TouchObject touchObj)
		{
			return base.CanBeTouchedDirectly(touchObj) && GetFirstUnusedFinger(touchObj) != null;
		}

		public override bool CanBeSwipedOverFromNothing(TouchObject touchObj)
		{
			return CanBeSwipedOverFromNothingDefault(touchObj) && GetFirstUnusedFinger(touchObj) != null;
		}

		public override bool CanBeSwipedOverFromRestrictedList(TouchObject touchObj)
		{
			return CanBeSwipedOverFromRestrictedListDefault(touchObj) && GetFirstUnusedFinger(touchObj) != null;
		}

		public override bool CanSwipeOverOthers(TouchObject touchObj)
		{
			if (swipeOverOthersMode == SwipeOverOthersMode.Disabled)
			{
				return false;
			}
			FingerState fingerByTouch = GetFingerByTouch(touchObj);
			return fingerByTouch != null && CanSwipeOverOthersDefault(touchObj, touchObj, fingerByTouch.touchStartType);
		}

		public override bool OnTouchStart(TouchObject touch, TouchControl sender, TouchStartType touchStartType)
		{
			if (!base.IsInitialized)
			{
				return false;
			}
			FingerState firstUnusedFinger = GetFirstUnusedFinger(touch);
			if (firstUnusedFinger == null)
			{
				return false;
			}
			if (emulatedFingers.OnSystemTouchStart(touch, sender, touchStartType))
			{
				return true;
			}
			if (!firstUnusedFinger.OnTouchStart(touch, 0f, touchStartType))
			{
				return false;
			}
			touch.AddControl(this);
			return true;
		}

		public override bool OnTouchEnd(TouchObject touch, TouchEndType touchEndType)
		{
			if (!base.IsInitialized)
			{
				return false;
			}
			if (emulatedFingers.OnSystemTouchEnd(touch, touchEndType))
			{
				return true;
			}
			for (int i = 0; i < fingers.Length; i++)
			{
				if (fingers[i].OnTouchEnd(touch, touchEndType))
				{
					return true;
				}
			}
			return false;
		}

		public override bool OnTouchMove(TouchObject touch)
		{
			if (!base.IsInitialized)
			{
				return false;
			}
			if (emulatedFingers.OnSystemTouchMove(touch))
			{
				return true;
			}
			for (int i = 0; i < fingers.Length; i++)
			{
				if (fingers[i].OnTouchMove(touch))
				{
					return true;
				}
			}
			return false;
		}

		public override bool OnTouchPressureChange(TouchObject touch)
		{
			for (int i = 0; i < fingers.Length; i++)
			{
				if (fingers[i].OnTouchPressureChange(touch))
				{
					return true;
				}
			}
			return false;
		}

		public override void ReleaseAllTouches()
		{
			if (base.IsInitialized)
			{
				for (int i = 0; i < fingers.Length; i++)
				{
					fingers[i].ReleaseTouch(cancel: true);
				}
				for (int j = 0; j < multiFingerTouch.Length; j++)
				{
					multiFingerTouch[j].End(cancel: true);
					multiFingerTouch[j].EndDrivingTouch(cancel: true);
				}
				emulatedFingers.EndMouseAndTouches(cancel: true);
				rawFingersPressedCur = 0;
			}
		}

		protected override bool OnIsBoundToAxis(string axisName, InputRig rig)
		{
			for (int i = 0; i < Mathf.Min(maxFingers, 3); i++)
			{
				if (multiFingerConfigs[i].binding.IsBoundToAxis(axisName, rig))
				{
					return true;
				}
			}
			return twistAnalogBinding.IsBoundToAxis(axisName, rig) || twistDeltaBinding.IsBoundToAxis(axisName, rig) || twistLeftDigitalBinding.IsBoundToAxis(axisName, rig) || twistRightDigitalBinding.IsBoundToAxis(axisName, rig) || pinchAnalogBinding.IsBoundToAxis(axisName, rig) || pinchDeltaBinding.IsBoundToAxis(axisName, rig) || pinchDigitalBinding.IsBoundToAxis(axisName, rig) || spreadDigitalBinding.IsBoundToAxis(axisName, rig) || pinchScrollDeltaBinding.IsBoundToAxis(axisName, rig) || twistScrollDeltaBinding.IsBoundToAxis(axisName, rig);
		}

		protected override bool OnIsBoundToKey(KeyCode key, InputRig rig)
		{
			for (int i = 0; i < Mathf.Min(maxFingers, 3); i++)
			{
				if (multiFingerConfigs[i].binding.IsBoundToKey(key, rig))
				{
					return true;
				}
			}
			return twistAnalogBinding.IsBoundToKey(key, rig) || twistDeltaBinding.IsBoundToKey(key, rig) || twistLeftDigitalBinding.IsBoundToKey(key, rig) || twistRightDigitalBinding.IsBoundToKey(key, rig) || pinchAnalogBinding.IsBoundToKey(key, rig) || pinchDigitalBinding.IsBoundToKey(key, rig) || spreadDigitalBinding.IsBoundToKey(key, rig) || pinchScrollDeltaBinding.IsBoundToKey(key, rig) || twistScrollDeltaBinding.IsBoundToKey(key, rig);
		}

		public override bool IsUsingKeyForEmulation(KeyCode key)
		{
			if (!emuWithKeys)
			{
				return false;
			}
			if (key == emuKeyOneFinger || key == emuKeyTwoFingers || key == emuKeyThreeFingers || key == emuKeyPinch || key == emuKeySpread || key == emuKeyTwistL || key == emuKeyTwistR || key == emuKeySwipeU || key == emuKeySwipeR || key == emuKeySwipeD || key == emuKeySwipeL || key == emuMouseTwoFingersKey || key == emuMouseThreeFingersKey || key == emuMousePinchKey || key == emuMouseTwistKey)
			{
				return true;
			}
			return false;
		}

		protected override bool OnIsEmulatingTouches()
		{
			if (separateFingersAsEmuTouchesBinding.IsEmulatingTouches())
			{
				return true;
			}
			for (int i = 0; i < Mathf.Min(maxFingers, 3); i++)
			{
				if (multiFingerConfigs[i].binding.IsEmulatingTouches())
				{
					return true;
				}
			}
			return false;
		}

		protected override bool OnIsEmulatingMousePosition()
		{
			for (int i = 0; i < Mathf.Min(maxFingers, 3); i++)
			{
				if (multiFingerConfigs[i].binding.IsEmulatingMousePosition())
				{
					return true;
				}
			}
			return false;
		}

		protected override void OnGetSubBindingDescriptions(BindingDescriptionList descList, UnityEngine.Object undoObject, string parentMenuPath)
		{
			for (int i = 0; i < multiFingerConfigs.Length; i++)
			{
				if (descList.addUnusedBindings || i + 1 <= maxFingers)
				{
					descList.Add(multiFingerConfigs[i].binding, (i + 1).ToString() + "-Finger Touch", parentMenuPath, this);
				}
			}
			descList.Add(separateFingersAsEmuTouchesBinding, "Separate Fingers as Emu Touches", parentMenuPath, this);
			if (descList.addUnusedBindings || maxFingers >= 2)
			{
				descList.Add(pinchAnalogBinding, InputRig.InputSource.Analog, "Pinch (Analog)", parentMenuPath, this);
				descList.Add(pinchDeltaBinding, InputRig.InputSource.NormalizedDelta, "Pinch (Analog)", parentMenuPath, this);
				descList.Add(pinchDigitalBinding, "Pinch (Digital)", parentMenuPath, this);
				descList.Add(spreadDigitalBinding, "Spread (Digital)", parentMenuPath, this);
				descList.Add(twistAnalogBinding, InputRig.InputSource.Analog, "Twist (Analog)", parentMenuPath, this);
				descList.Add(twistDeltaBinding, InputRig.InputSource.NormalizedDelta, "Twist (Analog)", parentMenuPath, this);
				descList.Add(twistLeftDigitalBinding, "Twist Left (Digital)", parentMenuPath, this);
				descList.Add(twistRightDigitalBinding, "Twist Right (Digital)", parentMenuPath, this);
				descList.Add(pinchScrollDeltaBinding, "Pinch Scroll Delta", parentMenuPath, this);
				descList.Add(twistScrollDeltaBinding, "Twist Scroll Delta", parentMenuPath, this);
			}
		}

		public void DrawMarkerGUI()
		{
			emulatedFingers.DrawMarkers();
		}

		private void InitEmulatedFingers()
		{
			emulatedFingers = new EmulatedFingerSystem(this);
		}

		private void ReleaseEmulatedFingers(bool cancel)
		{
			emulatedFingers.EndTouches(cancel);
		}

		private void UpdateEmulatedFingers()
		{
			emulatedFingers.Update();
		}
	}
}
