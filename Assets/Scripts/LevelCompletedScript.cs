using UnityEngine;
using UnityEngine.UI;

public class LevelCompletedScript : MonoBehaviour
{
	public Text _collectedOrangesText;

	public Text _videoBonusOrangesText;

	public Text _totalOrangesCountText;

	public Text _doubleYourOrangeText;

	public GameObject CommingSoonText;

	public GameObject NextLevelBtn;

	public Button _doubleOrangeBtn;

	public void CheckNextLevelExists(int currentLevel)
	{
		if (DataFunctions.GetIntData(Config.PP_CityLevel_Index + Config.LevelDiffucultyPrefix[DataFunctions.GetIntData(Config.PP_Diffuculty)]) < currentLevel + 1)
		{
			DataFunctions.SaveData(Config.PP_CityLevel_Index + Config.LevelDiffucultyPrefix[DataFunctions.GetIntData(Config.PP_Diffuculty)], currentLevel + 1);
		}
		CommingSoonText.SetActive(value: false);
		NextLevelBtn.SetActive(value: false);
		if (currentLevel + 1 > Config.CityTotalLevel)
		{
			CommingSoonText.SetActive(value: true);
		}
		else
		{
			NextLevelBtn.SetActive(value: true);
		}
	}

	public void FillValues(int collectedOranges, int videoBonusOranges, int totalOranges)
	{
		if (videoBonusOranges == 0)
		{
			_doubleOrangeBtn.interactable = true;
			_doubleOrangeBtn.GetComponent<CanvasGroup>().alpha = 1f;
			_doubleOrangeBtn.transform.GetChild(1).GetComponent<Animator>().enabled = true;
			_doubleOrangeBtn.transform.GetChild(2).GetComponent<Animator>().enabled = true;
		}
		else
		{
			_doubleOrangeBtn.interactable = false;
			_doubleOrangeBtn.GetComponent<CanvasGroup>().alpha = 0.5f;
			_doubleOrangeBtn.transform.GetChild(1).GetComponent<Animator>().enabled = false;
			_doubleOrangeBtn.transform.GetChild(2).GetComponent<Animator>().enabled = false;
		}
		_collectedOrangesText.text = collectedOranges.ToString();
		_videoBonusOrangesText.text = videoBonusOranges.ToString();
		_totalOrangesCountText.text = totalOranges.ToString();
		_doubleYourOrangeText.text = "+" + collectedOranges;
	}
}
