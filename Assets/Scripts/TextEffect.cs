using UnityEngine;
using UnityEngine.UI;

public class TextEffect : MonoBehaviour
{
	private Text text;

	public Gradient ColorTransition;

	public float speed = 3.5f;

	private void Start()
	{
		text = GetComponent<Text>();
	}

	private void Update()
	{
		if (text != null && text.gameObject.activeSelf)
		{
			float time = Mathf.PingPong(Time.time * speed, 1f);
			text.color = ColorTransition.Evaluate(time);
		}
	}
}
