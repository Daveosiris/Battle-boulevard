using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapons : MonoBehaviour
{
	[Header("Setup")]
	public bool _enemyCantPick;

	public float _damage;

	public float _throwDamage;

	[HideInInspector]
	public float _currentDamage;

	public float _pickUpRange = 1f;

	public bool _seeRange;

	public Hand _animationHand;

	public int _useCount;

	[Header("Materials")]
	public Material _defaultMaterail;

	public Material _shiningMaterial;

	[Header("Breakable")]
	public GameObject _broken;

	[Header("Combos")]
	public AttackO[] _weaponAttacks;

	public Vector2 _throwForce;

	public bool _instantHide;

	public float _throwCount;

	public AttackO _throwAttack;

	[Header("AttackEffect")]
	public GameObject _attackEffect;

	[Header("Sounds")]
	public string _hitSound;

	public string _breakSound;

	public string _dropSound;

	[HideInInspector]
	public Transform _myTransform;

	[HideInInspector]
	public Rigidbody _myRigidBody;

	private Flicker _flickerEffect;

	private GamePlayManager _gamePlayManager;

	private bool _throwed;

	private bool _throwedCheck;

	private int _throwDirection;

	private LayerMask HitLayerMask;

	[Header("Stats")]
	public Hand holdedHand;

	public bool _picked;

	public bool _enemyWantsToPick;

	public List<Collider> _hitBefore = new List<Collider>();

	private void Awake()
	{
		_myTransform = base.transform;
		_myRigidBody = GetComponent<Rigidbody>();
		_flickerEffect = GetComponent<Flicker>();
		if ((bool)_broken)
		{
			_broken.SetActive(value: false);
			_flickerEffect.GFX.Add(_broken);
		}
		else
		{
			_flickerEffect.GFX.Add(base.transform.GetChild(0).gameObject);
		}
		if (!_picked)
		{
			ShiningMaterial();
		}
	}

	private void Start()
	{
		_gamePlayManager = UnityEngine.Object.FindObjectOfType<GamePlayManager>();
		_gamePlayManager._allWeapons.Add(GetComponent<Weapons>());
		_gamePlayManager._availableWeapons.Add(GetComponent<Weapons>());
	}

	private void Update()
	{
		if (!_picked)
		{
			Vector3 position = _myTransform.position;
			position.x = Mathf.Clamp(position.x, _gamePlayManager._up, _gamePlayManager._down);
			if (_gamePlayManager._levelDesignScript._specialBossScene)
			{
				position.z = Mathf.Clamp(position.z, _gamePlayManager._left + 1f, _gamePlayManager._right - 1f);
			}
			_myTransform.position = position;
		}
	}

	public void DefaultMaterial()
	{
		base.transform.GetChild(0).GetComponent<MeshRenderer>().sharedMaterial = _defaultMaterail;
		if ((bool)_broken)
		{
			_broken.GetComponent<MeshRenderer>().sharedMaterial = _defaultMaterail;
		}
	}

	public void ShiningMaterial()
	{
		base.transform.GetChild(0).GetComponent<MeshRenderer>().sharedMaterial = _shiningMaterial;
		if ((bool)_broken)
		{
			_broken.GetComponent<MeshRenderer>().sharedMaterial = _shiningMaterial;
		}
	}

	public void BrokeWeapon()
	{
		_myTransform.GetChild(0).gameObject.SetActive(value: false);
		_broken.SetActive(value: true);
	}

	public void Throw(int direction, LayerMask hitLayerMask)
	{
		_hitBefore.Clear();
		_throwed = true;
		_throwedCheck = true;
		_throwDirection = direction;
		_throwCount -= 1f;
		HitLayerMask = hitLayerMask;
		_myRigidBody.isKinematic = false;
		Collider[] array = DrawHitBox(_throwAttack.CollSize);
		bool flag = false;
		Collider[] array2 = array;
		foreach (Collider collider in array2)
		{
			if (collider.gameObject.layer == LayerMask.NameToLayer("DestroyableObject"))
			{
				flag = true;
				continue;
			}
			CharacterControl component = collider.GetComponent<CharacterControl>();
			if (component._state != AnimStates.KNOCKDOWN && component._state != AnimStates.DEATH)
			{
				flag = true;
			}
		}
		if (flag)
		{
			AddForce(new Vector2(0.1f, 0f), direction);
		}
		else
		{
			AddForce(_throwForce, direction);
		}
		StartCoroutine(ThrowCheck4Hit());
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
				collider.GetComponent<BreakableObj>().Breake(_throwDirection);
				flag = true;
				continue;
			}
			CharacterControl component = collider.GetComponent<CharacterControl>();
			if (component.CanReact(_throwAttack) && component._state != AnimStates.KNOCKDOWN && component._state != AnimStates.DEATH)
			{
				flag = true;
				component._attackDirection = _throwDirection;
				component.MakeReaction(_throwAttack, _currentDamage, this, cantEskiv: true, slowMotionEffect: true);
			}
		}
		if (flag)
		{
			_throwedCheck = false;
			AddForce(new Vector2(UnityEngine.Random.Range(-0.5f, 0.5f), UnityEngine.Random.Range(0.9f, 1.1f)), 1f);
			if (_instantHide)
			{
				base.gameObject.SetActive(value: false);
			}
		}
	}

	public void AddForce(Vector2 force, float direction)
	{
		_myRigidBody.velocity = Vector3.zero;
		_myRigidBody.angularVelocity = Vector3.zero;
		_myRigidBody.AddForce(0f, force.y * Time.fixedDeltaTime * Config.FORCE_MULTIPLYER, direction * force.x * Time.fixedDeltaTime * Config.FORCE_MULTIPLYER);
		_myRigidBody.AddRelativeTorque(0f, 20f * Time.fixedDeltaTime * Config.FORCE_MULTIPLYER * (0f - direction), 0f);
	}

	private void OnCollisionEnter(Collision collision)
	{
		if (collision.collider.gameObject.layer != LayerMask.NameToLayer("Environment"))
		{
			return;
		}
		_throwedCheck = false;
		if (collision.relativeVelocity.magnitude > 4f)
		{
			_gamePlayManager._makeNoise.PlaySFX(_dropSound);
		}
		if (_throwed)
		{
			_throwed = false;
			if (_throwCount <= 0f)
			{
				_flickerEffect.StartCoroutine(_flickerEffect.FlickerCoroutine(0.5f));
				_gamePlayManager._allWeapons.Remove(this);
				return;
			}
			_picked = false;
			_enemyWantsToPick = false;
			_gamePlayManager._availableWeapons.Add(this);
			ShiningMaterial();
		}
	}
}
