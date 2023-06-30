using UnityEngine;

public class RandomMaterials : MonoBehaviour
{
	public Material[] _materials;

	private void Awake()
	{
		//GetComponent<SkinnedMeshRenderer>().material = _materials[Random.Range(0, _materials.Length)];
	}
}
