using UnityEngine;
using UnityEngine.UI;

public class GetPluginVersionScript : MonoBehaviour
{
	private GameObject Text;

	private void Start()
	{
		Text = GameObject.Find("PluginVersionText");
		Text.GetComponent<Text>().text = IronSource.pluginVersion();
	}

	private void Update()
	{
	}
}
