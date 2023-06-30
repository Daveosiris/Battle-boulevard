using UnityEngine;

namespace ControlFreak2.Demos.SwipeSlasher
{
	public class SwipeSlasherUserControl : MonoBehaviour
	{
		public SwipeSlasherChara character;

		private void OnEnable()
		{
			if (character == null)
			{
				character = GetComponent<SwipeSlasherChara>();
			}
		}

		private void Update()
		{
			if (!(character == null))
			{
				if (CF2Input.GetButtonDown("Left-Stab"))
				{
					character.ExecuteAction(SwipeSlasherChara.ActionType.LEFT_STAB);
				}
				else if (CF2Input.GetButtonDown("Left-Slash-U"))
				{
					character.ExecuteAction(SwipeSlasherChara.ActionType.LEFT_SLASH_U);
				}
				else if (CF2Input.GetButtonDown("Left-Slash-R"))
				{
					character.ExecuteAction(SwipeSlasherChara.ActionType.LEFT_SLASH_R);
				}
				else if (CF2Input.GetButtonDown("Left-Slash-L"))
				{
					character.ExecuteAction(SwipeSlasherChara.ActionType.LEFT_SLASH_L);
				}
				else if (CF2Input.GetButtonDown("Left-Slash-D"))
				{
					character.ExecuteAction(SwipeSlasherChara.ActionType.LEFT_SLASH_D);
				}
				else if (CF2Input.GetButtonDown("Right-Stab"))
				{
					character.ExecuteAction(SwipeSlasherChara.ActionType.RIGHT_STAB);
				}
				else if (CF2Input.GetButtonDown("Right-Slash-U"))
				{
					character.ExecuteAction(SwipeSlasherChara.ActionType.RIGHT_SLASH_U);
				}
				else if (CF2Input.GetButtonDown("Right-Slash-R"))
				{
					character.ExecuteAction(SwipeSlasherChara.ActionType.RIGHT_SLASH_R);
				}
				else if (CF2Input.GetButtonDown("Right-Slash-L"))
				{
					character.ExecuteAction(SwipeSlasherChara.ActionType.RIGHT_SLASH_L);
				}
				else if (CF2Input.GetButtonDown("Right-Slash-D"))
				{
					character.ExecuteAction(SwipeSlasherChara.ActionType.RIGHT_SLASH_D);
				}
				else if (CF2Input.GetButtonDown("Dodge-Right"))
				{
					character.ExecuteAction(SwipeSlasherChara.ActionType.DODGE_RIGHT);
				}
				else if (CF2Input.GetButtonDown("Dodge-Left"))
				{
					character.ExecuteAction(SwipeSlasherChara.ActionType.DODGE_LEFT);
				}
			}
		}
	}
}
