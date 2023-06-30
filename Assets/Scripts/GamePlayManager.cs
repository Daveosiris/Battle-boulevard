using System.Collections.Generic;
using UnityEngine;

public class GamePlayManager : MonoBehaviour
{
	public GameObject _playerPrefab;

	[Header("Stats")]
	public int _currentLevel;

	public PlayerControl _playerControl;

	public List<EnemyControl> _activeEnemies = new List<EnemyControl>();

	public List<Weapons> _allWeapons = new List<Weapons>();

	public List<Weapons> _availableWeapons = new List<Weapons>();

	public List<Orange> _oranges = new List<Orange>();

	public int _currentWaveInd;

	public bool _gameOver;

	public bool _secondChanceUsed;

	public bool _levelCompleted;

	public float _levelCompletePercent;

	public int _allEnemieCount;

	public int _currentEnemieCount;

	public int _totalOranges;

	public int _levelOrangeCount;

	public int _collectedOranges;

	[HideInInspector]
	public LevelDesignScript _levelDesignScript;

	[HideInInspector]
	public GamePlayMenu _gamePlayMenu;

	[HideInInspector]
	public HUDScript _hUDScript;

	[HideInInspector]
	public PowerUpScript _powerUpScript;

	[HideInInspector]
	public GameOverScript _gameOverScript;

	[HideInInspector]
	public LevelCompletedScript _levelCompletedScript;

	[HideInInspector]
	public HandPointer _handPointer;

	[HideInInspector]
	public CamFollow _camFollow;

	[HideInInspector]
	public CamShake _camShake;

	[HideInInspector]
	public CamSlowMotionDelay _camSlowMotionDelay;

	[HideInInspector]
	public ColorEffectScript _colorEffectScript;

	[HideInInspector]
	public GiveMeTheMusic _giveMeTheMusic;

	[HideInInspector]
	public MakeNoise _makeNoise;

	[HideInInspector]
	public float _up;

	[HideInInspector]
	public float _down;

	[HideInInspector]
	public float _left;

	[HideInInspector]
	public float _right;

	[HideInInspector]
	public bool _bossInTheScene;

	private bool _checkPlayerInWave;

	private float _prewiusWaveEndPoint;

	private Vector2 _randomSpawnOffset;

	private bool _spawnSameTime;

	private bool _spawnLeft;

    public GameObject review;

    private int playtime;

    [HideInInspector]
	public bool _clampCharacterPositions;

	[HideInInspector]
	public List<Transform> _bananaTransform = new List<Transform>();

	private void Awake()
	{
		if ((bool)UnityEngine.Object.FindObjectOfType<AdManager>())
		{
			//AdManager adManager = UnityEngine.Object.FindObjectOfType<AdManager>();
			//adManager.RequestInterstitial();
			//adManager.ShowBanner();
		}
		else
		{
			Application.targetFrameRate = 60;
		}
		_gameOver = false;
		_levelCompleted = false;
		_bossInTheScene = false;
		_clampCharacterPositions = false;
		_makeNoise = GetComponent<MakeNoise>();
		_giveMeTheMusic = GetComponent<GiveMeTheMusic>();
		_gamePlayMenu = UnityEngine.Object.FindObjectOfType<GamePlayMenu>();
		_gamePlayMenu._gamePlayManager = this;
		_hUDScript = UnityEngine.Object.FindObjectOfType<HUDScript>();
		_powerUpScript = UnityEngine.Object.FindObjectOfType<PowerUpScript>();
		_powerUpScript._gamePlayManager = this;
		_gameOverScript = UnityEngine.Object.FindObjectOfType<GameOverScript>();
		_levelCompletedScript = UnityEngine.Object.FindObjectOfType<LevelCompletedScript>();
		_handPointer = UnityEngine.Object.FindObjectOfType<HandPointer>();
		_handPointer._gamePlayManager = this;
		_colorEffectScript = UnityEngine.Object.FindObjectOfType<ColorEffectScript>();
		_colorEffectScript._effectVignette.SetActive(value: false);
		_colorEffectScript.AwakeProc();
		_currentLevel = GlobalVariables._level;
		if (_currentLevel == 0)
		{
			_currentLevel = 1;
		}
		if ((bool)UnityEngine.Object.FindObjectOfType<LevelDesignScript>())
		{
			_levelDesignScript = UnityEngine.Object.FindObjectOfType<LevelDesignScript>();
			_currentLevel = _levelDesignScript._level;
		}
		else
		{
			Object.Instantiate(Resources.Load("Levels/Level" + _currentLevel));
			_levelDesignScript = UnityEngine.Object.FindObjectOfType<LevelDesignScript>();
		}
		_down = _levelDesignScript._down;
		_up = _levelDesignScript._up;
		if (_levelDesignScript._specialBossScene)
		{
			_left = -4.5f;
			_right = 5.5f;
			_colorEffectScript.SpecialBossEffect();
		}
		if ((bool)GlobalVariables._player)
		{
			_playerPrefab = GlobalVariables._player;
		}
		GameObject gameObject = UnityEngine.Object.Instantiate(_playerPrefab);
		_playerControl = gameObject.GetComponent<PlayerControl>();
		_playerControl.AwakeProc();
		_playerControl._characterControl._gamePlayManager = this;
		_playerControl._characterControl._myTransform.position = GameObject.Find("PlayerSpawnPos").transform.position;
		_playerControl._characterControl._stayInStage = true;
		AnalyticsManager.SendEventInfo(_playerControl._name);
		_camFollow = UnityEngine.Object.FindObjectOfType<CamFollow>();
		_camFollow.SetPosition(_playerControl._characterControl._myTransform.position);
		_camFollow._target = _playerControl._characterControl._myTransform;
		_camFollow._minLeft = _levelDesignScript._startPoint;
		_camShake = UnityEngine.Object.FindObjectOfType<CamShake>();
		_camSlowMotionDelay = UnityEngine.Object.FindObjectOfType<CamSlowMotionDelay>();
		_hUDScript.ChangePlayerNamePortrait(_playerControl._name, _playerControl._portrait);
		_hUDScript.ChangePlayerHealth(_playerControl._characterControl._maximumHealth, _playerControl._characterControl._currentHealth);
		_hUDScript.ChangeLevelName(_currentLevel);
		_hUDScript.ChangeLevelStatus(_levelCompletePercent);
		_collectedOranges = 0;
		_hUDScript.ChangeCollectedOrangeText(_collectedOranges);
		GameObject gameObject2 = null;
		for (int i = 0; i < _levelDesignScript._waves.Length; i++)
		{
			_levelDesignScript._waves[i]._stillEnemies.Clear();
			_levelDesignScript._waves[i]._waveEnemies.Clear();
			_levelDesignScript._waves[i]._bossEnemies.Clear();
			for (int j = 0; j < _levelDesignScript._waves[i].stillEnemies.Length; j++)
			{
				gameObject2 = UnityEngine.Object.Instantiate(_levelDesignScript._waves[i].stillEnemies[j]._enemy, _levelDesignScript._waves[i].stillEnemies[j]._position, Quaternion.identity);
				_levelDesignScript._waves[i]._stillEnemies.Add(gameObject2.GetComponent<EnemyControl>());
			}
			for (int k = 0; k < _levelDesignScript._waves[i].waweEnemies.Length; k++)
			{
				gameObject2 = UnityEngine.Object.Instantiate(_levelDesignScript._waves[i].waweEnemies[k], Vector3.zero, Quaternion.identity);
				_levelDesignScript._waves[i]._waveEnemies.Add(gameObject2.GetComponent<EnemyControl>());
			}
			for (int l = 0; l < _levelDesignScript._waves[i].bossEnemies.Length; l++)
			{
				gameObject2 = UnityEngine.Object.Instantiate(_levelDesignScript._waves[i].bossEnemies[l], Vector3.zero, Quaternion.identity);
				_levelDesignScript._waves[i]._bossEnemies.Add(gameObject2.GetComponent<EnemyControl>());
			}
		}
		_currentWaveInd = 0;
		EnemyControl[] array = UnityEngine.Object.FindObjectsOfType<EnemyControl>();
		_allEnemieCount = (_currentEnemieCount = array.Length);
		float num = Config.DiffucultyMultiplayers[DataFunctions.GetIntData(Config.PP_Diffuculty)];
		_levelOrangeCount = (int)(Config.LevelOrangeCount * num) + (int)(Config.LevelOrangeCount * num * Config.LevelValuesMultiplyer * (float)(_currentLevel - 1));
		int orangeCount = _levelOrangeCount / _allEnemieCount;
		int num2 = _levelOrangeCount % _allEnemieCount;
		int num3 = 0;
		EnemyControl[] array2 = array;
		foreach (EnemyControl enemyControl in array2)
		{
			if (enemyControl._boss)
			{
				num3++;
			}
			enemyControl._orangeCount = orangeCount;
			if (num2 > 0)
			{
				enemyControl._orangeCount++;
				num2--;
			}
		}
		float num4 = Config.EnemyLevelHealth * num + Config.EnemyLevelHealth * num * Config.LevelValuesMultiplyer * (float)(_currentLevel - 1);
		EnemyControl[] array3 = array;
		foreach (EnemyControl enemyControl2 in array3)
		{
			enemyControl2._playerCharacterControl = _playerControl._characterControl;
			enemyControl2.AwakeProc();
			enemyControl2._characterControl._gamePlayManager = this;
			enemyControl2._characterControl._ShadowTransform.gameObject.SetActive(value: false);
			enemyControl2.gameObject.SetActive(value: false);
			if (enemyControl2._boss)
			{
				if (_levelDesignScript._specialBossScene)
				{
					enemyControl2._health = Config.SPBossHealth * num + Config.SPBossHealth * num * Config.LevelValuesMultiplyer * (float)(_currentLevel - 4);
					enemyControl2._punchPower = (enemyControl2._kickPower = Config.SPBossPower * num + Config.SPBossPower * num * Config.LevelValuesMultiplyer * (float)(_currentLevel - 4));
					enemyControl2._weaponTecnique = Config.SPBossWeaponTeqnique * num + Config.SPBossWeaponTeqnique * num * Config.LevelValuesMultiplyer * (float)(_currentLevel - 4);
					enemyControl2._seeDistance = 5f;
					enemyControl2._characterControl._stayInStage = true;
					enemyControl2._canPickBanana = true;
				}
				else
				{
					enemyControl2._health = Config.BossHealth * num + Config.BossHealth * num * Config.LevelValuesMultiplyer * (float)(_currentLevel - 1);
					enemyControl2._punchPower = (enemyControl2._kickPower = Config.BossPower * num + Config.BossPower * num * Config.LevelValuesMultiplyer * (float)(_currentLevel - 1));
					enemyControl2._weaponTecnique = Config.BossWeaponTeqnique * num + Config.BossWeaponTeqnique * num * Config.LevelValuesMultiplyer * (float)(_currentLevel - 1);
				}
			}
			else if (_levelDesignScript._specialBossScene)
			{
				enemyControl2._health = Config.ArcadeLevelFixedEnemyHealth * num + Config.ArcadeLevelFixedEnemyHealth * num * Config.LevelValuesMultiplyer * (float)(_currentLevel - 12);
				enemyControl2._punchPower = (enemyControl2._kickPower = Config.EnemyPower * num + Config.EnemyPower * num * Config.LevelValuesMultiplyer * (float)(_currentLevel - 1));
				enemyControl2._weaponTecnique = Config.EnemyWeaponTeqnique * num + Config.EnemyWeaponTeqnique * num * Config.LevelValuesMultiplyer * (float)(_currentLevel - 1);
			}
			else
			{
				enemyControl2._health = num4 / (float)(_allEnemieCount - num3);
				enemyControl2._punchPower = (enemyControl2._kickPower = Config.EnemyPower * num + Config.EnemyPower * num * Config.LevelValuesMultiplyer * (float)(_currentLevel - 1));
				enemyControl2._weaponTecnique = Config.EnemyWeaponTeqnique * num + Config.EnemyWeaponTeqnique * num * Config.LevelValuesMultiplyer * (float)(_currentLevel - 1);
			}
			enemyControl2.SetValues();
			_levelDesignScript._dropCreateDamage = Config.DropCrateDamage * num + Config.DropCrateDamage * num * Config.LevelValuesMultiplyer * (float)(_currentLevel - 1);
			if (enemyControl2._orangeCount > Config.MaximumSpawnOrangeCount)
			{
				int num5 = enemyControl2._orangeCount / Config.MaximumSpawnOrangeCount;
				int num6 = enemyControl2._orangeCount % Config.MaximumSpawnOrangeCount;
				enemyControl2._orangeCounts = new int[Config.MaximumSpawnOrangeCount];
				for (int num7 = 0; num7 < Config.MaximumSpawnOrangeCount; num7++)
				{
					GameObject gameObject3 = UnityEngine.Object.Instantiate(Resources.Load("Orange"), Vector3.zero, Quaternion.identity) as GameObject;
					gameObject3.SetActive(value: false);
					_oranges.Add(gameObject3.GetComponent<Orange>());
					enemyControl2._orangeCounts[num7] = num5;
					if (num6 > 0)
					{
						enemyControl2._orangeCounts[num7]++;
						num6--;
					}
				}
			}
			else
			{
				enemyControl2._orangeCounts = new int[enemyControl2._orangeCount];
				for (int num8 = 0; num8 < enemyControl2._orangeCount; num8++)
				{
					GameObject gameObject4 = UnityEngine.Object.Instantiate(Resources.Load("Orange"), Vector3.zero, Quaternion.identity) as GameObject;
					gameObject4.SetActive(value: false);
					_oranges.Add(gameObject4.GetComponent<Orange>());
					enemyControl2._orangeCounts[num8] = 1;
				}
			}
		}
		BreakableObj[] array4 = UnityEngine.Object.FindObjectsOfType<BreakableObj>();
		for (int num9 = 0; num9 < array4.Length; num9++)
		{
			array4[num9]._orangeCount = UnityEngine.Random.Range(0, Config.MaximumSpawnOrangeCount);
			for (int num10 = 0; num10 < array4[num9]._orangeCount; num10++)
			{
				GameObject gameObject5 = UnityEngine.Object.Instantiate(Resources.Load("Orange"), Vector3.zero, Quaternion.identity) as GameObject;
				gameObject5.SetActive(value: false);
				_oranges.Add(gameObject5.GetComponent<Orange>());
			}
		}
		_totalOranges = DataFunctions.GetIntData(Config.PP_Orange_Count);
		_playerControl._characterControl._dontRun = false;
	}

	private void Start()
	{
		SetWave();
		if (_levelDesignScript._dropPercent > 0)
		{
			InvokeRepeating("DropCrate", _levelDesignScript._dropCratefreqSecond, _levelDesignScript._dropCratefreqSecond);
		}
		Invoke("UnlockLeftRightCalculate", 0.5f);
	}

	private void Update()
	{
		if (!_levelDesignScript._specialBossScene)
		{
			CalculateLeftRightBounds();
		}
		if (_checkPlayerInWave)
		{
			float prewiusWaveEndPoint = _prewiusWaveEndPoint;
			Vector3 position = _playerControl._characterControl._myTransform.position;
			if (prewiusWaveEndPoint - position.z < 2f)
			{
				_checkPlayerInWave = false;
				_handPointer.DeActivateHandPointer();
			}
		}
	}

	private void UnlockLeftRightCalculate()
	{
		_clampCharacterPositions = true;
	}

	private void CalculateLeftRightBounds()
	{
		Plane[] array = GeometryUtility.CalculateFrustumPlanes(Camera.main);
		Vector3 vector = array[0].ClosestPointOnPlane(_playerControl._characterControl._myTransform.position);
		Vector3 vector2 = array[1].ClosestPointOnPlane(_playerControl._characterControl._myTransform.position);
		_left = vector.z + _playerControl._characterControl._wallCheckDist;
		_right = vector2.z - _playerControl._characterControl._wallCheckDist;
	}

	public void PlayerDie()
	{
		//AnalyticsManager.SendEventInfo("PlayerDie");
		_gameOver = true;
		_camFollow.Reset();
		CancelInvoke("GameOver");
		Invoke("GameOver", 2.5f);
	}

	private void GameOver()
	{
		DataFunctions.SaveData(Config.PP_Orange_Count, _totalOranges);
		_gameOverScript.FillValues(_levelCompletePercent, _collectedOranges, _totalOranges, _secondChanceUsed, _levelOrangeCount / 2);
		_gamePlayMenu.GameOverMenu();
		_colorEffectScript.DisableEffect();
		_powerUpScript.ResetUI();
		_giveMeTheMusic.PlayMusic("GameOver");
		CancelInvoke("DropCrate");
        Object.FindObjectOfType<Admobs>().ShowAds();

    }

	private void LevelCompLeted()
	{
		DataFunctions.SaveData(Config.PP_Orange_Count, _totalOranges);
		//AnalyticsManager.SendEventInfo("lvl " + _currentLevel + "fin");
		_levelCompleted = true;
		_levelCompletedScript.CheckNextLevelExists(_currentLevel);
		_levelCompletedScript.FillValues(_collectedOranges, 0, _totalOranges);
		_gamePlayMenu.LevelCompletedMenu();
		_colorEffectScript.DisableEffect();
		_powerUpScript.ResetUI();
		_giveMeTheMusic.PlayMusic("GameOver");
		CancelInvoke("DropCrate");
        playtime = PlayerPrefs.GetInt("PlayTime", 0);
        playtime += 1;
        if (playtime == 2 || playtime == 4 || playtime == 6 || playtime == 8 || playtime == 10 || playtime == 12 || playtime == 14)
        {
            review.SetActive(true);
        }
        PlayerPrefs.SetInt("PlayTime", playtime);
        PlayerPrefs.Save();
    }

	public void RevivePlayer()
	{
		_gameOver = false;
		_secondChanceUsed = true;
		_playerControl.Reset();
		_gamePlayMenu.HUDMenu();
		if (_levelDesignScript._dropPercent > 0)
		{
			InvokeRepeating("DropCrate", _levelDesignScript._dropCratefreqSecond, _levelDesignScript._dropCratefreqSecond);
		}
		if (_levelDesignScript._specialBossScene)
		{
			_giveMeTheMusic.PlayMusic("SPBoss");
		}
		else if (_bossInTheScene)
		{
			_camFollow.BossEffect();
			_giveMeTheMusic.PlayMusic("Boss");
		}
		else
		{
			_camFollow.Reset();
			_giveMeTheMusic.PlayMusic("Game");
		}
	}

	public void DoubleOranges()
	{
		_totalOranges += _collectedOranges;
		DataFunctions.SaveData(Config.PP_Orange_Count, _totalOranges);
		_levelCompletedScript.FillValues(_collectedOranges, _collectedOranges, _totalOranges);
		_makeNoise.PlaySFX("Pick");
	}

	public void EnemyDie(EnemyControl enemyControl)
	{
		//AnalyticsManager.SendEventInfo("EnemyDie");
		for (int i = 0; i < enemyControl._orangeCounts.Length; i++)
		{
			Orange orange = _oranges[0];
			orange.Spawn(enemyControl._characterControl._myTransform.position);
			orange._orangeCount = enemyControl._orangeCounts[i];
			_oranges.Remove(orange);
		}
		_currentEnemieCount--;
		if (_currentEnemieCount < 0)
		{
			_currentEnemieCount = 0;
		}
		_levelCompletePercent = (1f - (float)_currentEnemieCount / (float)_allEnemieCount) * 100f;
		_hUDScript.ChangeLevelStatus(_levelCompletePercent);
		_activeEnemies.Remove(enemyControl);
		enemyControl._characterControl._ShadowTransform.gameObject.SetActive(value: false);
		Wave wave = _levelDesignScript._waves[_currentWaveInd];
		if (wave._stillEnemies.Contains(enemyControl))
		{
			wave._stillEnemies.Remove(enemyControl);
		}
		if (wave._stillEnemies.Count > 0)
		{
			return;
		}
		if (wave._waveEnemies.Count > 0)
		{
			_bossInTheScene = false;
			_spawnSameTime = false;
			_randomSpawnOffset = new Vector2(2f, 5f);
			SpawnEnemy(wave._waveEnemies);
		}
		else
		{
			if (_activeEnemies.Count > 0)
			{
				return;
			}
			if (wave._bossEnemies.Count > 0)
			{
				if (!_bossInTheScene)
				{
					Invoke("BossMusic", 1.5f);
				}
				_bossInTheScene = true;
				_spawnSameTime = true;
				_randomSpawnOffset = new Vector2(7f, 10f);
				SpawnEnemy(wave._bossEnemies);
			}
			else if (_activeEnemies.Count <= 0)
			{
				if (_currentWaveInd < _levelDesignScript._waves.Length - 1)
				{
					_prewiusWaveEndPoint = _levelDesignScript._waves[_currentWaveInd]._endPoint;
					_handPointer.ActivateHandPointer();
					_checkPlayerInWave = true;
					_currentWaveInd++;
					SetWave();
				}
				else
				{
					_gameOver = true;
					_camFollow.Reset();
					_camSlowMotionDelay.StartSlowMotionDelay(1.5f);
					CancelInvoke("LevelCompLeted");
					Invoke("LevelCompLeted", 3f);
				}
			}
		}
	}

	private void BossMusic()
	{
		_camFollow.BossEffect();
		if (_playerControl._characterControl._powerUpSpeedMultiplyer == 1f)
		{
			_giveMeTheMusic.PlayMusic("Boss");
		}
	}

	private void SpawnEnemy(List<EnemyControl> enemyControls)
	{
		int num = enemyControls.Count;
		if (!_spawnSameTime)
		{
			if (_activeEnemies.Count >= _levelDesignScript._waves[_currentWaveInd]._maxSpawnEnemyCount + 1)
			{
				return;
			}
			num = UnityEngine.Random.Range(1, _levelDesignScript._waves[_currentWaveInd]._maxSpawnEnemyCount + 1 - _activeEnemies.Count);
			if (num > enemyControls.Count)
			{
				num = enemyControls.Count;
			}
		}
		for (int i = 0; i < num; i++)
		{
			EnemyControl enemyControl = enemyControls[Random.Range(0, enemyControls.Count)];
			enemyControl._characterControl._myTransform.position = FindEnemySpawnPoint();
			enemyControl._seeDistance = 50f;
			enemyControl.gameObject.SetActive(value: true);
			enemyControl._characterControl._ShadowTransform.gameObject.SetActive(value: true);
			enemyControls.Remove(enemyControl);
			_activeEnemies.Add(enemyControl);
			if (!enemyControl._boss)
			{
				GiveWeapon(enemyControl);
			}
		}
	}

	private Vector3 FindEnemySpawnPoint()
	{
		float num = UnityEngine.Random.Range(_randomSpawnOffset.x, _randomSpawnOffset.y);
		_spawnLeft = !_spawnLeft;
		float z = _left - num;
		if (!_spawnLeft)
		{
			z = _right + num;
		}
		Vector3 position = _playerControl._characterControl._myTransform.position;
		float x = position.x;
		Vector3 position2 = _playerControl._characterControl._myTransform.position;
		return new Vector3(x, position2.y, z);
	}

	private void SetWave()
	{
		if (_levelDesignScript._specialBossScene)
		{
			_giveMeTheMusic.PlayMusic("SPBoss");
		}
		else if (_playerControl._characterControl._powerUpSpeedMultiplyer == 1f)
		{
			_giveMeTheMusic.PlayMusic("Game");
		}
		foreach (EnemyControl stillEnemy in _levelDesignScript._waves[_currentWaveInd]._stillEnemies)
		{
			if (_levelDesignScript._specialBossScene)
			{
				stillEnemy._seeDistance = 5f;
			}
			stillEnemy.gameObject.SetActive(value: true);
			stillEnemy._characterControl._ShadowTransform.gameObject.SetActive(value: true);
			_activeEnemies.Add(stillEnemy);
			GiveWeapon(stillEnemy);
		}
		_camFollow._maxRight = _levelDesignScript._waves[_currentWaveInd]._endPoint;
	}

	public void PickOrange(Orange orange)
	{
		_oranges.Remove(orange);
		_collectedOranges += orange._orangeCount;
		_totalOranges += orange._orangeCount;
		_hUDScript.ChangeCollectedOrangeText(_collectedOranges);
		_makeNoise.PlaySFX("Pick");
	}

	public void BananaSpawned(Transform bananaT)
	{
		_bananaTransform.Add(bananaT);
	}

	public void PickBanana(float percent, Transform bananaT)
	{
		float health = _playerControl._characterControl._maximumHealth * percent / 100f;
		_playerControl._characterControl.SetHealth(health);
		_makeNoise.PlaySFX("Pick");
		_bananaTransform.Remove(bananaT);
	}

	public void EnemyPickedBanana(EnemyControl enemyControl, float percent, Transform bananaT)
	{
		float health = enemyControl._characterControl._maximumHealth * percent / 100f;
		enemyControl._characterControl.SetHealth(health);
		_makeNoise.PlaySFX("Pick");
		_bananaTransform.Remove(bananaT);
	}

	private void GiveWeapon(EnemyControl enemyControl)
	{
		if (enemyControl._characterControl._leftHandWeapon == 0 && enemyControl._characterControl._rightHandWeapon == 0 && enemyControl._characterControl._bothtHandWeapon == 0 && _levelDesignScript._giveWeapons.Length > 0)
		{
			GiveWeapon giveWeapon = _levelDesignScript._giveWeapons[Random.Range(0, _levelDesignScript._giveWeapons.Length)];
			if (giveWeapon._percent > UnityEngine.Random.Range(0, 100))
			{
				enemyControl._characterControl._rightHandWeapon = giveWeapon._weapon;
				enemyControl._characterControl.AlreadyHasWeapon();
			}
		}
	}

	private void DropCrate()
	{
		if (_levelDesignScript._dropPercent <= 0 || _gameOver || !(Time.timeScale > 0f) || _playerControl._waitForInput)
		{
			return;
		}
		DropCrate dropCrate = _levelDesignScript._dropCrate;
		if (_levelDesignScript._dropPercent > UnityEngine.Random.Range(0, 100))
		{
			GameObject gameObject = UnityEngine.Object.Instantiate(Resources.Load("Crate"), Vector3.zero, Quaternion.identity) as GameObject;
			BreakableObj component = gameObject.GetComponent<BreakableObj>();
			component._spawnBananaChance = dropCrate._bananaChance;
			component._spawnChance = dropCrate._spawnChance;
			component._spawnWeapons = dropCrate._weapons;
			gameObject.transform.position = new Vector3(UnityEngine.Random.Range(1f, -1f), 6f, UnityEngine.Random.Range(_left, _right));
			gameObject.transform.eulerAngles = new Vector3(45f, -20f, -15f);
			component._throwForce = new Vector2(0f, 0f);
			component._throwAttack._poweredAttack = true;
			component._throwAttack._knockDown = false;
			component._currentDamage = _levelDesignScript._dropCreateDamage;
			int direction = 1;
			if (UnityEngine.Random.Range(0, 2) == 0)
			{
				direction = -1;
			}
			component.Throw(direction, (1 << LayerMask.NameToLayer("Player")) | (1 << LayerMask.NameToLayer("DestroyableObject")) | (1 << LayerMask.NameToLayer("Enemy")));
		}
	}
}
