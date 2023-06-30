using System;
using UnityEngine;

namespace ControlFreak2.Internal
{
	[Serializable]
	public class SpriteConfig
	{
		public bool enabled;

		public Sprite sprite;

		public Color color;

		public float scale;

		public float rotation;

		public Vector2 offset;

		public bool resetOffset;

		public bool resetRotation;

		public bool resetScale;

		[NonSerialized]
		public bool oneShotState;

		public float duration;

		public float baseTransitionTime;

		public float colorTransitionFactor;

		public float scaleTransitionFactor;

		public float rotationTransitionFactor;

		public float offsetTransitionFactor;

		public SpriteConfig()
		{
			enabled = true;
			color = Color.white;
			rotation = 0f;
			scale = 1f;
			duration = 0.1f;
			baseTransitionTime = 0.1f;
			colorTransitionFactor = 1f;
			scaleTransitionFactor = 1f;
			rotationTransitionFactor = 1f;
			offsetTransitionFactor = 1f;
			oneShotState = false;
		}

		public SpriteConfig(Sprite sprite, Color color)
			: this()
		{
			this.sprite = sprite;
			this.color = color;
		}

		public SpriteConfig(bool enabled, bool oneShot, float scale)
			: this()
		{
			this.scale = scale;
			this.enabled = enabled;
			oneShotState = oneShot;
			resetOffset = oneShot;
			resetScale = oneShot;
			resetRotation = oneShot;
		}
	}
}
