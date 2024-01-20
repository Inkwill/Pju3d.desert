using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using AirFishLab.ScrollingList;
using AirFishLab.ScrollingList.ContentManagement;
using CreatorKitCode;

public class UIItemListBox : ListBox
{
	[SerializeField]
	Image _image;
	[SerializeField]
	Text _title;
	public Item item;

	protected override void UpdateDisplayContent(IListContent content)
	{
		item = (Item)content;
		if (item)
		{
			_image.sprite = item.ItemSprite;
			_title.text = item.ItemName;
		}
	}
	// public void OnClick()
	// {
	// 	Debug.Log("OnClick:" + m_weapon);
	// }
}
