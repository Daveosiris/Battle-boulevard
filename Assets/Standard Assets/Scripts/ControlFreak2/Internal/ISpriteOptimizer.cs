using UnityEngine;

namespace ControlFreak2.Internal
{
	public interface ISpriteOptimizer
	{
		Sprite GetOptimizedSprite(Sprite oldSprite);

		void AddSprite(Sprite sprite);
	}
}
