using ControlFreak2;
using UnityEngine;

public class PlayerControl : MonoBehaviour
{
	public string _name;

	public Sprite _portrait;

	public int _id;

	public bool _unlocked;

	[Header("Prices")]
	public int _price;

	public int _healthUpPrice = 20;

	public int _weaponTecUpPrice = 15;

	public int _punchUpgradePrice = 30;

	public int _kickUpgradePrice = 35;

	[Header("Values")]
	public float _health = 25f;

	public float _punchPower = 5f;

	public float _kickPower = 5f;

	public float _weaponTecnique = 1f;

	public int _eskivFreq = 30;

	[HideInInspector]
	public float _healthU;

	[HideInInspector]
	public float _weaponTecniqueU;

	[HideInInspector]
	public float _punchPowerU;

	[HideInInspector]
	public float _kickPowerU;

	[Header("GamePlay")]
	public bool _inactive;

	public bool _waitForInput;

	public bool _powerUpCharacter;

	public bool _poweredUp;

	[HideInInspector]
	public CharacterControl _characterControl;

	private Vector3 _axis;

	public void AwakeProc()
	{
		_characterControl = GetComponent<CharacterControl>();
		_characterControl.AwakeProc();
	}

	public void Reset()
	{
		_characterControl._currentHealth = _characterControl._maximumHealth;
		_characterControl.SetAnimBool("StandUp", tf: false);
		_characterControl.SetAnimBool("KnockingDown", tf: false);
		_characterControl.SetAnimBool("JumpAttacking", tf: false);
		_characterControl.SetAnimBool("Death", tf: false);
		_characterControl._state = AnimStates.IDLE;
		_characterControl.ResetTriggers();
		_characterControl.SetUIHealth();
	}

	public void SetValues()
	{
		_healthU = _health + (float)DataFunctions.GetIntData(_id.ToString() + Config.PP_CharacterHealthUpgradeCount) * 0.5f;
		_weaponTecniqueU = _weaponTecnique + (float)DataFunctions.GetIntData(_id.ToString() + Config.PP_CharacterWeaponTecniqueUpgradeCount) * 0.2f;
		_punchPowerU = _punchPower + (float)DataFunctions.GetIntData(_id.ToString() + Config.PP_CharacterPunchPowerUpgradeCount) * 0.2f;
		_kickPowerU = _kickPower + (float)DataFunctions.GetIntData(_id.ToString() + Config.PP_CharacterKickPowerUpgradeCount) * 0.2f;
	}

	public void UpdateProc()
	{
		if ((_powerUpCharacter && _characterControl._state == AnimStates.IDLE) || CF2Input.GetKeyDown(KeyCode.P))
		{
			MakeInaktive(2f);
			_inactive = true;
			_poweredUp = true;
			_powerUpCharacter = false;
			_characterControl.SetAnimBool("PowerUpMirror", _characterControl._direction < 0);
			_characterControl.SetAnimTrigger("PowerUp");
		}
		if (_characterControl._state != AnimStates.POWERINGUP)
		{
			Combat();
			if (!_characterControl._backAttack)
			{
				Movemet();
			}
		}
	}

	private void Combat()
	{
		_axis.x = CF2Input.GetAxis("Keyboard Up-Down");
		_axis.z = CF2Input.GetAxis("Keyboard Left-Right");
		if (CF2Input.GetKey(KeyCode.LeftArrow) || CF2Input.GetKey(KeyCode.RightArrow) || CF2Input.GetKey(KeyCode.UpArrow) || CF2Input.GetKey(KeyCode.DownArrow))
		{
			WaitforInput();
		}
		if (CF2Input.GetKey(KeyCode.V))
		{
			WaitforInput();
			_characterControl.Defence(tf: true);
		}
		else if (CF2Input.GetKeyUp(KeyCode.V))
		{
			WaitforInput();
			_characterControl.Defence(tf: false);
		}
		else if (CF2Input.GetKeyDown(KeyCode.Z))
		{
			WaitforInput();
			if (_axis.z < 0f && _characterControl._direction == 1)
			{
				_characterControl._backAttack = true;
				_characterControl.BackPunchCombo();
				_characterControl._direction = -1;
			}
			else if (_axis.z > 0f && _characterControl._direction == -1)
			{
				_characterControl._backAttack = true;
				_characterControl.BackPunchCombo();
				_characterControl._direction = 1;
			}
			else
			{
				_characterControl.PunchCombo();
			}
		}
		else if (CF2Input.GetKeyDown(KeyCode.X))
		{
			WaitforInput();
			if (CF2Input.GetKeyDown(KeyCode.LeftArrow) && _characterControl._direction == 1)
			{
				_characterControl._backAttack = true;
				_characterControl.BackKickCombo();
				_characterControl._direction = -1;
			}
			else if (CF2Input.GetKeyDown(KeyCode.RightArrow) && _characterControl._direction == -1)
			{
				_characterControl._backAttack = true;
				_characterControl.BackKickCombo();
				_characterControl._direction = 1;
			}
			else
			{
				_characterControl.KickCombo();
			}
		}
		else if (CF2Input.GetKeyDown(KeyCode.C))
		{
			WaitforInput();
			_characterControl.GrabCombo();
		}
		else if (CF2Input.GetKeyDown(KeyCode.Space))
		{
			WaitforInput();
			_characterControl.Jump();
		}
	}

	private void Movemet()
	{
		if (!_characterControl.CantRotate())
		{
			if (_axis.z > 0.2f)
			{
				_characterControl._direction = 1;
			}
			if (_axis.z < -0.2f)
			{
				_characterControl._direction = -1;
			}
		}
		if (_axis.z < 0.2f && _axis.z > -0.2f)
		{
			_axis.z = 0f;
		}
		_characterControl.LookatDirection();
		_characterControl.WalkAnimation(Mathf.Abs(_axis.x) + Mathf.Abs(_axis.z));
		_characterControl.Movement(_axis);
	}

	public void MakeInaktive(float delay)
	{
		_inactive = true;
		CancelInvoke("CancelInactive");
		Invoke("CancelInactive", delay);
	}

	public void CancelInactiveInvoke()
	{
		CancelInvoke("CancelInactive");
	}

	private void CancelInactive()
	{
		if (_characterControl._state != AnimStates.DEATH && !_waitForInput)
		{
			_inactive = false;
		}
	}

	private void WaitforInput()
	{
		if (_waitForInput)
		{
			_inactive = false;
			_waitForInput = false;
		}
	}
}
