using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Slider))]
public class UICharacterHp : MonoBehaviour
{
	Slider m_slider;
	Character m_character;
	void Start()
	{
		m_character = GetComponentInParent<Character>();
		m_slider = GetComponent<Slider>();
		m_slider.maxValue = m_character.Stats.stats.health;
		m_slider.value = m_character.Stats.CurrentHealth;
		m_slider.gameObject.SetActive(false);
		m_character.DamageEvent.AddListener((damage) =>
		{
			m_slider.gameObject.SetActive(true);
			m_slider.value = m_character.Stats.CurrentHealth;
			DamageUI.Instance.NewDamage(damage.GetFullDamage(), transform.position);
		});
		m_character.EffectAction += (effect) =>
		{
			if (effect.Type == EffectData.EffectType.HPCHANGE)
			{
				m_slider.gameObject.SetActive(true);
				m_slider.value = m_character.Stats.CurrentHealth;
			}
		};
		m_character.DeathEvent.AddListener((character) => m_slider.gameObject.SetActive(false));

		m_character.StateUpdateAction += (state) =>
		{
			if (state == AIBase.State.IDLE) m_slider.gameObject.SetActive(false);
		};
	}


}
