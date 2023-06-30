using UnityEngine;

[CreateAssetMenu(fileName = "New AttackSet", menuName = "Soner/AttackSet", order = 1)]
public class AttackSet : ScriptableObject
{
	public bool _comboStyle;

	public AttackO[] _attacks;
}
