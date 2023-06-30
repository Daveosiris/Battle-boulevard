using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakableObj : MonoBehaviour
{
	private GameObject _breakeGfx;

	private GamePlayManager _gamePlayManager;

	private Flicker _flicker;

	public string _breakSound = string.Empty;

	[Header("Lifting")]
	public AnimationO _liftAnimation;

	public AnimationO _liftOwerWriteAnimation;

	public AttackO _throwAttack;

	public float _throwDamage;

	[HideInInspector]
	public float _currentDamage;

	public Vector2 _throwForce = new Vector2(2f, 0.5f);

	[HideInInspector]
	public bool _lifted;

	private bool _throwed;

	private bool _throwedCheck;

	private int _throwDirection;

	private LayerMask HitLayerMask;

	private List<Collider> _hitBefore = new List<Collider>();

	private Rigidbody _myRigidBody;

	[Header("SpawnOrange")]
	public int _orangeCount;

	[Header("SpawnBanana")]
	public float _spawnBananaChance;

	public float _bananaHealthPercent;

	[Header("Spawn Weapon(s)")]
	public int[] _spawnWeapons;

	public float _spawnChance = 100f;

	private int EnvironmentLayer;

	private void Awake()
	{
		_gamePlayManager = UnityEngine.Object.FindObjectOfType<GamePlayManager>();
		_myRigidBody = GetComponent<Rigidbody>();
		_myRigidBody.isKinematic = true;
		base.transform.GetChild(0).gameObject.SetActive(value: true);
		_breakeGfx = base.transform.GetChild(1).gameObject;
		_flicker = _breakeGfx.GetComponent<Flicker>();
		_breakeGfx.transform.parent = _gamePlayManager.transform;
		_breakeGfx.SetActive(value: false);
		EnvironmentLayer = LayerMask.NameToLayer("Environment");
	}

	public void Breake(int direction)
	{
		if (Physics.Raycast(base.transform.position + Vector3.up, Vector3.down * 10f, out RaycastHit hitInfo, 10f, 1 << EnvironmentLayer))
		{
			_breakeGfx.transform.position = hitInfo.point;
		}
		_breakeGfx.SetActive(value: true);
		_gamePlayManager._makeNoise.PlaySFX(_breakSound);
		_flicker.StartCoroutine(_flicker.FlickerCoroutine(1f));
		for (int i = 0; i < _orangeCount; i++)
		{
			Orange orange = _gamePlayManager._oranges[0];
			orange._orangeCount = 1;
			orange.Spawn(base.transform.position);
			_gamePlayManager._oranges.Remove(orange);
		}
		if ((float)UnityEngine.Random.Range(0, 100) < _spawnBananaChance)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate(Resources.Load("Banana"), Vector3.zero, Quaternion.identity) as GameObject;
			gameObject.transform.position = base.transform.position;
			if (_bananaHealthPercent > 0f)
			{
				gameObject.GetComponent<Banana>()._healthPercent = _bananaHealthPercent;
			}
			gameObject.GetComponent<Banana>().Spawn(base.transform.position, direction);
		}
		if (_spawnWeapons.Length > 0 && (float)UnityEngine.Random.Range(0, 100) < _spawnChance)
		{
			int num = UnityEngine.Random.Range(0, _spawnWeapons.Length);
			GameObject gameObject2 = UnityEngine.Object.Instantiate(Resources.Load("items/" + Config.WeaponNames[_spawnWeapons[num]])) as GameObject;
			gameObject2.transform.position = base.transform.position;
			gameObject2.GetComponent<Rigidbody>().AddForce(new Vector3(UnityEngine.Random.Range(-0.2f, 0.2f) * Time.fixedDeltaTime * Config.FORCE_MULTIPLYER, 1.5f * Time.fixedDeltaTime * Config.FORCE_MULTIPLYER, UnityEngine.Random.Range(0.2f, 0.4f) * (float)direction * Time.fixedDeltaTime * Config.FORCE_MULTIPLYER));
		}
		base.gameObject.SetActive(value: false);
	}

	public void Drop(int direction)
	{
		_throwed = true;
		_myRigidBody.isKinematic = false;
		AddForce(new Vector2(UnityEngine.Random.Range(-0.5f, 0.5f), UnityEngine.Random.Range(0.5f, 1.2f)), direction);
	}

	public void Throw(int direction, LayerMask hitLayerMask)
	{
		_hitBefore.Clear();
		_throwed = true;
		_throwedCheck = true;
		_throwDirection = direction;
		HitLayerMask = hitLayerMask;
		_myRigidBody.isKinematic = false;
		AddForce(_throwForce, _throwDirection);
		try
		{
			StartCoroutine(ThrowCheck4Hit());
		}
		catch (Exception)
		{
			throw;
		}
	}

	private IEnumerator ThrowCheck4Hit()
	{
		yield return new WaitForSeconds(0.01f);
		while (_throwedCheck)
		{
			Check4Hit();
			yield return null;
		}
	}

	private Collider[] DrawHitBox(Vector2 CollSize)
	{
		Vector3 position = base.transform.position;
		return Physics.OverlapBox(position, new Vector3(1.5f, CollSize.y, CollSize.x / 2f), Quaternion.identity, HitLayerMask);
	}

	private void Check4Hit()
	{
		Collider[] array = DrawHitBox(_throwAttack.CollSize);
		bool flag = false;
		Collider[] array2 = array;
		foreach (Collider collider in array2)
		{
			if (_hitBefore.Contains(collider))
			{
				continue;
			}
			_hitBefore.Add(collider);
			if (collider.gameObject.layer == LayerMask.NameToLayer("DestroyableObject"))
			{
				if (collider.gameObject != base.gameObject)
				{
					UnityEngine.Debug.Log(collider.name);
					collider.GetComponent<BreakableObj>().Breake(_throwDirection);
					flag = true;
				}
				continue;
			}
			CharacterControl component = collider.GetComponent<CharacterControl>();
			if (component.CanReact(_throwAttack))
			{
				if (component._state != AnimStates.KNOCKDOWN && component._state != AnimStates.DEATH)
				{
					flag = true;
					component._attackDirection = _throwDirection;
					component.MakeReaction(_throwAttack, _currentDamage, null, cantEskiv: true, slowMotionEffect: false);
				}
			}
			else
			{
				flag = true;
			}
		}
		if (flag)
		{
			_throwedCheck = false;
			Breake(_throwDirection);
		}
	}

	public void AddForce(Vector2 force, float direction)
	{
		_myRigidBody.velocity = Vector3.zero;
		_myRigidBody.angularVelocity = Vector3.zero;
		_myRigidBody.AddForce(0f, force.y * Time.fixedDeltaTime * Config.FORCE_MULTIPLYER, direction * force.x * Time.fixedDeltaTime * Config.FORCE_MULTIPLYER);
		_myRigidBody.AddRelativeTorque(0f, 40f * Time.fixedDeltaTime * Config.FORCE_MULTIPLYER * (0f - direction), 0f);
	}

	private void OnCollisionEnter(Collision collision)
	{
		if (collision.collider.gameObject.layer == LayerMask.NameToLayer("Environment"))
		{
			_throwedCheck = false;
			if (_throwed)
			{
				_throwed = false;
				Breake(_throwDirection);
			}
		}
	}
}
