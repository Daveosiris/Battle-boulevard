using SA.Analytics.Firebase;
using System.Collections.Generic;
using UnityEngine;

namespace SA.Analytics.Google
{
	public class UseExample : MonoBehaviour
	{
		private void Start()
		{
			GA_Manager.StartTracking();
			SA.Analytics.Firebase.Analytics.Init();
		}

		private void OnGUI()
		{
			if (GUI.Button(new Rect(10f, 10f, 150f, 50f), "Page Hit"))
			{
				GA_Manager.Client.SendPageHit("mydemo.com ", "/home", "homepage", string.Empty, string.Empty);
			}
			if (GUI.Button(new Rect(10f, 70f, 150f, 50f), "Event Hit"))
			{
				GA_Manager.Client.SendEventHit("video", "play", "holiday", 300);
			}
			if (GUI.Button(new Rect(10f, 130f, 150f, 50f), "Transaction Hit"))
			{
				GA_Manager.Client.SendTransactionHit("12345", "westernWear", "EUR", 50f, 32f, 12f);
			}
			if (GUI.Button(new Rect(10f, 190f, 150f, 50f), "Item Hit"))
			{
				GA_Manager.Client.SendItemHit("12345", "sofa", "u3eqds43", 300f, 2, "furniture", "EUR");
			}
			if (GUI.Button(new Rect(190f, 10f, 150f, 50f), "Social Hit"))
			{
				GA_Manager.Client.SendSocialHit("like", "facebook", "/home ");
			}
			if (GUI.Button(new Rect(190f, 70f, 150f, 50f), "Exception Hit"))
			{
				GA_Manager.Client.SendExceptionHit("IOException", IsFatal: true);
			}
			if (GUI.Button(new Rect(190f, 130f, 150f, 50f), "Timing Hit"))
			{
				GA_Manager.Client.SendUserTimingHit("jsonLoader", "load", 5000, "jQuery");
			}
			if (GUI.Button(new Rect(190f, 190f, 150f, 50f), "Screen Hit"))
			{
				GA_Manager.Client.SendScreenHit("MainMenu");
			}
			if (GUI.Button(new Rect(10f, 270f, 150f, 50f), "Firebase Event"))
			{
				Dictionary<string, object> dictionary = new Dictionary<string, object>();
				dictionary.Add("str_data", "hello_from_firebase");
				dictionary.Add("numeric_data", 10101);
				SA.Analytics.Firebase.Analytics.LogEvent("ga_event", dictionary);
			}
		}

		public void CustomBuildersExamples()
		{
			GA_Manager.Client.CreateHit(HitType.PAGEVIEW);
			GA_Manager.Client.SetDocumentHostName("mydemo.com");
			GA_Manager.Client.SetDocumentPath("/home");
			GA_Manager.Client.SetDocumentTitle("homepage");
			GA_Manager.Client.Send();
			GA_Manager.Client.CreateHit(HitType.EVENT);
			GA_Manager.Client.SetEventCategory("video");
			GA_Manager.Client.SetEventAction("play");
			GA_Manager.Client.SetEventLabel("holiday");
			GA_Manager.Client.SetEventValue(300);
			GA_Manager.Client.Send();
			GA_Manager.Client.CreateHit(HitType.PAGEVIEW);
			GA_Manager.Client.SetDocumentHostName("mydemo.com");
			GA_Manager.Client.SetDocumentPath("/receipt");
			GA_Manager.Client.SetDocumentTitle("Receipt Page");
			GA_Manager.Client.SetTransactionID("T12345");
			GA_Manager.Client.SetTransactionAffiliation("Google Store - Online");
			GA_Manager.Client.SetTransactionRevenue(37.39f);
			GA_Manager.Client.SetTransactionTax(2.85f);
			GA_Manager.Client.SetTransactionShipping(5.34f);
			GA_Manager.Client.SetTransactionCouponCode("SUMMER2013");
			GA_Manager.Client.SetProductAction("purchase");
			GA_Manager.Client.SetProductSKU(1, "P12345");
			GA_Manager.Client.SetSetProductName(1, "Android Warhol T-Shirt");
			GA_Manager.Client.SetProductCategory(1, "Apparel");
			GA_Manager.Client.SetProductBrand(1, "Google");
			GA_Manager.Client.SetProductVariant(1, "Black");
			GA_Manager.Client.SetProductPosition(1, 1);
			GA_Manager.Client.Send();
			GA_Manager.Client.CreateHit(HitType.EVENT);
			GA_Manager.Client.SetEventCategory("Ecommerce");
			GA_Manager.Client.SetEventAction("Refund");
			GA_Manager.Client.SetNonInteractionFlag();
			GA_Manager.Client.SetTransactionID("T12345");
			GA_Manager.Client.SetProductAction("refund");
			GA_Manager.Client.Send();
			GA_Manager.Client.CreateHit(HitType.EVENT);
			GA_Manager.Client.SetEventCategory("Ecommerce");
			GA_Manager.Client.SetEventAction("Refund");
			GA_Manager.Client.SetNonInteractionFlag();
			GA_Manager.Client.SetTransactionID("T12345");
			GA_Manager.Client.SetProductAction("refund");
			GA_Manager.Client.SetProductSKU(1, "P12345");
			GA_Manager.Client.SetProductQuantity(1, 1);
			GA_Manager.Client.Send();
			GA_Manager.Client.CreateHit(HitType.PAGEVIEW);
			GA_Manager.Client.SetDocumentHostName("mydemo.com");
			GA_Manager.Client.SetDocumentPath("/receipt");
			GA_Manager.Client.SetDocumentTitle("Receipt Page");
			GA_Manager.Client.SetTransactionID("T12345");
			GA_Manager.Client.SetTransactionAffiliation("Google Store - Online");
			GA_Manager.Client.SetTransactionRevenue(37.39f);
			GA_Manager.Client.SetTransactionTax(2.85f);
			GA_Manager.Client.SetTransactionShipping(5.34f);
			GA_Manager.Client.SetTransactionCouponCode("SUMMER2013");
			GA_Manager.Client.SetProductAction("purchase");
			GA_Manager.Client.SetProductSKU(1, "P12345");
			GA_Manager.Client.SetSetProductName(1, "Android Warhol T-Shirt");
			GA_Manager.Client.SetProductCategory(1, "Apparel");
			GA_Manager.Client.SetProductBrand(1, "Google");
			GA_Manager.Client.SetProductVariant(1, "Black");
			GA_Manager.Client.SetProductPrice(1, 29.9f);
			GA_Manager.Client.SetProductQuantity(1, 1);
			GA_Manager.Client.SetCheckoutStep(1);
			GA_Manager.Client.SetCheckoutStepOption("Visa");
			GA_Manager.Client.Send();
			GA_Manager.Client.CreateHit(HitType.EVENT);
			GA_Manager.Client.SetEventCategory("Checkout");
			GA_Manager.Client.SetEventAction("Option");
			GA_Manager.Client.SetProductAction("checkout_option");
			GA_Manager.Client.SetCheckoutStep(2);
			GA_Manager.Client.SetCheckoutStepOption("FedEx");
			GA_Manager.Client.Send();
			GA_Manager.Client.CreateHit(HitType.PAGEVIEW);
			GA_Manager.Client.SetDocumentHostName("mydemo.com");
			GA_Manager.Client.SetDocumentPath("/home");
			GA_Manager.Client.SetDocumentTitle("homepage");
			GA_Manager.Client.SetPromotionID(1, "PROMO_1234");
			GA_Manager.Client.SetPromotionName(1, "Summer Sale");
			GA_Manager.Client.SetPromotionCreative(1, "summer_banner2");
			GA_Manager.Client.SetPromotionPosition(1, "banner_slot1");
			GA_Manager.Client.Send();
			GA_Manager.Client.CreateHit(HitType.EVENT);
			GA_Manager.Client.SetEventCategory("Internal Promotions");
			GA_Manager.Client.SetEventAction("click");
			GA_Manager.Client.SetEventLabel("Summer Sale");
			GA_Manager.Client.SetPromotionAction("click");
			GA_Manager.Client.SetPromotionID(1, "PROMO_1234");
			GA_Manager.Client.SetPromotionName(1, "Summer Sale");
			GA_Manager.Client.SetPromotionCreative(1, "summer_banner2");
			GA_Manager.Client.SetPromotionPosition(1, "banner_slot1");
			GA_Manager.Client.Send();
			GA_Manager.Client.CreateHit(HitType.TRANSACTION);
			GA_Manager.Client.SetTransactionID("12345");
			GA_Manager.Client.SetTransactionAffiliation("westernWear");
			GA_Manager.Client.SetTransactionRevenue(50f);
			GA_Manager.Client.SetTransactionShipping(32f);
			GA_Manager.Client.SetTransactionTax(12f);
			GA_Manager.Client.SetCurrencyCode("EUR");
			GA_Manager.Client.Send();
			GA_Manager.Client.CreateHit(HitType.ITEM);
			GA_Manager.Client.SetTransactionID("12345");
			GA_Manager.Client.SetItemName("sofa");
			GA_Manager.Client.SetItemPrice(300f);
			GA_Manager.Client.SetItemQuantity(2);
			GA_Manager.Client.SetItemCode("u3eqds43");
			GA_Manager.Client.SetItemCategory("furniture");
			GA_Manager.Client.SetCurrencyCode("EUR");
			GA_Manager.Client.Send();
			GA_Manager.Client.CreateHit(HitType.SOCIAL);
			GA_Manager.Client.SetSocialAction("like");
			GA_Manager.Client.SetSocialNetwork("facebook");
			GA_Manager.Client.SetSocialActionTarget("/home  ");
			GA_Manager.Client.Send();
			GA_Manager.Client.CreateHit(HitType.EXCEPTION);
			GA_Manager.Client.SetExceptionDescription("IOException");
			GA_Manager.Client.SetIsFatalException(isFatal: true);
			GA_Manager.Client.Send();
			GA_Manager.Client.CreateHit(HitType.TIMING);
			GA_Manager.Client.SetUserTimingCategory("jsonLoader");
			GA_Manager.Client.SetUserTimingVariableName("load");
			GA_Manager.Client.SetUserTimingTime(5000);
			GA_Manager.Client.SetUserTimingLabel("jQuery");
			GA_Manager.Client.SetDNSTime(100);
			GA_Manager.Client.SetPageDownloadTime(20);
			GA_Manager.Client.SetRedirectResponseTime(32);
			GA_Manager.Client.SetTCPConnectTime(56);
			GA_Manager.Client.SetServerResponseTime(12);
			GA_Manager.Client.Send();
			GA_Manager.Client.CreateHit(HitType.PAGEVIEW);
			GA_Manager.Client.SetDocumentHostName("mydemo.com");
			GA_Manager.Client.SetDocumentPath("/home");
			GA_Manager.Client.SetDocumentTitle("homepage");
			GA_Manager.Client.Send();
			GA_Manager.Client.CreateHit(HitType.PAGEVIEW);
			GA_Manager.Client.AppendData("dh", "mydemo.com");
			GA_Manager.Client.AppendData("dp", "/home");
			GA_Manager.Client.AppendData("dt", "homepage");
			GA_Manager.Client.Send();
		}
	}
}
