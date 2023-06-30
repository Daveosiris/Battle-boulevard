using System.Collections.Generic;
using UnityEngine;

namespace SA.Common.Util
{
	public static class IconManager
	{
		private static Dictionary<string, Texture2D> s_icons = new Dictionary<string, Texture2D>();

		private static Dictionary<float, Texture2D> s_colorIcons = new Dictionary<float, Texture2D>();

		public static Texture2D GetIconFromHtmlColorString(string htmlString)
		{
			return GetIconFromHtmlColorString(htmlString, Color.black);
		}

		public static Texture2D GetIconFromHtmlColorString(string htmlString, Color fallback)
		{
			return GetIcon(fallback);
		}

		public static Texture2D GetIcon(Color color, int width = 1, int height = 1)
		{
			float key = color.r * 100000f + color.g * 10000f + color.b * 1000f + color.a * 100f + (float)width * 10f + (float)height;
			if (s_colorIcons.ContainsKey(key) && s_colorIcons[key] != null)
			{
				return s_colorIcons[key];
			}
			Texture2D texture2D = new Texture2D(width, height);
			for (int i = 0; i < width; i++)
			{
				for (int j = 0; j < height; j++)
				{
					texture2D.SetPixel(i, j, color);
				}
			}
			texture2D.Apply();
			s_colorIcons[key] = texture2D;
			return GetIcon(color, width, height);
		}

		public static Texture2D GetIconAtPath(string path)
		{
			if (s_icons.ContainsKey(path))
			{
				return s_icons[path];
			}
			Texture2D texture2D = Resources.Load(path) as Texture2D;
			if (texture2D == null)
			{
				texture2D = new Texture2D(1, 1);
			}
			s_icons.Add(path, texture2D);
			return GetIconAtPath(path);
		}

		public static Texture2D Rotate(Texture2D tex, float angle)
		{
			Texture2D texture2D = new Texture2D(tex.width, tex.height);
			int width = tex.width;
			int height = tex.height;
			float num = rot_x(angle, (float)(-width) / 2f, (float)(-height) / 2f) + (float)width / 2f;
			float num2 = rot_y(angle, (float)(-width) / 2f, (float)(-height) / 2f) + (float)height / 2f;
			float num3 = rot_x(angle, 1f, 0f);
			float num4 = rot_y(angle, 1f, 0f);
			float num5 = rot_x(angle, 0f, 1f);
			float num6 = rot_y(angle, 0f, 1f);
			float num7 = num;
			float num8 = num2;
			for (int i = 0; i < tex.width; i++)
			{
				float num9 = num7;
				float num10 = num8;
				for (int j = 0; j < tex.height; j++)
				{
					num9 += num3;
					num10 += num4;
					texture2D.SetPixel((int)Mathf.Floor(i), (int)Mathf.Floor(j), getPixel(tex, num9, num10));
				}
				num7 += num5;
				num8 += num6;
			}
			texture2D.Apply();
			return texture2D;
		}

		private static Color getPixel(Texture2D tex, float x, float y)
		{
			int num = (int)Mathf.Floor(x);
			int num2 = (int)Mathf.Floor(y);
			if (num > tex.width || num < 0 || num2 > tex.height || num2 < 0)
			{
				return Color.clear;
			}
			return tex.GetPixel(num, num2);
		}

		private static float rot_x(float angle, float x, float y)
		{
			float num = Mathf.Cos(angle / 180f * 3.14159274f);
			float num2 = Mathf.Sin(angle / 180f * 3.14159274f);
			return x * num + y * (0f - num2);
		}

		private static float rot_y(float angle, float x, float y)
		{
			float num = Mathf.Cos(angle / 180f * 3.14159274f);
			float num2 = Mathf.Sin(angle / 180f * 3.14159274f);
			return x * num2 + y * num;
		}
	}
}
