using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

namespace CreatorKitCode
{
	/// <summary>
	/// This handles the inventory of our character. The inventory has a maximum of 32 slot, each slot can hold one
	/// TYPE of object, but those can be stacked without limit (e.g. 1 slot used by health potions, but contains 20
	/// health potions)
	/// </summary>
	public class InventorySystem
	{
		/// <summary>
		/// One entry in the inventory. Hold the type of Item and how many there is in that slot.
		/// </summary>
		public class InventoryEntry
		{
			public int Count;
			public Item Item;
		}

		public class ResInventory
		{
			public ResInventoryItem data;
			public int count;

			public float Volume { get { return count * 1.0f / data.capacity; } }

			public ResInventory(ResInventoryItem item)
			{
				data = item;
			}
		}

		public class ItemDemand
		{
			public ItemDemand(Dictionary<string, int> demand)
			{
				Demand = demand;
				DemandLeft = new Dictionary<string, int>(demand);
			}

			public bool Completed
			{
				get { return DemandLeft.Values.Sum() < 1; }
				set { }
			}
			public Dictionary<string, int> Demand;
			public Dictionary<string, int> DemandLeft;
			public Dictionary<string, int> Submittable(InventorySystem inventory)
			{
				var submits = new Dictionary<string, int>();
				foreach (var left in DemandLeft)
				{
					if (left.Key == ResItem.GetResItemByType(ResItem.ResType.Water).ItemName && left.Value > 0)
					{
						submits[left.Key] = Math.Min(left.Value, inventory.GetResInventory(ResItem.ResType.Water).count);
					}
					else if (left.Key == ResItem.GetResItemByType(ResItem.ResType.Wood).ItemName && left.Value > 0)
					{
						submits[left.Key] = Math.Min(left.Value, inventory.GetResInventory(ResItem.ResType.Wood).count);
					}
					else
					{
						int id = inventory.EntryID(left.Key);
						if (id != -1 && left.Value > 0)
						{
							submits[left.Key] = Math.Min(left.Value, inventory.Entries[id].Count);
						}
					}
				}
				return submits;
			}

			public bool Fulfill(InventorySystem inventory)
			{
				if (Completed) { Debug.LogError("ItemDemand had completed! : " + Helpers.DictionaryToString(DemandLeft)); return false; }
				var submits = Submittable(inventory);
				if (submits.Count < 1) return false;
				foreach (var submit in submits)
				{
					if (submit.Key == ResItem.GetResItemByType(ResItem.ResType.Water).ItemName && submit.Value > 0)
					{
						DemandLeft[submit.Key] -= submit.Value;
						inventory.GetResInventory(ResItem.ResType.Water).count -= submit.Value;
						inventory.ItemAction?.Invoke(ResItem.GetResItemByType(ResItem.ResType.Water), "Fulfill", submit.Value);
					}
					else if (submit.Key == ResItem.GetResItemByType(ResItem.ResType.Wood).ItemName && submit.Value > 0)
					{
						DemandLeft[submit.Key] -= submit.Value;
						inventory.GetResInventory(ResItem.ResType.Wood).count -= submit.Value;
						inventory.ItemAction?.Invoke(ResItem.GetResItemByType(ResItem.ResType.Wood), "Fulfill", submit.Value);
					}
					else
					{
						int entryId = inventory.EntryID(submit.Key);
						DemandLeft[submit.Key] -= submit.Value;
						inventory.ItemAction?.Invoke(inventory.Entries[entryId].Item, "Fulfill", submit.Value);
						inventory.MinusItem(entryId, submit.Value);
					}

				}
				return true;
			}

			public float GetProgress(string itemKey)
			{
				float result = 0;
				if (Demand.Keys.Contains(itemKey)) result = 1.0f - 1.0f * DemandLeft[itemKey] / Demand[itemKey];
				return result;
			}
		}

		//Only 32 slots in inventory
		public InventoryEntry[] Entries = new InventoryEntry[m_baseSlots];
		public Action<Item, string, int> ItemAction;
		public List<ResInventory> ResInventories;
		CharacterData m_Owner;
		public int SlotsNum { get { return m_baseSlots; } }
		public int CurSlotsNum { get { return Entries.Where(en => en != null).Count(); } }
		static int m_baseSlots = 8;

		public void Init(CharacterData owner)
		{
			m_Owner = owner;
			ResInventories = new List<ResInventory>();
		}

		public int EntryID(string itemKey)
		{
			for (int i = 0; i < SlotsNum; ++i)
			{
				if (Entries[i] != null)
				{
					if (Entries[i].Item.ItemName == itemKey) return i;
				}
			}
			return -1;
		}

		public int ItemCount(string ItemName)
		{
			for (int i = 0; i < SlotsNum; ++i)
			{
				if (Entries[i] != null)
				{
					if (Entries[i].Item.ItemName == ItemName) return Entries[i].Count;
				}
			}
			return -1;
		}
		/// <summary>
		/// Add an item to the inventory. This will look if this item already exist in one of the slot and increment the
		/// stack counter there instead of using another slot.
		/// </summary>
		/// <param name="item">The item to add to the inventory</param>
		public bool AddItem(Item item, int num = 1)
		{
			if (item is ResItem) return AddResItem(item as ResItem, num);
			if (AutoUseItem(item, num)) return true;
			else if (PutItem(item, num))
			{
				ItemAction?.Invoke(item, "Add", num);
				if (item is ResInventoryItem) AddResInventory(item as ResInventoryItem);
				return true;
			}
			return false;
			// bool found = false;
			// int firstEmpty = -1;
			// for (int i = 0; i < SlotsNum; ++i)
			// {
			// 	if (Entries[i] == null)
			// 	{
			// 		if (firstEmpty == -1)
			// 			firstEmpty = i;
			// 	}
			// 	else if (Entries[i].Item == item)
			// 	{
			// 		Entries[i].Count += num;
			// 		found = true;
			// 		ItemAction?.Invoke(item, "Add", num);
			// 	}
			// }

			// if (!found && firstEmpty != -1)
			// {
			// 	InventoryEntry entry = new InventoryEntry();
			// 	entry.Item = item;
			// 	entry.Count = num;

			// 	Entries[firstEmpty] = entry;
			// 	ItemAction?.Invoke(item, "Add", num);
			// }
		}

		bool PutItem(Item item, int num = 1)
		{
			var entry = Entries.Where(en => (en != null && en.Item.ItemName == item.ItemName)).FirstOrDefault();
			if (entry != null && item is ResInventoryItem)
			{
				ItemAction?.Invoke(item, "DuplicateContainer", num);
				return false;
			}
			if (entry != null) { entry.Count += num; return true; }
			else
			{
				for (int i = 0; i < SlotsNum; ++i)
				{
					if (Entries[i] == null)
					{
						entry = new InventoryEntry();
						entry.Item = item;
						entry.Count = num;
						Entries[i] = entry;
						return true;
					}
				}
				ItemAction?.Invoke(item, "Full", num);
				return false;
			}
		}

		public void RemoveItem(int InventoryID)
		{
			for (int i = 0; i < SlotsNum; ++i)
			{
				if (i == InventoryID)
				{
					ItemAction?.Invoke(Entries[i].Item, "Remove", Entries[i].Count);
					if (Entries[i].Item is ResInventoryItem) RemoveResInventory(Entries[i].Item as ResInventoryItem);
					Entries[i] = null;
					break;
				}
			}
		}

		public int MinusItem(int InventoryID, int num = 1)
		{
			return MinusItem(Entries[InventoryID], num);
		}
		public int MinusItem(InventoryEntry minusEntry, int num = 1)
		{
			int minusNum = 0;
			if (minusEntry == null) return minusNum;
			for (int i = 0; i < SlotsNum; ++i)
			{
				if (Entries[i] == minusEntry)
				{
					minusNum = Math.Min(num, Entries[i].Count);
					Entries[i].Count -= minusNum;
					if (minusNum > 0) ItemAction?.Invoke(Entries[i].Item, "Minus", minusNum);
					if (Entries[i].Count < 1)
					{
						RemoveItem(i);
					}
					break;
				}
			}
			return minusNum;
		}

		/// <summary>
		/// This will *try* to use the item. If the item return true when used, this will decrement the stack count and
		/// if the stack count reach 0 this will free the slot. If it return false, it will just ignore that call.
		/// (e.g. a potion will return false if the user is at full health, not consuming the potion in that case)
		/// </summary>
		/// <param name="item"></param>
		/// <returns></returns>
		public bool UseItem(InventoryEntry item)
		{
			//true mean it get consumed and so would be removed from inventory.
			//(note "consumed" is a loose sense here, e.g. armor get consumed to be removed from inventory and added to
			//equipement by their subclass, and de-equiping will re-add the equipement to the inventory 
			if (item.Item.UsedBy(m_Owner))
			{
				item.Count -= 1;

				if (item.Count <= 0)
				{
					//maybe store the index in the InventoryEntry to avoid having to find it again here
					for (int i = 0; i < SlotsNum; ++i)
					{
						if (Entries[i] == item)
						{
							Entries[i] = null;
							break;
						}
					}
				}
				ItemAction?.Invoke(item.Item, "Use", 1);
				return true;
			}

			return false;
		}

		bool AutoUseItem(Item item, int count)
		{
			var useable = item as UsableItem;
			if (useable && useable.autoUse)
			{
				ItemAction?.Invoke(item, "Add", count);
				return item.UsedBy(m_Owner, count);
			}
			return false;
		}

		public void EquipItem(EquipmentItem equip)
		{
			Weapon wp = equip as Weapon;
			if (wp) m_Owner.Equipment.EquipWeapon(wp);
			else m_Owner.Equipment.Equip(equip);
			RemoveItem(EntryID(equip.ItemName));
			//SFXManager.PlayClip("equiped");
		}

		public void UnEquipItem(EquipmentItem equip)
		{
			Weapon wp = equip as Weapon;
			if (wp == m_Owner.DefaultWeapon) m_Owner.Equipment.UnWeapon(wp);
			else if (wp != null && PutItem(wp)) m_Owner.Equipment.UnWeapon(wp);
			else if (wp == null) m_Owner.Equipment.Unequip(equip.Slot);
			// RemoveItem(EntryID(equip.ItemName));
			// SFXManager.PlayClip("equiped");
		}

		void AddResInventory(ResInventoryItem item)
		{
			ResInventory resInvent = new ResInventory(item);
			ResInventories.Add(resInvent);
			ItemAction?.Invoke(item, "AddResInventory", 1);
		}

		void RemoveResInventory(ResInventoryItem item)
		{
			var resInventory = GetResInventory(item.Type);
			if (resInventory != null)
			{
				ResInventories.Remove(resInventory);
				GameObject lootObj = Resources.Load("Loot") as GameObject;
				for (int i = 0; i < resInventory.count; i++)
				{
					Loot loot = GameObject.Instantiate(lootObj, m_Owner.transform.position, Quaternion.Euler(0, 0, 0)).GetComponent<Loot>();
					loot.Item = item;
				}
				ItemAction?.Invoke(item, "RemoveResInventory", 1);
			}
		}

		bool AddResItem(ResItem item, int num)
		{
			bool added = false;
			var resInventory = GetResInventory(item.Type);
			if (resInventory == null) added = false;
			else added = resInventory.count < resInventory.data.capacity;
			if (!added) ItemAction?.Invoke(item, "NotEnoughSpace", num);
			else
			{
				resInventory.count += num;
				ItemAction?.Invoke(item, "AddRes", num);
			}
			return added;
		}

		bool MinusResItem(ResItem item, int num)
		{
			bool minused = false;
			var resInventory = GetResInventory(item.Type);
			if (resInventory == null) minused = false;
			else minused = resInventory.count >= num;
			if (!minused) ItemAction?.Invoke(item, "NotEnoughRes", num);
			else
			{
				resInventory.count -= num;
				ItemAction?.Invoke(item, "MinusRes", num);
			}
			return minused;
		}

		public ResInventory GetResInventory(ResItem.ResType type)
		{
			return ResInventories.Where(resin => resin.data.Type == type).FirstOrDefault();
		}
	}
}