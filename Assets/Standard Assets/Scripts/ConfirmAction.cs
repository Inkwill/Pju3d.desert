using System;
using System.Collections.Generic;
using UnityEngine;

public class ConfirmAction
{
	Action m_confirm;
	Action m_cancle;
	public ConfirmAction(Action confirm, Action cancle)
	{
		m_confirm = confirm;
		m_cancle = cancle;
	}

	public void AddListener()
	{

	}
}
