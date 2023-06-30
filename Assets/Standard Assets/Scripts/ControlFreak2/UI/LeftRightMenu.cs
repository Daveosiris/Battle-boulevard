using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace ControlFreak2.UI
{
	public class LeftRightMenu : Selectable
	{
		public delegate void OptionSwitchCallback(int dir);

		public Button buttonLeft;

		public Button buttonRight;

		public Button buttonBack;

		public RectTransform contentPlaceholder;

		public Text titleText;

		public Action onBackPressed;

		public OptionSwitchCallback onOptionSwitch;

		public RectTransform[] optionItems;

		public override void OnSelect(BaseEventData data)
		{
			base.OnSelect(data);
		}

		public override void OnDeselect(BaseEventData data)
		{
			base.OnDeselect(data);
		}

		public override void OnMove(AxisEventData data)
		{
			if (data.moveDir == MoveDirection.Left || data.moveDir == MoveDirection.Right)
			{
				OnSwitch((data.moveDir != 0) ? 1 : (-1));
				data.Use();
			}
			else
			{
				base.OnMove(data);
			}
		}

		public int GetItemCount()
		{
			return optionItems.Length;
		}

		public void SetItemActive(int id)
		{
			for (int i = 0; i < optionItems.Length; i++)
			{
				RectTransform rectTransform = optionItems[i];
				if (!(rectTransform == null))
				{
					rectTransform.gameObject.SetActive(id == i);
				}
			}
		}

		public void SetTitle(string title)
		{
			if (titleText != null)
			{
				titleText.text = title;
			}
		}

		protected override void OnEnable()
		{
			base.OnEnable();
			buttonLeft.onClick.AddListener(OnLeftPressed);
			buttonRight.onClick.AddListener(OnRightPressed);
			buttonBack.onClick.AddListener(OnBackPressed);
			for (int i = 0; i < optionItems.Length; i++)
			{
				RectTransform rectTransform = optionItems[i];
				if (!(rectTransform == null))
				{
					rectTransform.SetParent(contentPlaceholder, worldPositionStays: false);
					rectTransform.gameObject.SetActive(value: false);
				}
			}
		}

		protected override void OnDisable()
		{
			buttonLeft.onClick.RemoveAllListeners();
			buttonRight.onClick.RemoveAllListeners();
			buttonBack.onClick.RemoveAllListeners();
			base.OnDisable();
		}

		private void OnSwitch(int dir)
		{
			if (onOptionSwitch != null)
			{
				onOptionSwitch(dir);
			}
		}

		private void OnBackPressed()
		{
			if (onBackPressed != null)
			{
				onBackPressed();
			}
		}

		private void OnLeftPressed()
		{
			OnSwitch(-1);
		}

		private void OnRightPressed()
		{
			OnSwitch(1);
		}
	}
}
