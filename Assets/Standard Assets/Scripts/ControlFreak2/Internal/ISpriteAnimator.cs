using UnityEngine;

namespace ControlFreak2.Internal
{
	public interface ISpriteAnimator
	{
		void AddUsedSprites(ISpriteOptimizer optimizer);

		void OnSpriteOptimization(ISpriteOptimizer optimizer);

		MonoBehaviour GetComponent();
	}
}
