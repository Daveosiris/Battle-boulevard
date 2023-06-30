using ControlFreak2.Internal;
using UnityEngine;
using UnityEngine.UI;

namespace ControlFreak2
{
	[RequireComponent(typeof(RectTransform))]
	[RequireComponent(typeof(Image))]
	[ExecuteInEditMode]
	public class TouchButtonSpriteAnimator : TouchControlSpriteAnimatorBase, ISpriteAnimator
	{
		public enum ControlState
		{
			Neutral,
			Pressed,
			Toggled,
			ToggledAndPressed,
			All
		}

		public SpriteConfig spritePressed;

		public SpriteConfig spriteToggled;

		public SpriteConfig spriteToggledAndPressed;

		public TouchButtonSpriteAnimator()
			: base(typeof(TouchButton))
		{
			spritePressed = new SpriteConfig(enabled: true, oneShot: false, 1.2f);
			spriteToggled = new SpriteConfig(enabled: false, oneShot: false, 1.1f);
			spriteToggledAndPressed = new SpriteConfig(enabled: false, oneShot: false, 1.3f);
			spritePressed.scale = 1.25f;
			spriteToggled.scale = 1.1f;
			spriteToggledAndPressed.scale = 1.3f;
		}

		public void SetSprite(Sprite sprite)
		{
			spriteNeutral.sprite = sprite;
			spritePressed.sprite = sprite;
			spriteToggled.sprite = sprite;
			spriteToggledAndPressed.sprite = sprite;
		}

		public void SetColor(Color color)
		{
			spriteNeutral.color = color;
			spritePressed.color = color;
			spriteToggled.color = color;
			spriteToggledAndPressed.color = color;
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
			case ControlState.Toggled:
				return spriteToggled;
			case ControlState.ToggledAndPressed:
				return spriteToggledAndPressed;
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
			TouchButton touchButton = (TouchButton)sourceControl;
			if (!(touchButton == null) && !(image == null))
			{
				bool flag = touchButton.Pressed();
				bool flag2 = touchButton.Toggled();
				SpriteConfig spriteConfig = null;
				if (flag && flag2 && (spriteConfig == null || !spriteConfig.enabled))
				{
					spriteConfig = spriteToggledAndPressed;
				}
				if (flag2 && (spriteConfig == null || !spriteConfig.enabled))
				{
					spriteConfig = spriteToggled;
				}
				if (flag && (spriteConfig == null || !spriteConfig.enabled))
				{
					spriteConfig = spritePressed;
				}
				if (spriteConfig == null || !spriteConfig.enabled)
				{
					spriteConfig = spriteNeutral;
				}
				BeginSpriteAnim((spriteConfig != null) ? spriteConfig : spriteNeutral, skipAnim: false);
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
			optimizer.AddSprite(spriteToggled.sprite);
			optimizer.AddSprite(spriteToggledAndPressed.sprite);
		}

		void ISpriteAnimator.OnSpriteOptimization(ISpriteOptimizer optimizer)
		{
			spriteNeutral.sprite = optimizer.GetOptimizedSprite(spriteNeutral.sprite);
			spritePressed.sprite = optimizer.GetOptimizedSprite(spritePressed.sprite);
			spriteToggled.sprite = optimizer.GetOptimizedSprite(spriteToggled.sprite);
			spriteToggledAndPressed.sprite = optimizer.GetOptimizedSprite(spriteToggledAndPressed.sprite);
		}
	}
}
