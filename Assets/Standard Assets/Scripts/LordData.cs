using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class LordData : MonoBehaviour
{
	public int Lv => m_level;
	int m_level;
	int m_maxLv = 99;
	public int Exp => m_exp;
	public float ExpPercent => 1.0f * m_exp / expTemplate[m_level - 1];
	int m_exp;
	public int Money => m_money;
	int m_money;
	[SerializeField]
	int[] expTemplate;

	public void Init()
	{
		m_level = 1;
		m_exp = 0;
	}

	public void AddExp(int exp)
	{
		m_exp += exp;
		while (m_exp >= expTemplate[m_level - 1])
		{
			UpdateLv();
		}
	}

	public void AddMoney(int money)
	{
		m_money += money;
	}

	void UpdateLv()
	{
		m_exp -= expTemplate[m_level - 1];
		m_level++;
	}
}
