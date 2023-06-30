using UnityEngine;
using UnityEngine.UI;

namespace ControlFreak2
{
	public class NotificationElementGUI : MonoBehaviour
	{
		public Text targetText;

		public Image targetIcon;

		private float duration;

		private float elapsed;

		public void End()
		{
			base.gameObject.SetActive(value: false);
		}

		public void Show(string msg, Sprite icon, RectTransform parent, float duration)
		{
			RectTransform rectTransform = (RectTransform)base.transform;
			rectTransform.SetParent(parent, worldPositionStays: false);
			rectTransform.SetSiblingIndex(0);
			base.gameObject.SetActive(value: true);
			this.duration = duration;
			elapsed = 0f;
			targetText.text = msg;
			if (targetIcon != null)
			{
				bool flag = icon != null;
				targetIcon.enabled = flag;
				if (flag)
				{
					targetIcon.sprite = icon;
				}
			}
		}

		public void UpdateNotification(float dt)
		{
			if (IsActive())
			{
				elapsed += dt;
				if (elapsed > duration)
				{
					base.gameObject.SetActive(value: false);
				}
				OnUpdate();
			}
		}

		public float GetTimeElapsed()
		{
			return elapsed;
		}

		protected virtual void OnUpdate()
		{
			float num = elapsed / duration;
			base.transform.localScale = Vector3.one * Mathf.Clamp01((1f - num) * 8f);
		}

		public bool IsActive()
		{
			return base.gameObject.activeSelf;
		}

		public static int Compare(NotificationElementGUI a, NotificationElementGUI b)
		{
			if (a == null || b == null)
			{
				return 0;
			}
			if (a.IsActive() != b.IsActive())
			{
				return (!a.IsActive()) ? 1 : (-1);
			}
			if (!a.IsActive())
			{
				return 0;
			}
			return (a.elapsed != b.elapsed) ? ((!(a.elapsed < b.elapsed)) ? 1 : (-1)) : 0;
		}
	}
}
