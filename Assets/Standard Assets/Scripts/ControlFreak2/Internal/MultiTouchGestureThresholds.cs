using System;
using UnityEngine;

namespace ControlFreak2.Internal
{
	[Serializable]
	public class MultiTouchGestureThresholds : TouchGestureThresholds
	{
		public float pinchDistThreshCm;

		public float pinchAnalogRangeCm;

		public float pinchDeltaRangeCm;

		public float pinchDigitalThreshCm;

		public float pinchScrollStepCm;

		public float pinchScrollMagnet;

		public float twistMinDistCm;

		public float twistAngleThresh;

		public float twistAnalogRange;

		public float twistDeltaRange;

		public float twistDigitalThresh;

		public float twistScrollStep;

		public float twistScrollMagnet;

		public float multiFingerTapMaxFingerDistCm;

		private float _pinchDistThreshPx;

		private float _pinchAnalogRangePx;

		private float _pinchDeltaRangePx;

		private float _pinchDigitalThreshPx;

		private float _pinchScrollStepPx;

		private float _twistMinDistPx;

		private float _multiFingerTapMaxFingerDistPx;

		public float pinchDistThreshPx => _pinchDistThreshPx;

		public float pinchDistThreshPxSq => _pinchDistThreshPx * _pinchDistThreshPx;

		public float pinchAnalogRangePx => _pinchAnalogRangePx;

		public float pinchAnalogRangePxSq => _pinchAnalogRangePx * _pinchAnalogRangePx;

		public float pinchDeltaRangePx => _pinchDeltaRangePx;

		public float pinchDeltaRangePxSq => _pinchDeltaRangePx * _pinchDeltaRangePx;

		public float pinchDigitalThreshPx => _pinchDigitalThreshPx;

		public float pinchDigitalThreshPxSq => _pinchDigitalThreshPx * _pinchDigitalThreshPx;

		public float pinchScrollStepPx => _pinchScrollStepPx;

		public float pinchScrollStepPxSq => _pinchScrollStepPx * _pinchScrollStepPx;

		public float twistMinDistPx => _twistMinDistPx;

		public float twistMinDistPxSq => _twistMinDistPx * _twistMinDistPx;

		public float multiFingerTapMaxFingerDistPx => _multiFingerTapMaxFingerDistPx;

		public float multiFingerTapMaxFingerDistPxSq => _multiFingerTapMaxFingerDistPx * _multiFingerTapMaxFingerDistPx;

		public MultiTouchGestureThresholds()
		{
			twistMinDistCm = 0.5f;
			_twistMinDistPx = 2f;
			twistAngleThresh = 2f;
			twistDigitalThresh = 25f;
			twistAnalogRange = 45f;
			twistDeltaRange = 90f;
			pinchAnalogRangeCm = 1.5f;
			_pinchAnalogRangePx = 30f;
			pinchDeltaRangeCm = 2.5f;
			_pinchDeltaRangePx = 60f;
			pinchDistThreshCm = 0.2f;
			_pinchDistThreshPx = 2f;
			pinchDigitalThreshCm = 1f;
			_pinchDigitalThreshPx = 20f;
			pinchScrollStepCm = 0.5f;
			_pinchScrollStepPx = 10f;
			twistScrollStep = 30f;
			multiFingerTapMaxFingerDistCm = 3f;
			_multiFingerTapMaxFingerDistPx = 90f;
		}

		protected override void OnRecalc(float dpcm)
		{
			base.OnRecalc(dpcm);
			_pinchDistThreshPx = Mathf.Max(2f, pinchDistThreshCm * dpcm);
			_pinchDigitalThreshPx = Mathf.Max(2f, pinchDigitalThreshCm * dpcm);
			_twistMinDistPx = Mathf.Max(2f, twistMinDistCm * dpcm);
			_pinchAnalogRangePx = Mathf.Max(2f, pinchAnalogRangeCm * dpcm);
			_pinchDeltaRangePx = Mathf.Max(2f, pinchDeltaRangeCm * dpcm);
			_multiFingerTapMaxFingerDistPx = Mathf.Max(2f, multiFingerTapMaxFingerDistCm * dpcm);
			_pinchScrollStepPx = Mathf.Max(2f, pinchScrollStepCm * dpcm);
		}
	}
}
