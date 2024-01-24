using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CreatorKitCode;
using AirFishLab.ScrollingList;
using AirFishLab.ScrollingList.ContentManagement;

public class ItemListBank : BaseListBank
{
	[SerializeField]
	private List<Item> _contents = new List<Item>();

	public void Init(CharacterData data)
	{
		_contents.Clear();
		if (data.Equipment.Weapon) _contents.Add(data.Equipment.Weapon);
		if (data.Equipment.ViceWeapon) _contents.Add(data.Equipment.ViceWeapon);
	}

	public override IListContent GetListContent(int index)
	{
		return _contents[index];
	}

	public override int GetContentCount()
	{
		return _contents.Count;
	}

}

