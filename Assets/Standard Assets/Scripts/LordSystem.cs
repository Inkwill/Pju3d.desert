using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class LordSystem
{
	public class Lord
	{
		public int Lv => m_level;
		int m_level;
		public int Exp => m_exp;
		public float ExpPercent => 1.0f * m_exp / m_expTemlate[m_level - 1];
		public List<TroopSystem.Troop> troops;
		public TroopSystem.Troop rpgTroop;
		public InventorySystem Inventory;
		int m_exp;
		int[] m_expTemlate;
		public Lord(int[] expTemplate)
		{
			m_level = 1;
			m_exp = 0;
			m_expTemlate = expTemplate;
			troops = new List<TroopSystem.Troop>();
			Inventory = new InventorySystem();
		}

		public TroopSystem.Troop AddTroop(Character hero)
		{
			var troop = new TroopSystem.Troop();
			troop.leader = hero;
			troops.Add(troop);
			return troop;
		}

		public void AddExp(int exp)
		{
			m_exp += exp;
			while (m_exp >= m_expTemlate[m_level - 1])
			{
				m_exp -= m_expTemlate[m_level - 1];
				m_level++;
			}
		}

		public void SetRpgTroop(TroopSystem.Troop troop)
		{
			rpgTroop = troop;
			Inventory.BindToCharacter(rpgTroop.leader);
		}
	}
}
