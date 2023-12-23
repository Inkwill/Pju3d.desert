using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CreatorKitCodeInternal;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
	public Text infoPos;
	public Text infoTerrian;

	public Button btDig;

	public Button btPlant;

	public CharacterControl player;
	public CameraController camera;

	DigTool m_digtool;

	List<Button> switch_buttons;

	List<UIWindow> m_winList;

	private void Start()
	{
		m_digtool = player.GetComponentInChildren<DigTool>();
		switch_buttons = new List<Button>();
		m_winList = new List<UIWindow>();
		switch_buttons.Add(btDig);
		switch_buttons.Add(btPlant);
	}
	// Update is called once per frame
	void FixedUpdate()
	{
		infoPos.text = player.gameObject.transform.position.ToString();
		infoTerrian.text = m_digtool?.SceneBoxInfo(true);
		switch (m_digtool?.SceneBoxInfo(false))
		{
			case null:
				if (player.canWork && !btDig.interactable) SwitchButton(btDig);
				break;
			case "pit":
				if (player.canWork && !btPlant.interactable) SwitchButton(btPlant);
				break;
			default:
				SwitchButton(null);
				break;
		}
	}

	public void OnButtonClick(string eventName)
	{
		switch (eventName)
		{
			case "dig":
				m_digtool.DigPit();
				break;
			case "plant":
				OpenWindow("winPlant");
				break;
			case "camCtrl":
				camera.SwitchModel();
				break;
			default:
				break;
		}
	}

	void SetButton(Button bt, bool active)
	{
		if (bt.interactable == active) return;

		bt.interactable = active;
		string anim = active ? "move_up" : "move_down";
		Animator animator = bt.GetComponent<Animator>();
		if (animator) animator.Play(anim);

		// Image[] imgs = bt.gameObject.GetComponentsInChildren<Image>();
		// foreach (Image img in imgs)
		// {
		// 	img.color = active ? Color.red : Color.gray;
		// }
	}

	void SwitchButton(Button bt)
	{
		foreach (Button button in switch_buttons)
		{
			SetButton(button, bt == button);
		}
	}

	public void OpenWindow(string winName)
	{
		foreach (UIWindow win in m_winList)
		{
			if (win.gameObject.name == winName)
			{
				win.gameObject.SetActive(true);
				return;
			}
		}

		UIWindow newWindow = Resources.Load<UIWindow>(winName);
		if (newWindow)
		{
			newWindow = Instantiate(newWindow, transform);
			newWindow.gameObject.name = winName;
			newWindow.winTitle.text = winName;
			m_winList.Add(newWindow);
		}
	}
}
