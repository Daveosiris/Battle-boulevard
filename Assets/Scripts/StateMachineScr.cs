using UnityEngine;

public class StateMachineScr : StateMachineBehaviour
{
	public override void OnStateEnter(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
	{
		animator.transform.parent.GetComponent<CharacterControl>().ToIDLE();
	}
}
