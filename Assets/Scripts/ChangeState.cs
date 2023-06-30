using UnityEngine;

public class ChangeState : StateMachineBehaviour
{
	public bool _checkGrabCondition;

	public AnimStates _animStates;

	public override void OnStateEnter(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
	{
		CharacterControl component = animator.transform.parent.GetComponent<CharacterControl>();
		if (!_checkGrabCondition || (component._state != AnimStates.GRAB && component._state != AnimStates.GRABBED))
		{
			component._state = _animStates;
		}
	}
}
