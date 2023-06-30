using UnityEngine;

namespace SA.Common.Pattern
{
	public abstract class Singleton<T> : MonoBehaviour where T : MonoBehaviour
	{
		private static T _instance;

		private static bool applicationIsQuitting;

		public static T Instance
		{
			get
			{
				if (applicationIsQuitting)
				{
					return (T)null;
				}
				if ((Object)_instance == (Object)null)
				{
					_instance = (UnityEngine.Object.FindObjectOfType(typeof(T)) as T);
					if ((Object)_instance == (Object)null)
					{
						_instance = new GameObject().AddComponent<T>();
						_instance.gameObject.name = _instance.GetType().FullName;
					}
				}
				return _instance;
			}
		}

		public static bool HasInstance => !IsDestroyed;

		public static bool IsDestroyed
		{
			get
			{
				if ((Object)_instance == (Object)null)
				{
					return true;
				}
				return false;
			}
		}

		protected virtual void OnDestroy()
		{
			_instance = (T)null;
			applicationIsQuitting = true;
		}

		protected virtual void OnApplicationQuit()
		{
			_instance = (T)null;
			applicationIsQuitting = true;
		}
	}
}
