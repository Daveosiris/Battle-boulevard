using UnityEngine;

namespace ControlFreak2.Demos.RPG
{
	public class CharacterAction : MonoBehaviour
	{
		public LevelData levelData;

		public string actionButonName = "Action";

		public string actionAnimatorTrigger = "Action";

		private Animator animator;

		private void OnEnable()
		{
			animator = GetComponent<Animator>();
		}

		private void Update()
		{
			if (!string.IsNullOrEmpty(actionButonName) && CF2Input.GetButtonDown(actionButonName))
			{
				PerformAction();
			}
		}

		public void PerformAction()
		{
			if (levelData == null)
			{
				return;
			}
			InteractiveObjectBase interactiveObjectBase = levelData.FindInteractiveObjectFor(this);
			if (interactiveObjectBase != null)
			{
				if (!string.IsNullOrEmpty(actionAnimatorTrigger) && animator != null)
				{
					animator.SetTrigger(actionAnimatorTrigger);
				}
				interactiveObjectBase.OnCharacterAction(this);
			}
		}
	}
}
