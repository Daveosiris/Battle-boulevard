using System;
using System.Collections;
using System.Threading;
using UnityEngine;

namespace SA.Common.Models
{
	public class PrefabAsyncLoader : MonoBehaviour
	{
		private string PrefabPath;

		public event Action<GameObject> ObjectLoadedAction;

		public PrefabAsyncLoader()
		{
			this.ObjectLoadedAction = delegate
			{
			};
			//base._002Ector();
		}

		public static PrefabAsyncLoader Create()
		{
			return new GameObject("PrefabAsyncLoader").AddComponent<PrefabAsyncLoader>();
		}

		private void Awake()
		{
			UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
		}

		public void LoadAsync(string name)
		{
			PrefabPath = name;
			StartCoroutine(Load());
		}

		private IEnumerator Load()
		{
			ResourceRequest request = Resources.LoadAsync(PrefabPath);
			yield return request;
			if (request.asset == null)
			{
				UnityEngine.Debug.LogWarning("Prefab not found at path: " + PrefabPath);
				this.ObjectLoadedAction(null);
			}
			else
			{
				GameObject obj = UnityEngine.Object.Instantiate(request.asset) as GameObject;
				this.ObjectLoadedAction(obj);
			}
			UnityEngine.Object.Destroy(base.gameObject);
		}
	}
}
