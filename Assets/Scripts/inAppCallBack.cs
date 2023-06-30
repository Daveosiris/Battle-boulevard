using UnityEngine;
using UnityEngine.Purchasing;

public class inAppCallBack : MonoBehaviour
{
	public void PleaseWait()
	{
		Object.FindObjectOfType<StoreScr>()._pleaseWait.SetActive(value: true);
	}

	public void PurchaseSuccess(Product p)
	{
		Object.FindObjectOfType<StoreScr>().PurchaseCompleted(p.definition.id);
		Object.FindObjectOfType<StoreScr>()._pleaseWait.SetActive(value: false);
	}

	public void Purchasefail(Product p, PurchaseFailureReason r)
	{
		Object.FindObjectOfType<StoreScr>()._pleaseWait.SetActive(value: false);
	}
}
