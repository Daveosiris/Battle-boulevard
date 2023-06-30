using UnityEngine;

public class MakeNoise : MonoBehaviour
{
	public GameObject _sourcePrefab;

	private AudioSource[] _sources;

	public int _sourceCount = 10;

	public AudioClips[] AudioList;

	private float sfxVolume = 1f;

	private void Awake()
	{
		_sources = new AudioSource[_sourceCount];
		for (int i = 0; i < _sources.Length; i++)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate(_sourcePrefab, Vector3.zero, Quaternion.identity);
			gameObject.transform.parent = Camera.main.transform;
			gameObject.transform.localPosition = Vector3.zero;
			_sources[i] = gameObject.GetComponent<AudioSource>();
		}
	}

	public void PlaySFX(string name)
	{
		if (DataFunctions.GetIntData(Config.PP_Sound) == 0)
		{
			return;
		}
		AudioClips[] audioList = AudioList;
		foreach (AudioClips audioClips in audioList)
		{
			if (!(audioClips.name == name))
			{
				continue;
			}
			AudioSource audioSource = _sources[0];
			AudioSource[] sources = _sources;
			foreach (AudioSource audioSource2 in sources)
			{
				if (audioSource2.isPlaying)
				{
					if (audioSource2.name == name)
					{
						audioSource = audioSource2;
						audioSource.Stop();
						break;
					}
					continue;
				}
				audioSource = audioSource2;
				break;
			}
			audioSource.name = name;
			audioSource.volume = audioClips.volume * sfxVolume * (float)DataFunctions.GetIntData(Config.PP_Sound);
			audioSource.loop = audioClips.loop;
			audioSource.PlayOneShot(audioClips.clip[Random.Range(0, audioClips.clip.Length)]);
		}
	}

	public void ChangeVolumeImmediate()
	{
		AudioSource[] sources = _sources;
		foreach (AudioSource audioSource in sources)
		{
			audioSource.volume *= DataFunctions.GetIntData(Config.PP_Sound);
		}
	}

	private void Update()
	{
		AudioSource[] sources = _sources;
		foreach (AudioSource audioSource in sources)
		{
			if (!audioSource.isPlaying)
			{
				audioSource.name = "Audio";
			}
		}
	}
}
