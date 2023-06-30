using UnityEngine;

public class LevelDesignScript : MonoBehaviour
{
	[Header("Setup")]
	public int _diffuculty;

	public int _level;

	public bool _specialBossScene;

	public Vector3 _handleOffset;

	[Header("GiveWeapon")]
	public GiveWeapon[] _giveWeapons;

	[Header("Drop Crate")]
	public int _dropPercent;

	public float _dropCratefreqSecond = 1f;

	public float _dropCreateDamage = 6f;

	public DropCrate _dropCrate;

	[Header("Enemy Waves")]
	public float _down;

	public float _up;

	public float _startPoint;

	public Wave[] _waves;
}
