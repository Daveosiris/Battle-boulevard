using System;

namespace UnityEngine.Purchasing.Security
{
	public class GooglePlayTangle
	{
		private static byte[] data = Convert.FromBase64String("UOJhQlBtZmlK5ijml21hYWFlYGMoM++RniXuVM2eGA88TA7GiOo8Gxvm5x6NyUuc3cPiIya5dfDvtp7CDXCjLfkdHteWKCRzc3BmRHKLhhBip83l5Ew1SdlJoJqXnT24Jz5i+QerI4D/L3+ixOXoEa92445DaLTsL/aiT7fUSp7tvKQfdKNnOd5acuVUX5k8W/RTsKorQ5dulUJAkRCt/nAsxTEUpwv4Ka1F3CF4RGyPaWcUUYP4jlVTD5Abfo/xRJOxAaSaCGHiYW9gUOJhamLiYWFg+z42/W2h2TXqBoPwJtYGctL9IWRqXSPREync6vuLN7yBL5TU1YtL3PWwfvrTHaZbmjPrlfmviyUL7MfbDgSm2pLR3+ZxQIx4R5wZE2JjYWBh");

		private static int[] order = new int[15]
		{
			0,
			4,
			10,
			7,
			13,
			11,
			6,
			8,
			10,
			10,
			13,
			13,
			12,
			13,
			14
		};

		private static int key = 96;

		public static readonly bool IsPopulated = true;

		public static byte[] Data()
		{
			if (!IsPopulated)
			{
				return null;
			}
			return Obfuscator.DeObfuscate(data, order, key);
		}
	}
}
