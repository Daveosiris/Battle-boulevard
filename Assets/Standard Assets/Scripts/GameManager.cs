using System;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UDP;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
	public class InitListener : IInitListener
	{
		public void OnInitialized(UserInfo userInfo)
		{
			UnityEngine.Debug.Log("[Game]On Initialized suceeded");
			Show("Initialize succeeded");
			_initialized = true;
		}

		public void OnInitializeFailed(string message)
		{
			UnityEngine.Debug.Log("[Game]OnInitializeFailed: " + message);
			Show("Initialize Failed: " + message);
		}
	}

	public class PurchaseListener 
	{
		public void OnPurchase(PurchaseInfo purchaseInfo)
		{
			string message = $"[Game] Purchase Succeeded, productId: {purchaseInfo.ProductId}, cpOrderId: {purchaseInfo.GameOrderId}, developerPayload: {purchaseInfo.DeveloperPayload}, storeJson: {purchaseInfo.StorePurchaseJsonString}";
			UnityEngine.Debug.Log(message);
			Show(message);
			if (m_consumeOnPurchase)
			{
				UnityEngine.Debug.Log("Consuming");
				//StoreService.ConsumePurchase(purchaseInfo, this);
			}
		}

		public void OnPurchaseFailed(string message, PurchaseInfo purchaseInfo)
		{
			UnityEngine.Debug.Log("Purchase Failed: " + message);
			Show("Purchase Failed: " + message);
		}

		public void OnPurchaseRepeated(string productCode)
		{
			throw new NotImplementedException();
		}

		public void OnPurchaseConsume(PurchaseInfo purchaseInfo)
		{
			Show("Consume success for " + purchaseInfo.ProductId, append: true);
			UnityEngine.Debug.Log("Consume success: " + purchaseInfo.ProductId);
		}

		public void OnMultiPurchaseConsume(List<bool> successful, List<PurchaseInfo> purchaseInfos, List<string> messages)
		{
			int count = successful.Count;
			for (int i = 0; i < count; i++)
			{
				if (successful[i])
				{
					string message = $"Consuming succeeded for {purchaseInfos[i].ProductId}\n";
					Show(message, append: true);
					UnityEngine.Debug.Log(message);
				}
				else
				{
					string message = $"Consuming failed for {purchaseInfos[i].ProductId}, reason: {messages[i]}";
					Show(message, append: true);
					UnityEngine.Debug.Log(message);
				}
			}
		}

		public void OnPurchaseConsumeFailed(string message, PurchaseInfo purchaseInfo)
		{
			UnityEngine.Debug.Log("Consume Failed: " + message);
			Show("Consume Failed: " + message);
		}

		public void OnQueryInventory(Inventory inventory)
		{
			UnityEngine.Debug.Log("OnQueryInventory");
			UnityEngine.Debug.Log("[Game] Product List: ");
			string str = "Product List: \n";
			foreach (KeyValuePair<string, ProductInfo> item in inventory.GetProductDictionary())
			{
				UnityEngine.Debug.Log("[Game] Returned product: " + item.Key + " " + item.Value.ProductId);
				str += $"{item.Key}:\n\tTitle: {item.Value.Title}\n\tDescription: {item.Value.Description}\n\tConsumable: {item.Value.Consumable}\n\tPrice: {item.Value.Price}\n\tCurrency: {item.Value.Currency}\n\tPriceAmountMicros: {item.Value.PriceAmountMicros}\n\tItemType: {item.Value.ItemType}\n";
			}
			str += "\nPurchase List: \n";
			foreach (KeyValuePair<string, PurchaseInfo> item2 in inventory.GetPurchaseDictionary())
			{
				UnityEngine.Debug.Log("[Game] Returned purchase: " + item2.Key);
				str += $"{item2.Value.ProductId}\n";
			}
			Show(str);
			if (_consumeOnQuery)
			{
				//StoreService.ConsumePurchase(inventory.GetPurchaseList(), this);
			}
		}

		public void OnQueryInventoryFailed(string message)
		{
			UnityEngine.Debug.Log("OnQueryInventory Failed: " + message);
			Show("QueryInventory Failed: " + message);
		}
	}

	public string Product1;

	public string Product2;

	private static bool m_consumeOnPurchase;

	private static bool _consumeOnQuery;

	private Dropdown _dropdown;

	private List<Dropdown.OptionData> options;

	private static Text _textField;

	private static bool _initialized;

	private PurchaseListener purchaseListener;

	private InitListener initListener;

	private AppInfo appInfo;

	private void Start()
	{
		purchaseListener = new PurchaseListener();
		initListener = new InitListener();
		appInfo = new AppInfo();
		AppStoreSettings appStoreSettings = Resources.Load<AppStoreSettings>("GameSettings");
		appInfo.AppSlug = appStoreSettings.AppSlug;
		appInfo.ClientId = appStoreSettings.UnityClientID;
		appInfo.ClientKey = appStoreSettings.UnityClientKey;
		appInfo.RSAPublicKey = appStoreSettings.UnityClientRSAPublicKey;
		UnityEngine.Debug.Log("App Name: " + appStoreSettings.AppName);
		GameObject gameObject = GameObject.Find("Information");
		_textField = gameObject.GetComponent<Text>();
		_textField.text = "Please Click Init to Start";
		gameObject = GameObject.Find("Dropdown");
		_dropdown = gameObject.GetComponent<Dropdown>();
		_dropdown.ClearOptions();
		_dropdown.options.Add(new Dropdown.OptionData(Product1));
		_dropdown.options.Add(new Dropdown.OptionData(Product2));
		_dropdown.RefreshShownValue();
		InitUI();
	}

	private static void Show(string message, bool append = false)
	{
		_textField.text = ((!append) ? message : $"{_textField.text}\n{message}");
	}

	private void InitUI()
	{
		GetButton("InitButton").onClick.AddListener(delegate
		{
			_initialized = false;
			UnityEngine.Debug.Log("Init button is clicked.");
			Show("Initializing");
			StoreService.Initialize(initListener);
		});
		GetButton("BuyButton").onClick.AddListener(delegate
		{
			if (!_initialized)
			{
				Show("Please Initialize first");
			}
			else
			{
				string text2 = _dropdown.options[_dropdown.value].text;
				UnityEngine.Debug.Log("Buy button is clicked.");
				Show("Buying Product: " + text2);
				m_consumeOnPurchase = false;
				UnityEngine.Debug.Log(_dropdown.options[_dropdown.value].text + " will be bought");
			//	StoreService.Purchase(text2, null, "{\"AnyKeyYouWant:\" : \"AnyValueYouWant\"}", purchaseListener);
			}
		});
		GetButton("BuyConsumeButton").onClick.AddListener(delegate
		{
			if (!_initialized)
			{
				Show("Please Initialize first");
			}
			else
			{
				string text = _dropdown.options[_dropdown.value].text;
				Show("Buying Product: " + text);
				UnityEngine.Debug.Log("Buy&Consume button is clicked.");
				m_consumeOnPurchase = true;
				//StoreService.Purchase(text, null, "buy and consume developer payload", purchaseListener);
			}
		});
		List<string> productIds = new List<string>
		{
			Product1,
			Product2
		};
		GetButton("QueryButton").onClick.AddListener(delegate
		{
			if (!_initialized)
			{
				Show("Please Initialize first");
			}
			else
			{
				_consumeOnQuery = false;
				UnityEngine.Debug.Log("Query button is clicked.");
				Show("Querying Inventory");
				//StoreService.QueryInventory(productIds, purchaseListener);
			}
		});
		GetButton("QueryConsumeButton").onClick.AddListener(delegate
		{
			if (!_initialized)
			{
				Show("Please Initialize first");
			}
			else
			{
				_consumeOnQuery = true;
				Show("Querying Inventory");
				UnityEngine.Debug.Log("QueryConsume button is clicked.");
				//StoreService.QueryInventory(productIds, purchaseListener);
			}
		});
	}

	private Button GetButton(string buttonName)
	{
		GameObject gameObject = GameObject.Find(buttonName);
		if (gameObject != null)
		{
			return gameObject.GetComponent<Button>();
		}
		return null;
	}
}
