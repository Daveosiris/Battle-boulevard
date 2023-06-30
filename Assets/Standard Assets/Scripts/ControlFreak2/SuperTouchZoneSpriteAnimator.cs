using ControlFreak2.Internal;
using UnityEngine;
using UnityEngine.UI;

namespace ControlFreak2
{
	[RequireComponent(typeof(RectTransform))]
	[RequireComponent(typeof(Image))]
	[ExecuteInEditMode]
	public class SuperTouchZoneSpriteAnimator : TouchControlSpriteAnimatorBase, ISpriteAnimator
	{
		public enum ControlState
		{
			Neutral,
			RawPress,
			NormalPress,
			LongPress,
			Tap,
			DoubleTap,
			LongTap,
			NormalScrollU,
			NormalScrollR,
			NormalScrollD,
			NormalScrollL,
			LongScrollU,
			LongScrollR,
			LongScrollD,
			LongScrollL,
			All
		}

		public SpriteConfig spriteRawPress;

		public SpriteConfig spriteNormalPress;

		public SpriteConfig spriteLongPress;

		public SpriteConfig spriteTap;

		public SpriteConfig spriteDoubleTap;

		public SpriteConfig spriteLongTap;

		public SpriteConfig spriteNormalScrollU;

		public SpriteConfig spriteNormalScrollR;

		public SpriteConfig spriteNormalScrollD;

		public SpriteConfig spriteNormalScrollL;

		public SpriteConfig spriteLongScrollU;

		public SpriteConfig spriteLongScrollR;

		public SpriteConfig spriteLongScrollD;

		public SpriteConfig spriteLongScrollL;

		public const ControlState ControlStateFirst = ControlState.Neutral;

		public const ControlState ControlStateCount = ControlState.All;

		public SuperTouchZoneSpriteAnimator()
			: base(typeof(SuperTouchZone))
		{
			spriteRawPress = new SpriteConfig(enabled: true, oneShot: false, 1.2f);
			spriteNormalPress = new SpriteConfig(enabled: false, oneShot: false, 1.2f);
			spriteLongPress = new SpriteConfig(enabled: false, oneShot: false, 1.2f);
			spriteTap = new SpriteConfig(enabled: false, oneShot: true, 1.2f);
			spriteDoubleTap = new SpriteConfig(enabled: false, oneShot: true, 1.2f);
			spriteLongTap = new SpriteConfig(enabled: false, oneShot: true, 1.2f);
			spriteNormalScrollU = new SpriteConfig(enabled: false, oneShot: true, 1.2f);
			spriteNormalScrollR = new SpriteConfig(enabled: false, oneShot: true, 1.2f);
			spriteNormalScrollD = new SpriteConfig(enabled: false, oneShot: true, 1.2f);
			spriteNormalScrollL = new SpriteConfig(enabled: false, oneShot: true, 1.2f);
			spriteLongScrollU = new SpriteConfig(enabled: false, oneShot: true, 1.2f);
			spriteLongScrollR = new SpriteConfig(enabled: false, oneShot: true, 1.2f);
			spriteLongScrollD = new SpriteConfig(enabled: false, oneShot: true, 1.2f);
			spriteLongScrollL = new SpriteConfig(enabled: false, oneShot: true, 1.2f);
		}

		public void SetSprite(Sprite sprite)
		{
			for (ControlState controlState = ControlState.Neutral; controlState < ControlState.All; controlState++)
			{
				GetStateSpriteConfig(controlState).sprite = sprite;
			}
		}

		public void SetColor(Color color)
		{
			for (ControlState controlState = ControlState.Neutral; controlState < ControlState.All; controlState++)
			{
				GetStateSpriteConfig(controlState).color = color;
			}
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
			case ControlState.RawPress:
				return spriteRawPress;
			case ControlState.NormalPress:
				return spriteNormalPress;
			case ControlState.LongPress:
				return spriteLongPress;
			case ControlState.Tap:
				return spriteTap;
			case ControlState.DoubleTap:
				return spriteDoubleTap;
			case ControlState.LongTap:
				return spriteLongTap;
			case ControlState.NormalScrollU:
				return spriteNormalScrollU;
			case ControlState.NormalScrollR:
				return spriteNormalScrollR;
			case ControlState.NormalScrollD:
				return spriteNormalScrollD;
			case ControlState.NormalScrollL:
				return spriteNormalScrollL;
			case ControlState.LongScrollU:
				return spriteLongScrollU;
			case ControlState.LongScrollR:
				return spriteLongScrollR;
			case ControlState.LongScrollD:
				return spriteLongScrollD;
			case ControlState.LongScrollL:
				return spriteLongScrollL;
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
			SuperTouchZone superTouchZone = (SuperTouchZone)sourceControl;
			if (superTouchZone == null || image == null)
			{
				return;
			}
			Vector2 scrollDelta = superTouchZone.GetScrollDelta(1);
			SpriteConfig spriteConfig = null;
			if ((superTouchZone.JustTapped(1, 2) || superTouchZone.JustTapped(2, 2) || superTouchZone.JustTapped(3, 2)) && (spriteConfig == null || !spriteConfig.enabled))
			{
				spriteConfig = spriteDoubleTap;
			}
			if ((superTouchZone.JustTapped(1, 1) || superTouchZone.JustTapped(2, 1) || superTouchZone.JustTapped(3, 1)) && (spriteConfig == null || !spriteConfig.enabled))
			{
				spriteConfig = spriteTap;
			}
			if ((superTouchZone.JustLongTapped(1) || superTouchZone.JustLongTapped(2) || superTouchZone.JustLongTapped(3)) && (spriteConfig == null || !spriteConfig.enabled))
			{
				spriteConfig = spriteLongTap;
			}
			if ((superTouchZone.PressedLong(1) || superTouchZone.PressedLong(2) || superTouchZone.PressedLong(3)) && (spriteConfig == null || !spriteConfig.enabled))
			{
				if (scrollDelta.x > 0f && (spriteConfig == null || !spriteConfig.enabled))
				{
					spriteConfig = spriteLongScrollR;
				}
				if (scrollDelta.x < 0f && (spriteConfig == null || !spriteConfig.enabled))
				{
					spriteConfig = spriteLongScrollL;
				}
				if (scrollDelta.y > 0f && (spriteConfig == null || !spriteConfig.enabled))
				{
					spriteConfig = spriteLongScrollU;
				}
				if (scrollDelta.y < 0f && (spriteConfig == null || !spriteConfig.enabled))
				{
					spriteConfig = spriteLongScrollD;
				}
				if (spriteConfig == null || !spriteConfig.enabled)
				{
					spriteConfig = spriteLongPress;
				}
			}
			else if ((superTouchZone.PressedNormal(1) || superTouchZone.PressedNormal(2) || superTouchZone.PressedNormal(3)) && (spriteConfig == null || !spriteConfig.enabled))
			{
				if (scrollDelta.x > 0f && (spriteConfig == null || !spriteConfig.enabled))
				{
					spriteConfig = spriteNormalScrollR;
				}
				if (scrollDelta.x < 0f && (spriteConfig == null || !spriteConfig.enabled))
				{
					spriteConfig = spriteNormalScrollL;
				}
				if (scrollDelta.y > 0f && (spriteConfig == null || !spriteConfig.enabled))
				{
					spriteConfig = spriteNormalScrollU;
				}
				if (scrollDelta.y < 0f && (spriteConfig == null || !spriteConfig.enabled))
				{
					spriteConfig = spriteNormalScrollD;
				}
				if (spriteConfig == null || !spriteConfig.enabled)
				{
					spriteConfig = spriteNormalPress;
				}
			}
			if ((superTouchZone.PressedRaw(1) || superTouchZone.PressedRaw(2) || superTouchZone.PressedRaw(3)) && (spriteConfig == null || !spriteConfig.enabled))
			{
				spriteConfig = spriteRawPress;
			}
			if (spriteConfig == null || !spriteConfig.enabled)
			{
				spriteConfig = spriteNeutral;
			}
			BeginSpriteAnim(spriteConfig, skipAnim);
			UpdateSpriteAnimation(skipAnim);
		}

		MonoBehaviour ISpriteAnimator.GetComponent()
		{
			return this;
		}

		void ISpriteAnimator.AddUsedSprites(ISpriteOptimizer optimizer)
		{
			for (ControlState controlState = ControlState.Neutral; controlState < ControlState.All; controlState++)
			{
				optimizer.AddSprite(GetStateSpriteConfig(controlState).sprite);
			}
		}

		void ISpriteAnimator.OnSpriteOptimization(ISpriteOptimizer optimizer)
		{
			for (ControlState controlState = ControlState.Neutral; controlState < ControlState.All; controlState++)
			{
				SpriteConfig stateSpriteConfig = GetStateSpriteConfig(controlState);
				stateSpriteConfig.sprite = optimizer.GetOptimizedSprite(stateSpriteConfig.sprite);
			}
		}
	}
}
