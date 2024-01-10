using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CreatorKitCode;

public class Building : TimerBehaviour
{
	CharacterData m_CharacterData;
	void Start()
	{
		m_CharacterData = GetComponent<CharacterData>();
		m_CharacterData.Init();

	}
}
