using UnityEngine;

namespace ControlFreak2.Demos.Guns
{
	public class GunActivator : MonoBehaviour
	{
		public Gun[] gunList;

		public string buttonName = "Fire1";

		public KeyCode key;

		private void Update()
		{
			bool triggerState = (key != 0 && CF2Input.GetKey(key)) || (!string.IsNullOrEmpty(buttonName) && CF2Input.GetButton(buttonName));
			for (int i = 0; i < gunList.Length; i++)
			{
				Gun gun = gunList[i];
				if (gun != null)
				{
					gun.SetTriggerState(triggerState);
				}
			}
		}
	}
}
