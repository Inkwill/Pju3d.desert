using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIHudHandle : MonoBehaviour
{
	[SerializeField]
	GameObject hudPrefab;
	[SerializeField]
	GameObject m_hud;
	Vector3 m_worldPos;
	Vector3 m_worldScale;
	bool showUI;

	void Start()
	{
		m_worldPos = m_hud.transform.position;
		m_worldScale = m_hud.transform.localScale;
		//ShowWorldHud();
	}
	public void ShowWorldHud()
	{
		if (m_hud == null)
		{
			m_hud = Instantiate(hudPrefab, transform);
			m_hud.transform.forward = Camera.main.transform.forward;
			m_hud.gameObject.SetActive(true);
		}
		else
		{
			m_hud.transform.SetParent(transform);
			m_hud.transform.position = m_worldPos;
			m_hud.transform.localScale = m_worldScale;
			m_hud.transform.forward = Camera.main.transform.forward;
			m_hud.gameObject.SetActive(true);
		}
		showUI = false;
	}
	public void ShowUIHud()
	{
		if (m_hud == null)
		{
			m_hud = Instantiate(hudPrefab, GameManager.GameUI.transform);
			m_hud.transform.position = Camera.main.WorldToScreenPoint(transform.position);
			m_hud.transform.rotation = Quaternion.identity;
			m_hud.gameObject.SetActive(true);
		}
		else
		{
			m_hud.transform.SetParent(GameManager.GameUI.transform);
			m_hud.transform.position = Camera.main.WorldToScreenPoint(transform.position);
			m_hud.transform.rotation = Quaternion.identity;
			m_hud.transform.localScale = Vector3.one;
			m_hud.gameObject.SetActive(true);
		}
		showUI = true;
	}
	// void ShowHud(HudType type)
	// {
	// 	switch (type)
	// 	{
	// 		case HudType.World:
	// 			if (m_hud == null)
	// 			{
	// 				m_hud = Instantiate(hudPrefab, transform);
	// 				m_hud.gameObject.SetActive(true);
	// 			}
	// 			else { m_hud.gameObject.SetActive(true); m_hud?.gameObject.SetActive(false); }
	// 			break;
	// 		case HudType.UI:
	// 			if (m_hud == null)
	// 			{
	// 				m_hud = Instantiate(hudPrefab, GameManager.GameUI.transform);
	// 				m_hud.transform.position = Camera.main.WorldToScreenPoint(transform.position);
	// 				m_hud.gameObject.SetActive(true);
	// 			}
	// 			else { m_hud.gameObject.SetActive(true); m_hud?.gameObject.SetActive(false); }
	// 			break;
	// 		default:
	// 			break;
	// 	}
	// }
	private void Update()
	{
		if (!showUI) m_hud.transform.forward = Camera.main.transform.forward;
		else m_hud.transform.position = Camera.main.WorldToScreenPoint(transform.position);
	}
}
