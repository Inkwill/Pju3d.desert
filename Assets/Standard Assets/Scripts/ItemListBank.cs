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

	public void Init(List<Item> items)
	{
		// for (var i = 0; i < _numOfContents; ++i)
		// 	_contents.Add(i + 1);
		_contents = items;
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

