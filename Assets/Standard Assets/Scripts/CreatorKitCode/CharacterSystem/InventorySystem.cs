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
			public int capacity;
			public float Volume { get { return count * 1.0f / capacity; } }

			public ResInventory(ResInventoryItem item)
			{
				data = item;
				capacity = item.capacity;
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
					submits[left.Key] = Math.Min(left.Value, inventory.ItemCount(left.Key));
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
					Item item = KeyValueData.GetValue<Item>(GameManager.Config.Item, submit.Key);
					ResItem resitem = item as ResItem;
					if (resitem && resitem.requireContainer && submit.Value > 0)
					{
						DemandLeft[submit.Key] -= submit.Value;
						inventory.ResInventories[resitem.resType].count -= submit.Value;
						inventory.ItemAction?.Invoke(resitem, "Fulfill", submit.Value);
					}
					else
					{
						var entryIds = inventory.EntryID(submit.Key);
						if (entryIds.Count > 0)
						{
							foreach (int id in entryIds)
							{
								int count = Math.Min(submit.Value, inventory.Entries[id].Count);
								DemandLeft[submit.Key] -= count;
								inventory.MinusItem(id, count);
								if (DemandLeft[submit.Key] == 0) break;
							}
							inventory.ItemAction?.Invoke(item, "Fulfill", submit.Value);
						}
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
		public Dictionary<ResItem.ResType, ResInventory> ResInventories;
		Character m_Owner;
		public int SlotsNum { get { return m_baseSlots; } }
		public int CurSlotsNum { get { return Entries.Where(en => en != null).Count(); } }
		static int m_baseSlots = 10;

		public void Init(Character owner)
		{
			m_Owner = owner;
			ResInventories = new Dictionary<ResItem.ResType, ResInventory>();
		}

		public List<int> EntryID(string itemKey)
		{
			List<int> result = new List<int>();
			for (int i = 0; i < SlotsNum; ++i)
			{
				if (Entries[i] != null)
				{
					if (Entries[i].Item.ItemName == itemKey) result.Add(i);
				}
			}
			return result;
		}

		public int ItemCount(string ItemName)
		{
			Item item = KeyValueData.GetValue<Item>(GameManager.Config.Item, ItemName);
			ResItem resitem = item as ResItem;
			int count = 0;
			if (resitem && resitem.requireContainer)
			{
				if (!ResInventories.ContainsKey(resitem.resType)) return 0;
				else return ResInventories[resitem.resType].count;
			}

			for (int i = 0; i < SlotsNum; ++i)
			{
				if (Entries[i] != null)
				{
					if (Entries[i].Item.ItemName == ItemName) count += Entries[i].Count;
				}
			}
			return count;
		}
		public int ItemCount<T>()
		{
			int count = 0;
			for (int i = 0; i < SlotsNum; ++i)
			{
				if (Entries[i] != null)
				{
					if (Entries[i].Item is T) count += Entries[i].Count;
				}
			}
			return count;
		}

		/// <summary>
		/// Add an item to the inventory. This will look if this item already exist in one of the slot and increment the
		/// stack counter there instead of using another slot.
		/// </summary>
		/// <param name="item">The item to add to the inventory</param>
		public bool AddItem(Item item, int num = 1)
		{
			ResItem resi = item as ResItem;
			if (resi && resi.requireContainer) return AddResItemToContainer(resi, num);
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
			if (entry != null && item.onlyOne)
			{
				ItemAction?.Invoke(item, "DuplicateOnlyOne", num);
				return false;
			}
			if (entry != null && item.stackNum >= (entry.Count + num)) { entry.Count += num; return true; }
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
				ItemAction?.Invoke(item, "NotEnoughSpace", num);
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
		public int MinusItem(string itemKey, int num = 1)
		{
			int count = num;
			var entryIds = EntryID(itemKey);
			foreach (var id in entryIds)
			{
				count -= MinusItem(Entries[id], count);
				if (count == 0) break;
			}
			return num - count;
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
			MinusItem(equip.ItemName);
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
			if (ResInventories.ContainsKey(item.resType)) ResInventories[item.resType].capacity += resInvent.capacity;
			else ResInventories.Add(item.resType, resInvent);
			ItemAction?.Invoke(item, "AddResInventory", 1);
		}

		void RemoveResInventory(ResInventoryItem item)
		{
			var resInventory = ResInventories[item.resType];
			if (resInventory != null)
			{
				ResInventories.Remove(item.resType);
				GameObject lootObj = Resources.Load("Loot") as GameObject;
				for (int i = 0; i < resInventory.count; i++)
				{
					Loot loot = GameObject.Instantiate(lootObj, m_Owner.transform.position, Quaternion.Euler(0, 0, 0)).GetComponent<Loot>();
					loot.Item = item;
				}
				ItemAction?.Invoke(item, "RemoveResInventory", 1);
			}
		}

		bool AddResItemToContainer(ResItem item, int num)
		{
			bool added = false;
			if (ResInventories.ContainsKey(item.resType))
			{
				var resInventory = ResInventories[item.resType];
				added = resInventory.count + num <= resInventory.capacity;
				if (added) { resInventory.count += num; ItemAction?.Invoke(item, "Add", num); }
				else ItemAction?.Invoke(item, "NotEnoughSpace", num);
			}
			return added;
		}
	}
}