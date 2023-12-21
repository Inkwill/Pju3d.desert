using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CreatorKitCodeInternal;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
	public Text infoPos;
	public Text infoTerrian;

	public CharacterControl player;

	DigTool m_digtool;
	private void Start()
	{
		m_digtool = player.GetComponentInChildren<DigTool>();
	}
	// Update is called once per frame
	void FixedUpdate()
	{
		infoPos.text = player.gameObject.transform.position.ToString();
		infoTerrian.text = m_digtool?.SceneBoxInfo();
	}
}
