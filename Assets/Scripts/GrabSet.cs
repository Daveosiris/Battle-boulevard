using UnityEngine;

[CreateAssetMenu(fileName = "New GrabSet", menuName = "Soner/GrabSet", order = 3)]
public class GrabSet : ScriptableObject
{
	public float _grapPosition;

	public float _releaseTime;

	public GrabAnimations[] _grabbCombos;

	public GrabAnimations _escape;

	[HideInInspector]
	public int _comboInd;
}
