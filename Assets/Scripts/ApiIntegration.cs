using System.Collections.Generic;
using UnityEngine;

public class ApiIntegration : MonoBehaviour
{
	private static string tenjinApiKey = "NFVPXYHXVEDSO2WHT3HVCMZDAQGOPS5X";

	private void Start()
	{
		Object.DontDestroyOnLoad(base.gameObject);
		List<string> list = new List<string>();
		list.Add("ip_address");
		list.Add("advertising_id");
		list.Add("developer_device_id");
		list.Add("limit_ad_tracking");
		list.Add("referrer");
		list.Add("iad");
		List<string> list2 = list;
		list = new List<string>();
		list.Add("locale");
		list.Add("timezone");
		list.Add("build_id");
		List<string> list3 = list;
		BaseTenjin instance = Tenjin.getInstance(tenjinApiKey);
		instance.Connect();
	}

	private void Update()
	{
	}

	public void Call()
	{
		MethodWithCustomEvent();
		UnityEngine.Debug.Log("Call method fired");
	}

	private void OnApplicationPause(bool pauseStatus)
	{
		if (!pauseStatus)
		{
			BaseTenjin instance = Tenjin.getInstance(tenjinApiKey);
			instance.Connect();
		}
	}

	private static void CompletedAndroidPurchase(string ProductId, string CurrencyCode, int Quantity, double UnitPrice, string Receipt, string Signature)
	{
		BaseTenjin instance = Tenjin.getInstance(tenjinApiKey);
		instance.Transaction(ProductId, CurrencyCode, Quantity, UnitPrice, null, Receipt, Signature);
	}

	private static void CompletedIosPurchase(string ProductId, string CurrencyCode, int Quantity, double UnitPrice, string TransactionId, string Receipt)
	{
		BaseTenjin instance = Tenjin.getInstance(tenjinApiKey);
		instance.Transaction(ProductId, CurrencyCode, Quantity, UnitPrice, TransactionId, Receipt, null);
	}

	public void CompletedPurchase(string ProductId, string CurrencyCode, int Quantity, double UnitPrice)
	{
		BaseTenjin instance = Tenjin.getInstance(tenjinApiKey);
		instance.Transaction(ProductId, CurrencyCode, Quantity, UnitPrice, null, null, null);
	}

	private void MethodWithCustomEvent()
	{
		BaseTenjin instance = Tenjin.getInstance(tenjinApiKey);
		instance.SendEvent("name");
		instance.SendEvent("nameWithValue", "1");
	}
}
