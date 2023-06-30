using UnityEngine;

namespace ControlFreak2.Demos.RPG
{
	public abstract class InteractiveObjectBase : MonoBehaviour
	{
		public float radius = 2f;

		public abstract void OnCharacterAction(CharacterAction chara);

		public virtual bool IsNear(CharacterAction chara)
		{
			return (chara.transform.position - base.transform.position).sqrMagnitude < radius * radius;
		}

		private void OnDrawGizmos()
		{
			DrawDefaultGizmo();
		}

		protected void DrawDefaultGizmo()
		{
			Gizmos.matrix = Matrix4x4.identity;
			Gizmos.DrawWireSphere(base.transform.position, radius);
		}
	}
}
