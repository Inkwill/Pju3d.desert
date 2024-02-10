using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CreatorKitCodeInternal;
using CreatorKitCode;
using MyBox;

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

	public enum TargetType
	{
		SELF,
		SCENEBOX,
		CURRENT,
		NONE
	}

	[Header("Limit")]
	public TargetType TType;
	public float CD;
	public float MP;
	public bool IdleSkill;
	[Header("Effect")]
	public List<KeyValueData.KeyValue<EffectData, string[]>> implementEffects;
	public List<KeyValueData.KeyValue<EffectData, string[]>> stepEffects;
	public List<KeyValueData.KeyValue<EffectData, string[]>> operatEffects;
	[ConditionalField(nameof(TType), false, TargetType.NONE)]
	public int radius;
	[ConditionalField(nameof(TType), false, TargetType.NONE)]
	public int AddTargets;
	[ConditionalField(nameof(TType), false, TargetType.NONE)]
	public LayerMask layers;


	public virtual bool CanUsedBy(RoleControl user)
	{
		if (IdleSkill) return user.isIdle;
		else return user.isStandBy;
	}

	public virtual void Implement(RoleControl user, List<GameObject> targets = null)
	{
		if (implementEffects.Count > 0) TakeEffects(implementEffects, user, targets);
		Debug.Log("Implement Skill:" + SkillName);
	}

	public virtual void Operating(RoleControl user, List<GameObject> targets = null)
	{
		if (operatEffects.Count > 0) TakeEffects(operatEffects, user, targets);
	}

	public virtual void StepEffect(RoleControl user, List<GameObject> targets = null)
	{
		if (stepEffects.Count > 0) TakeEffects(stepEffects, user, targets);
		var Effectpos = (targets != null && targets.Count > 0) ? targets[0].transform.position : user.transform.position;
		user.Data.AudioPlayer.Attack(Effectpos);
		VFXManager.PlayVFX(fxStep, Effectpos);
		//Debug.Log("StepEffect Skill:" + SkillName);
	}

	void TakeEffects(List<KeyValueData.KeyValue<EffectData, string[]>> effectGroup, RoleControl user, List<GameObject> targets)
	{
		foreach (var effect in effectGroup)
		{
			foreach (var target in targets)
			{
				effect.Key.Take(user.Data, effect.Value, target);
				Debug.Log("OnSkillTakeEffects: target= " + target);
			}
		}
	}
}
