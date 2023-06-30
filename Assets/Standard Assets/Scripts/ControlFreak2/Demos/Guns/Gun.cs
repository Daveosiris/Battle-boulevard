using UnityEngine;

namespace ControlFreak2.Demos.Guns
{
	public class Gun : MonoBehaviour
	{
		public ParticleSystem shotParticles;

		public AudioClip shotSound;

		public AudioClip emptySound;

		public AudioClip reloadSound;

		private bool isFiring;

		public float shotInterval = 0.175f;

		private float lastShotTime;

		public bool unlimitedAmmo;

		public int bulletCapacity = 40;

		public int bulletCount = 40;

		public Transform projectileOrigin;

		public Bullet bulletPrefab;

		private AudioSource audioSource;

		private void OnEnable()
		{
			audioSource = GetComponent<AudioSource>();
			isFiring = false;
		}

		public void SetTriggerState(bool fire)
		{
			if (fire != isFiring)
			{
				isFiring = fire;
				if (fire)
				{
					FireBullet();
				}
			}
		}

		private void FixedUpdate()
		{
			if (isFiring)
			{
				FireBullet();
			}
		}

		public void Reload()
		{
			bulletCount = bulletCapacity;
			if (audioSource != null && reloadSound != null)
			{
				audioSource.loop = false;
				audioSource.PlayOneShot(reloadSound);
			}
		}

		private void FireBullet()
		{
			if (!(Time.time - lastShotTime >= shotInterval))
			{
				return;
			}
			lastShotTime = Time.time;
			if (unlimitedAmmo || bulletCount > 0)
			{
				if (!unlimitedAmmo)
				{
					bulletCount--;
				}
				if (shotParticles != null)
				{
					shotParticles.Play();
				}
				if (projectileOrigin != null && bulletPrefab != null)
				{
					Bullet bullet = UnityEngine.Object.Instantiate(bulletPrefab, projectileOrigin.position, projectileOrigin.rotation);
					if (bullet != null)
					{
						bullet.Init(this);
					}
				}
				if (audioSource != null && shotSound != null)
				{
					audioSource.loop = false;
					audioSource.PlayOneShot(shotSound);
				}
			}
			else if (audioSource != null && emptySound != null)
			{
				audioSource.loop = false;
				audioSource.PlayOneShot(emptySound);
			}
		}
	}
}
