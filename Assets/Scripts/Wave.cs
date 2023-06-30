using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Wave
{
	public int _maxSpawnEnemyCount;

	public StillEnemy[] stillEnemies;

	public GameObject[] waweEnemies;

	public GameObject[] bossEnemies;

	[HideInInspector]
	public List<EnemyControl> _stillEnemies = new List<EnemyControl>();

	[HideInInspector]
	public List<EnemyControl> _waveEnemies = new List<EnemyControl>();

	[HideInInspector]
	public List<EnemyControl> _bossEnemies = new List<EnemyControl>();

	public float _endPoint;
}
