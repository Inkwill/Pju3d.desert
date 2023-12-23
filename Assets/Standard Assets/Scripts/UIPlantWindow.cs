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
		player?.GetComponentInChildren<DigTool>().Plant(m_selectedPlant);
		Close();
	}

	public void OnSelected(string selected)
	{
		m_selectedPlant = selected;
		infoArea.SetActive(true);
	}
}
