using UnityEngine;

namespace ControlFreak2.Demos.Guns
{
	public class Bullet : MonoBehaviour
	{
		private Gun gun;

		private Rigidbody rigidBody;

		public float maxLifetime = 5f;

		private float lifetime;

		public float initialSpeed = 20f;

		public float maxSpeed = 30f;

		public float accel = 20f;

		private float speed;

		private void OnEnable()
		{
			rigidBody = GetComponent<Rigidbody>();
		}

		public void Init(Gun gun)
		{
			this.gun = gun;
			lifetime = 0f;
			speed = Mathf.Max(0f, initialSpeed);
		}

		public Gun GetGun()
		{
			return gun;
		}

		private void FixedUpdate()
		{
			speed += Time.deltaTime * accel;
			if (speed > maxSpeed)
			{
				speed = maxSpeed;
			}
			Transform transform = base.transform;
			Vector3 position = transform.position + transform.forward * speed * Time.deltaTime;
			if (rigidBody != null)
			{
				rigidBody.MovePosition(position);
			}
			else
			{
				transform.position = position;
			}
			if ((lifetime += Time.deltaTime) > maxLifetime)
			{
				UnityEngine.Object.Destroy(base.gameObject);
			}
		}
	}
}
