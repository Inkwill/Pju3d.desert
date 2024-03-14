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

	public void SetDemandInfo(KeyValuePair<string, int> itemInfo, int leftNum)
	{
		Item item = KeyValueData.GetValue<Item>(GameManager.Config.Item, itemInfo.Key);
		if (item) _image.sprite = item.ItemSprite;
		_num.text = $"{itemInfo.Value - leftNum}/ {itemInfo.Value}";
		_num.color = leftNum < 1 ? Color.green : Color.black;
		gameObject.SetActive(true);
	}

	public void SetRequiredInfo(Item item, int requireCount)
	{
		_image.sprite = item.ItemSprite;
		int owned = GameManager.CurHero.Inventory.ItemCount(item.ItemName);
		_num.text = $"{owned}/ {requireCount}";
		_num.color = owned >= requireCount ? Color.green : Color.red;
		gameObject.SetActive(true);
	}

	public void SetRequiredInfo(string itemKey, int requireCount)
	{
		Item it = KeyValueData.GetValue<Item>(GameManager.Config.Item, itemKey);
		SetRequiredInfo(it, requireCount);
	}
}
