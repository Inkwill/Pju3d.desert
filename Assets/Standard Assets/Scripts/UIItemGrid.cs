using UnityEngine;
using CreatorKitCode;
using UnityEngine.UI;


public class UIItemGrid : MonoBehaviour
{
	[SerializeField]
	Sprite icon;
	Image[] iconList;
	public void Init(int maxCount)
	{
		iconList = new Image[maxCount];
		GameObject iconObj = Resources.Load("ui_itemIcon") as GameObject;
		for (int i = 0; i < maxCount; i++)
		{
			iconList[i] = Instantiate(iconObj.GetComponent<Image>(), transform);
			iconList[i].sprite = icon;
			iconList[i].gameObject.SetActive(false);
		}
	}
	public void ShowItem(int Count)
	{
		for (int i = 0; i < iconList.Length; i++)
		{
			if (i < Count) iconList[i].gameObject.SetActive(true);
			else iconList[i].gameObject.SetActive(false);
		}
	}

}
