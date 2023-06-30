using UnityEngine;

public class DeactivateMe : MonoBehaviour
{
	public float LifeTime = 1f;

	private void OnEnable()
	{
		Invoke("DAME", LifeTime);
	}

	private void OnDisable()
	{
		CancelInvoke("DAME");
	}

	private void DAME()
	{
		base.gameObject.SetActive(value: false);
	}
}
