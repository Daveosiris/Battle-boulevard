using ControlFreak2.Internal;
using UnityEngine;
using UnityEngine.UI;

namespace ControlFreak2
{
	[RequireComponent(typeof(RectTransform))]
	[RequireComponent(typeof(Image))]
	[ExecuteInEditMode]
	public class TouchJoystickSpriteAnimator : TouchControlSpriteAnimatorBase, ISpriteAnimator
	{
		public enum SpriteMode
		{
			Simple,
			FourWay,
			EightWay
		}

		public enum RotationMode
		{
			Disabled,
			SimpleHorizontal,
			SimpleVertical,
			Compass
		}

		public enum ControlState
		{
			Neutral,
			NeutralPressed,
			U,
			UR,
			R,
			DR,
			D,
			DL,
			L,
			UL,
			All
		}

		public SpriteMode spriteMode;

		public SpriteConfig spriteNeutralPressed;

		public SpriteConfig spriteUp;

		public SpriteConfig spriteUpRight;

		public SpriteConfig spriteRight;

		public SpriteConfig spriteDownRight;

		public SpriteConfig spriteDown;

		public SpriteConfig spriteDownLeft;

		public SpriteConfig spriteLeft;

		public SpriteConfig spriteUpLeft;

		public bool animateTransl;

		public Vector2 moveScale;

		public float translationSmoothingTime = 0.1f;

		public RotationMode rotationMode;

		public float simpleRotationRange;

		public float rotationSmoothingTime = 0.1f;

		private float lastSafeCompassAngle;

		public TouchJoystickSpriteAnimator()
			: base(typeof(TouchJoystick))
		{
			animateTransl = false;
			moveScale = new Vector2(0.5f, 0.5f);
			rotationMode = RotationMode.Disabled;
			rotationSmoothingTime = 0.01f;
			simpleRotationRange = 45f;
			lastSafeCompassAngle = 0f;
			spriteNeutralPressed = new SpriteConfig(enabled: true, oneShot: false, 1.2f);
			spriteUp = new SpriteConfig(enabled: false, oneShot: false, 1.2f);
			spriteUpRight = new SpriteConfig(enabled: false, oneShot: false, 1.2f);
			spriteRight = new SpriteConfig(enabled: false, oneShot: false, 1.2f);
			spriteDownRight = new SpriteConfig(enabled: false, oneShot: false, 1.2f);
			spriteDown = new SpriteConfig(enabled: false, oneShot: false, 1.2f);
			spriteDownLeft = new SpriteConfig(enabled: false, oneShot: false, 1.2f);
			spriteLeft = new SpriteConfig(enabled: false, oneShot: false, 1.2f);
			spriteUpLeft = new SpriteConfig(enabled: false, oneShot: false, 1.2f);
		}

		public void SetSprite(Sprite sprite)
		{
			spriteNeutral.sprite = sprite;
			spriteNeutralPressed.sprite = sprite;
			spriteUp.sprite = sprite;
			spriteUpRight.sprite = sprite;
			spriteRight.sprite = sprite;
			spriteDownRight.sprite = sprite;
			spriteDown.sprite = sprite;
			spriteDownLeft.sprite = sprite;
			spriteLeft.sprite = sprite;
			spriteUpLeft.sprite = sprite;
		}

		public void SetColor(Color color)
		{
			spriteNeutral.color = color;
			spriteNeutralPressed.color = color;
			spriteUp.color = color;
			spriteUpRight.color = color;
			spriteRight.color = color;
			spriteDownRight.color = color;
			spriteDown.color = color;
			spriteDownLeft.color = color;
			spriteLeft.color = color;
			spriteUpLeft.color = color;
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
			case ControlState.NeutralPressed:
				return spriteNeutralPressed;
			case ControlState.U:
				return spriteUp;
			case ControlState.UR:
				return spriteUpRight;
			case ControlState.R:
				return spriteRight;
			case ControlState.DR:
				return spriteDownRight;
			case ControlState.D:
				return spriteDown;
			case ControlState.DL:
				return spriteDownLeft;
			case ControlState.L:
				return spriteLeft;
			case ControlState.UL:
				return spriteUpLeft;
			default:
				return null;
			}
		}

		protected override void OnInitComponent()
		{
			base.OnInitComponent();
			if (image != null && image.sprite != null && spriteNeutral.sprite == null)
			{
				spriteNeutral.sprite = image.sprite;
				spriteNeutralPressed.sprite = image.sprite;
				spriteUp.sprite = image.sprite;
				spriteUpRight.sprite = image.sprite;
				spriteRight.sprite = image.sprite;
				spriteDownRight.sprite = image.sprite;
				spriteDown.sprite = image.sprite;
				spriteDownLeft.sprite = image.sprite;
				spriteLeft.sprite = image.sprite;
				spriteUpLeft.sprite = image.sprite;
			}
		}

		protected override void OnUpdateAnimator(bool skipAnim)
		{
			TouchJoystick touchJoystick = (TouchJoystick)sourceControl;
			if (touchJoystick == null || image == null)
			{
				return;
			}
			JoystickState state = touchJoystick.GetState();
			SpriteConfig spriteConfig = null;
			if (spriteMode == SpriteMode.FourWay || spriteMode == SpriteMode.EightWay)
			{
				Dir dir = Dir.N;
				if (spriteMode == SpriteMode.FourWay)
				{
					dir = state.GetDir4();
				}
				else if (spriteMode == SpriteMode.EightWay)
				{
					dir = state.GetDir8();
				}
				switch (dir)
				{
				case Dir.U:
					spriteConfig = spriteUp;
					break;
				case Dir.UR:
					spriteConfig = spriteUpRight;
					break;
				case Dir.R:
					spriteConfig = spriteRight;
					break;
				case Dir.DR:
					spriteConfig = spriteDownRight;
					break;
				case Dir.D:
					spriteConfig = spriteDown;
					break;
				case Dir.DL:
					spriteConfig = spriteDownLeft;
					break;
				case Dir.L:
					spriteConfig = spriteLeft;
					break;
				case Dir.UL:
					spriteConfig = spriteUpLeft;
					break;
				}
			}
			if (touchJoystick.Pressed() && (spriteConfig == null || !spriteConfig.enabled))
			{
				spriteConfig = spriteNeutralPressed;
			}
			if (spriteConfig == null || !spriteConfig.enabled)
			{
				spriteConfig = spriteNeutral;
			}
			if (!CFUtils.editorStopped && !IsIllegallyAttachedToSource())
			{
				Vector2 vectorEx = state.GetVectorEx(touchJoystick.shape == TouchControl.Shape.Rectangle || touchJoystick.shape == TouchControl.Shape.Square);
				if (animateTransl)
				{
					extraOffset = CFUtils.SmoothTowardsVec2(extraOffset, Vector2.Scale(vectorEx, moveScale), translationSmoothingTime, CFUtils.realDeltaTimeClamped, 0.0001f);
				}
				else
				{
					extraOffset = Vector2.zero;
				}
				if (rotationMode != 0)
				{
					float num = 0f;
					if (touchJoystick.Pressed())
					{
						Vector2 vector = state.GetVector();
						if (rotationMode == RotationMode.Compass)
						{
							if (vector.sqrMagnitude > 0.0001f)
							{
								lastSafeCompassAngle = state.GetAngle();
							}
							num = 0f - lastSafeCompassAngle;
						}
						else
						{
							num = ((rotationMode != RotationMode.SimpleHorizontal) ? vector.y : vector.x) * (0f - simpleRotationRange);
						}
					}
					else
					{
						lastSafeCompassAngle = 0f;
						num = 0f;
					}
					extraRotation = CFUtils.SmoothTowardsAngle(extraRotation, num, rotationSmoothingTime, CFUtils.realDeltaTimeClamped, 0.0001f);
				}
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
			optimizer.AddSprite(spriteNeutral.sprite);
			optimizer.AddSprite(spriteNeutralPressed.sprite);
			optimizer.AddSprite(spriteUp.sprite);
			optimizer.AddSprite(spriteUpRight.sprite);
			optimizer.AddSprite(spriteRight.sprite);
			optimizer.AddSprite(spriteDownRight.sprite);
			optimizer.AddSprite(spriteDown.sprite);
			optimizer.AddSprite(spriteDownLeft.sprite);
			optimizer.AddSprite(spriteLeft.sprite);
			optimizer.AddSprite(spriteUpLeft.sprite);
		}

		void ISpriteAnimator.OnSpriteOptimization(ISpriteOptimizer optimizer)
		{
			spriteNeutral.sprite = optimizer.GetOptimizedSprite(spriteNeutral.sprite);
			spriteNeutralPressed.sprite = optimizer.GetOptimizedSprite(spriteNeutralPressed.sprite);
			spriteUp.sprite = optimizer.GetOptimizedSprite(spriteUp.sprite);
			spriteUpRight.sprite = optimizer.GetOptimizedSprite(spriteUpRight.sprite);
			spriteRight.sprite = optimizer.GetOptimizedSprite(spriteRight.sprite);
			spriteDownRight.sprite = optimizer.GetOptimizedSprite(spriteDownRight.sprite);
			spriteDown.sprite = optimizer.GetOptimizedSprite(spriteDown.sprite);
			spriteDownLeft.sprite = optimizer.GetOptimizedSprite(spriteDownLeft.sprite);
			spriteLeft.sprite = optimizer.GetOptimizedSprite(spriteLeft.sprite);
			spriteUpLeft.sprite = optimizer.GetOptimizedSprite(spriteUpLeft.sprite);
		}
	}
}
