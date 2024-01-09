using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIPlantWindow : UIWindow
{

	public GameObject infoArea;
	string m_selectedPlant;
	public void DoPlant()
	{
		m_player?.GetComponentInChildren<DigTool>().DoPlant(m_selectedPlant);
		Back();
	}

	public void OnSelected(string selected)
	{
		m_selectedPlant = selected;
		infoArea.SetActive(true);
	}
}
