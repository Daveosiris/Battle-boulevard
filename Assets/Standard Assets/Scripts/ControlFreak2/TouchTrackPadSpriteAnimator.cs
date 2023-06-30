using ControlFreak2.Internal;
using UnityEngine;
using UnityEngine.UI;

namespace ControlFreak2
{
	[RequireComponent(typeof(RectTransform))]
	[RequireComponent(typeof(Image))]
	[ExecuteInEditMode]
	public class TouchTrackPadSpriteAnimator : TouchControlSpriteAnimatorBase, ISpriteAnimator
	{
		public enum ControlState
		{
			Neutral,
			Pressed,
			All
		}

		public SpriteConfig spritePressed;

		public TouchTrackPadSpriteAnimator()
			: base(typeof(TouchTrackPad))
		{
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
		}

		protected override void OnUpdateAnimator(bool skipAnim)
		{
			TouchTrackPad touchTrackPad = (TouchTrackPad)sourceControl;
			if (!(touchTrackPad == null) && !(image == null))
			{
				SpriteConfig spriteConfig = null;
				if (touchTrackPad.Pressed() && (spriteConfig == null || !spriteConfig.enabled))
				{
					spriteConfig = spritePressed;
				}
				if (spriteConfig == null || !spriteConfig.enabled)
				{
					spriteConfig = spriteNeutral;
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
