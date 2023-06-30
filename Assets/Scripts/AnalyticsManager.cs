using SA.Analytics.Google;

public class AnalyticsManager
{
	public static void SendEventInfo(string eventMessage)
	{
		eventMessage = ((Config.platform != 0) ? ("N_i_" + eventMessage) : ("N_g_" + eventMessage));
		GA_Manager.Client.SendEventHit(eventMessage, "e", string.Empty, 0);
	}
}
