using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UINpcTalkWindow : UIWindow
{
	Npc m_npc;
	public override void OnOpen()
	{
		m_npc = m_digtool.GetLastInner<Npc>();
		if (m_npc)
		{
			m_npc.StartTalk();
			winTitle.text = m_npc.name;
		}
	}

	public override void OnClose()
	{
		m_npc?.EndTalk();
	}
}
