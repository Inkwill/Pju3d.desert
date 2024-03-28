using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Slider))]
public class UICharacterHp : MonoBehaviour
{
	Slider m_slider;
	Character m_character;

	[SerializeField]
	bool isHud = true;

	void Awake()
	{
		m_slider = GetComponent<Slider>();
		m_slider.interactable = false;
	}
	void Start()
	{
		if (isHud) SetCharacter(GetComponentInParent<Character>());
	}

	public void SetCharacter(Character character)
	{
		if (m_character != null && m_character != character)
		{
			m_character.DamageEvent.RemoveListener(OnDamage);
			m_character.EffectAction -= OnEffect;
			if (isHud) m_character.DeathEvent.RemoveListener(OnDeath);
			if (isHud) m_character.StateUpdateAction -= OnStateUpdate;
		}

		m_character = character;
		if (m_character == null) { m_slider.gameObject.SetActive(false); return; }

		m_slider.maxValue = m_character.Stats.stats.health;
		m_slider.value = m_character.Stats.CurrentHealth;

		if (!isHud) m_slider.gameObject.SetActive(true);

		m_character.DamageEvent.AddListener(OnDamage);
		m_character.EffectAction += OnEffect;
		if (isHud) m_character.DeathEvent.AddListener(OnDeath);
		if (isHud) m_character.StateUpdateAction += OnStateUpdate;
	}

	void OnDamage(Damage damage)
	{
		m_slider.gameObject.SetActive(true);
		m_slider.value = m_character.Stats.CurrentHealth;
		DamageUI.Instance.NewDamage(damage.GetFullDamage(), transform.position);
	}

	void OnEffect(EffectData effect)
	{
		if (effect.Type == EffectData.EffectType.HPCHANGE)
		{
			m_slider.gameObject.SetActive(true);
			m_slider.value = m_character.Stats.CurrentHealth;
		}
	}

	void OnDeath(Character character)
	{
		m_slider.gameObject.SetActive(false);
	}

	void OnStateUpdate(AIBase.State state)
	{
		if (state == AIBase.State.IDLE) m_slider.gameObject.SetActive(false);
	}
}
