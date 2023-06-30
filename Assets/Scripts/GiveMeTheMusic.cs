using UnityEngine;

public class GiveMeTheMusic : MonoBehaviour
{
	public GameObject _sourcePrefab;

	private AudioSource _source;

	public AudioClips[] AudioList;

	private float musicVolume = 0.75f;

	private float currentSourceVolume;

	private AudioClip _currentClip;

	public bool _menu;

	private void Awake()
	{
		GameObject gameObject = UnityEngine.Object.Instantiate(_sourcePrefab, Vector3.zero, Quaternion.identity);
		gameObject.transform.parent = Camera.main.transform;
		gameObject.transform.localPosition = Vector3.zero;
		_source = gameObject.GetComponent<AudioSource>();
		if (_menu)
		{
			PlayMusic("Menu");
		}
	}

	public void PlayMusic(string name)
	{
		AudioClips[] audioList = AudioList;
		foreach (AudioClips audioClips in audioList)
		{
			if (audioClips.name == name)
			{
				_currentClip = audioClips.clip[Random.Range(0, audioClips.clip.Length)];
				if (_source.clip == _currentClip && _source.isPlaying)
				{
					break;
				}
				_source.Stop();
				currentSourceVolume = audioClips.volume * musicVolume;
				_source.loop = audioClips.loop;
				_source.clip = _currentClip;
				_source.volume = currentSourceVolume * (float)DataFunctions.GetIntData(Config.PP_Music);
				_source.Play();
			}
		}
	}

	public void ChangeVolumeImmediate()
	{
		_source.volume = currentSourceVolume * (float)DataFunctions.GetIntData(Config.PP_Music);
	}
}
