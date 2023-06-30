using System;
using UnityEngine;
using UnityEngine.UI;

namespace ControlFreak2.Internal
{
	[ExecuteInEditMode]
	public abstract class TouchControlSpriteAnimatorBase : TouchControlAnimatorBase
	{
		protected RectTransform rectTr;

		protected Image image;

		protected CanvasGroup canvasGroup;

		protected Vector3 initialTransl;

		protected Vector3 initialScale;

		protected Quaternion initialRotation;

		public SpriteConfig spriteNeutral;

		[NonSerialized]
		private SpriteConfig curSprite;

		[NonSerialized]
		private SpriteConfig nextSprite;

		private float spriteAnimElapsed;

		protected Vector2 animOffsetStart;

		protected Vector2 animOffsetCur;

		protected Vector2 animScaleStart;

		protected Vector2 animScaleCur;

		protected float animRotationStart;

		protected float animRotationCur;

		protected Color animColorStart;

		protected Color animColorCur;

		protected Vector2 extraOffset;

		protected Vector2 extraScale;

		protected float extraRotation;

		public TouchControlSpriteAnimatorBase(Type sourceType)
			: base(sourceType)
		{
			spriteNeutral = new SpriteConfig();
			spriteNeutral.color = new Color(1f, 1f, 1f, 0.33f);
		}

		protected override void OnInitComponent()
		{
			base.OnInitComponent();
			rectTr = GetComponent<RectTransform>();
			if (image == null)
			{
				image = GetComponent<Image>();
				if (image == null)
				{
					image = base.gameObject.AddComponent<Image>();
				}
			}
			if (image != null)
			{
				image.raycastTarget = false;
			}
			canvasGroup = base.gameObject.GetComponent<CanvasGroup>();
			Transform transform = base.transform;
			initialTransl = transform.localPosition;
			initialScale = transform.localScale;
			initialRotation = transform.localRotation;
			animOffsetCur = (animOffsetStart = (extraOffset = Vector2.zero));
			animScaleCur = (animScaleStart = (extraScale = Vector2.one));
			animRotationCur = (animRotationStart = (extraRotation = 0f));
			animColorStart = (animColorCur = Color.white);
			BeginSpriteAnim(spriteNeutral, skipAnim: true, forceStart: true);
		}

		protected override void OnDisableComponent()
		{
			if (!CFUtils.editorStopped)
			{
				base.transform.localPosition = initialTransl;
				base.transform.localScale = initialScale;
				base.transform.localRotation = initialRotation;
			}
			base.OnDisableComponent();
		}

		protected void BeginSpriteAnim(SpriteConfig spriteConfig, bool skipAnim, bool forceStart = false)
		{
			if (curSprite == spriteConfig && !spriteConfig.oneShotState && !skipAnim)
			{
				return;
			}
			if (curSprite != null && curSprite.oneShotState && !skipAnim && !forceStart)
			{
				nextSprite = spriteConfig;
				return;
			}
			curSprite = spriteConfig;
			spriteAnimElapsed = 0f;
			if (!skipAnim)
			{
				animColorStart = animColorCur;
			}
			else
			{
				animColorStart = (animColorCur = spriteConfig.color);
			}
			if (CFUtils.editorStopped)
			{
				return;
			}
			if (!skipAnim)
			{
				animOffsetStart = animOffsetCur;
				animScaleStart = animScaleCur;
				animRotationStart = animRotationCur;
				if (curSprite.resetScale)
				{
					animScaleStart = Vector2.one;
				}
				if (curSprite.resetOffset)
				{
					animOffsetStart = Vector2.zero;
				}
				if (curSprite.resetRotation)
				{
					animRotationStart = 0f;
				}
			}
			else
			{
				animOffsetStart = (animOffsetCur = spriteConfig.offset);
				animScaleStart = (animScaleCur = Vector2.one * spriteConfig.scale);
				animRotationStart = (animRotationCur = spriteConfig.rotation);
				ApplySpriteAnimation();
			}
		}

		protected void UpdateSpriteAnimation(bool skipAnim)
		{
			spriteAnimElapsed += CFUtils.realDeltaTimeClamped;
			if (curSprite.oneShotState && spriteAnimElapsed >= curSprite.duration)
			{
				BeginSpriteAnim((nextSprite == null) ? spriteNeutral : nextSprite, skipAnim, forceStart: true);
			}
			animColorCur = Color.Lerp(animColorStart, curSprite.color, GetAnimLerpFactor((!skipAnim) ? (curSprite.colorTransitionFactor * curSprite.baseTransitionTime) : 0f));
			animColorCur = CFUtils.ScaleColorAlpha(animColorCur, sourceControl.GetAlpha());
			if (!CFUtils.editorStopped && !IsIllegallyAttachedToSource())
			{
				animOffsetCur = Vector2.Lerp(animOffsetStart, curSprite.offset, GetAnimLerpFactor((!skipAnim) ? (curSprite.offsetTransitionFactor * curSprite.baseTransitionTime) : 0f));
				animScaleCur = Vector2.Lerp(animScaleStart, Vector2.one * curSprite.scale, GetAnimLerpFactor((!skipAnim) ? (curSprite.scaleTransitionFactor * curSprite.baseTransitionTime) : 0f));
				animRotationCur = Mathf.LerpAngle(animRotationStart, curSprite.rotation, GetAnimLerpFactor((!skipAnim) ? (curSprite.rotationTransitionFactor * curSprite.baseTransitionTime) : 0f));
			}
			ApplySpriteAnimation();
		}

		public void ApplySpriteAnimation()
		{
			if (animColorCur.a > 1E-05f != image.enabled)
			{
				image.enabled = !image.enabled;
			}
			Color color = animColorCur;
			if (canvasGroup != null)
			{
				canvasGroup.alpha = animColorCur.a;
				color.a = 1f;
			}
			image.color = color;
			image.sprite = ((!(curSprite.sprite == null)) ? curSprite.sprite : spriteNeutral.sprite);
			if (!CFUtils.editorStopped && !IsIllegallyAttachedToSource())
			{
				Rect localRect = sourceControl.GetLocalRect();
				base.transform.localPosition = initialTransl + (Vector3)Vector2.Scale(localRect.size * 0.5f, animOffsetCur + extraOffset);
				base.transform.localRotation = Quaternion.Euler(new Vector3(0f, 0f, 0f - animRotationCur + extraRotation)) * initialRotation;
				base.transform.localScale = Vector3.Scale(initialScale, Vector2.Scale(animScaleCur, extraScale));
			}
		}

		private float GetAnimLerpFactor(float duration)
		{
			if (spriteAnimElapsed > duration || duration < 1E-05f)
			{
				return 1f;
			}
			return spriteAnimElapsed / duration;
		}
	}
}
