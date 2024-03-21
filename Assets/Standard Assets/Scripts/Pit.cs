using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using MyBox;


public class Pit : MonoBehaviour
{
	public float survival_time;
	float m_initHeight;
	bool m_actived;
	float m_step = 0.1f;
	Timer m_timer;

	void Start()
	{
		m_initHeight = Terrain.activeTerrain.SampleHeight(transform.position);
		m_actived = true;
		m_timer = Timer.SetTimer(gameObject, survival_time);
		m_timer.processAction += OnProcessing;
		m_timer.behaveAction += () => Destroy(gameObject);
		m_timer.StartTimer();
	}

	void OnProcessing(float max, float passed)
	{
		float completed = passed / max;
		if (completed - m_step >= 0)
		{
			GameManager.Instance.TerrainTool.RaiseTerrain(transform.position, 0.03f * 0.01f, 5, 5);
			m_step += 0.1f;
		}
		if (m_actived && completed > 0.7f)
		{
			m_actived = false;
		}
	}

	void OnDestroy()
	{
		GameManager.Instance.TerrainTool.SetTerrainHeight(transform.position, (m_initHeight + 0.45f) * 0.01f, 5, 5);
	}
}
