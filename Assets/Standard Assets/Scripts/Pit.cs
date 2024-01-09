using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class Pit : Creater
{

	public Text text_Title;
	public Text text_Info;
	public Text text_help;



	public float curDeep
	{
		get { return 10 - Terrain.activeTerrain.SampleHeight(transform.position); }
		set
		{
			terrainTool.SetTerrainHeight(transform.position, (10 - value) * 0.01f, 8, 8);
		}
	}
	protected override void OnProcessing(float completed)
	{
		curDeep -= 0.01f;
		if (showObj) showObj.transform.Translate(new Vector3(0, 0.5f, 0));
	}
	protected override void OnInterval()
	{
		if (curDeep <= 0)
		{
			Destroy(gameObject, interval / 2.0f);
			curDeep = 0;
		}
		else
		{
			curDeep -= 0.01f;

		}
		// terrainTool.RaiseTerrain(transform.position, 0.01f * 0.01f, 8, 8);
		// float curDeep = 10 - Terrain.activeTerrain.SampleHeight(transform.position);
		text_Info.text = "deep:" + curDeep.ToString("f2");

	}
	//{
	// if (m_digging)
	// {
	// 	m_timer += Time.deltaTime;
	// 	m_step += Time.deltaTime;
	// 	if (digTime <= m_timer)
	// 	{
	// 		m_timer = 0f;
	// 		isDigging = false;
	// 		GetComponent<EventSender>()?.Send(gameObject, "event_work_completed");
	// 		return;
	// 	}
	// 	else if (m_step >= 1.0f)
	// 	{
	// 		OnProcessing();
	// 		m_step = 0f;
	// 	}
	// 	if (progressSlider) progressSlider.value = m_timer / digTime;
	// 	text_help.text = "";
	// 	
	// }
	// else if (m_rebacking)
	// {
	// 	m_back_step += Time.deltaTime;
	// 	float cur_deep = Terrain.activeTerrain.SampleHeight(transform.position);
	// 	if (cur_deep < m_initialDeep)
	// 	{
	// 		if (m_back_step >= m_back_stepTime)
	// 		{
	// 			OnRebacking();
	// 			m_back_step = 0;
	// 		}
	// 	}
	// 	else
	// 	{
	// 		m_rebacking = false;
	// 		Destroy(gameObject, 1.0f);
	// 	}
	// 	text_help.text = "点击按钮建造";
	// 	text_Info.text = "deep:" + (10 - Terrain.activeTerrain.SampleHeight(transform.position)).ToString("f2");
	// }
	//}
	// void OnProcessing()
	// {
	// 	terrainTool.LowerTerrain(transform.position, digDeep * 0.01f, 8, 8);
	// }

	// void OnRebacking()
	// {
	// 	terrainTool.RaiseTerrain(transform.position, 0.01f * 0.01f, 8, 8);
	// }

	void OnDisable()
	{
		curDeep = 0;
	}

	// public void OnPlanted(Creater creater)
	// {
	// 	m_rebacking = false;
	// 	text_help.text = "建造中..";
	// 	text_Title.text = "风车塔";
	// 	text_Info.text = "法术攻击: 5";
	// 	Destroy(gameObject, creater.interval);
	// }
}
