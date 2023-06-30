using UnityEngine;

public class Orange : MonoBehaviour
{
	private GamePlayManager _gamePlayManager;

	private float _pickUpRange = 1.5f;

	private GameObject _pickUpEffect;

	private Transform _myTransform;

	private Rigidbody _myRigidBody;

	private bool _pickable;

	public int _orangeCount;

	private void Awake()
	{
		_gamePlayManager = UnityEngine.Object.FindObjectOfType<GamePlayManager>();
		_pickUpEffect = base.transform.GetChild(1).gameObject;
		_pickUpEffect.SetActive(value: false);
		_myRigidBody = GetComponent<Rigidbody>();
		_myTransform = base.transform;
	}

	public void Spawn(Vector3 position)
	{
		base.gameObject.SetActive(value: true);
		_myTransform.position = position;
		_myRigidBody.AddForce(new Vector3(UnityEngine.Random.Range(-0.2f, 0.2f) * Time.fixedDeltaTime * Config.FORCE_MULTIPLYER, 1.5f * Time.fixedDeltaTime * Config.FORCE_MULTIPLYER, UnityEngine.Random.Range(-0.4f, 0.4f) * Time.fixedDeltaTime * Config.FORCE_MULTIPLYER));
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
			position.z = Mathf.Clamp(position.z, _gamePlayManager._left, _gamePlayManager._right);
		}
		_myTransform.position = position;
		if (_pickable)
		{
			float num = Vector3.Distance(_gamePlayManager._playerControl._characterControl._myTransform.position, _myTransform.position);
			if (num <= _pickUpRange)
			{
				_pickUpEffect.SetActive(value: true);
				_pickUpEffect.transform.parent = null;
				_gamePlayManager.PickOrange(this);
				base.gameObject.SetActive(value: false);
			}
		}
	}
}
