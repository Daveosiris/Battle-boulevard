using UnityEngine;

[CreateAssetMenu(fileName = "New ReactionSet", menuName = "Soner/ReactionSet", order = 2)]
public class ReactionSet : ScriptableObject
{
	[Header("Face Weak")]
	public AnimationO[] _leftFaceWeak;

	public AnimationO[] _rightFaceWeak;

	[Header("Face Heavy")]
	public AnimationO[] _leftFaceHeavy;

	public AnimationO[] _rightFaceHeavy;

	[Header("Body Weak")]
	public AnimationO[] _leftBodyWeak;

	public AnimationO[] _rightBodyWeak;

	[Header("Body Heavy")]
	public AnimationO[] _leftBodyHeavy;

	public AnimationO[] _rightBodyHeavy;

	[Header("Leg Weak")]
	public AnimationO[] _leftLegWeak;

	public AnimationO[] _rightLegWeak;

	[Header("Leg Heavy")]
	public AnimationO[] _leftLegHeavy;

	public AnimationO[] _rightLegHeavy;

	[Header("Ground")]
	public AnimationO[] _ground;

	[Header("Eskiv")]
	public AnimationO[] _leftFaceEskiv;

	public AnimationO[] _rightFaceEskiv;

	[Header("Dies")]
	public AnimationO[] _diesGround;

	public AnimationO[] _diesStandingUp;

	public AnimationO[] _diesStand;
}
