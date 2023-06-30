using UnityEngine;

public class JoystickSetup : MonoBehaviour
{
	public GameObject[] _joysticks;

	private void OnEnable()
	{
		GameObject[] joysticks = _joysticks;
		foreach (GameObject gameObject in joysticks)
		{
			gameObject.SetActive(value: false);
		}
		_joysticks[DataFunctions.GetIntData(Config.PP_DynmJoy)].SetActive(value: true);
	}
}
