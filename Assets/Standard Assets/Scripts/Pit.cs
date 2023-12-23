using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class Pit : MonoBehaviour
{
	public float digDeep = 0.1f;
	public float digTime = 10.0f;

	public Text text_Title;
	public Text text_Info;
	public Text text_help;
	public Slider progressSlider;

	public GameObject fxProgress;

	float m_initialDeep;
	bool m_digging;
	bool m_rebacking;
	float m_timer;
	float m_step;
	float m_back_step;

	float m_back_stepTime = 1.0f;
	TerrainTool terrainTool;

	public bool isDigging
	{
		get { return m_digging; }
		set
		{
			m_digging = value;
			m_rebacking = !value;
			if (progressSlider) progressSlider.gameObject.SetActive(value);
			if (fxProgress) fxProgress.SetActive(value);
			if (value) text_Title.text = "挖个坑...";
			else text_Title.text = "坑会消失...";
		}
	}
	private void Start()
	{
		terrainTool = GetComponent<TerrainTool>();
		m_initialDeep = Terrain.activeTerrain.SampleHeight(transform.position);
		text_Info.text = m_initialDeep.ToString();
		isDigging = true;
	}

	public void Fill(float speed)
	{
		Collider box = GetComponent<Collider>();
		if (box) box.enabled = false;
		m_back_stepTime = m_back_stepTime / speed;
	}
	void FixedUpdate()
	{
		if (m_digging)
		{
			m_timer += Time.deltaTime;
			m_step += Time.deltaTime;
			if (digTime <= m_timer)
			{
				m_timer = 0f;
				isDigging = false;
				GetComponent<EventSender>()?.Send(gameObject, "event_pit_completed");
				return;
			}
			else if (m_step >= 1.0f)
			{
				OnProcessing();
				m_step = 0f;
			}
			if (progressSlider) progressSlider.value = m_timer / digTime;
			text_help.text = "";
			text_Info.text = "deep:" + (10 - Terrain.activeTerrain.SampleHeight(transform.position)).ToString("f2");
		}
		else if (m_rebacking)
		{
			m_back_step += Time.deltaTime;
			float cur_deep = Terrain.activeTerrain.SampleHeight(transform.position);
			if (cur_deep < m_initialDeep)
			{
				if (m_back_step >= m_back_stepTime)
				{
					OnRebacking();
					m_back_step = 0;
				}
			}
			else
			{
				m_rebacking = false;
				Destroy(gameObject, 1.0f);
			}
			text_help.text = "点击按钮建造";
			text_Info.text = "deep:" + (10 - Terrain.activeTerrain.SampleHeight(transform.position)).ToString("f2");
		}
	}
	void OnProcessing()
	{
		terrainTool.LowerTerrain(transform.position, digDeep * 0.01f, 8, 8);
	}

	void OnRebacking()
	{
		terrainTool.RaiseTerrain(transform.position, 0.01f * 0.01f, 8, 8);
	}

	void OnDisable()
	{
		terrainTool.SetTerrainHeight(transform.position, m_initialDeep * 0.01f, 8, 8);
	}

	public void OnPlanted(Creater creater)
	{
		m_rebacking = false;
		text_help.text = "建造中..";
		text_Title.text = "风车塔";
		text_Info.text = "法术攻击: 5";
		Destroy(gameObject, creater.interval);
	}
}
