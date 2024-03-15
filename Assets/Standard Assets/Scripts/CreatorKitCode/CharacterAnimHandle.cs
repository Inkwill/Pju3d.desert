using UnityEngine;
using UnityEngine.Events;
using CreatorKitCode;

/// <summary>
/// Need to be added to the GameObject that have the AnimatorController. This will receive the Event defined in the
/// import of the animations and can dispatch them to some receivers. Used by step event and attack frame event on
/// characters.
/// </summary>
public class CharacterAnimHandle : MonoBehaviour
{
	public UnityEvent FootStep;
	public UnityEvent SkillStep;
	Character m_character;
	Animator m_Animator;

	[SerializeField]
	string DeathTrigger = "Death";
	[SerializeField]
	string SpeedTrigger = "Speed";
	[SerializeField]
	string AttackTrigger = "Attack";
	[SerializeField]
	string HitTrigger = "Hit";
	[SerializeField]
	string RespawnTrigger = "Respawn";
	[SerializeField]
	string FaintTrigger = "Faint";
	void Start()
	{
		m_character = GetComponentInParent<Character>();
		m_character.DamageEvent.AddListener((damage) => { if (HitTrigger != "" && damage.GetFullDamage() > 0) m_Animator.SetTrigger(HitTrigger); });
		m_character.DeathEvent.AddListener((character) => { if (DeathTrigger != "") m_Animator.SetTrigger(DeathTrigger); });
		m_character.AttackEvent.AddListener((attacker) => { if (AttackTrigger != "") m_Animator.SetTrigger(AttackTrigger); });
		if (m_character.BaseAI != null)
		{
			m_character.StateUpdateAction += OnCharacterStating;
		}
		m_Animator = GetComponent<Animator>();
	}

	void OnCharacterStating(AIBase.State curState)
	{
		if (SpeedTrigger == "") return;
		if (curState != AIBase.State.DEAD && curState != AIBase.State.INACTIVE)
			m_Animator.SetFloat(SpeedTrigger, m_character.BaseAI.SpeedScale);
		if (curState == AIBase.State.INTERACTING)
		{
			string anim = m_character.CurrentInteractor.Data.interactAnim;
			if (anim != "") m_Animator.SetTrigger(anim);
			else m_character.CurrentInteractor.InteractWith(m_character);
		}
	}

	void StepEvent()
	{
		if (m_character.BaseAI.CurState == AIBase.State.ATTACKING)
		{
			AttackEvent();
		}

		if (m_character.BaseAI.CurState == AIBase.State.SKILLING)
		{
			SkillStep?.Invoke();
		}
		if (m_character.BaseAI.CurState == AIBase.State.INTERACTING)
		{
			m_character.CurrentInteractor.InteractWith(m_character);
		}
	}

	void AttackEvent()
	{
		m_character.AttackFrame();
	}
	void FootstepEvent()
	{
		FootStep?.Invoke();
	}
}
