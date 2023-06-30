using UnityEngine;
using UnityEngine.UI;

public class ImageEffectt : MonoBehaviour
{
	private Image image;

	public Gradient ColorTransition;

	public float speed = 3.5f;

	private void Awake()
	{
		image = GetComponent<Image>();
	}

	private void Update()
	{
		if (image != null && image.gameObject.activeSelf)
		{
			float time = Mathf.PingPong(Time.time * speed, 1f);
			image.color = ColorTransition.Evaluate(time);
		}
	}
}
