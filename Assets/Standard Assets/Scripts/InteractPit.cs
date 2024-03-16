using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using MyBox;


public class InteractPit : TimerBehaviour, IInteractable
{
	[SerializeField]
	InteractData m_data;
	public InteractData Data { get { return m_data; } }
	float m_initHeight;
	bool m_actived;
	float m_step = 0.1f;
	public IInteractable CurrentInteractor { get; set; }
	IInteractable m_interactor;
	public bool CanInteract(IInteractable target)
	{
		return m_actived && target.CanInteract(this) && m_interactor == null;
	}
	public void InteractWith(IInteractable target)
	{
		if (m_interactor == null && target is Character)
		{
			var character = target as Character;
			if (character == GameManager.CurHero)
			{
				UIInteractWindow win = GameManager.GameUI.OpenWindow("winInteract") as UIInteractWindow;
				//win.Init(this);
				GetComponent<InteractHandle>()?.ExitEvent.AddListener(() => win.Close());
				win.bt_Confirm.onClick.AddListener(() => OnClick_Interact(win));
			}
		}
		else { target.CurrentInteractor = null; }
	}

	public void OnClick_Interact(UIInteractWindow win)
	{
		m_interactor = GameManager.CurHero;
		win.Close();
	}

	protected override void OnStart()
	{
		base.OnStart();
		GetComponent<InteractHandle>()?.SetHandle(true);
		m_initHeight = Terrain.activeTerrain.SampleHeight(transform.position);
		m_actived = true;

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
	}

	void OnDestroy()
	{
		GameManager.Instance.TerrainTool.SetTerrainHeight(transform.position, (m_initHeight + 0.45f) * 0.01f, 5, 5);
	}

	public void Place()
	{

	}
}
