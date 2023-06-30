namespace UnityEngine.Purchasing
{
	[AddComponentMenu("")]
	public class DemoInventory : MonoBehaviour
	{
		public void Fulfill(string productId)
		{
			if (productId != null && productId == "100.gold.coins")
			{
				UnityEngine.Debug.Log("You Got Money!");
			}
			else
			{
				UnityEngine.Debug.Log($"Unrecognized productId \"{productId}\"");
			}
		}
	}
}
