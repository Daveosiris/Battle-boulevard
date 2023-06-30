using UnityEngine;

namespace ControlFreak2.Demos.Helpers
{
	[ExecuteInEditMode]
	public class SplitScreenCamAddOn : MonoBehaviour
	{
		public float rotationAngle;

		private Camera cam;

		public bool preMult = true;

		private void OnEnable()
		{
			cam = GetComponent<Camera>();
		}

		private void OnDisable()
		{
			if (cam != null)
			{
				cam.ResetProjectionMatrix();
			}
		}

		private void LateUpdate()
		{
			if (!(cam == null))
			{
				float num = cam.rect.width * (float)Screen.width;
				float num2 = cam.rect.height * (float)Screen.height;
				Vector2 vector = new Vector3(Mathf.Abs(Mathf.Sin(0.0174532924f * rotationAngle)), Mathf.Abs(Mathf.Cos(0.0174532924f * rotationAngle)));
				Vector2 vector2 = new Vector2(num * vector.x + num2 * vector.y, num2 * vector.x + num * vector.y);
				float aspect = vector2.x / vector2.y;
				Matrix4x4 matrix4x = (!cam.orthographic) ? Matrix4x4.Perspective(cam.fieldOfView, aspect, cam.nearClipPlane, cam.farClipPlane) : Matrix4x4.Ortho((0f - vector2.x) * cam.orthographicSize, vector2.x * cam.orthographicSize, (0f - vector2.y) * cam.orthographicSize, vector2.y * cam.orthographicSize, cam.nearClipPlane, cam.farClipPlane);
				matrix4x = ((!preMult) ? (matrix4x * Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(0f, 0f, rotationAngle), Vector3.one)) : (Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(0f, 0f, rotationAngle), Vector3.one) * matrix4x));
				cam.projectionMatrix = matrix4x;
			}
		}
	}
}
