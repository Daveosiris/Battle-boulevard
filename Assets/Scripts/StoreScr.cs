using UnityEngine;

public class StoreScr : MonoBehaviour
{
	public GameObject _pleaseWait;

	public MenuManager _menuManager;

	public GameObject _restorePurchase;

	private void Awake()
	{
		if (Config.platform == Platform.Android)
		{
			_restorePurchase.SetActive(value: false);
		}
		else
		{
			_restorePurchase.SetActive(value: true);
		}
	}

	private void OnEnable()
	{
		_pleaseWait.SetActive(value: false);
	}

	public void PurchaseCompleted(string product)
	{
		switch (product)
		{
		case "product1":
			_menuManager.SetOrangeCount(250);
			break;
		case "product2":
			_menuManager.SetOrangeCount(600);
			DataFunctions.SaveData(Config.PP_Removed_Ads, 1);
			Object.FindObjectOfType<AdManager>().DestroyBanner();
			break;
		case "product3":
			_menuManager.SetOrangeCount(1500);
			DataFunctions.SaveData(Config.PP_Removed_Ads, 1);
			Object.FindObjectOfType<AdManager>().DestroyBanner();
			break;
		case "product4":
			_menuManager.SetOrangeCount(3000);
			DataFunctions.SaveData(Config.PP_Removed_Ads, 1);
			Object.FindObjectOfType<AdManager>().DestroyBanner();
			break;
		case "product5":
			_menuManager.SetOrangeCount(6500);
			DataFunctions.SaveData(Config.PP_Removed_Ads, 1);
			Object.FindObjectOfType<AdManager>().DestroyBanner();
			break;
		case "product6":
			_menuManager.SetOrangeCount(13000);
			DataFunctions.SaveData(Config.PP_Removed_Ads, 1);
			Object.FindObjectOfType<AdManager>().DestroyBanner();
			break;
		case "product7":
			_menuManager._makeNoise.PlaySFX("ButtonStart");
			DataFunctions.SaveData(Config.PP_Removed_Ads, 1);
			Object.FindObjectOfType<AdManager>().DestroyBanner();
			break;
		case "product8":
			_menuManager.SetOrangeCount(600);
			break;
		case "product9":
			_menuManager.SetOrangeCount(1500);
			break;
		case "product10":
			_menuManager.SetOrangeCount(3000);
			break;
		case "product11":
			_menuManager.SetOrangeCount(6500);
			break;
		case "product12":
			_menuManager.SetOrangeCount(13000);
			break;
		case "Acfvssg2":
			_menuManager.SetOrangeCount(750);
			DataFunctions.SaveData(Config.PP_Removed_Ads, 1);
			Object.FindObjectOfType<AdManager>().DestroyBanner();
			break;
		case "Acfvssg3":
			_menuManager.SetOrangeCount(1250);
			DataFunctions.SaveData(Config.PP_Removed_Ads, 1);
			Object.FindObjectOfType<AdManager>().DestroyBanner();
			break;
		case "Acfvssg4":
			DataFunctions.SaveData(Config.PP_Removed_Ads, 1);
			_menuManager._makeNoise.PlaySFX("ButtonStart");
			Object.FindObjectOfType<AdManager>().DestroyBanner();
			break;
		default:
			UnityEngine.Debug.Log("Unrecognized productId" + product);
			break;
		}
		_menuManager.OpenCloseStoreMenu();
	}
}
