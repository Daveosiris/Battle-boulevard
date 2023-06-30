using UnityEngine;

namespace ControlFreak2.Demos.RPG
{
	public class SimpleInteractiveObject : InteractiveObjectBase
	{
		public Color activatedColor = Color.green;

		public Renderer targetRenderer;

		public AudioClip soundEffect;

		private bool isActivated;

		public override void OnCharacterAction(CharacterAction chara)
		{
			isActivated = !isActivated;
			if (targetRenderer != null)
			{
				targetRenderer.material.color = ((!isActivated) ? Color.white : activatedColor);
			}
			if (soundEffect != null)
			{
				AudioSource.PlayClipAtPoint(soundEffect, base.transform.position);
			}
		}
	}
}
