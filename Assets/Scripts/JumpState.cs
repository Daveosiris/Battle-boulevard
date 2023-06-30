using UnityEngine;

public class JumpState : StateMachineBehaviour
{
	public override void OnStateEnter(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
	{
		animator.transform.parent.GetComponent<CharacterControl>().Jumping();
	}
}
