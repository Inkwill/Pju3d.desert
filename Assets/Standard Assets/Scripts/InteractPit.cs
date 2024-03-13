using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using MyBox;


public class InteractPit : TimerBehaviour
{
	[SerializeField]
	GameObject uiPlaceRoot;
	[SerializeField]
	Button btPlace;
	float m_initHeight;
	bool m_actived;
	float m_step = 0.1f;

	protected override void OnStart()
	{
		base.OnStart();
		m_initHeight = Terrain.activeTerrain.SampleHeight(transform.position);
		m_actived = true;
		uiPlaceRoot.SetActive(false);
		btPlace.interactable = true;
		GetComponentInChildren<UIWorldHud>()?.ShowUIHud();
	}

	protected override void OnProcessing(float completed)
	{
		if (completed - m_step >= 0)
		{
			GameManager.Instance.TerrainTool.RaiseTerrain(transform.position, 0.03f * 0.01f, 5, 5);
			m_step += 0.1f;
		}
		if (m_actived && completed > 0.7f)
		{
			DisActived();
		}
	}

	void DisActived()
	{
		m_actived = false;
		btPlace.interactable = false;
	}

	void OnDestroy()
	{
		GameManager.Instance.TerrainTool.SetTerrainHeight(transform.position, (m_initHeight + 0.45f) * 0.01f, 5, 5);
	}

	public void Place()
	{
		uiPlaceRoot.gameObject.SetActive(true);
	}
}
