using System;
using UnityEngine;

[Serializable]
public class AttackO
{
	public AnimationO Attack;

	public AttackType _attackType;

	public AttackDirection _attackDirection;

	public float _hitPosition;

	public bool _poweredAttack;

	public bool _knockDown;

	[Header("Hit Collider Settings")]
	public Vector2 CollSize;

	[Header("Custom Reactions")]
	public AnimationO[] _customReactions;
}
