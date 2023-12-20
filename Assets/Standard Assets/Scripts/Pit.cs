using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class Pit : MonoBehaviour
{
	public float digDeep = 0.1f;
	public float digTime = 10.0f;
	public TMP_Text text;
	public Slider progressSlider;

	public GameObject fxProgress;

	float m_initialDeep;
	bool m_digging;
	bool m_rebacking;
	float m_timer;
	float m_step;
	float m_back_step;
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
		}
	}
	private void Start()
	{
		terrainTool = GetComponent<TerrainTool>();
		m_initialDeep = Terrain.activeTerrain.SampleHeight(transform.position);
		if (text) text.text = m_initialDeep.ToString();
		isDigging = true;
	}

	void Update()
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
			if (text) text.text = Terrain.activeTerrain.SampleHeight(transform.position).ToString();
		}
		else if (m_rebacking)
		{
			m_back_step += Time.deltaTime;
			float cur_deep = Terrain.activeTerrain.SampleHeight(transform.position);
			if (cur_deep < m_initialDeep)
			{
				if (m_back_step >= 1.0f)
				{
					OnRebacking();
					m_back_step = 0;
				}
			}
			else
			{
				m_rebacking = false;
			}
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
}
