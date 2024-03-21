using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UILevelWindow : MonoBehaviour
{
	public Text mainGoalInfo;
	public Text monsterInfo;
	public Text countDown;
	public int CurWave { get { return m_timer.MaxTimes - m_timer.LeftTimes; } }
	Timer m_timer;
	List<SpawnerSample> m_spawners;
	List<Character> m_enemies;

	void Awake()
	{
		m_timer = GetComponent<Timer>();
		m_spawners = GameManager.Instance.spawners;
		m_enemies = new List<Character>();
	}
	void Start()
	{
		mainGoalInfo.text = "";
		countDown.text = "";
		m_timer.behaveAction += StartWave;
		m_timer.SetTimer(m_spawners.Count);
		m_timer.autoStart = true;
		m_timer.processAction += (max, passed) =>
		{
			if (passed < max)
				countDown.text = $"{max - passed:F0}s后开始下一波进攻";
			Debug.Log("processing.....");
		};
	}
	void StartWave()
	{
		if (m_spawners.Count > CurWave - 1)
		{
			m_spawners[CurWave - 1].spawnEvent += AddMonster;
			m_spawners[CurWave - 1].StartSpawn();
		}
		mainGoalInfo.text = $"抵挡敌人的进攻:{CurWave}/{m_timer.MaxTimes}";
		countDown.text = $"";
		Debug.Log("StartWave.....");
	}

	void AddMonster(Character enemy)
	{
		m_enemies.Add(enemy);
		enemy.DeathEvent.AddListener(character => { m_enemies.Remove(character); RefreshMonsterInfo(); });
		RefreshMonsterInfo();
	}

	void RefreshMonsterInfo()
	{
		monsterInfo.text = $"当前怪物:{m_enemies.Count}";
	}

	void Update()
	{
		//countDown.text = $"{m_timer.CurCd:F0}s后开始下一波进攻";
	}

}
