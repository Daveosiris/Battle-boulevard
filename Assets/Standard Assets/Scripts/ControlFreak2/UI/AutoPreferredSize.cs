using UnityEngine;
using UnityEngine.UI;

namespace ControlFreak2.UI
{
	[ExecuteInEditMode]
	public class AutoPreferredSize : MonoBehaviour
	{
		public LayoutElement target;

		public bool autoWidth;

		public bool autoHeight;

		public float horzPadding = 10f;

		public float vertPadding = 10f;

		public RectTransform source;

		private void OnEnable()
		{
			if (target == null)
			{
				target = GetComponent<LayoutElement>();
			}
		}

		private void Update()
		{
			UpdatePreferredDimensions();
		}

		public void UpdatePreferredDimensions()
		{
			if ((autoWidth || autoHeight) && !(source == null))
			{
				if (autoHeight)
				{
					target.minHeight = source.rect.height + vertPadding;
				}
				if (autoWidth)
				{
					target.minWidth = source.rect.width + horzPadding;
				}
			}
		}
	}
}
