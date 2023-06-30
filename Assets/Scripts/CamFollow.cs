using UnityEngine;

public class CamFollow : MonoBehaviour
{
	public Transform _target;

	[Header("Follow Settings")]
	public float _distanceToTarget = 13f;

	private float _distanceToTargetT;

	private float _distanceToTargetB = 11f;

	public float _heightOffset = 3f;

	private float _heightOffsetT;

	private float _heightOffsetB = 2.5f;

	public Vector3 _viewAngle = new Vector3(6.3f, -90f, 0f);

	[Header("Damp Settings")]
	public Vector3 _damp = new Vector3(3f, 2f, 3f);

	[Header("View Area")]
	public float _minLeft;

	public float _maxRight;

	private void Awake()
	{
		Reset();
		base.transform.position = new Vector3(0f, 8f, 0f);
	}

	public void PowerUpEffect()
	{
		_distanceToTargetT = 14f;
		_heightOffsetT = 3f;
	}

	public void CloserEffect()
	{
		_distanceToTargetT = 8f;
		_heightOffsetT = 2f;
	}

	public void BossEffect()
	{
		_distanceToTargetT = _distanceToTargetB;
		_heightOffsetT = _heightOffsetB;
	}

	public void Reset()
	{
		_distanceToTargetT = _distanceToTarget;
		_heightOffsetT = _heightOffset;
	}

	public void SetPosition(Vector3 position)
	{
		Vector3 vector = position;
		base.transform.position = new Vector3(vector.x + _distanceToTargetT, vector.y + _heightOffsetT, vector.z);
	}

	private void Update()
	{
		if ((bool)_target)
		{
			Vector3 position = base.transform.position;
			float x = position.x;
			Vector3 position2 = base.transform.position;
			float y = position2.y;
			Vector3 position3 = base.transform.position;
			float z = position3.z;
			Vector3 position4 = _target.transform.position;
			x = Mathf.Lerp(x, position4.x + _distanceToTargetT, _damp.x * Time.deltaTime);
			y = Mathf.Lerp(y, position4.y + _heightOffsetT, _damp.y * Time.deltaTime);
			z = Mathf.Lerp(z, position4.z, _damp.z * Time.deltaTime);
			base.transform.position = new Vector3(x, y, Mathf.Clamp(z, _minLeft, _maxRight));
			base.transform.localEulerAngles = _viewAngle;
		}
	}
}
