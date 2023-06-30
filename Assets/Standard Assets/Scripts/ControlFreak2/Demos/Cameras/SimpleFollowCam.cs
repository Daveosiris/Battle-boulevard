using UnityEngine;

namespace ControlFreak2.Demos.Cameras
{
	public class SimpleFollowCam : MonoBehaviour
	{
		public Transform targetTransform;

		public float smoothingTime = 0.1f;

		private Vector3 targetOfs;

		private Vector3 smoothPosVel;

		private float camZoomFactor;

		public float perspZoomOutOffset = 10f;

		public float orthoZoomInSize = 2f;

		public float orthoZoomOutSize = 5f;

		public string camZoomDeltaAxis = "Cam-Zoom-Delta";

		private Camera cam;

		private void OnEnable()
		{
			cam = GetComponent<Camera>();
			if (cam != null && cam.orthographic)
			{
				camZoomFactor = 0.5f;
			}
			if (targetTransform != null)
			{
				targetOfs = base.transform.position - targetTransform.position;
			}
		}

		private void Update()
		{
			if (!string.IsNullOrEmpty(camZoomDeltaAxis))
			{
				camZoomFactor += CF2Input.GetAxis(camZoomDeltaAxis);
			}
			camZoomFactor = Mathf.Clamp01(camZoomFactor);
		}

		private void FixedUpdate()
		{
			if (!(targetTransform == null))
			{
				Vector3 vector = targetTransform.position + targetOfs;
				if (cam != null && cam.orthographic)
				{
					cam.orthographicSize = CFUtils.SmoothTowards(cam.orthographicSize, Mathf.Lerp(orthoZoomInSize, orthoZoomOutSize, camZoomFactor), smoothingTime, CFUtils.realDeltaTimeClamped, 0.0001f);
				}
				else
				{
					vector -= base.transform.forward * (camZoomFactor * perspZoomOutOffset);
				}
				if (smoothingTime > 0.001f)
				{
					vector = Vector3.SmoothDamp(base.transform.position, vector, ref smoothPosVel, smoothingTime);
				}
				base.transform.position = vector;
			}
		}
	}
}
