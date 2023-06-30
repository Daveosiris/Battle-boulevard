using ControlFreak2.Internal;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace ControlFreak2
{
	public class GamepadInputModule : BaseInputModule
	{
		public GamepadManager.GamepadKey submitGamepadButton;

		public GamepadManager.GamepadKey submitGamepadButtonAlt = GamepadManager.GamepadKey.Start;

		public GamepadManager.GamepadKey cancelGamepadButton = GamepadManager.GamepadKey.FaceRight;

		public GamepadManager.GamepadKey cancelGamepadButtonAlt = GamepadManager.GamepadKey.Select;

		public override bool IsModuleSupported()
		{
			return true;
		}

		private bool JustPressedSubmitButton()
		{
			if (UnityEngine.Input.GetKeyDown(KeyCode.Return))
			{
				return true;
			}
			GamepadManager activeManager = GamepadManager.activeManager;
			if (activeManager != null && (activeManager.GetCombinedGamepad().GetKeyDown(submitGamepadButton) || activeManager.GetCombinedGamepad().GetKeyDown(submitGamepadButtonAlt)))
			{
				return true;
			}
			return false;
		}

		private bool JustPressedCancelButton()
		{
			if (UnityEngine.Input.GetKeyDown(KeyCode.Escape))
			{
				return true;
			}
			GamepadManager activeManager = GamepadManager.activeManager;
			if (activeManager != null && (activeManager.GetCombinedGamepad().GetKeyDown(cancelGamepadButton) || activeManager.GetCombinedGamepad().GetKeyDown(cancelGamepadButtonAlt)))
			{
				return true;
			}
			return false;
		}

		private MoveDirection JustPressedDirectionKey()
		{
			bool flag = UnityEngine.Input.GetKeyDown(KeyCode.LeftArrow);
			bool flag2 = UnityEngine.Input.GetKeyDown(KeyCode.RightArrow);
			bool flag3 = UnityEngine.Input.GetKeyDown(KeyCode.UpArrow);
			bool flag4 = UnityEngine.Input.GetKeyDown(KeyCode.DownArrow);
			GamepadManager activeManager = GamepadManager.activeManager;
			if (activeManager != null)
			{
				flag |= (activeManager.GetCombinedGamepad().leftStick.state.JustPressedDir4(Dir.L) || activeManager.GetCombinedGamepad().dpad.state.JustPressedDir4(Dir.L));
				flag2 |= (activeManager.GetCombinedGamepad().leftStick.state.JustPressedDir4(Dir.R) || activeManager.GetCombinedGamepad().dpad.state.JustPressedDir4(Dir.R));
				flag3 |= (activeManager.GetCombinedGamepad().leftStick.state.JustPressedDir4(Dir.U) || activeManager.GetCombinedGamepad().dpad.state.JustPressedDir4(Dir.U));
				flag4 |= (activeManager.GetCombinedGamepad().leftStick.state.JustPressedDir4(Dir.D) || activeManager.GetCombinedGamepad().dpad.state.JustPressedDir4(Dir.D));
			}
			if (flag && flag2)
			{
				flag = (flag2 = false);
			}
			if (flag3 && flag4)
			{
				flag3 = (flag4 = false);
			}
			return flag3 ? MoveDirection.Up : (flag4 ? MoveDirection.Down : ((!flag) ? ((!flag2) ? MoveDirection.None : MoveDirection.Right) : MoveDirection.Left));
		}

		public override bool ShouldActivateModule()
		{
			if (!base.ShouldActivateModule())
			{
				return false;
			}
			return JustPressedCancelButton() || JustPressedSubmitButton() || JustPressedDirectionKey() != MoveDirection.None;
		}

		public override void ActivateModule()
		{
			base.ActivateModule();
			GameObject currentSelectedGameObject = base.eventSystem.currentSelectedGameObject;
			if (currentSelectedGameObject != null)
			{
				base.eventSystem.SetSelectedGameObject(null, GetBaseEventData());
				base.eventSystem.SetSelectedGameObject(currentSelectedGameObject, GetBaseEventData());
				SendMoveEventToSelectedObject();
				return;
			}
			currentSelectedGameObject = ((!(currentSelectedGameObject == null)) ? FindFirstSelectableInScene() : base.eventSystem.firstSelectedGameObject);
			if (currentSelectedGameObject != null)
			{
				base.eventSystem.SetSelectedGameObject(null, GetBaseEventData());
				base.eventSystem.SetSelectedGameObject(currentSelectedGameObject, GetBaseEventData());
			}
		}

		private GameObject FindFirstSelectableInScene()
		{
			Selectable selectable = null;
			Selectable[] array = (Selectable[])Object.FindObjectsOfType(typeof(Selectable));
			foreach (Selectable selectable2 in array)
			{
				if (selectable2.navigation.mode != 0)
				{
					selectable = selectable2;
				}
			}
			return (!(selectable != null)) ? null : selectable.gameObject;
		}

		public override void DeactivateModule()
		{
			base.DeactivateModule();
		}

		public override void Process()
		{
			bool flag = SendUpdateEventToSelectedObject();
			if (base.eventSystem.sendNavigationEvents)
			{
				if (!flag)
				{
					flag |= SendMoveEventToSelectedObject();
				}
				if (!flag)
				{
					SendSubmitEventToSelectedObject();
				}
			}
		}

		protected bool SendSubmitEventToSelectedObject()
		{
			if (base.eventSystem.currentSelectedGameObject == null)
			{
				return false;
			}
			GamepadManager activeManager = GamepadManager.activeManager;
			if (activeManager == null)
			{
				return false;
			}
			BaseEventData baseEventData = GetBaseEventData();
			if (JustPressedSubmitButton())
			{
				ExecuteEvents.Execute(base.eventSystem.currentSelectedGameObject, baseEventData, ExecuteEvents.submitHandler);
			}
			if (JustPressedCancelButton())
			{
				ExecuteEvents.Execute(base.eventSystem.currentSelectedGameObject, baseEventData, ExecuteEvents.cancelHandler);
			}
			return baseEventData.used;
		}

		protected bool SendMoveEventToSelectedObject()
		{
			GamepadManager activeManager = GamepadManager.activeManager;
			if (activeManager == null)
			{
				return false;
			}
			JoystickState stick = activeManager.GetCombinedGamepad().GetStick(GamepadManager.GamepadStick.LeftAnalog);
			JoystickState stick2 = activeManager.GetCombinedGamepad().GetStick(GamepadManager.GamepadStick.Dpad);
			Dir dir = Dir.N;
			if (stick.JustReleasedDir4(Dir.N))
			{
				dir = stick.GetDir4();
			}
			else if (stick2.JustReleasedDir4(Dir.N))
			{
				dir = stick2.GetDir4();
			}
			Vector2 vector = CFUtils.DirToVector(dir, circular: false);
			Vector2 vector2 = new Vector2(UnityEngine.Input.GetKeyDown(KeyCode.RightArrow) ? 1f : ((!Input.GetKeyDown(KeyCode.LeftArrow)) ? 0f : (-1f)), UnityEngine.Input.GetKeyDown(KeyCode.UpArrow) ? 1f : ((!Input.GetKeyDown(KeyCode.DownArrow)) ? 0f : (-1f)));
			vector.x = CFUtils.ApplyDeltaInput(vector.x, vector2.x);
			vector.y = CFUtils.ApplyDeltaInput(vector.y, vector2.y);
			if (vector.sqrMagnitude < 1E-05f)
			{
				return false;
			}
			AxisEventData axisEventData = GetAxisEventData(vector.x, vector.y, 0.3f);
			ExecuteEvents.Execute(base.eventSystem.currentSelectedGameObject, axisEventData, ExecuteEvents.moveHandler);
			if (base.eventSystem.currentSelectedGameObject != null)
			{
				base.eventSystem.firstSelectedGameObject = base.eventSystem.currentSelectedGameObject;
			}
			return axisEventData.used;
		}

		protected bool SendUpdateEventToSelectedObject()
		{
			if (base.eventSystem.currentSelectedGameObject == null)
			{
				return false;
			}
			BaseEventData baseEventData = GetBaseEventData();
			ExecuteEvents.Execute(base.eventSystem.currentSelectedGameObject, baseEventData, ExecuteEvents.updateSelectedHandler);
			return baseEventData.used;
		}
	}
}
