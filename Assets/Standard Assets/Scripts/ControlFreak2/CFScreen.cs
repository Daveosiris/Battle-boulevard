using UnityEngine;

namespace ControlFreak2
{
	public static class CFScreen
	{
		private const float DEFAULT_FALLBACK_SCREEN_DIAMETER = 7f;

		private static bool mForceFallbackDpi;

		private static Resolution mLastScreenResolution;

		private static Resolution mNativeScreenResolution;

		private static float mFallbackDpi = 96f;

		private static float mFallbackDiameter = 7f;

		private static float mLastScreenWidth = -1f;

		private static float mLastScreenHeight = -1f;

		private static float mLastScreenDpi = -1f;

		private static float mNativeScreenDpi = -1f;

		private static float mDpi = 100f;

		private static float mDpcm = 100f;

		private static float mInvDpi = 1f;

		private static float mInvDpcm = 1f;

		public static float dpi
		{
			get
			{
				UpdateDpiIfNeeded();
				return mDpi;
			}
		}

		public static float dpcm
		{
			get
			{
				UpdateDpiIfNeeded();
				return mDpcm;
			}
		}

		public static float invDpi
		{
			get
			{
				UpdateDpiIfNeeded();
				return mInvDpi;
			}
		}

		public static float invDpcm
		{
			get
			{
				UpdateDpiIfNeeded();
				return mInvDpcm;
			}
		}

		public static float width => Screen.width;

		public static float height => Screen.height;

		public static bool lockCursor
		{
			get
			{
				return CFCursor.lockState == CursorLockMode.Locked;
			}
			set
			{
				CFCursor.lockState = (value ? CursorLockMode.Locked : CursorLockMode.None);
			}
		}

		public static bool showCursor
		{
			get
			{
				return CFCursor.visible;
			}
			set
			{
				CFCursor.visible = value;
			}
		}

		public static bool fullScreen
		{
			get
			{
				return Screen.fullScreen;
			}
			set
			{
				UpdateDpi();
				Screen.fullScreen = value;
			}
		}

		public static void SetResolution(int width, int height, bool fullScreen, int refreshRate = 0)
		{
			UpdateDpi();
			Screen.SetResolution(width, height, fullScreen, refreshRate);
		}

		public static void ForceFallbackDpi(bool enableFallbackDpi)
		{
			mForceFallbackDpi = enableFallbackDpi;
			UpdateDpi();
		}

		public static void SetFallbackScreenDiameter(float diameterInInches)
		{
			mLastScreenWidth = -1f;
			mFallbackDiameter = Mathf.Max(1f, diameterInInches);
			UpdateDpi();
		}

		private static void UpdateDpiIfNeeded()
		{
			Resolution currentResolution = Screen.currentResolution;
			if (mLastScreenWidth != (float)Screen.width || mLastScreenHeight != (float)Screen.height || mLastScreenDpi != Screen.dpi || currentResolution.width != mLastScreenResolution.width || currentResolution.height != mLastScreenResolution.height)
			{
				UpdateDpi();
			}
		}

		public static void UpdateDpi()
		{
			Resolution currentResolution = Screen.currentResolution;
			if (mLastScreenDpi != Screen.dpi || mNativeScreenResolution.width <= 0 || mNativeScreenResolution.height <= 0)
			{
				mNativeScreenResolution = currentResolution;
				mNativeScreenDpi = Screen.dpi;
			}
			mLastScreenWidth = Screen.width;
			mLastScreenHeight = Screen.height;
			mLastScreenDpi = Screen.dpi;
			mFallbackDpi = Mathf.Sqrt(mLastScreenWidth * mLastScreenWidth + mLastScreenHeight * mLastScreenHeight) / mFallbackDiameter;
			mLastScreenResolution = currentResolution;
			float num = mLastScreenDpi;
			if (mNativeScreenDpi > 0f && mLastScreenDpi > 0f && mNativeScreenResolution.width > 0 && mNativeScreenResolution.height > 0 && currentResolution.width > 0 && currentResolution.height > 0)
			{
				num = mNativeScreenDpi * (Mathf.Sqrt(currentResolution.width * currentResolution.width + currentResolution.height * currentResolution.height) / Mathf.Sqrt(mNativeScreenResolution.width * mNativeScreenResolution.width + mNativeScreenResolution.height * mNativeScreenResolution.height));
			}
			if (num < 1f || mForceFallbackDpi)
			{
				mDpi = mFallbackDpi;
			}
			else
			{
				mDpi = num;
			}
			mDpcm = mDpi / 2.54f;
			mInvDpi = 1f / mDpi;
			mInvDpcm = 1f / mDpcm;
		}
	}
}
