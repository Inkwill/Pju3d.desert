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
	public UnityEvent AttackStep;
	public UnityEvent FootStep;
	public UnityEvent SkillStep;
	CharacterData m_character;
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
		m_character = GetComponentInParent<CharacterData>();
		m_character.OnDamage.AddListener((damage) => { if (HitTrigger != "" && damage.GetFullDamage() > 0) m_Animator.SetTrigger(HitTrigger); });
		m_character.OnDeath.AddListener((character) => { if (DeathTrigger != "") m_Animator.SetTrigger(DeathTrigger); });
		m_character.OnAttack += (attacker) => { if (AttackTrigger != "") m_Animator.SetTrigger(AttackTrigger); };
		//m_character.GetComponent<EventSender>()?.events.AddListener(OnCharacterEvent);
		if (m_character.BaseAI != null) m_character.BaseAI.StateUpdateEvent += OnCharacterStating;
		m_Animator = GetComponent<Animator>();
	}

	void OnCharacterStating(AIBase.State curState)
	{
		if (SpeedTrigger == "") return;
		if (curState == AIBase.State.IDLE || curState == AIBase.State.MOVE || curState == AIBase.State.PURSUING || curState == AIBase.State.ATTACKING)
			m_Animator.SetFloat(SpeedTrigger, m_character.BaseAI.SpeedScale);
	}

	void AttackEvent()
	{
		m_character.AttackFrame();
		AttackStep?.Invoke();
	}

	void FootstepEvent()
	{
		FootStep?.Invoke();
	}
	void SkillstepEvent()
	{
		SkillStep?.Invoke();
	}
}
