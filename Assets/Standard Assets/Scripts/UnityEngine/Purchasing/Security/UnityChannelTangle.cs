using System;

namespace UnityEngine.Purchasing.Security
{
	public class UnityChannelTangle
	{
		private static byte[] data = Convert.FromBase64String("T5B8+YpcrHwIqIdbHhAnWatpU6Yh4EmR74PV8V9xlr2hdH7coOirpXcK2VeDZ2St7FJeCQkKHD4I8fxqVYzYNc2uMOSXxt5lDtkdQ6QgCJ8KVr9Lbt1xglPXP6ZbAj4W9RMdbi4l40YhjinK0FE57RTvODrrateEUkmV6+RflC635GJ1RjZ0vPKQRmEqmBs4KhccEzCcUpztFxsbGx8aGZgbFRoqmBsQGJgbGxqBREyHF9ujGN23n542TzOjM9rg7edHwl1EGIOQgfFNxvtV7q6v8TGmj8oEgKln3Cv5gvQvKXXqYQT1iz7py3ve4HIbYZydZPezMeanuZhZXMMPipXM5Lh90Vn6hVUF2L6fkmvVDJn0ORLOlpwLOvYCPeZjaRgZGxob");

		private static int[] order = new int[15]
		{
			9,
			5,
			7,
			6,
			7,
			8,
			7,
			9,
			8,
			13,
			12,
			13,
			12,
			13,
			14
		};

		private static int key = 26;

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
