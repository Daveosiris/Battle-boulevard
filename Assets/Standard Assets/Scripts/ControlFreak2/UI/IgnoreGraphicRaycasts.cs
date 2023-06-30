using UnityEngine;

namespace ControlFreak2.UI
{
	public class IgnoreGraphicRaycasts : MonoBehaviour, ICanvasRaycastFilter
	{
		bool ICanvasRaycastFilter.IsRaycastLocationValid(Vector2 sp, Camera eventCamera)
		{
			return false;
		}
	}
}
