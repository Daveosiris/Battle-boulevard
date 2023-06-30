using System;
using UnityEngine;


namespace AppStoresSupport
{
	[Serializable]
	public class AppStoreSettings : ScriptableObject
	{
		public string UnityClientID = string.Empty;

		public string UnityClientKey = string.Empty;

		public string UnityClientRSAPublicKey = string.Empty;

		public AppStoreSetting XiaomiAppStoreSetting = new AppStoreSetting();

		
	}
}
