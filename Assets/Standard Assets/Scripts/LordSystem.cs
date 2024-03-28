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
		public List<TroopSystem.Team> Teams;
		public TroopSystem.Team rpgTeam;
		public InventorySystem Inventory;
		int m_exp;
		int[] m_expTemlate;
		public Lord(int[] expTemplate)
		{
			m_level = 1;
			m_exp = 0;
			m_expTemlate = expTemplate;
			Teams = new List<TroopSystem.Team>();
			Inventory = new InventorySystem();
		}

		public TroopSystem.Team AddTeam(Character hero)
		{
			var Team = new TroopSystem.Team();
			Team.leader = hero;
			Teams.Add(Team);
			return Team;
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

		public void SetRpgTeam(TroopSystem.Team Team)
		{
			rpgTeam = Team;
			Inventory.BindToCharacter(rpgTeam.leader);
		}
	}
}
