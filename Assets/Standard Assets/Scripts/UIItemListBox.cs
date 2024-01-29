using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using AirFishLab.ScrollingList;
using AirFishLab.ScrollingList.ContentManagement;
using CreatorKitCode;

public class UIItemListBox : MonoBehaviour
{
	[SerializeField]
	Image _image;
	[SerializeField]
	Text _num;
	public Item item;

	public void Init(KeyValuePair<string, int> itemInfo)
	{
		item = KeyValueData.GetValue<Item>(GameManager.Data.Item, itemInfo.Key);
		if (item) _image.sprite = item.ItemSprite;
		_num.text = itemInfo.Value.ToString();

	}
	// protected override void UpdateDisplayContent(IListContent content)
	// {
	// 	item = (Item)content;
	// 	if (item)
	// 	{
	// 		_image.sprite = item.ItemSprite;
	// 		_title.text = item.ItemName;
	// 	}
	// }
}
