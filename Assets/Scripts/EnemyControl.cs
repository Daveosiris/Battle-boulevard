using System.Collections.Generic;
using UnityEngine;

public class EnemyControl : MonoBehaviour
{
	public bool _boss;

	[Header("Values")]
	public float _health = 10f;

	public float _punchPower = 2f;

	public float _kickPower = 2f;

	public float _weaponTecnique = 0.5f;

	[Space(5f)]
	public string _name;

	public bool _seeSawRange;

	public float _seeDistance = 5f;

	public Vector2 _combatRate = new Vector2(1f, 3f);

	public int _attackFreq = 50;

	public int _grabFreq = 10;

	public int _defenceFreq = 10;

	public int _eskivFreq = 10;

	public int _avoidGrabFreq = 10;

	public int _jumpAttackFreq;

	public bool _canPickWeapon;

	public bool _canLift;

	public bool _canAttackWhilePlayerInAir;

	public bool _canPickBanana;

	private Vector3 destinationPos = Vector3.zero;

	private float _stopDistance = 1f;

	private float _moveSpeed;

	private bool _combatLock;

	private bool _iSaw;

	private bool _jumpingAtackInProgress;

	private Weapons _gonnaPickWeapon;

	public bool _dontMove;

	public int _orangeCount;

	public int[] _orangeCounts;

	[HideInInspector]
	public CharacterControl _characterControl;

	[HideInInspector]
	public CharacterControl _playerCharacterControl;

	[HideInInspector]
	private List<Weapons> _canPickableWeapons = new List<Weapons>();

	public void AwakeProc()
	{
		_characterControl = GetComponent<CharacterControl>();
		_characterControl.AwakeProc();
		Seed();
	}

	public void SetValues()
	{
		_characterControl._maximumHealth = (_characterControl._currentHealth = _health);
		_characterControl._punchDamage = _punchPower / 3f;
		_characterControl._kickDamage = _kickPower / 2f;
		_characterControl._grabDamage = (_characterControl._punchDamage + _characterControl._kickDamage) / 2f;
		_characterControl._weaponDamageMuliplyer = _weaponTecnique;
	}

	public void Seed()
	{
		_stopDistance = UnityEngine.Random.Range(0.75f, 1.5f);
		_moveSpeed = UnityEngine.Random.Range(_characterControl._speed - 0.3f, _characterControl._speed + 0.3f);
		_jumpingAtackInProgress = false;
		CombatLock();
	}

	public void UpdateProc()
	{
		if (_characterControl._state != 0)
		{
			_dontMove = true;
			if (_jumpingAtackInProgress)
			{
				_characterControl.Movement(new Vector3(0f, 0f, _characterControl._direction));
			}
			else if (_characterControl._state == AnimStates.GRAB)
			{
				Combat();
			}
		}
		else if (!_dontMove)
		{
			float num = Vector3.Distance(_characterControl._myTransform.position, _playerCharacterControl._myTransform.position);
			if (!_iSaw && num < _seeDistance)
			{
				_iSaw = true;
			}
			if (_iSaw && !Combat())
			{
				Movement();
			}
		}
	}

	private bool CantCombo()
	{
		if (UnityEngine.Random.Range(0, 100) >= _attackFreq)
		{
			return true;
		}
		if (_canAttackWhilePlayerInAir)
		{
			if (_combatLock || _playerCharacterControl._playerControl._inactive || _playerCharacterControl._state == AnimStates.ATTACK || _playerCharacterControl._state == AnimStates.GRAB || _playerCharacterControl._state == AnimStates.JUMPATTACK || _playerCharacterControl._state == AnimStates.KNOCKINGDOWN || _playerCharacterControl._state == AnimStates.PICK)
			{
				return true;
			}
		}
		else if (_combatLock || _playerCharacterControl._playerControl._inactive || _playerCharacterControl._state == AnimStates.ATTACK || _playerCharacterControl._state == AnimStates.GRAB || _playerCharacterControl._state == AnimStates.JUMPATTACK || _playerCharacterControl._state == AnimStates.KNOCKINGDOWN || _playerCharacterControl._state == AnimStates.AIR || _playerCharacterControl._state == AnimStates.JUMP || _playerCharacterControl._state == AnimStates.PICK)
		{
			return true;
		}
		return false;
	}

	private bool Combat()
	{
		float num = Vector3.Distance(destinationPos, _characterControl._myTransform.position);
		if (num > 0.5f && num <= 3.5f && !CantCombo())
		{
			if (_characterControl._holdedWeaopons.Count == 1 && _characterControl._holdedWeaopons[0]._useCount <= 0)
			{
				_characterControl.PunchCombo();
				return true;
			}
			if (_characterControl._lifting && num <= 2f)
			{
				_characterControl.PunchCombo();
				return true;
			}
			if (UnityEngine.Random.Range(0, 100) <= _jumpAttackFreq && _gonnaPickWeapon == null && num <= 1.5f)
			{
				_jumpingAtackInProgress = true;
				_characterControl.Jump();
				CancelJumpingAttackInvoke();
				Invoke("JumpingAttack", 0.25f);
				return true;
			}
		}
		if (num <= 0.5f || _characterControl._state == AnimStates.GRAB)
		{
			if (_gonnaPickWeapon != null)
			{
				_characterControl.PunchCombo();
				_gonnaPickWeapon = null;
				CombatLock();
				return true;
			}
			if (!CantCombo())
			{
				if (UnityEngine.Random.Range(0, 100) < _grabFreq)
				{
					_characterControl.GrabCombo();
				}
				else if (UnityEngine.Random.Range(0, 2) == 0)
				{
					_characterControl.PunchCombo();
				}
				else
				{
					_characterControl.KickCombo();
				}
				CombatLock();
				return true;
			}
			return false;
		}
		return false;
	}

	private void CombatLock()
	{
		_combatLock = true;
		CancelInvoke("CombatUnLock");
		if (_characterControl._state == AnimStates.GRAB)
		{
			Invoke("CombatUnLock", UnityEngine.Random.Range(0.1f, 0.5f));
		}
		else
		{
			Invoke("CombatUnLock", UnityEngine.Random.Range(_combatRate.x, _combatRate.y));
		}
	}

	private void CombatUnLock()
	{
		_combatLock = false;
	}

	private void Movement()
	{
		Vector3 position = _playerCharacterControl._myTransform.position;
		float z = position.z;
		Vector3 position2 = _characterControl._myTransform.position;
		if (z <= position2.z)
		{
			_characterControl._direction = -1;
		}
		else
		{
			_characterControl._direction = 1;
		}
		bool flag = false;
		if (_gonnaPickWeapon != null && !_gonnaPickWeapon._picked)
		{
			flag = true;
		}
		if (flag)
		{
			float num = Vector3.Distance(_characterControl._myTransform.position, _playerCharacterControl._myTransform.position);
			float num2 = Vector3.Distance(_characterControl._myTransform.position, _gonnaPickWeapon._myTransform.position);
			if (num < num2)
			{
				_gonnaPickWeapon._enemyWantsToPick = false;
				_gonnaPickWeapon = null;
			}
			else
			{
				destinationPos = _gonnaPickWeapon._myTransform.position;
			}
		}
		else
		{
			destinationPos = _playerCharacterControl._myTransform.position + Vector3.forward * -_characterControl._direction * _stopDistance;
			WeaponChecker();
		}
		if (_characterControl._gamePlayManager._bananaTransform.Count > 0 && _canPickBanana)
		{
			destinationPos = _characterControl._gamePlayManager._bananaTransform[0].position;
		}
		float num3 = 1f;
		float z2 = destinationPos.z;
		Vector3 position3 = _characterControl._myTransform.position;
		if ((z2 - position3.z) * (float)_characterControl._direction < -0.01f)
		{
			num3 = -1f;
		}
		float num4 = Vector3.Distance(destinationPos, _characterControl._myTransform.position);
		float num5 = 0f;
		if (num4 > 0.05f)
		{
			num5 = 1f;
		}
		_characterControl.LookatDirection();
		Vector3 normalized = (destinationPos - _characterControl._myTransform.position).normalized;
		Vector3 position4 = _characterControl._myTransform.position;
		normalized.y = position4.y;
		_characterControl._myTransform.position += normalized * _moveSpeed * num5 * Time.deltaTime;
		_characterControl.SetAnimFloat("Speed", num3 * num5);
	}

	private void WeaponChecker()
	{
		if (_canPickWeapon)
		{
			_canPickableWeapons.Clear();
			if ((bool)_gonnaPickWeapon)
			{
				_gonnaPickWeapon._enemyWantsToPick = false;
			}
			_gonnaPickWeapon = null;
			if (_characterControl._holdedWeaopons.Count < 2)
			{
				foreach (Weapons availableWeapon in _characterControl._gamePlayManager._availableWeapons)
				{
					if (Vector3.Distance(_characterControl._myTransform.position, availableWeapon._myTransform.position) < Vector3.Distance(_characterControl._myTransform.position, _playerCharacterControl._myTransform.position) && !availableWeapon._picked && !availableWeapon._enemyWantsToPick && !availableWeapon._enemyCantPick)
					{
						if (_characterControl._holdedWeaopons.Count == 0)
						{
							_canPickableWeapons.Add(availableWeapon);
						}
						else if (_characterControl._holdedWeaopons.Count == 1 && availableWeapon._animationHand != Hand.Both && _characterControl._holdedWeaopons[0].holdedHand != Hand.Both)
						{
							_canPickableWeapons.Add(availableWeapon);
						}
					}
				}
			}
			if (_canPickableWeapons.Count > 0)
			{
				Weapons weapons = _canPickableWeapons[0];
				foreach (Weapons canPickableWeapon in _canPickableWeapons)
				{
					if (Vector3.Distance(_characterControl._myTransform.position, canPickableWeapon._myTransform.position) < Vector3.Distance(_characterControl._myTransform.position, weapons._myTransform.position))
					{
						weapons = canPickableWeapon;
					}
				}
				_gonnaPickWeapon = weapons;
				weapons._enemyWantsToPick = true;
			}
		}
	}

	private void JumpingAttack()
	{
		if (UnityEngine.Random.Range(0, 2) == 0)
		{
			_characterControl.PunchCombo();
		}
		else
		{
			_characterControl.KickCombo();
		}
	}

	public void CancelJumpingAttackInvoke()
	{
		CancelInvoke("JumpingAttack");
		CancelInvoke("CombatLock");
		CancelInvoke("Attack");
	}

	public void Defence()
	{
		if (UnityEngine.Random.Range(0, 100) <= _defenceFreq)
		{
			_characterControl.Defence(tf: true);
			CancelInvoke("DefenceOff");
			Invoke("DefenceOff", 0.5f);
		}
	}

	private void DefenceOff()
	{
		_characterControl.Defence(tf: false);
	}

	public void AvoidGrab()
	{
		if (UnityEngine.Random.Range(0, 100) <= _avoidGrabFreq)
		{
			AvoidGrabbing();
		}
	}

	public void AvoidGrabbing()
	{
		if (UnityEngine.Random.Range(0, 2) == 0)
		{
			if (UnityEngine.Random.Range(0, 2) == 0)
			{
				_characterControl.PunchCombo();
			}
			else
			{
				_characterControl.KickCombo();
			}
		}
		else
		{
			_characterControl.Defence(tf: true);
			CancelInvoke("DefenceOff");
			Invoke("DefenceOff", 0.5f);
		}
	}

	public void AttackAfterEskiv()
	{
		CancelInvoke("Attack");
		Invoke("Attack", UnityEngine.Random.Range(0.2f, 0.5f));
	}

	private void Attack()
	{
		_characterControl.ToIDLE();
		if (UnityEngine.Random.Range(0, 2) == 0)
		{
			_characterControl.PunchCombo();
		}
		else
		{
			_characterControl.KickCombo();
		}
	}
}
