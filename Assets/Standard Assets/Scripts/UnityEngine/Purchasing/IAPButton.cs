using System;
using UnityEngine.Events;
using UnityEngine.UI;

namespace UnityEngine.Purchasing
{
	[RequireComponent(typeof(Button))]
	[AddComponentMenu("Unity IAP/IAP Button")]
	[HelpURL("https://docs.unity3d.com/Manual/UnityIAP.html")]
	public class IAPButton : MonoBehaviour
	{
		public enum ButtonType
		{
			Purchase,
			Restore
		}

		[Serializable]
		public class OnPurchaseCompletedEvent : UnityEvent<Product>
		{
		}

		[Serializable]
		public class OnPurchaseFailedEvent : UnityEvent<Product, PurchaseFailureReason>
		{
		}

		[HideInInspector]
		public string productId;

		[Tooltip("The type of this button, can be either a purchase or a restore button")]
		public ButtonType buttonType;

		[Tooltip("Consume the product immediately after a successful purchase")]
		public bool consumePurchase = true;

		[Tooltip("Event fired after a successful purchase of this product")]
		public OnPurchaseCompletedEvent onPurchaseComplete;

		[Tooltip("Event fired after a failed purchase of this product")]
		public OnPurchaseFailedEvent onPurchaseFailed;

		[Tooltip("[Optional] Displays the localized title from the app store")]
		public Text titleText;

		[Tooltip("[Optional] Displays the localized description from the app store")]
		public Text descriptionText;

		[Tooltip("[Optional] Displays the localized price from the app store")]
		public Text priceText;

		private void Start()
		{
			Button component = GetComponent<Button>();
			if (buttonType == ButtonType.Purchase)
			{
				if ((bool)component)
				{
					component.onClick.AddListener(PurchaseProduct);
				}
				if (string.IsNullOrEmpty(productId))
				{
					UnityEngine.Debug.LogError("IAPButton productId is empty");
				}
				if (!CodelessIAPStoreListener.Instance.HasProductInCatalog(productId))
				{
					UnityEngine.Debug.LogWarning("The product catalog has no product with the ID \"" + productId + "\"");
				}
			}
			else if (buttonType == ButtonType.Restore && (bool)component)
			{
				component.onClick.AddListener(Restore);
			}
		}

		private void OnEnable()
		{
			if (buttonType == ButtonType.Purchase)
			{
				CodelessIAPStoreListener.Instance.AddButton(this);
				if (CodelessIAPStoreListener.initializationComplete)
				{
					UpdateText();
				}
			}
		}

		private void OnDisable()
		{
			if (buttonType == ButtonType.Purchase)
			{
				CodelessIAPStoreListener.Instance.RemoveButton(this);
			}
		}

		private void PurchaseProduct()
		{
			if (buttonType == ButtonType.Purchase)
			{
				UnityEngine.Debug.Log("IAPButton.PurchaseProduct() with product ID: " + productId);
				CodelessIAPStoreListener.Instance.InitiatePurchase(productId);
			}
		}

		private void Restore()
		{
			if (buttonType == ButtonType.Restore)
			{
				if (Application.platform == RuntimePlatform.MetroPlayerX86 || Application.platform == RuntimePlatform.MetroPlayerX64 || Application.platform == RuntimePlatform.MetroPlayerARM)
				{
					CodelessIAPStoreListener.Instance.ExtensionProvider.GetExtension<IMicrosoftExtensions>().RestoreTransactions();
				}
				else if (Application.platform == RuntimePlatform.IPhonePlayer || Application.platform == RuntimePlatform.OSXPlayer || Application.platform == RuntimePlatform.tvOS)
				{
					CodelessIAPStoreListener.Instance.ExtensionProvider.GetExtension<IAppleExtensions>().RestoreTransactions(OnTransactionsRestored);
				}
				
				else
				{
					UnityEngine.Debug.LogWarning(Application.platform.ToString() + " is not a supported platform for the Codeless IAP restore button");
				}
			}
		}

		private void OnTransactionsRestored(bool success)
		{
			UnityEngine.Debug.Log("Transactions restored: " + success);
		}

		public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs e)
		{
			UnityEngine.Debug.Log($"IAPButton.ProcessPurchase(PurchaseEventArgs {e} - {e.purchasedProduct.definition.id})");
			onPurchaseComplete.Invoke(e.purchasedProduct);
			return (!consumePurchase) ? PurchaseProcessingResult.Pending : PurchaseProcessingResult.Complete;
		}

		public void OnPurchaseFailed(Product product, PurchaseFailureReason reason)
		{
			UnityEngine.Debug.Log($"IAPButton.OnPurchaseFailed(Product {product}, PurchaseFailureReason {reason})");
			onPurchaseFailed.Invoke(product, reason);
		}

		internal void UpdateText()
		{
			Product product = CodelessIAPStoreListener.Instance.GetProduct(productId);
			if (product != null)
			{
				if (titleText != null)
				{
					titleText.text = product.metadata.localizedTitle;
				}
				if (descriptionText != null)
				{
					descriptionText.text = product.metadata.localizedDescription;
				}
				if (priceText != null)
				{
					priceText.text = product.metadata.localizedPriceString;
				}
			}
		}
	}
}
