using UnityEngine;

namespace ControlFreak2.Demos.Helpers
{
	public class AudioIgnoreListenerPauseAndVolume : MonoBehaviour
	{
		public AudioSource[] targetSources;

		public bool ignoreListenerVolume = true;

		public bool ignoreListenerPause = true;

		private void OnEnable()
		{
			if (targetSources == null || targetSources.Length == 0)
			{
				targetSources = GetComponents<AudioSource>();
			}
			for (int i = 0; i < targetSources.Length; i++)
			{
				if (targetSources[i] != null)
				{
					targetSources[i].ignoreListenerPause = ignoreListenerPause;
					targetSources[i].ignoreListenerVolume = ignoreListenerVolume;
				}
			}
		}
	}
}
