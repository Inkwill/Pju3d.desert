using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CreatorKitCodeInternal;

public abstract class Skill : ScriptableObject
{
	public string SkillName;
	public Sprite SkillSprite;
	public string Description;
	public string SkillAnim;
	public VFXType fxStep;
	public VFXType fxOperating;
	public VFXType fxImplement;
	public AudioClip SkillClip;
	public float Duration;
	public float CD;

	public virtual bool CanUsedBy(RoleControl user)
	{
		return true;
	}

	public virtual void Implement(RoleControl user)
	{
		Debug.Log("Implement Skill:" + SkillName);
	}

	public virtual void Operating(RoleControl user)
	{
		//Debug.Log("Operating Skill:" + SkillName);
	}

	public virtual void StepEffect(RoleControl user)
	{
		//Debug.Log("StepEffect Skill:" + SkillName);
	}

}
