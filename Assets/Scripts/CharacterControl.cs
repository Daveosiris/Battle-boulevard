using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterControl : MonoBehaviour
{
	public bool _dontRun;

	public bool _stayInStage;

	[Header("Setup")]
	public cinsiyet _cinsiyet;

	public float _rotSpeed = 10f;

	public float _speed = 2.5f;

	public float _jumpForce = 4f;

	private float _hitZdistance = 1.5f;

	public float _standUpTime;

	[Header("Idle")]
	public AnimationO _idle;

	[Header("Walk")]
	public AnimationO _walk;

	[Space(10f)]
	public bool _test;

	[Header("Forward Fight")]
	public AttackSet[] _punches;

	private int _punchInd;

	public AttackSet[] _kicks;

	private int _kickInd;

	[Header("Back Fight")]
	public AttackSet _punchBacks;

	public AttackSet _kickBacks;

	[Header("Air Fight")]
	public AttackSet _jumpPunches;

	public AttackSet _jumpKicks;

	[Header("Ground Fight")]
	public AttackSet _groundPunches;

	public AttackSet _groundKicks;

	[Header("Reactions")]
	public ReactionSet _reactions;

	[Header("Weapon Picker")]
	public Transform LeftHandWeaponHolder;

	public Transform RightHandWeaponHolder;

	[Header("AlreadyHasWeapon?")]
	public int _bothtHandWeapon;

	public int _leftHandWeapon;

	public int _rightHandWeapon;

	[HideInInspector]
	public List<Weapons> _holdedWeaopons = new List<Weapons>();

	private List<Weapons> _weaponsInRange = new List<Weapons>();

	private Weapons _pickedWeapon;

	private Weapons _usedWeapon;

	private Weapons _throwedWeapon;

	private int _switchHandInd;

	[Header("Grabber")]
	public AnimationO _grabEmpty;

	public GrabSet[] _standingGrabs;

	public GrabSet[] _groundGrabs;

	[HideInInspector]
	public GrabSet _currentGrab;

	private int _grabbedDirection;

	[HideInInspector]
	public bool _iamGrabbed;

	private bool _grabbed;

	private bool _waitingGrabReaction;

	private bool _groundGrab;

	private int _standingGrabCount;

	private CharacterControl _enemyCharacterControl;

	private BreakableObj _liftedObj;

	public bool _lifting;

	[HideInInspector]
	public Transform _ShadowTransform;

	private float _blobShadowSize = 1f;

	private float _distanceScale = 2f;

	private bool _followTerrainRotation;

	private float _rayDist = 10f;

	private float _yoffset = 0.1f;

	public GameObject _customHitEffect;

	[HideInInspector]
	public GameObject HitEffect;

	[HideInInspector]
	public GameObject DefendEffect;

	[HideInInspector]
	public GameObject DustEffectJump;

	[HideInInspector]
	public GameObject DustEffectLand;

	[Space(10f)]
	[Header("Stats")]
	public float _maximumHealth;

	public float _currentHealth;

	public float _punchDamage;

	public float _kickDamage;

	public float _grabDamage;

	public float _weaponDamageMuliplyer;

	[Space(5f)]
	public AnimStates _state;

	public int _direction = 1;

	public int _attackDirection;

	private float _knockDownForce;

	[HideInInspector]
	public AttackO _currentAttackObject;

	[HideInInspector]
	public float _currentDamage;

	[HideInInspector]
	public bool _attackLock;

	[HideInInspector]
	public bool _backAttack;

	private int _comboInd;

	[HideInInspector]
	public float _wallCheckDist = 0.75f;

	[HideInInspector]
	public float _powerUpSpeedMultiplyer = 1f;

	private GameObject powerUpEffectLeft;

	private GameObject powerUpEffectRight;

	private float _powerUpAttackValue = 0.095f;

	private float _powerUpHitValue = 0.3f;

	[HideInInspector]
	public Transform _myTransform;

	[HideInInspector]
	public Transform _animatorTransform;

	private Animator _animator;

	[HideInInspector]
	public AnimatorOverrideController _animatorOverrideController;

	private Rigidbody _myRigidbody;

	private AnimationEvents _animationEvents;

	[HideInInspector]
	public PlayerControl _playerControl;

	private EnemyControl _enemyControl;

	[HideInInspector]
	public GamePlayManager _gamePlayManager;

	[HideInInspector]
	public Flicker _flicker;

	[HideInInspector]
	public string _femalePrefix;

	private int EnemyLayer;

	private int DestroyableObjectLayer;

	private int EnvironmentLayer;

	public float _rot;

	[HideInInspector]
	public LayerMask HitLayerMask;

	private List<Collider> _hitBefore = new List<Collider>();

	public void AwakeProc()
	{
		_myTransform = base.transform;
		_myRigidbody = _myTransform.GetComponent<Rigidbody>();
		_animator = GetComponentInChildren<Animator>();
		_animatorOverrideController = new AnimatorOverrideController(_animator.runtimeAnimatorController);
		_animator.runtimeAnimatorController = _animatorOverrideController;
		_animatorTransform = _animator.transform;
		_animationEvents = GetComponentInChildren<AnimationEvents>();
		_flicker = GetComponent<Flicker>();
		GameObject gameObject = UnityEngine.Object.Instantiate(Resources.Load("Shadow"), Vector3.zero, Quaternion.identity) as GameObject;
		_ShadowTransform = gameObject.transform;
		_ShadowTransform.parent = null;
		HitEffect = (UnityEngine.Object.Instantiate(Resources.Load("HitEffect"), Vector3.zero, Quaternion.identity) as GameObject);
		HitEffect.SetActive(value: false);
		DefendEffect = (UnityEngine.Object.Instantiate(Resources.Load("DefendEffect"), Vector3.zero, Quaternion.identity) as GameObject);
		DefendEffect.SetActive(value: false);
		DustEffectJump = (UnityEngine.Object.Instantiate(Resources.Load("DustEffectJump"), Vector3.zero, Quaternion.identity) as GameObject);
		DustEffectJump.SetActive(value: false);
		DustEffectLand = (UnityEngine.Object.Instantiate(Resources.Load("DustEffectLand"), Vector3.zero, Quaternion.identity) as GameObject);
		DustEffectLand.SetActive(value: false);
		if ((bool)_customHitEffect)
		{
			_customHitEffect.SetActive(value: false);
		}
		_femalePrefix = string.Empty;
		if (_cinsiyet == cinsiyet.Female)
		{
			_femalePrefix = "F";
		}
		if ((bool)GetComponent<PlayerControl>())
		{
			EnemyLayer = LayerMask.NameToLayer("Enemy");
			_playerControl = GetComponent<PlayerControl>();
			_playerControl.SetValues();
			_maximumHealth = (_currentHealth = _playerControl._healthU);
			_punchDamage = _playerControl._punchPowerU / 3f;
			_kickDamage = _playerControl._kickPowerU / 2f;
			_grabDamage = (_punchDamage + _kickDamage) / 2f;
			_weaponDamageMuliplyer = _playerControl._weaponTecniqueU;
			powerUpEffectLeft = (UnityEngine.Object.Instantiate(Resources.Load("PowerUpEffect"), Vector3.zero, Quaternion.identity) as GameObject);
			powerUpEffectLeft.transform.parent = LeftHandWeaponHolder;
			powerUpEffectLeft.transform.localPosition = Vector3.zero;
			Transform transform = powerUpEffectLeft.transform;
			Vector3 one = Vector3.one;
			Vector3 localScale = LeftHandWeaponHolder.localScale;
			transform.localScale = one * localScale.x / 20f;
			powerUpEffectLeft.SetActive(value: false);
			powerUpEffectRight = (UnityEngine.Object.Instantiate(Resources.Load("PowerUpEffect"), Vector3.zero, Quaternion.identity) as GameObject);
			powerUpEffectRight.transform.parent = RightHandWeaponHolder;
			powerUpEffectRight.transform.localPosition = Vector3.zero;
			Transform transform2 = powerUpEffectRight.transform;
			Vector3 one2 = Vector3.one;
			Vector3 localScale2 = RightHandWeaponHolder.localScale;
			transform2.localScale = one2 * localScale2.x / 20f;
			powerUpEffectRight.SetActive(value: false);
			_rot = 0f;
			_direction = 1;
		}
		else
		{
			EnemyLayer = LayerMask.NameToLayer("Player");
			_enemyControl = GetComponent<EnemyControl>();
			if (string.IsNullOrEmpty(_enemyControl._name))
			{
				if (_cinsiyet == cinsiyet.Male)
				{
					_enemyControl._name = Config.MaleNames[Random.Range(0, Config.MaleNames.Length)];
				}
				else
				{
					_enemyControl._name = Config.FemaleNames[Random.Range(0, Config.FemaleNames.Length)];
				}
			}
			_rot = 180f;
			_direction = -1;
		}
		DestroyableObjectLayer = LayerMask.NameToLayer("DestroyableObject");
		EnvironmentLayer = LayerMask.NameToLayer("Environment");
		HitLayerMask = ((1 << EnemyLayer) | (1 << DestroyableObjectLayer));
		AlreadyHasWeapon();
	}

	public void Start()
	{
		_powerUpSpeedMultiplyer = 1f;
		_animator = GetComponentInChildren<Animator>();
		_animatorOverrideController = new AnimatorOverrideController(_animator.runtimeAnimatorController);
		_animator.runtimeAnimatorController = _animatorOverrideController;
		_animatorTransform = _animator.transform;
		SetLocomotionAnimationStates();
	}

	private void Update()
	{
		if (!_dontRun)
		{
			if (_state != AnimStates.ATTACK && _state != AnimStates.PICK && _state != AnimStates.LIFT)
			{
				_attackLock = false;
				_backAttack = false;
			}
			SetAnimFloat("GroundDist", DistanceToGround());
			CheckWeaponInRange();
			GrabCheck();
			AnimatorMovement();
			ShadowFollow();
			if ((bool)_playerControl)
			{
				_playerControl.UpdateProc();
			}
			if ((bool)_enemyControl)
			{
				_enemyControl.UpdateProc();
			}
		}
	}

	private void LateUpdate()
	{
		if (_dontRun || !_gamePlayManager._clampCharacterPositions)
		{
			return;
		}
		Vector3 position = _myTransform.position;
		position.x = Mathf.Clamp(position.x, _gamePlayManager._up, _gamePlayManager._down);
		if (_stayInStage)
		{
			if ((bool)_playerControl)
			{
				position.z = Mathf.Clamp(position.z, _gamePlayManager._left, _gamePlayManager._right);
			}
			else
			{
				position.z = Mathf.Clamp(position.z, _gamePlayManager._left + 1f, _gamePlayManager._right - 1f);
			}
		}
		_myTransform.position = position;
	}

	public void WalkAnimation(float value)
	{
		if (!CantMove())
		{
			_animator.SetFloat("Speed", value, 0.05f, Time.deltaTime);
		}
	}

	public void Movement(Vector3 axis)
	{
		if (!CantMove())
		{
			_myTransform.Translate((0f - _speed) * _powerUpSpeedMultiplyer * axis.x * Time.deltaTime, 0f, _speed * _powerUpSpeedMultiplyer * axis.z * Time.deltaTime);
		}
	}

	public void LookatDirection()
	{
		if (!CantRotate())
		{
			if (_direction == 1)
			{
				_rot = Mathf.Lerp(_rot, 0f, _rotSpeed * Time.deltaTime);
			}
			else
			{
				_rot = Mathf.Lerp(_rot, 180f, _rotSpeed * Time.deltaTime);
			}
			_animatorTransform.eulerAngles = new Vector3(0f, _rot, 0f);
		}
	}

	public void ChangeDirection()
	{
		_direction = -_attackDirection;
		if (_direction == 1)
		{
			_rot = 0f;
		}
		else
		{
			_rot = 180f;
		}
		_animatorTransform.eulerAngles = new Vector3(0f, _rot, 0f);
	}

	private void AnimatorMovement()
	{
		if (_animator.transform.localPosition != Vector3.zero)
		{
			Vector3 localPosition = _animatorTransform.localPosition;
			float y = localPosition.y;
			Vector3 localPosition2 = _animatorTransform.localPosition;
			Vector3 a = new Vector3(0f, y, localPosition2.z);
			_animatorTransform.localPosition = Vector3.zero;
			if (_direction == 1)
			{
				_myTransform.position += a * _direction;
			}
			else
			{
				_myTransform.position += a * -_direction;
			}
		}
	}

	public bool CantMove()
	{
		return _state == AnimStates.DEATH || _state == AnimStates.ATTACK || _state == AnimStates.HIT || _state == AnimStates.PICK || _state == AnimStates.LIFT || _state == AnimStates.KNOCKDOWN || _state == AnimStates.STANDINGUP || _state == AnimStates.KNOCKINGDOWN || _state == AnimStates.GRAB || _state == AnimStates.GRABBED || _state == AnimStates.DEFENCE;
	}

	public bool CantRotate()
	{
		return _state == AnimStates.DEATH || _state == AnimStates.KNOCKDOWN || _state == AnimStates.KNOCKINGDOWN || _state == AnimStates.STANDINGUP || _state == AnimStates.GRAB || _state == AnimStates.GRABBED || _state == AnimStates.HIT || _state == AnimStates.PICK || _state == AnimStates.LIFT || _state == AnimStates.DEFENCE;
	}

	private bool CantAttack()
	{
		return _attackLock || _state == AnimStates.DEATH || _state == AnimStates.HIT || _state == AnimStates.GRABBED || _state == AnimStates.KNOCKDOWN || _state == AnimStates.KNOCKINGDOWN || _state == AnimStates.STANDINGUP || _state == AnimStates.LIFT || _state == AnimStates.JUMPATTACK;
	}

	private bool CantJump()
	{
		return _lifting || _state == AnimStates.DEATH || _state == AnimStates.GRABBED || _state == AnimStates.DEFENCE || _state == AnimStates.HIT || _state == AnimStates.PICK || _state == AnimStates.LIFT || _state == AnimStates.KNOCKDOWN || _state == AnimStates.KNOCKINGDOWN || _state == AnimStates.STANDINGUP || _state == AnimStates.JUMP || _state == AnimStates.AIR || _state == AnimStates.JUMPATTACK || DistanceToGround() > 0.5f;
	}

	private bool CantGrab()
	{
		return _attackLock || _state == AnimStates.DEATH || _state == AnimStates.HIT || _state == AnimStates.PICK || _state == AnimStates.LIFT || _state == AnimStates.KNOCKDOWN || _state == AnimStates.KNOCKINGDOWN || _state == AnimStates.STANDINGUP || _state == AnimStates.AIR || _state == AnimStates.JUMP || _state == AnimStates.GRABBED || _state == AnimStates.JUMPATTACK;
	}

	private bool CantDefence()
	{
		return _lifting || _state == AnimStates.DEATH || _state == AnimStates.ATTACK || _state == AnimStates.HIT || _state == AnimStates.PICK || _state == AnimStates.LIFT || _state == AnimStates.KNOCKDOWN || _state == AnimStates.KNOCKINGDOWN || _state == AnimStates.STANDINGUP || _state == AnimStates.AIR || _state == AnimStates.JUMP || _state == AnimStates.JUMPATTACK;
	}

	private bool CantEskiv()
	{
		return _state == AnimStates.DEATH || _state == AnimStates.PICK || _state == AnimStates.LIFT || _state == AnimStates.KNOCKDOWN || _state == AnimStates.KNOCKINGDOWN || _state == AnimStates.STANDINGUP || _state == AnimStates.AIR || _state == AnimStates.JUMP || _state == AnimStates.JUMPATTACK;
	}

	private Collider[] DrawHitBox(Vector2 CollSize)
	{
		Vector3 center = _myTransform.position + new Vector3(0f, 1f, (float)_direction * CollSize.x / 2f);
		return Physics.OverlapBox(center, new Vector3(_hitZdistance, CollSize.y, CollSize.x / 2f), Quaternion.identity, HitLayerMask);
	}

	public void Check4Hit()
	{
		Collider[] array = DrawHitBox(_currentAttackObject.CollSize);
		bool flag = false;
		Collider[] array2 = array;
		foreach (Collider collider in array2)
		{
			if (_hitBefore.Contains(collider))
			{
				continue;
			}
			_hitBefore.Add(collider);
			if (collider.gameObject.layer == DestroyableObjectLayer)
			{
				BreakableObj component = collider.GetComponent<BreakableObj>();
				if (!component._lifted)
				{
					collider.GetComponent<BreakableObj>().Breake(_direction);
				}
				continue;
			}
			CharacterControl component2 = collider.GetComponent<CharacterControl>();
			if (component2.CanReact(_currentAttackObject))
			{
				if ((bool)_playerControl)
				{
					collider.GetComponent<EnemyControl>().Defence();
				}
				if (component2._state != AnimStates.KNOCKDOWN && component2._state != AnimStates.KNOCKINGDOWN)
				{
					flag = true;
				}
				component2._attackDirection = _direction;
				bool cantEskiv = false;
				if (_usedWeapon != null)
				{
					cantEskiv = true;
				}
				float num = _currentDamage;
				if (_powerUpSpeedMultiplyer > 1f)
				{
					num *= 2f;
				}
				component2.MakeReaction(_currentAttackObject, num, _usedWeapon, cantEskiv, slowMotionEffect: true);
			}
		}
		if (_test)
		{
			_comboInd++;
			return;
		}
		if (array.Length == 0)
		{
			_comboInd = 0;
			return;
		}
		if ((bool)_playerControl)
		{
			_standingGrabCount = 0;
		}
		_gamePlayManager._camShake.Shake(0.1f);
		if (!flag)
		{
			return;
		}
		_comboInd++;
		if (_usedWeapon != null)
		{
			_usedWeapon._useCount--;
			if (_usedWeapon._useCount == 0 && (bool)_usedWeapon._broken)
			{
				_usedWeapon.BrokeWeapon();
				_gamePlayManager._makeNoise.PlaySFX(_usedWeapon._breakSound);
			}
		}
	}

	private bool GroundFight()
	{
		Collider[] array = DrawHitBox(new Vector2(1f, 1.7f));
		Collider[] array2 = array;
		foreach (Collider collider in array2)
		{
			if (collider.gameObject.layer != DestroyableObjectLayer && collider.GetComponent<CharacterControl>()._state == AnimStates.KNOCKDOWN)
			{
				return true;
			}
		}
		return false;
	}

	private CharacterControl GrabFight()
	{
		Vector3 center = _myTransform.position + Vector3.forward * _direction + Vector3.up * 1f;
		Collider[] array = Physics.OverlapBox(center, new Vector3(0.1f, 1.7f, 0.3f), Quaternion.identity, HitLayerMask);
		Collider[] array2 = array;
		foreach (Collider collider in array2)
		{
			if (collider.gameObject.layer != DestroyableObjectLayer)
			{
				CharacterControl component = collider.GetComponent<CharacterControl>();
				Vector3 position = _myTransform.position;
				float y = position.y;
				Vector3 position2 = component._myTransform.position;
				if (Mathf.Abs(y - position2.y) < 0.5f)
				{
					return component;
				}
			}
		}
		return null;
	}

	private BreakableObj LiftObject()
	{
		Vector3 center = _myTransform.position + Vector3.forward * _direction * 0.5f + Vector3.up * 1f;
		Collider[] array = Physics.OverlapBox(center, new Vector3(1f, 1f, 0.2f), Quaternion.identity, HitLayerMask);
		Collider[] array2 = array;
		foreach (Collider collider in array2)
		{
			if (collider.gameObject.layer == DestroyableObjectLayer)
			{
				return collider.GetComponent<BreakableObj>();
			}
		}
		return null;
	}

	public void PoweringUpStart()
	{
		_state = AnimStates.POWERINGUP;
		_gamePlayManager._camFollow.PowerUpEffect();
		_gamePlayManager._makeNoise.PlaySFX(_femalePrefix + "PowerUp");
		_gamePlayManager._giveMeTheMusic.PlayMusic("PowerUp");
		_gamePlayManager._powerUpScript._powerUpInfoText.gameObject.SetActive(value: true);
		_gamePlayManager._colorEffectScript.PowerUpEffect();
		_animationEvents.ShowDustEffectLand();
		PowerUpStart();
		AnalyticsManager.SendEventInfo("PowerUp");
	}

	public void PoweringUpEnd()
	{
		if (_gamePlayManager._bossInTheScene)
		{
			_gamePlayManager._camFollow.BossEffect();
		}
		else
		{
			_gamePlayManager._camFollow.Reset();
		}
	}

	public void PowerUpStart()
	{
		_powerUpSpeedMultiplyer = 1.2f;
		SetAnimFloat("IdleSpeed", _idle._animationSpeed * _powerUpSpeedMultiplyer);
		SetAnimFloat("WalkSpeed", _walk._animationSpeed * _powerUpSpeedMultiplyer);
		powerUpEffectLeft.SetActive(value: true);
		powerUpEffectRight.SetActive(value: true);
	}

	public void PowerUpEnd()
	{
		_playerControl._poweredUp = false;
		_powerUpSpeedMultiplyer = 1f;
		SetAnimFloat("IdleSpeed", _idle._animationSpeed * _powerUpSpeedMultiplyer);
		SetAnimFloat("WalkSpeed", _walk._animationSpeed * _powerUpSpeedMultiplyer);
		powerUpEffectLeft.SetActive(value: false);
		powerUpEffectRight.SetActive(value: false);
		_gamePlayManager._colorEffectScript.DisableEffect();
		if (!_gamePlayManager._gameOver)
		{
			if (_gamePlayManager._levelDesignScript._specialBossScene)
			{
				_gamePlayManager._giveMeTheMusic.PlayMusic("SPBoss");
			}
			else if (_gamePlayManager._bossInTheScene)
			{
				_gamePlayManager._giveMeTheMusic.PlayMusic("Boss");
			}
			else
			{
				_gamePlayManager._giveMeTheMusic.PlayMusic("Game");
			}
		}
	}

	public void PunchCombo()
	{
		if (CantAttack())
		{
			return;
		}
		if (_lifting)
		{
			LiftThrow();
			return;
		}
		_currentDamage = _punchDamage;
		if (_state == AnimStates.GRAB)
		{
			if (!_waitingGrabReaction)
			{
				if (_groundGrab)
				{
					_currentDamage = _punchDamage;
					PrepeareAttack(_groundPunches);
				}
				else
				{
					_punchInd = UnityEngine.Random.Range(0, _punches.Length);
					_comboInd = _punches[_punchInd]._attacks.Length - 1;
					PrepeareAttack(_punches[_punchInd]);
				}
			}
			return;
		}
		SetAnimFloat("Speed", 0f);
		if (_state == AnimStates.AIR || _state == AnimStates.JUMP)
		{
			if (DistanceToGround() > 0.6f)
			{
				PrepeareAttack(_jumpPunches);
			}
		}
		else
		{
			if (WeaponAction())
			{
				return;
			}
			if (GroundFight() && _groundPunches._attacks.Length > 0)
			{
				PrepeareAttack(_groundPunches);
				return;
			}
			_kickInd = UnityEngine.Random.Range(0, _kicks.Length);
			if (_comboInd >= _punches[_punchInd]._attacks.Length)
			{
				_comboInd = 0;
			}
			if (_comboInd == 0)
			{
				_punchInd = UnityEngine.Random.Range(0, _punches.Length);
			}
			if (_test)
			{
				_punchInd = _punches.Length - 1;
			}
			PrepeareAttack(_punches[_punchInd]);
		}
	}

	public void KickCombo()
	{
		if (CantAttack())
		{
			return;
		}
		if (_lifting)
		{
			LiftThrow();
			return;
		}
		_currentDamage = _kickDamage;
		if (_state == AnimStates.GRAB)
		{
			if (!_waitingGrabReaction)
			{
				if (_groundGrab)
				{
					PrepeareAttack(_groundKicks);
					return;
				}
				_kickInd = UnityEngine.Random.Range(0, _kicks.Length);
				_comboInd = _kicks[_kickInd]._attacks.Length - 1;
				PrepeareAttack(_kicks[_kickInd]);
			}
			return;
		}
		SetAnimFloat("Speed", 0f);
		if (_state == AnimStates.AIR || _state == AnimStates.JUMP)
		{
			if (DistanceToGround() > 0.6f)
			{
				PrepeareAttack(_jumpKicks);
			}
			return;
		}
		if (GroundFight() && _groundKicks._attacks.Length > 0)
		{
			PrepeareAttack(_groundKicks);
			return;
		}
		_punchInd = UnityEngine.Random.Range(0, _punches.Length);
		if (_comboInd >= _kicks[_kickInd]._attacks.Length)
		{
			_comboInd = 0;
		}
		if (_comboInd == 0)
		{
			_kickInd = UnityEngine.Random.Range(0, _kicks.Length);
		}
		if (_test)
		{
			_kickInd = _kicks.Length - 1;
		}
		PrepeareAttack(_kicks[_kickInd]);
	}

	public void BackPunchCombo()
	{
		if (CantAttack())
		{
			return;
		}
		if (_lifting)
		{
			LiftThrow();
			return;
		}
		_currentDamage = _punchDamage;
		if (_state == AnimStates.GRAB)
		{
			if (!_waitingGrabReaction)
			{
				if (_groundGrab)
				{
					PrepeareAttack(_groundPunches);
					return;
				}
				_punchInd = UnityEngine.Random.Range(0, _punches.Length);
				_comboInd = _punches[_punchInd]._attacks.Length - 1;
				PrepeareAttack(_punches[_punchInd]);
			}
		}
		else
		{
			SetAnimFloat("Speed", 0f);
			PrepeareAttack(_punchBacks);
		}
	}

	public void BackKickCombo()
	{
		if (CantAttack())
		{
			return;
		}
		if (_lifting)
		{
			LiftThrow();
			return;
		}
		_currentDamage = _kickDamage;
		if (_state == AnimStates.GRAB)
		{
			if (!_waitingGrabReaction)
			{
				if (_groundGrab)
				{
					PrepeareAttack(_groundKicks);
					return;
				}
				_kickInd = UnityEngine.Random.Range(0, _kicks.Length);
				_comboInd = _kicks[_kickInd]._attacks.Length - 1;
				PrepeareAttack(_kicks[_kickInd]);
			}
		}
		else
		{
			SetAnimFloat("Speed", 0f);
			PrepeareAttack(_kickBacks);
		}
	}

	public void GrabCombo()
	{
		if (_grabbed)
		{
			GrabAttack();
		}
		else if (!CantGrab())
		{
			if (_lifting)
			{
				LiftThrow();
			}
			else
			{
				Grab();
			}
		}
	}

	private void Grab()
	{
		CharacterControl characterControl = GrabFight();
		if (characterControl != null)
		{
			SetAnimFloat("Speed", 0f);
			if ((bool)_playerControl && characterControl._iamGrabbed && !_attackLock)
			{
				GrabEmpty();
			}
			else
			{
				Grabbing(characterControl);
			}
			return;
		}
		BreakableObj breakableObj = LiftObject();
		if (breakableObj != null)
		{
			SetAnimFloat("Speed", 0f);
			Lift(breakableObj);
		}
		else if ((bool)_playerControl && !_attackLock)
		{
			GrabEmpty();
		}
	}

	private void GrabAttack()
	{
		SetAnimFloat("Speed", 0f);
		GrabbingAttack();
	}

	public void Jump()
	{
		if (_lifting)
		{
			LiftThrow();
		}
		else if (!CantJump())
		{
			SetAnimBool("Jump", tf: true);
		}
	}

	public void Jumping()
	{
		if (!CantJump())
		{
			SetAnimBool("Col2Ground", tf: false);
			SetAnimBool("JumpAttacking", tf: false);
			CancelStandUpInvoke();
			_animationEvents.ShowDustEffectJump();
			_gamePlayManager._makeNoise.PlaySFX(_femalePrefix + "Jump");
			SetAnimBool("Jump", tf: false);
			_state = AnimStates.JUMP;
			_myRigidbody.AddForce(0f, _jumpForce * Time.fixedDeltaTime * Config.FORCE_MULTIPLYER, 0f);
		}
	}

	public void Defence(bool tf)
	{
		if (!CantDefence() || !tf)
		{
			SetAnimBool("Defence", tf);
			if (tf)
			{
				_state = AnimStates.DEFENCE;
			}
		}
	}

	private void PrepeareAttack(AttackSet attackSet)
	{
		if (attackSet._comboStyle)
		{
			StartCoroutine(MakeAttack(attackSet._attacks[_comboInd]));
			CancelInvoke("CancelCombo");
			Invoke("CancelCombo", 1f);
		}
		else
		{
			StartCoroutine(MakeAttack(attackSet._attacks[Random.Range(0, attackSet._attacks.Length)]));
		}
	}

	private void CancelCombo()
	{
		_comboInd = 0;
	}

	public void AttackLock()
	{
		_attackLock = false;
		if (_backAttack)
		{
			_backAttack = false;
			if (_direction == 1)
			{
				_rot = 0f;
			}
			else
			{
				_rot = 180f;
			}
		}
	}

	public IEnumerator MakeAttack(AttackO attackObject)
	{
		_hitBefore.Clear();
		_usedWeapon = null;
		_state = AnimStates.ATTACK;
		_attackLock = true;
		_currentAttackObject = attackObject;
		SetAnimationState("Attack", attackObject.Attack);
		if (attackObject._attackType == AttackType.JumpAttack)
		{
			SetAnimBool("JumpAttacking", tf: true);
			_state = AnimStates.JUMPATTACK;
			yield return new WaitForSeconds(0.1f);
			while (_state == AnimStates.JUMPATTACK)
			{
				Check4Hit();
				yield return null;
			}
		}
	}

	public bool CanReact(AttackO attackObject)
	{
		if (_state == AnimStates.KNOCKDOWN && attackObject._attackType != AttackType.GroundAttack)
		{
			return false;
		}
		if ((bool)_playerControl)
		{
			if (_playerControl._inactive || _state == AnimStates.DEATH || _state == AnimStates.GRAB || _state == AnimStates.GRABBED || _state == AnimStates.KNOCKINGDOWN || _state == AnimStates.LIFT || _state == AnimStates.PICK)
			{
				return false;
			}
		}
		else if (_state == AnimStates.DEATH || _state == AnimStates.GRAB || _state == AnimStates.GRABBED || _state == AnimStates.KNOCKINGDOWN || _state == AnimStates.LIFT || _state == AnimStates.PICK)
		{
			return false;
		}
		return true;
	}

	public void MakeReaction(AttackO attackObject, float damage, Weapons weapon, bool cantEskiv, bool slowMotionEffect)
	{
		if ((bool)_playerControl && _playerControl._inactive)
		{
			return;
		}
		if (_lifting)
		{
			LiftDrop();
		}
		if ((bool)_enemyControl)
		{
			_enemyControl._dontMove = true;
			_enemyControl.CancelJumpingAttackInvoke();
		}
		ResetTriggers();
		SetAnimFloat("Speed", 0f);
		bool flag = attackObject._knockDown;
		bool flag2 = attackObject._poweredAttack;
		AttackType attackType = attackObject._attackType;
		int num = 0;
		AnimationO animationObj = null;
		if (_state == AnimStates.DEFENCE)
		{
			if (_attackDirection == _direction)
			{
				Defence(tf: false);
				_state = AnimStates.HIT;
			}
			else
			{
				if ((bool)_playerControl)
				{
					if (attackType == AttackType.FaceAttack || attackType == AttackType.JumpAttack)
					{
						num = _playerControl._eskivFreq;
						if (!cantEskiv && !CantEskiv())
						{
							Defence(tf: false);
							_state = AnimStates.HIT;
							num = _playerControl._eskivFreq;
							switch (attackObject._attackDirection)
							{
							case AttackDirection.Left:
								animationObj = _reactions._leftFaceEskiv[Random.Range(0, _reactions._leftFaceEskiv.Length)];
								break;
							case AttackDirection.Right:
								animationObj = _reactions._rightFaceEskiv[Random.Range(0, _reactions._rightFaceEskiv.Length)];
								break;
							}
							_gamePlayManager._makeNoise.PlaySFX(_femalePrefix + "Jump");
							AddForce(new Vector2(UnityEngine.Random.Range(2f, 2.5f), 0f));
							_attackLock = true;
							SetAnimationState("Attack", animationObj);
							return;
						}
					}
					if (!(weapon != null))
					{
						_gamePlayManager._makeNoise.PlaySFX("DefendHit");
						ShowDefendEffect(attackObject._hitPosition);
						AddForce(new Vector2(UnityEngine.Random.Range(1.5f, 2f), 0f));
						SetUIHealth();
						return;
					}
					Defence(tf: false);
					_state = AnimStates.HIT;
					damage *= 0.5f;
					flag2 = false;
					flag = false;
				}
				if (!flag && !flag2)
				{
					_gamePlayManager._makeNoise.PlaySFX("DefendHit");
					ShowDefendEffect(attackObject._hitPosition);
					AddForce(new Vector2(UnityEngine.Random.Range(1.5f, 2f), 0f));
					SetUIHealth();
					return;
				}
				Defence(tf: false);
				_state = AnimStates.HIT;
				damage *= 0.5f;
				if (attackType == AttackType.JumpAttack)
				{
					attackType = AttackType.FaceAttack;
				}
				if (flag2)
				{
					flag2 = false;
				}
				if (flag)
				{
					flag = false;
					flag2 = true;
				}
			}
		}
		else if (attackType == AttackType.FaceAttack || attackType == AttackType.JumpAttack)
		{
			if ((bool)_playerControl)
			{
				num = _playerControl._eskivFreq;
			}
			else if ((bool)_enemyControl)
			{
				num = _enemyControl._eskivFreq;
			}
			if (!cantEskiv && _attackDirection != _direction && num > UnityEngine.Random.Range(0, 100) && !CantEskiv())
			{
				switch (attackObject._attackDirection)
				{
				case AttackDirection.Left:
					animationObj = _reactions._leftFaceEskiv[Random.Range(0, _reactions._leftFaceEskiv.Length)];
					break;
				case AttackDirection.Right:
					animationObj = _reactions._rightFaceEskiv[Random.Range(0, _reactions._rightFaceEskiv.Length)];
					break;
				}
				_gamePlayManager._makeNoise.PlaySFX(_femalePrefix + "Jump");
				AddForce(new Vector2(UnityEngine.Random.Range(2f, 2.5f), 0f));
				_attackLock = true;
				SetAnimationState("Attack", animationObj);
				if ((bool)_enemyControl)
				{
					_enemyControl.AttackAfterEskiv();
				}
				return;
			}
		}
		float num2 = 1f;
		if (_state == AnimStates.AIR || _state == AnimStates.JUMP || _state == AnimStates.JUMPATTACK)
		{
			flag = true;
			num2 = 0.5f;
		}
		if (weapon != null)
		{
			_gamePlayManager._makeNoise.PlaySFX(weapon._hitSound);
		}
		ShowHitEffect(attackObject._hitPosition);
		if (_powerUpSpeedMultiplyer > 1f)
		{
			damage /= 2f;
		}
		if ((bool)_playerControl)
		{
			_standingGrabCount = 0;
			if (flag2)
			{
				_playerControl.MakeInaktive(1.5f);
			}
			else
			{
				_playerControl.MakeInaktive(0.75f);
			}
			_gamePlayManager._powerUpScript.SetPowerUpBarValue(0f - _powerUpHitValue);
		}
		else if (!_gamePlayManager._playerControl._poweredUp)
		{
			_gamePlayManager._powerUpScript.SetPowerUpBarValue(_powerUpAttackValue);
		}
		SetHealth(0f - damage);
		if (IsDeath())
		{
			return;
		}
		if (flag || flag2)
		{
			ChangeDirection();
			DropWeapon();
			_gamePlayManager._makeNoise.PlaySFX("HeavyHit");
			if (flag)
			{
				_state = AnimStates.KNOCKINGDOWN;
				if ((bool)_enemyControl)
				{
					_gamePlayManager._hUDScript.InfoTextEffect("Knockdown!!", _direction);
					_gamePlayManager._makeNoise.PlaySFX(_femalePrefix + "EnemyKO");
					if (slowMotionEffect)
					{
						_gamePlayManager._camSlowMotionDelay.StartSlowMotionDelay(0.2f);
					}
				}
				else
				{
					_gamePlayManager._makeNoise.PlaySFX(_femalePrefix + "PlayerKO");
				}
				SetAnimBool("KnockingDown", tf: true);
				SetAnimBool("StandUp", tf: false);
				SetAnimTrigger("KnockDown");
				_knockDownForce = UnityEngine.Random.Range(2.5f, 3.25f);
				AddForce(new Vector2(_knockDownForce, UnityEngine.Random.Range(2f, 3f) * num2));
				return;
			}
			_gamePlayManager._makeNoise.PlaySFX(_femalePrefix + "Hurt");
			if (flag2)
			{
				AddForce(new Vector2(UnityEngine.Random.Range(0.5f, 1.5f), 0f));
				if ((bool)_enemyControl)
				{
					_gamePlayManager._hUDScript.InfoTextEffect("Powered!", _direction);
				}
			}
		}
		else
		{
			_gamePlayManager._makeNoise.PlaySFX(_femalePrefix + "Hurt");
			_gamePlayManager._makeNoise.PlaySFX("NormalHit");
		}
		if (_state != AnimStates.KNOCKDOWN)
		{
			_state = AnimStates.HIT;
		}
		if (attackObject._customReactions.Length > 0)
		{
			animationObj = attackObject._customReactions[Random.Range(0, attackObject._customReactions.Length)];
		}
		else
		{
			switch (attackType)
			{
			case AttackType.FaceAttack:
				switch (attackObject._attackDirection)
				{
				case AttackDirection.Left:
					animationObj = ((!flag2) ? _reactions._leftFaceWeak[Random.Range(0, _reactions._leftFaceWeak.Length)] : _reactions._leftFaceHeavy[Random.Range(0, _reactions._leftFaceHeavy.Length)]);
					break;
				case AttackDirection.Right:
					animationObj = ((!flag2) ? _reactions._rightFaceWeak[Random.Range(0, _reactions._rightFaceWeak.Length)] : _reactions._rightFaceHeavy[Random.Range(0, _reactions._rightFaceHeavy.Length)]);
					break;
				}
				break;
			case AttackType.BoddyAttack:
				switch (attackObject._attackDirection)
				{
				case AttackDirection.Left:
					animationObj = ((!flag2) ? _reactions._leftBodyWeak[Random.Range(0, _reactions._leftBodyWeak.Length)] : _reactions._leftBodyHeavy[Random.Range(0, _reactions._leftBodyHeavy.Length)]);
					break;
				case AttackDirection.Right:
					animationObj = ((!flag2) ? _reactions._rightBodyWeak[Random.Range(0, _reactions._rightBodyWeak.Length)] : _reactions._rightBodyHeavy[Random.Range(0, _reactions._rightBodyHeavy.Length)]);
					break;
				}
				break;
			case AttackType.LegAttack:
				switch (attackObject._attackDirection)
				{
				case AttackDirection.Left:
					animationObj = ((!flag2) ? _reactions._leftLegWeak[Random.Range(0, _reactions._leftLegWeak.Length)] : _reactions._leftLegHeavy[Random.Range(0, _reactions._leftLegHeavy.Length)]);
					break;
				case AttackDirection.Right:
					animationObj = ((!flag2) ? _reactions._rightLegWeak[Random.Range(0, _reactions._rightLegWeak.Length)] : _reactions._rightLegHeavy[Random.Range(0, _reactions._rightLegHeavy.Length)]);
					break;
				}
				break;
			case AttackType.GroundAttack:
				if (_state == AnimStates.KNOCKDOWN)
				{
					CancelStandUpInvoke();
					StandUpInvoke(0.2f);
					animationObj = _reactions._ground[Random.Range(0, _reactions._ground.Length)];
				}
				else
				{
					animationObj = ((UnityEngine.Random.Range(0, 2) != 0) ? _reactions._rightBodyHeavy[Random.Range(0, _reactions._rightBodyHeavy.Length)] : _reactions._leftBodyHeavy[Random.Range(0, _reactions._leftBodyHeavy.Length)]);
				}
				break;
			case AttackType.JumpAttack:
				switch (attackObject._attackDirection)
				{
				case AttackDirection.Left:
					animationObj = _reactions._leftFaceHeavy[Random.Range(0, _reactions._leftFaceHeavy.Length)];
					break;
				case AttackDirection.Right:
					animationObj = _reactions._rightFaceHeavy[Random.Range(0, _reactions._rightFaceHeavy.Length)];
					break;
				}
				break;
			}
		}
		SetAnimationState("Reaction", animationObj);
	}

	public void SetHealth(float value)
	{
		_currentHealth += value;
		if (_currentHealth <= 0f)
		{
			_currentHealth = 0f;
		}
		if (_currentHealth > _maximumHealth)
		{
			_currentHealth = _maximumHealth;
		}
		if ((bool)_enemyControl)
		{
			_gamePlayManager._hUDScript.HitTextEffect(_direction);
		}
		SetUIHealth();
	}

	public void SetUIHealth()
	{
		if ((bool)_playerControl)
		{
			_gamePlayManager._hUDScript.ChangePlayerHealth(_maximumHealth, _currentHealth);
		}
		else if ((bool)_enemyControl)
		{
			_gamePlayManager._hUDScript.ChangeEnemyStatus(_maximumHealth, _currentHealth, _enemyControl._name, base.gameObject);
		}
	}

	public bool IsDeath()
	{
		if (_currentHealth == 0f)
		{
			StopAllCoroutines();
			CancelInvoke();
			DropWeapon();
			if (_state == AnimStates.STANDINGUP)
			{
				SetAnimationState("Die", _reactions._diesStandingUp[Random.Range(0, _reactions._diesStandingUp.Length)]);
			}
			else if (_animator.GetBool("KnockingDown"))
			{
				SetAnimationState("Die", _reactions._diesGround[Random.Range(0, _reactions._diesGround.Length)]);
			}
			else
			{
				SetAnimationState("Die", _reactions._diesStand[Random.Range(0, _reactions._diesStand.Length)]);
			}
			SetAnimBool("Death", tf: true);
			_gamePlayManager._makeNoise.PlaySFX("HeavyHit");
			if ((bool)_playerControl)
			{
				_playerControl._inactive = true;
				_gamePlayManager._makeNoise.PlaySFX(_femalePrefix + "PlayerDeath");
				_gamePlayManager._camSlowMotionDelay.StartSlowMotionDelay(0.5f);
				_gamePlayManager.PlayerDie();
			}
			else
			{
				_gamePlayManager._hUDScript.InfoTextEffect("Defeated", _direction);
				if (!_enemyControl._boss)
				{
					_gamePlayManager._makeNoise.PlaySFX(_femalePrefix + "EnemyDeath");
				}
				else
				{
					_gamePlayManager._makeNoise.PlaySFX("BossDeath");
				}
				_flicker.StartCoroutine(_flicker.FlickerCoroutine(1.5f));
				_gamePlayManager._camSlowMotionDelay.StartSlowMotionDelay(0.1f);
				_gamePlayManager.EnemyDie(_enemyControl);
			}
			_state = AnimStates.DEATH;
			SetAnimBool("KnockingDown", tf: false);
			CancelStandUpInvoke();
			SetAnimBool("StandUp", tf: false);
			return true;
		}
		return false;
	}

	private void SetLocomotionAnimationStates()
	{
		SetAnimFloat("IdleSpeed", _idle._animationSpeed * _powerUpSpeedMultiplyer);
		SetAnimBool("IdleMirror", _idle._mirrorAnimation);
		_animatorOverrideController["Idle"] = _idle._animation;
		SetAnimFloat("WalkSpeed", _walk._animationSpeed * _powerUpSpeedMultiplyer);
		SetAnimBool("WalkMirror", _walk._mirrorAnimation);
		_animatorOverrideController["Walk"] = _walk._animation;
	}

	public void SetAnimationState(string stateName, AnimationO animationObj)
	{
		SetAnimFloat(stateName + "Speed", animationObj._animationSpeed * _powerUpSpeedMultiplyer);
		SetAnimBool(stateName + "Mirror", animationObj._mirrorAnimation);
		_animatorOverrideController[stateName] = animationObj._animation;
		SetAnimTrigger(stateName);
	}

	public void SetAnimTrigger(string name)
	{
		_animator.SetTrigger(name);
	}

	public void SetAnimBool(string anim, bool tf)
	{
		_animator.SetBool(anim, tf);
	}

	public void SetAnimFloat(string anim, float value)
	{
		_animator.SetFloat(anim, value);
	}

	public void ShowHitEffect(float position)
	{
		_animationEvents.ShowHitEffect(position);
		if ((bool)_customHitEffect)
		{
			_customHitEffect.transform.position = base.transform.position + Vector3.up * position;
			_customHitEffect.SetActive(value: false);
			_customHitEffect.SetActive(value: true);
		}
	}

	public void ShowDefendEffect(float position)
	{
		_animationEvents.ShowDefendEffect(position);
	}

	public void ResetTriggers()
	{
		_animator.ResetTrigger("Attack");
		_animator.ResetTrigger("Reaction");
		_animator.ResetTrigger("Die");
		_animator.ResetTrigger("PickUp");
		_animator.ResetTrigger("Lift");
		_animator.ResetTrigger("KnockDown");
		_animator.ResetTrigger("Throw");
		SetAnimBool("JumpAttacking", tf: false);
		SetAnimBool("Jump", tf: false);
	}

	public void ToIDLE()
	{
		if (_dontRun || _state == AnimStates.KNOCKINGDOWN || _state == AnimStates.JUMP || _state == AnimStates.DEATH || _state == AnimStates.IDLE || _state == AnimStates.GRABBED)
		{
			return;
		}
		if (_currentHealth <= 0f)
		{
			SetAnimationState("Die", _reactions._diesStand[Random.Range(0, _reactions._diesStand.Length)]);
			SetAnimBool("Death", tf: true);
			_state = AnimStates.DEATH;
			return;
		}
		if (_iamGrabbed)
		{
			_iamGrabbed = false;
		}
		_state = AnimStates.IDLE;
		SetAnimBool("KnockingDown", tf: false);
		SetAnimBool("JumpAttacking", tf: false);
		if ((bool)_enemyControl && _enemyControl._dontMove)
		{
			_enemyControl._dontMove = false;
			_enemyControl.Seed();
		}
	}

	public void KnockDown()
	{
		if (_state != AnimStates.KNOCKDOWN)
		{
			_state = AnimStates.KNOCKDOWN;
			_iamGrabbed = false;
			_animationEvents.ShowDustEffectLand();
			_gamePlayManager._makeNoise.PlaySFX("Drop");
			_gamePlayManager._makeNoise.PlaySFX(_femalePrefix + "Land");
			CancelStandUpInvoke();
			StandUpInvoke(_standUpTime);
			SetAnimBool("Col2Ground", tf: true);
			SetAnimBool("JumpAttacking", tf: false);
			_gamePlayManager._camShake.Shake(0.3f);
		}
	}

	public void StandUp()
	{
		if (_state != AnimStates.KNOCKINGDOWN && _state != AnimStates.DEATH && _state != AnimStates.JUMP && _state != AnimStates.JUMPATTACK && _state != AnimStates.AIR)
		{
			if ((bool)_playerControl)
			{
				_playerControl.MakeInaktive(0.5f);
			}
			_state = AnimStates.STANDINGUP;
			SetAnimBool("StandUp", tf: true);
		}
	}

	public void CancelStandUpInvoke()
	{
		CancelInvoke("StandUp");
	}

	public void StandUpInvoke(float standUpTime)
	{
		CancelStandUpInvoke();
		Invoke("StandUp", standUpTime);
	}

	public void AddForce(Vector2 force)
	{
		_myRigidbody.velocity = Vector3.zero;
		_myRigidbody.AddForce(0f, force.y * Time.fixedDeltaTime * Config.FORCE_MULTIPLYER, (float)_attackDirection * force.x * Time.fixedDeltaTime * Config.FORCE_MULTIPLYER);
	}

	private float DistanceToGround()
	{
		RaycastHit hitInfo = default(RaycastHit);
		Vector3 position = _myTransform.position;
		float x = position.x;
		Vector3 position2 = _myTransform.position;
		float y = position2.y + 0.2f;
		Vector3 position3 = _myTransform.position;
		if (Physics.Raycast(new Vector3(x, y, position3.z), Vector3.down, out hitInfo, 10f, 1 << EnvironmentLayer))
		{
			return hitInfo.distance;
		}
		return 0f;
	}

	private void OnCollisionEnter(Collision collision)
	{
		if (_dontRun || _state == AnimStates.DEATH)
		{
			return;
		}
		if (collision.gameObject.layer == EnvironmentLayer)
		{
			if (_state == AnimStates.KNOCKINGDOWN)
			{
				AddForce(new Vector2(_knockDownForce * 1.3f, 0f));
				_state = AnimStates.KNOCKDOWN;
				_iamGrabbed = false;
				_animationEvents.ShowDustEffectLand();
				_gamePlayManager._makeNoise.PlaySFX("Drop");
				_gamePlayManager._makeNoise.PlaySFX(_femalePrefix + "Land");
				CancelStandUpInvoke();
				StandUpInvoke(_standUpTime);
				SetAnimBool("Col2Ground", tf: true);
				SetAnimBool("JumpAttacking", tf: false);
				_gamePlayManager._camShake.Shake(0.3f);
				return;
			}
			if (_state == AnimStates.AIR || _state == AnimStates.JUMP || _state == AnimStates.JUMPATTACK || _state == AnimStates.ATTACK)
			{
				_animationEvents.ShowDustEffectLand();
				_gamePlayManager._makeNoise.PlaySFX(_femalePrefix + "Land");
				SetAnimBool("Col2Ground", tf: true);
				SetAnimBool("JumpAttacking", tf: false);
				return;
			}
		}
		if (collision.gameObject.layer != DestroyableObjectLayer)
		{
			return;
		}
		if (_state == AnimStates.KNOCKINGDOWN)
		{
			collision.gameObject.GetComponent<BreakableObj>().Breake(-_direction);
		}
		else if (_state == AnimStates.AIR || _state == AnimStates.JUMP)
		{
			collision.gameObject.GetComponent<BreakableObj>().Breake(_direction);
		}
		else
		{
			if (_state != 0 || !_enemyControl)
			{
				return;
			}
			if (_enemyControl._canLift)
			{
				if (UnityEngine.Random.Range(0, 2) == 0)
				{
					GrabCombo();
				}
				else
				{
					PunchCombo();
				}
			}
			else
			{
				PunchCombo();
			}
		}
	}

	private void OnCollisionExit(Collision collision)
	{
		if (!_dontRun && _state != AnimStates.DEATH && collision.gameObject.layer == EnvironmentLayer)
		{
			if (_state == AnimStates.KNOCKINGDOWN || _state == AnimStates.JUMP)
			{
				SetAnimBool("Col2Ground", tf: false);
			}
			else if (DistanceToGround() > 1f && _state == AnimStates.IDLE)
			{
				_state = AnimStates.AIR;
				SetAnimBool("Col2Ground", tf: false);
			}
		}
	}

	public void CheckWeaponInRange()
	{
		foreach (Weapons availableWeapon in _gamePlayManager._availableWeapons)
		{
			float num = Vector3.Distance(_myTransform.position, availableWeapon._myTransform.position);
			if (num < availableWeapon._pickUpRange && !availableWeapon._picked)
			{
				if (!_weaponsInRange.Contains(availableWeapon))
				{
					_weaponsInRange.Add(availableWeapon);
				}
			}
			else if (_weaponsInRange.Contains(availableWeapon))
			{
				_weaponsInRange.Remove(availableWeapon);
			}
		}
	}

	public bool WeaponAction()
	{
		if (CanPickWeapon())
		{
			return true;
		}
		if (_holdedWeaopons.Count > 0)
		{
			UseWeapon();
			return true;
		}
		return false;
	}

	private bool CanPickWeapon()
	{
		if (_weaponsInRange.Count == 0 || _state != 0 || _lifting)
		{
			return false;
		}
		Weapons weapons = _weaponsInRange[0];
		float num = Vector3.Distance(_myTransform.position, _weaponsInRange[0]._myTransform.position);
		for (int i = 0; i < _weaponsInRange.Count; i++)
		{
			if (Vector3.Distance(_myTransform.position, _weaponsInRange[i]._myTransform.position) < num && !weapons._picked)
			{
				weapons = _weaponsInRange[i];
			}
		}
		if (weapons == null)
		{
			return false;
		}
		if (weapons._picked)
		{
			return false;
		}
		if (weapons._animationHand == Hand.Both)
		{
			if (_holdedWeaopons.Count == 0)
			{
				weapons.holdedHand = Hand.Both;
				_holdedWeaopons.Add(weapons);
				PickWeapon(weapons, 0f);
				return true;
			}
		}
		else
		{
			if (_holdedWeaopons.Count == 0)
			{
				weapons.holdedHand = Hand.Right;
				_holdedWeaopons.Add(weapons);
				PickWeapon(weapons, 1f);
				return true;
			}
			if (_holdedWeaopons.Count == 1 && _holdedWeaopons[0].holdedHand == Hand.Left)
			{
				weapons.holdedHand = Hand.Right;
				_holdedWeaopons.Add(weapons);
				PickWeapon(weapons, 1f);
				return true;
			}
			if (_holdedWeaopons.Count == 1 && _holdedWeaopons[0].holdedHand == Hand.Right)
			{
				weapons.holdedHand = Hand.Left;
				_holdedWeaopons.Add(weapons);
				PickWeapon(weapons, -1f);
				return true;
			}
		}
		return false;
	}

	private void UseWeapon()
	{
		Weapons weapons = null;
		if (_holdedWeaopons.Count == 2)
		{
			weapons = _holdedWeaopons[_switchHandInd];
			if (_switchHandInd == 0)
			{
				_switchHandInd = 1;
			}
			else
			{
				_switchHandInd = 0;
			}
		}
		else
		{
			weapons = _holdedWeaopons[0];
		}
		if (weapons._useCount <= 0)
		{
			ThrowWeapon(weapons);
			return;
		}
		if ((bool)weapons._attackEffect)
		{
			weapons._attackEffect.SetActive(value: false);
			weapons._attackEffect.SetActive(value: true);
		}
		int num = UnityEngine.Random.Range(0, weapons._weaponAttacks.Length);
		AttackO attackO = new AttackO();
		AnimationO animationO = new AnimationO();
		animationO._animation = weapons._weaponAttacks[num].Attack._animation;
		animationO._mirrorAnimation = weapons._weaponAttacks[num].Attack._mirrorAnimation;
		animationO._animationSpeed = weapons._weaponAttacks[num].Attack._animationSpeed;
		attackO.Attack = animationO;
		attackO._attackDirection = weapons._weaponAttacks[num]._attackDirection;
		attackO._attackType = weapons._weaponAttacks[num]._attackType;
		attackO._poweredAttack = weapons._weaponAttacks[num]._poweredAttack;
		attackO._knockDown = weapons._weaponAttacks[num]._knockDown;
		attackO._hitPosition = weapons._weaponAttacks[num]._hitPosition;
		attackO._customReactions = weapons._weaponAttacks[num]._customReactions;
		attackO.CollSize = weapons._weaponAttacks[num].CollSize;
		if (weapons.holdedHand != weapons._animationHand)
		{
			animationO._mirrorAnimation = !animationO._mirrorAnimation;
			if (attackO._attackDirection == AttackDirection.Left)
			{
				attackO._attackDirection = AttackDirection.Right;
			}
			if (attackO._attackDirection == AttackDirection.Right)
			{
				attackO._attackDirection = AttackDirection.Left;
			}
		}
		_currentDamage = weapons._currentDamage;
		StartCoroutine(MakeAttack(attackO));
		_usedWeapon = weapons;
	}

	private void ThrowWeapon(Weapons weapon)
	{
		weapon._currentDamage = weapon._throwDamage * _weaponDamageMuliplyer;
		if (_powerUpSpeedMultiplyer > 1f)
		{
			weapon._currentDamage *= 2f;
		}
		_throwedWeapon = weapon;
		_state = AnimStates.ATTACK;
		_attackLock = true;
		AnimationO animationO = new AnimationO();
		animationO._animation = weapon._throwAttack.Attack._animation;
		animationO._mirrorAnimation = weapon._throwAttack.Attack._mirrorAnimation;
		animationO._animationSpeed = weapon._throwAttack.Attack._animationSpeed;
		if (weapon.holdedHand != weapon._animationHand)
		{
			animationO._mirrorAnimation = !animationO._mirrorAnimation;
		}
		SetAnimationState("Attack", animationO);
	}

	public void WeaponThrowed()
	{
		_holdedWeaopons.Remove(_throwedWeapon);
		_attackLock = false;
		_throwedWeapon._myTransform.parent = null;
		_gamePlayManager._availableWeapons.Remove(_throwedWeapon);
		_throwedWeapon.Throw(_direction, HitLayerMask);
		_throwedWeapon = null;
	}

	private void PickWeapon(Weapons weapon, float ind)
	{
		SetWeaponValues(weapon);
		_pickedWeapon = weapon;
		_state = AnimStates.PICK;
		_attackLock = true;
		SetAnimFloat("PickHand", ind);
		SetAnimTrigger("PickUp");
	}

	private void SetWeaponValues(Weapons weapon)
	{
		weapon._myRigidBody.velocity = Vector3.zero;
		weapon._myRigidBody.angularVelocity = Vector3.zero;
		weapon._myRigidBody.isKinematic = true;
		weapon._picked = true;
		weapon.DefaultMaterial();
		weapon._currentDamage = weapon._damage * _weaponDamageMuliplyer;
	}

	public void WeaponPicked()
	{
		if ((bool)_playerControl)
		{
			_gamePlayManager._makeNoise.PlaySFX("Pick");
		}
		ParentWeaponToHand();
	}

	private void ParentWeaponToHand()
	{
		Transform parent = null;
		switch (_pickedWeapon.holdedHand)
		{
		case Hand.Left:
			parent = LeftHandWeaponHolder;
			break;
		case Hand.Right:
			parent = RightHandWeaponHolder;
			break;
		case Hand.Both:
			parent = RightHandWeaponHolder;
			break;
		}
		_pickedWeapon._myTransform.parent = parent;
		_pickedWeapon._myTransform.localPosition = Vector3.zero;
		_pickedWeapon._myTransform.localEulerAngles = Vector3.zero;
		_pickedWeapon = null;
		_attackLock = false;
	}

	public void DropWeapon()
	{
		if (_holdedWeaopons.Count != 0)
		{
			foreach (Weapons holdedWeaopon in _holdedWeaopons)
			{
				holdedWeaopon._picked = false;
				holdedWeaopon._enemyWantsToPick = false;
				holdedWeaopon._myTransform.parent = null;
				holdedWeaopon._myRigidBody.isKinematic = false;
				holdedWeaopon.AddForce(new Vector2(UnityEngine.Random.Range(-0.5f, 0.5f), UnityEngine.Random.Range(0.5f, 1.2f)), 1f);
				holdedWeaopon.ShiningMaterial();
			}
			_holdedWeaopons.Clear();
		}
	}

	public void AlreadyHasWeapon()
	{
		if (_bothtHandWeapon != 0)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate(Resources.Load("items/" + Config.WeaponNames[_bothtHandWeapon]), Vector3.zero, Quaternion.identity) as GameObject;
			_pickedWeapon = gameObject.GetComponent<Weapons>();
			_pickedWeapon.holdedHand = Hand.Both;
			_holdedWeaopons.Add(_pickedWeapon);
			SetWeaponValues(_pickedWeapon);
			ParentWeaponToHand();
			return;
		}
		if (_leftHandWeapon != 0)
		{
			GameObject gameObject2 = UnityEngine.Object.Instantiate(Resources.Load("items/" + Config.WeaponNames[_leftHandWeapon]), Vector3.zero, Quaternion.identity) as GameObject;
			_pickedWeapon = gameObject2.GetComponent<Weapons>();
			_pickedWeapon.holdedHand = Hand.Left;
			_holdedWeaopons.Add(_pickedWeapon);
			SetWeaponValues(_pickedWeapon);
			ParentWeaponToHand();
		}
		if (_rightHandWeapon != 0)
		{
			GameObject gameObject3 = UnityEngine.Object.Instantiate(Resources.Load("items/" + Config.WeaponNames[_rightHandWeapon]), Vector3.zero, Quaternion.identity) as GameObject;
			_pickedWeapon = gameObject3.GetComponent<Weapons>();
			_pickedWeapon.holdedHand = Hand.Right;
			_holdedWeaopons.Add(_pickedWeapon);
			SetWeaponValues(_pickedWeapon);
			ParentWeaponToHand();
		}
	}

	private void Lift(BreakableObj breakableObj)
	{
		_attackLock = true;
		_lifting = true;
		_state = AnimStates.LIFT;
		breakableObj._lifted = true;
		_liftedObj = breakableObj;
		breakableObj.gameObject.layer = LayerMask.NameToLayer("Item");
		SetAnimationState("Lift", breakableObj._liftAnimation);
	}

	public void Lifting()
	{
		_gamePlayManager._makeNoise.PlaySFX(_femalePrefix + "Jump");
		_liftedObj.transform.parent = RightHandWeaponHolder;
		Transform transform = _liftedObj.transform;
		Vector3 localScale = _liftedObj.transform.localScale;
		transform.localPosition = new Vector3(-4f * localScale.x / 10f, 0f, 0f);
		_liftedObj.transform.localEulerAngles = Vector3.zero;
	}

	public void Lifted()
	{
		_attackLock = false;
		_gamePlayManager._makeNoise.PlaySFX(_femalePrefix + "Land");
		_animatorOverrideController["Idle"] = _liftedObj._liftOwerWriteAnimation._animation;
		_animatorOverrideController["LiftLayer"] = _liftedObj._liftOwerWriteAnimation._animation;
		_animator.SetLayerWeight(1, 1f);
	}

	private void LiftThrow()
	{
		_state = AnimStates.ATTACK;
		_attackLock = true;
		_animatorOverrideController["Idle"] = _idle._animation;
		_animator.SetLayerWeight(1, 0f);
		_liftedObj._currentDamage = _liftedObj._throwDamage * _weaponDamageMuliplyer;
		if (_powerUpSpeedMultiplyer > 1f)
		{
			_liftedObj._currentDamage *= 2f;
		}
		SetAnimationState("Attack", _liftedObj._throwAttack.Attack);
	}

	public void LiftThrowed()
	{
		_gamePlayManager._makeNoise.PlaySFX(_femalePrefix + "Jump");
		_gamePlayManager._makeNoise.PlaySFX("Whoosh");
		_attackLock = false;
		_lifting = false;
		_liftedObj._lifted = false;
		_liftedObj.transform.parent = null;
		_liftedObj.Throw(_direction, HitLayerMask);
		_liftedObj = null;
	}

	private void LiftDrop()
	{
		_attackLock = false;
		_lifting = false;
		_liftedObj._lifted = false;
		_liftedObj.transform.parent = null;
		_liftedObj.Drop(_direction);
		_liftedObj = null;
		_animatorOverrideController["Idle"] = _idle._animation;
		_animator.SetLayerWeight(1, 0f);
	}

	public void GrabCheck()
	{
		if (!_grabbed || _state == AnimStates.GRABBED || _enemyCharacterControl._state == AnimStates.DEATH)
		{
			return;
		}
		if (_enemyCharacterControl._state != AnimStates.GRABBED || _state != AnimStates.GRAB)
		{
			CancelInvoke("Escape");
			if (_state != AnimStates.JUMP && _state != AnimStates.ATTACK)
			{
				ReleaseMe();
			}
			if (_enemyCharacterControl._state != AnimStates.KNOCKINGDOWN)
			{
				ReleaseEnemy();
			}
			SetRelease();
		}
		Transform animatorTransform = _enemyCharacterControl._animatorTransform;
		Vector3 position = _animatorTransform.position;
		float x = position.x;
		Vector3 position2 = _enemyCharacterControl._animatorTransform.position;
		float y = position2.y;
		Vector3 position3 = _animatorTransform.position;
		animatorTransform.position = new Vector3(x, y, position3.z + (float)_grabbedDirection * _currentGrab._grapPosition);
	}

	private void GrabEmpty()
	{
		if (_state == AnimStates.IDLE)
		{
			_attackLock = true;
			SetAnimFloat("AttackSpeed", _grabEmpty._animationSpeed * _powerUpSpeedMultiplyer);
			_animatorOverrideController["Attack"] = _grabEmpty._animation;
			SetAnimTrigger("Attack");
		}
	}

	public void Grabbing(CharacterControl enemyCharacterControl)
	{
		if (_grabbed || enemyCharacterControl._state == AnimStates.GRABBED || enemyCharacterControl._state == AnimStates.DEATH || enemyCharacterControl._state == AnimStates.AIR || enemyCharacterControl._state == AnimStates.JUMP || enemyCharacterControl._state == AnimStates.JUMPATTACK || enemyCharacterControl._state == AnimStates.KNOCKINGDOWN || enemyCharacterControl._state == AnimStates.STANDINGUP)
		{
			return;
		}
		if (enemyCharacterControl._lifting)
		{
			enemyCharacterControl.LiftDrop();
		}
		if ((bool)_playerControl)
		{
			if (!_gamePlayManager._levelDesignScript._specialBossScene)
			{
				_gamePlayManager._camFollow.CloserEffect();
			}
			if (_standingGrabCount < 2)
			{
				_standingGrabCount++;
				enemyCharacterControl._enemyControl.AvoidGrab();
			}
			else
			{
				enemyCharacterControl._enemyControl.AvoidGrabbing();
			}
		}
		else
		{
			_standingGrabCount = 0;
		}
		_grabbedDirection = _direction;
		enemyCharacterControl.CancelStandUpInvoke();
		enemyCharacterControl.SetAnimBool("StandUp", tf: false);
		_enemyCharacterControl = enemyCharacterControl;
		_enemyCharacterControl._iamGrabbed = true;
		if (_enemyCharacterControl._state == AnimStates.KNOCKDOWN || _enemyCharacterControl._state == AnimStates.STANDINGUP)
		{
			_groundGrab = true;
			_currentGrab = _groundGrabs[Random.Range(0, _groundGrabs.Length)];
		}
		else
		{
			_groundGrab = false;
			_enemyCharacterControl.SetAnimBool("KnockingDown", tf: false);
			_currentGrab = _standingGrabs[Random.Range(0, _standingGrabs.Length)];
		}
		if (enemyCharacterControl._state == AnimStates.ATTACK)
		{
			GrabEmpty();
			if (_gamePlayManager._bossInTheScene)
			{
				_gamePlayManager._camFollow.BossEffect();
			}
			else
			{
				_gamePlayManager._camFollow.Reset();
			}
			return;
		}
		if (_enemyCharacterControl._state == AnimStates.DEFENCE)
		{
			CancelInvoke("Escape");
			Invoke("Escape", 0.25f);
			SetAnimFloat("AttackSpeed", _currentGrab._grabbCombos[0]._animSpeed);
			_animatorOverrideController["Attack"] = _currentGrab._grabbCombos[0].Attack;
			SetAnimTrigger("Attack");
			return;
		}
		_grabbed = true;
		_waitingGrabReaction = false;
		_state = AnimStates.GRAB;
		_enemyCharacterControl._state = AnimStates.GRABBED;
		SetAnimBool("Grab", tf: true);
		_enemyCharacterControl.SetAnimBool("Grab", tf: true);
		_currentGrab._comboInd = 0;
		_animatorTransform.LookAt(_enemyCharacterControl._animatorTransform.position);
		_enemyCharacterControl._attackDirection = _direction;
		_enemyCharacterControl.ChangeDirection();
		_enemyCharacterControl.DropWeapon();
		_enemyCharacterControl.SetUIHealth();
		GrabbingAttack();
	}

	public void GrabbingAttack()
	{
		if (!_waitingGrabReaction)
		{
			CancelInvoke("Escape");
			_waitingGrabReaction = true;
			SetAnimFloat("AttackSpeed", _currentGrab._grabbCombos[_currentGrab._comboInd]._animSpeed);
			_animatorOverrideController["Attack"] = _currentGrab._grabbCombos[_currentGrab._comboInd].Attack;
			SetAnimTrigger("Attack");
			if ((bool)_currentGrab._grabbCombos[_currentGrab._comboInd].AttackIdle)
			{
				_animatorOverrideController["GrabIdle"] = _currentGrab._grabbCombos[_currentGrab._comboInd].AttackIdle;
			}
			_enemyCharacterControl.SetAnimFloat("ReactionSpeed", _currentGrab._grabbCombos[_currentGrab._comboInd]._animSpeed);
			_enemyCharacterControl._animatorOverrideController["Reaction"] = _currentGrab._grabbCombos[_currentGrab._comboInd].Reaction;
			_enemyCharacterControl.SetAnimTrigger("Reaction");
			if ((bool)_currentGrab._grabbCombos[_currentGrab._comboInd].ReactionIdle)
			{
				_enemyCharacterControl._animatorOverrideController["GrabIdle"] = _currentGrab._grabbCombos[_currentGrab._comboInd].ReactionIdle;
			}
		}
	}

	public void GrabHit()
	{
		if ((bool)_playerControl && !_playerControl._poweredUp)
		{
			_gamePlayManager._powerUpScript.SetPowerUpBarValue(_powerUpAttackValue);
		}
		_enemyCharacterControl.ShowHitEffect(_currentGrab._grabbCombos[_currentGrab._comboInd]._hitPosition);
		_enemyCharacterControl._gamePlayManager._makeNoise.PlaySFX("HeavyHit");
		_enemyCharacterControl.SetHealth(0f - _grabDamage);
		_enemyCharacterControl._gamePlayManager._camShake.Shake(0.1f);
		if (_enemyCharacterControl.IsDeath())
		{
			CancelInvoke("Escape");
			SetRelease();
		}
		else if (_currentGrab._comboInd == _currentGrab._grabbCombos.Length - 1)
		{
			if ((bool)_enemyCharacterControl._playerControl)
			{
				_enemyCharacterControl._gamePlayManager._makeNoise.PlaySFX(_enemyCharacterControl._femalePrefix + "PlayerKO");
				return;
			}
			_enemyCharacterControl._gamePlayManager._makeNoise.PlaySFX(_enemyCharacterControl._femalePrefix + "EnemyKO");
			_enemyCharacterControl._gamePlayManager._camSlowMotionDelay.StartSlowMotionDelay(0.2f);
		}
		else
		{
			_enemyCharacterControl._gamePlayManager._makeNoise.PlaySFX(_enemyCharacterControl._femalePrefix + "Hurt");
		}
	}

	public void GrabReaction()
	{
		if (_enemyCharacterControl._state != AnimStates.DEATH)
		{
			_waitingGrabReaction = false;
			_currentGrab._comboInd++;
			if (_currentGrab._comboInd >= _currentGrab._grabbCombos.Length)
			{
				SetRelease();
			}
			else
			{
				Invoke("Escape", _currentGrab._releaseTime);
			}
		}
	}

	private void Escape()
	{
		ReleaseMe();
		ReleaseEnemy();
		SetRelease();
		_gamePlayManager._makeNoise.PlaySFX("Whoosh");
		_gamePlayManager._makeNoise.PlaySFX(_femalePrefix + "Jump");
	}

	private void ReleaseMe()
	{
		SetAnimFloat("AttackSpeed", _currentGrab._escape._animSpeed);
		_animatorOverrideController["Attack"] = _currentGrab._escape.Attack;
		SetAnimTrigger("Attack");
	}

	private void ReleaseEnemy()
	{
		_enemyCharacterControl.SetAnimBool("Defence", tf: false);
		_enemyCharacterControl.SetAnimFloat("ReactionSpeed", _currentGrab._escape._animSpeed);
		_enemyCharacterControl._animatorOverrideController["Reaction"] = _currentGrab._escape.Reaction;
		_enemyCharacterControl.SetAnimTrigger("Reaction");
	}

	public void SetRelease()
	{
		if ((bool)_enemyCharacterControl)
		{
			_enemyCharacterControl.SetAnimBool("Grab", tf: false);
			if (_enemyCharacterControl._state != AnimStates.DEATH)
			{
				_enemyCharacterControl._state = AnimStates.HIT;
				if (_groundGrab)
				{
					_enemyCharacterControl._state = AnimStates.KNOCKDOWN;
					if (_currentGrab._comboInd >= _currentGrab._grabbCombos.Length)
					{
						_enemyCharacterControl.StandUpInvoke(_enemyCharacterControl._standUpTime);
					}
					else
					{
						_enemyCharacterControl.StandUpInvoke(0.75f);
					}
				}
			}
		}
		SetAnimBool("Grab", tf: false);
		_grabbed = false;
		_waitingGrabReaction = false;
		if (!_playerControl)
		{
			return;
		}
		_playerControl.MakeInaktive(0.5f);
		if (!_gamePlayManager._levelDesignScript._specialBossScene)
		{
			if (_gamePlayManager._bossInTheScene)
			{
				_gamePlayManager._camFollow.BossEffect();
			}
			else
			{
				_gamePlayManager._camFollow.Reset();
			}
		}
	}

	private void ShadowFollow()
	{
		if (Physics.Raycast(_myTransform.position, Vector3.down * _rayDist, out RaycastHit hitInfo, _rayDist, 1 << EnvironmentLayer))
		{
			SetShadowPosition(hitInfo);
			Vector3 position = _myTransform.position;
			float y = position.y;
			Vector3 point = hitInfo.point;
			SetShadowScale(y - point.y);
			if (_followTerrainRotation)
			{
				SetShadowRotation(hitInfo.normal);
			}
		}
	}

	private void SetShadowPosition(RaycastHit hit)
	{
		_ShadowTransform.position = hit.point + Vector3.up * _yoffset;
	}

	private void SetShadowRotation(Vector3 normal)
	{
		_ShadowTransform.rotation = Quaternion.FromToRotation(Vector3.up, normal);
	}

	private void SetShadowScale(float floorDistance)
	{
		float num = floorDistance / _distanceScale;
		float num2 = _blobShadowSize + num;
		_ShadowTransform.localScale = new Vector3(num2, num2, num2);
	}
}
