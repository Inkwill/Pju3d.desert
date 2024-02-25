using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CreatorKitCode;
using CreatorKitCodeInternal;


public class UILootElement : MonoBehaviour
{
	[SerializeField]
	Image iconItem;
	public Loot loot;
	//UITargetInfo owner;
	public void Init(Loot lt)
	{
		loot = lt;
		iconItem.sprite = loot.Item.ItemSprite;
		gameObject.SetActive(true);
	}
	public void Pick()
	{
		if (loot)
		{
			loot.InteractWith(GameManager.CurHero);
			loot = null;
		}
		gameObject.SetActive(false);
	}
}
