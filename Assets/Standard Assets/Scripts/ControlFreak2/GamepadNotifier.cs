using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace ControlFreak2
{
	public class GamepadNotifier : MonoBehaviour
	{
		public int maxNotifications = 8;

		public float msgDuration = 5f;

		public NotificationElementGUI notificationTemplate;

		public RectTransform notificationListBox;

		private List<NotificationElementGUI> notificationList;

		public string msgUnknownGamepadConnected = "Unknown gamepad {0} has been connected!";

		public string msgKnownGamepadConnected = "Gamepad {0} has been connected!";

		public string msgGamepadDisconnected = "Gamepad {0} at slot {1} has been disconnected.";

		public string msgGamepadActivated = "Gamepad {0} activated at slot {1}.";

		public string msgGamepadDisactivated = "Gamepad {0} disactivated at slot {1}.";

		public Sprite iconUnknownGamepadConnected;

		public Sprite iconKnownGamepadConnected;

		public Sprite iconGamepadDisconnected;

		public Sprite iconGamepadActivated;

		public Sprite iconGamepadDisactivated;

		[CompilerGenerated]
		private static Comparison<NotificationElementGUI> _003C_003Ef__mg_0024cache0;

		public GamepadNotifier()
		{
			notificationList = new List<NotificationElementGUI>(8);
		}

		private void OnEnable()
		{
			if (CFUtils.editorStopped)
			{
				return;
			}
			maxNotifications = Mathf.Max(maxNotifications, 1);
			if (notificationTemplate == null)
			{
				return;
			}
			notificationTemplate.gameObject.SetActive(value: false);
			while (notificationList.Count < maxNotifications)
			{
				notificationList.Add(null);
			}
			for (int i = 0; i < notificationList.Count; i++)
			{
				if (notificationList[i] == null)
				{
					notificationList[i] = UnityEngine.Object.Instantiate(notificationTemplate);
				}
				notificationList[i].End();
				notificationList[i].transform.SetParent(notificationListBox, worldPositionStays: false);
			}
			GamepadManager.onGamepadActivated += OnGamepadActivated;
			GamepadManager.onGamepadConnected += OnGamepadConnected;
			GamepadManager.onGamepadDisconnected += OnGamepadDisconnected;
			GamepadManager.onGamepadDisactivated += OnGamepadDisactivated;
		}

		private void OnDisable()
		{
			if (!CFUtils.editorStopped)
			{
				GamepadManager.onGamepadActivated -= OnGamepadActivated;
				GamepadManager.onGamepadConnected -= OnGamepadConnected;
				GamepadManager.onGamepadDisconnected -= OnGamepadDisconnected;
				GamepadManager.onGamepadDisactivated -= OnGamepadDisactivated;
			}
		}

		private void Update()
		{
			if (GamepadManager.activeManager != null)
			{
			}
			float realDeltaTimeClamped = CFUtils.realDeltaTimeClamped;
			for (int i = 0; i < notificationList.Count; i++)
			{
				if (notificationList[i] != null)
				{
					notificationList[i].UpdateNotification(realDeltaTimeClamped);
				}
			}
		}

		private void OnGamepadConnected(GamepadManager.Gamepad gamepad)
		{
			string profileName = gamepad.GetProfileName();
			if (string.IsNullOrEmpty(profileName))
			{
				AddNotification(string.Format(msgUnknownGamepadConnected, gamepad.GetInternalJoyName()), iconUnknownGamepadConnected);
			}
			else
			{
				AddNotification(string.Format(msgKnownGamepadConnected, gamepad.GetProfileName()), iconKnownGamepadConnected);
			}
		}

		private void OnGamepadActivated(GamepadManager.Gamepad gamepad)
		{
			AddNotification(string.Format(msgGamepadActivated, gamepad.GetProfileName(), gamepad.GetSlot()), iconGamepadDisactivated);
		}

		private void OnGamepadDisconnected(GamepadManager.Gamepad gamepad, GamepadManager.DisconnectionReason reason)
		{
			AddNotification(string.Format(msgGamepadDisconnected, gamepad.GetProfileName(), gamepad.GetSlot()), iconGamepadDisconnected);
		}

		private void OnGamepadDisactivated(GamepadManager.Gamepad gamepad, GamepadManager.DisconnectionReason reason)
		{
		}

		private void AddNotification(string msg, Sprite icon)
		{
			if (string.IsNullOrEmpty(msg))
			{
				return;
			}
			NotificationElementGUI notificationElementGUI = null;
			for (int i = 0; i < notificationList.Count; i++)
			{
				NotificationElementGUI notificationElementGUI2 = notificationList[i];
				if (!(notificationElementGUI2 == null))
				{
					if (!notificationElementGUI2.IsActive())
					{
						notificationElementGUI = notificationElementGUI2;
						break;
					}
					if (notificationElementGUI == null || notificationElementGUI2.GetTimeElapsed() > notificationElementGUI.GetTimeElapsed())
					{
						notificationElementGUI = notificationElementGUI2;
					}
				}
			}
			if (!(notificationElementGUI == null))
			{
				notificationElementGUI.End();
				notificationElementGUI.Show(msg, icon, notificationListBox, msgDuration);
				SortNotifications();
			}
		}

		private void SortNotifications()
		{
			notificationList.Sort(NotificationElementGUI.Compare);
		}
	}
}
