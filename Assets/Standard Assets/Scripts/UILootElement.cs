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
	UITargetInfo owner;
	public void Init(UITargetInfo ow, Loot lt)
	{
		owner = ow;
		loot = lt;
		iconItem.sprite = loot.Item.ItemSprite;
		gameObject.SetActive(true);
		//GetComponent<Animator>()?.SetBool("show", true);
	}

	public void GetLoot()
	{
		owner.OnLooted(loot);
		loot.InteractWith(GameManager.Player.Data);
		gameObject.SetActive(false);
	}
}
