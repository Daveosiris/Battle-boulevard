using UnityEngine;

public class Banana : MonoBehaviour
{
	private GamePlayManager _gamePlayManager;

	public float _healthPercent;

	private float _pickUpRange = 1.2f;

	private GameObject _pickUpEffect;

	private Transform _myTransform;

	private Rigidbody _myRigidBody;

	private bool _pickable;

	private void Awake()
	{
		_gamePlayManager = UnityEngine.Object.FindObjectOfType<GamePlayManager>();
		_pickUpEffect = base.transform.GetChild(1).gameObject;
		_pickUpEffect.SetActive(value: false);
		_myRigidBody = GetComponent<Rigidbody>();
		_myTransform = base.transform;
		_gamePlayManager.BananaSpawned(base.transform);
	}

	public void Spawn(Vector3 position, int direction)
	{
		base.gameObject.SetActive(value: true);
		_myTransform.position = position;
		_myRigidBody.AddForce(new Vector3(UnityEngine.Random.Range(-0.2f, 0.2f) * Time.fixedDeltaTime * Config.FORCE_MULTIPLYER, 1.5f * Time.fixedDeltaTime * Config.FORCE_MULTIPLYER, UnityEngine.Random.Range(0.2f, 0.4f) * (float)direction * Time.fixedDeltaTime * Config.FORCE_MULTIPLYER));
		Invoke("SetPickable", 1f);
	}

	private void SetPickable()
	{
		_pickable = true;
	}

	private void Update()
	{
		Vector3 position = _myTransform.position;
		position.x = Mathf.Clamp(position.x, _gamePlayManager._up, _gamePlayManager._down);
		if (_gamePlayManager._levelDesignScript._specialBossScene)
		{
			position.z = Mathf.Clamp(position.z, _gamePlayManager._left + 1f, _gamePlayManager._right - 1f);
		}
		_myTransform.position = position;
		if (_pickable)
		{
			float num = Vector3.Distance(_gamePlayManager._playerControl._characterControl._myTransform.position, _myTransform.position);
			if (num <= _pickUpRange)
			{
				_pickUpEffect.SetActive(value: true);
				_pickUpEffect.transform.parent = null;
				_gamePlayManager.PickBanana(_healthPercent, base.transform);
				base.gameObject.SetActive(value: false);
			}
			foreach (EnemyControl activeEnemy in _gamePlayManager._activeEnemies)
			{
				if (activeEnemy._canPickBanana)
				{
					float num2 = Vector3.Distance(activeEnemy._characterControl._myTransform.position, _myTransform.position);
					if (num2 <= _pickUpRange)
					{
						_pickUpEffect.SetActive(value: true);
						_pickUpEffect.transform.parent = null;
						_gamePlayManager.EnemyPickedBanana(activeEnemy, _healthPercent / 4f, base.transform);
						base.gameObject.SetActive(value: false);
					}
				}
			}
		}
	}
}
