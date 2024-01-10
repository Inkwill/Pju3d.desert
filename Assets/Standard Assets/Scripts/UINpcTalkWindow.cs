using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UINpcTalkWindow : UIWindow
{
	Npc m_npc;
	protected override void OnOpen()
	{
		m_npc = m_digtool.GetLastInner<Npc>();
		if (m_npc)
		{
			m_npc.StartTalk();
			winTitle.text = m_npc.name;
		}
	}

	protected override void OnClose()
	{
		m_npc?.EndTalk();
	}
}
