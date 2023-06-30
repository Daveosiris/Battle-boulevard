using ControlFreak2.Internal;
using UnityEngine;

namespace ControlFreak2
{
	[RequireComponent(typeof(RectTransform))]
	public class TouchSteeringWheelSpriteAnimator : TouchControlSpriteAnimatorBase, ISpriteAnimator
	{
		public enum ControlState
		{
			Neutral,
			Pressed,
			All
		}

		public SpriteConfig spritePressed;

		public float rotationRange = 45f;

		public float rotationSmoothingTime = 0.1f;

		public TouchSteeringWheelSpriteAnimator()
			: base(typeof(TouchSteeringWheel))
		{
			rotationRange = 45f;
			rotationSmoothingTime = 0.05f;
			spritePressed = new SpriteConfig(enabled: true, oneShot: false, 1.2f);
		}

		public void SetSprite(Sprite sprite)
		{
			spriteNeutral.sprite = sprite;
			spritePressed.sprite = sprite;
		}

		public void SetColor(Color color)
		{
			spriteNeutral.color = color;
			spritePressed.color = color;
		}

		public void SetStateSprite(ControlState state, Sprite sprite)
		{
			SpriteConfig stateSpriteConfig = GetStateSpriteConfig(state);
			if (stateSpriteConfig == null)
			{
				SetSprite(sprite);
			}
			else
			{
				stateSpriteConfig.sprite = sprite;
			}
		}

		public void SetStateColor(ControlState state, Color color)
		{
			SpriteConfig stateSpriteConfig = GetStateSpriteConfig(state);
			if (stateSpriteConfig == null)
			{
				SetColor(color);
			}
			else
			{
				stateSpriteConfig.color = color;
			}
		}

		public SpriteConfig GetStateSpriteConfig(ControlState state)
		{
			switch (state)
			{
			case ControlState.Neutral:
				return spriteNeutral;
			case ControlState.Pressed:
				return spritePressed;
			default:
				return null;
			}
		}

		protected override void OnInitComponent()
		{
			base.OnInitComponent();
			if (image.sprite != null && spriteNeutral.sprite == null && spritePressed.sprite == null)
			{
				spriteNeutral.sprite = image.sprite;
				spritePressed.sprite = image.sprite;
			}
		}

		protected override void OnUpdateAnimator(bool skipAnim)
		{
			if (!(sourceControl == null) && !(image == null))
			{
				TouchSteeringWheel touchSteeringWheel = (TouchSteeringWheel)sourceControl;
				SpriteConfig spriteConfig = null;
				if (touchSteeringWheel.Pressed() && (spriteConfig == null || !spriteConfig.enabled))
				{
					spriteConfig = spritePressed;
				}
				if (spriteConfig == null || !spriteConfig.enabled)
				{
					spriteConfig = spriteNeutral;
				}
				if (!CFUtils.editorStopped && !IsIllegallyAttachedToSource())
				{
					extraRotation = CFUtils.SmoothTowardsAngle(extraRotation, 0f - touchSteeringWheel.GetValue() * ((touchSteeringWheel.wheelMode != 0) ? touchSteeringWheel.maxTurnAngle : rotationRange), rotationSmoothingTime, CFUtils.realDeltaTimeClamped, 0.001f);
				}
				else
				{
					extraRotation = 0f;
				}
				BeginSpriteAnim(spriteConfig, skipAnim);
				UpdateSpriteAnimation(skipAnim);
			}
		}

		MonoBehaviour ISpriteAnimator.GetComponent()
		{
			return this;
		}

		void ISpriteAnimator.AddUsedSprites(ISpriteOptimizer optimizer)
		{
			optimizer.AddSprite(spriteNeutral.sprite);
			optimizer.AddSprite(spritePressed.sprite);
		}

		void ISpriteAnimator.OnSpriteOptimization(ISpriteOptimizer optimizer)
		{
			spriteNeutral.sprite = optimizer.GetOptimizedSprite(spriteNeutral.sprite);
			spritePressed.sprite = optimizer.GetOptimizedSprite(spritePressed.sprite);
		}
	}
}
