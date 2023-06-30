using UnityEngine;

public class PoweringUpScr : StateMachineBehaviour
{
	public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		animator.transform.parent.GetComponent<CharacterControl>().PoweringUpStart();
	}

	public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		animator.transform.parent.GetComponent<CharacterControl>().PoweringUpEnd();
	}
}
