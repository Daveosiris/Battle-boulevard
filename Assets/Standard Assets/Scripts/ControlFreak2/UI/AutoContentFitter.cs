using UnityEngine;

namespace ControlFreak2.UI
{
	[ExecuteInEditMode]
	public class AutoContentFitter : MonoBehaviour
	{
		public RectTransform source;

		public bool autoWidth;

		public bool autoHeight;

		public float horzPadding = 10f;

		public float vertPadding = 10f;

		private void Update()
		{
			UpdatePreferredDimensions();
		}

		public void UpdatePreferredDimensions()
		{
			if ((autoWidth || autoHeight) && !(source == null))
			{
				RectTransform rectTransform = (RectTransform)base.transform;
				if (autoHeight)
				{
					rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, source.rect.height + vertPadding);
				}
				if (autoWidth)
				{
					rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, source.rect.width + horzPadding);
				}
			}
		}
	}
}
