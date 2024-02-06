using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CreatorKitCode;
using CreatorKitCodeInternal;
public abstract class Effect
{
	public CharacterData Target => m_Target;
	public CharacterData Source => m_Source;

	protected CharacterData m_Target;
	protected CharacterData m_Source;


	public virtual void Take(CharacterData target) { }
	public virtual void Take() { Take(m_Target); }

}

