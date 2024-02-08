using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CreatorKitCodeInternal;
using CreatorKitCode;

[CreateAssetMenu(fileName = "Skill", menuName = "Data/Skill", order = 1)]
public class Skill : ScriptableObject
{
	[Header("Base")]
	public string SkillName;
	public Sprite SkillSprite;
	public string Description;
	public float Duration;

	[Header("Performance")]
	public string SkillAnim;
	public AudioClip SkillClip;
	public VFXType fxStep;
	public VFXType fxOperating;
	public VFXType fxImplement;

	[Header("Limit")]
	public float CD;
	public float MP;
	public bool IdleSkill;
	[Header("Effect")]
	public List<KeyValueData.KeyValue<EffectData, string[]>> implementEffects;
	public List<KeyValueData.KeyValue<EffectData, string[]>> stepEffects;

	public virtual bool CanUsedBy(RoleControl user)
	{
		if (IdleSkill) return user.isIdle;
		else return user.isStandBy;
	}

	public virtual void Implement(RoleControl user, GameObject target = null)
	{
		foreach (var effect in implementEffects)
		{
			effect.Key.Take(user.Data, effect.Value, target);
		}
		Debug.Log("Implement Skill:" + SkillName);
	}

	public virtual void Operating(RoleControl user)
	{
		//Debug.Log("Operating Skill:" + SkillName);
	}

	public virtual void StepEffect(RoleControl user, GameObject target = null)
	{
		foreach (var effect in stepEffects)
		{
			effect.Key.Take(user.Data, effect.Value, target);
		}
		var Effectpos = (target != null) ? target.transform.position : user.transform.position;
		user.AudioPlayer.Attack(Effectpos);
		VFXManager.PlayVFX(fxStep, Effectpos);
		Debug.Log("StepEffect Skill:" + SkillName);
	}

}
