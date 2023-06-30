using UnityEngine;
using UnityEngine.UI;

public class GameOverScript : MonoBehaviour
{
	private string[] GetupMessages = new string[7]
	{
		"Stand Up And FIGHT!",
		"Giving Up That Easy?",
		"HEY! Gang Is Still There!",
		"Can't Reach To BOSS?",
		"Get Up! You Have More To Fight.",
		"Hey! Give Yourself Up To This!",
		"Hey! Beat Them. They Are Not Real!"
	};

	private string[] GetupMessagesEarly = new string[4]
	{
		"Wow! That Was Quick!",
		"Is That All You Got!",
		"Do You Know You Can Punch?",
		"Why Don't You Kick Next Time!"
	};

	public Text _getupMessageText;

	public Text _levelCompletedPercentText;

	public Text _collectedOrangesText;

	public Text _totalOrangesCountText;

	public Button _reviveForOrangeBtn;

	public Text _reviveForOrangeCountText;

	public GameObject _leftSide;

	public RectTransform _rightSide;

	public void FillValues(float percent, int collectedOranges, int totalOranges, bool secondChanceUsed, int reviveForOrangeCount)
	{
		if (percent > 15f)
		{
			_getupMessageText.text = GetupMessages[Random.Range(0, GetupMessages.Length)];
		}
		else
		{
			_getupMessageText.text = GetupMessagesEarly[Random.Range(0, GetupMessagesEarly.Length)];
		}
		_levelCompletedPercentText.text = "% " + percent.ToString("f1");
		_collectedOrangesText.text = collectedOranges.ToString();
		_totalOrangesCountText.text = totalOranges.ToString();
		if (secondChanceUsed)
		{
			_leftSide.SetActive(value: false);
			_rightSide.anchoredPosition = new Vector2(0f, 0f);
			return;
		}
		_leftSide.SetActive(value: true);
		_rightSide.anchoredPosition = new Vector2(440f, 0f);
		_reviveForOrangeCountText.text = $"Revive for\n{reviveForOrangeCount} Oranges";
		if (totalOranges < reviveForOrangeCount)
		{
			_reviveForOrangeBtn.interactable = false;
			_reviveForOrangeBtn.GetComponent<CanvasGroup>().alpha = 0.5f;
			_reviveForOrangeBtn.transform.GetChild(0).GetComponent<Animator>().enabled = false;
		}
		else
		{
			_reviveForOrangeBtn.interactable = true;
			_reviveForOrangeBtn.GetComponent<CanvasGroup>().alpha = 1f;
			_reviveForOrangeBtn.transform.GetChild(0).GetComponent<Animator>().enabled = true;
		}
	}
}
