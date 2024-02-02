using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CreatorKitCode;

public class UIItemListBox : MonoBehaviour
{
	[SerializeField]
	Image _image;
	[SerializeField]
	Text _num;

	public void UpdateInfo(KeyValuePair<string, int> itemInfo, int leftNum)
	{
		Item item = KeyValueData.GetValue<Item>(GameManager.Data.Item, itemInfo.Key);
		if (item) _image.sprite = item.ItemSprite;
		_num.text = $"{itemInfo.Value - leftNum}/ {itemInfo.Value}";
		_num.color = leftNum < 1 ? Color.green : Color.black;
	}

}
