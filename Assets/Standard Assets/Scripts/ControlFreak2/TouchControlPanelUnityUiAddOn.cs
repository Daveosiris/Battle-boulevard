using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace ControlFreak2
{
	[ExecuteInEditMode]
	public class TouchControlPanelUnityUiAddOn : Graphic, IPointerDownHandler, IPointerUpHandler, IDragHandler, IEventSystemHandler
	{
		private TouchControlPanel panel;

		private TouchControlPanel.SystemTouchEventData eventData;

		public TouchControlPanelUnityUiAddOn()
		{
			eventData = new TouchControlPanel.SystemTouchEventData();
		}

		public bool IsConnectedToPanel()
		{
			return panel != null;
		}

		private TouchControlPanel.SystemTouchEventData TranslateEvent(PointerEventData data)
		{
			eventData.id = data.pointerId;
			eventData.pos = data.position;
			eventData.isMouseEvent = (data.pointerId < 0);
			eventData.cam = data.pressEventCamera;
			return eventData;
		}

		protected override void Awake()
		{
			base.Awake();
			panel = GetComponent<TouchControlPanel>();
		}

		protected override void OnEnable()
		{
			base.OnEnable();
		}

		public override bool Raycast(Vector2 sp, Camera eventCamera)
		{
			if (panel == null)
			{
				return false;
			}
			if (panel.Raycast(sp, eventCamera))
			{
				return true;
			}
			return false;
		}

		void IPointerDownHandler.OnPointerDown(PointerEventData data)
		{
			if (panel != null)
			{
				panel.OnSystemTouchStart(TranslateEvent(data));
			}
		}

		void IPointerUpHandler.OnPointerUp(PointerEventData data)
		{
			if (panel != null)
			{
				panel.OnSystemTouchEnd(TranslateEvent(data));
			}
		}

		void IDragHandler.OnDrag(PointerEventData data)
		{
			if (panel != null)
			{
				panel.OnSystemTouchMove(TranslateEvent(data));
			}
		}

		protected override void UpdateGeometry()
		{
			if (base.canvasRenderer != null)
			{
				base.canvasRenderer.Clear();
			}
		}

		private void OnDrawGizmos()
		{
			DrawGizmos(selected: false);
		}

		private void OnDrawGizmosSelected()
		{
			DrawGizmos(selected: true);
		}

		private void DrawGizmos(bool selected)
		{
			RectTransform rectTransform = base.transform as RectTransform;
			if (!(rectTransform == null))
			{
				Gizmos.color = ((!selected) ? Color.white : Color.red);
				Gizmos.matrix = rectTransform.localToWorldMatrix;
				Gizmos.DrawWireCube(rectTransform.rect.center, rectTransform.rect.size);
			}
		}
	}
}
