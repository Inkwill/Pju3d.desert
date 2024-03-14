using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIInteractWindow : UIWindow
{
	public Button bt_interact;
	public Text Name;
	public Image icon;
	public Text Desc;
	IInteractable m_interactTarget;

	public void Init(IInteractable target)
	{
		m_interactTarget = target;
		Name.text = target.Data.Key;
		icon.sprite = target.Data.icon;
		Desc.text = target.Data.Description;
	}
}
