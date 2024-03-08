using System.Collections;
using System.Collections.Generic;
using CreatorKitCode;
using UnityEngine;

public class CharacterAudio : MonoBehaviour
{
	public AudioClip[] FootstepClips;
	public AudioClip[] SpurSoundClips;
	public AudioClip[] SpottedAudioClip;
	public AudioClip[] VocalAttack;
	public AudioClip[] VocalHit;
	public AudioClip[] HitClip;
	public AudioClip[] DeathClips;
	public SFXManager.Use UseType;
	CharacterData m_Character;
	void Start()
	{
		m_Character = GetComponent<CharacterData>();
		m_Character.DamageEvent.AddListener((damage) => { Hit(damage.Target.gameObject.transform.position); });
		m_Character.DeathEvent.AddListener((character) => { Death(character.transform.position); });
		m_Character.AttackEvent.AddListener((attacker) => { Attack(attacker.transform.position); });

		CharacterAnimHandle animHandle = GetComponentInChildren<CharacterAnimHandle>();
		if (animHandle)
		{
			animHandle.FootStep.AddListener(() => { Step(transform.position); VFXManager.PlayVFX(VFXType.StepPuff, transform.position); });
		}
		m_Character.Inventory.ItemAction += OnItemEvent;
		m_Character.Equipment.OnEquiped += OnEquiped;

		if (m_Character.BaseAI != null) m_Character.StateUpdateAction += (state) =>
		{
			if (state == AIBase.State.PURSUING && SpottedAudioClip.Length != 0)
			{
				SFXManager.PlaySound(SFXManager.Use.Enemies, new SFXManager.PlayData()
				{
					Clip = SpottedAudioClip[Random.Range(0, SpottedAudioClip.Length)],
					Position = m_Character.transform.position,
				});
			}
		};
	}


	void OnItemEvent(Item item, string eventName, int num)
	{
		// if (eventName == "Equip")
		// 	

	}

	void OnEquiped(EquipmentItem equip)
	{
		SFXManager.PlaySound(SFXManager.Use.Sound2D, new SFXManager.PlayData() { Clip = SFXManager.ItemEquippedSound });
	}
	public void Attack(Vector3 position)
	{
		if (VocalAttack.Length > 0)
		{
			SFXManager.PlaySound(UseType, new SFXManager.PlayData()
			{
				Clip = VocalAttack[Random.Range(0, VocalAttack.Length)],
				Position = position,
			});
		}
		SFXManager.PlaySound(UseType, new SFXManager.PlayData()
		{
			Clip = m_Character.Equipment.Weapon.GetSwingSound(),
			Position = position
		});
	}

	void Hit(Vector3 position)
	{
		if (VocalHit.Length > 0)
		{
			SFXManager.PlaySound(UseType, new SFXManager.PlayData()
			{
				Clip = VocalHit[Random.Range(0, VocalHit.Length)],
				Position = position,
			});
		}
		if (HitClip.Length > 0)
		{
			SFXManager.PlaySound(UseType, new SFXManager.PlayData()
			{
				Clip = HitClip[Random.Range(0, HitClip.Length)],
				PitchMax = 1.1f,
				PitchMin = 0.8f,
				Position = position
			});
		}
		// SFXManager.PlaySound(m_Source.AudioPlayer.UseType, new SFXManager.PlayData()
		//  { Clip = m_Source.Equipment.Weapon.GetHitSound(), PitchMin = 0.8f, PitchMax = 1.2f, Position = damagePos });
	}

	void Step(Vector3 position)
	{
		if (SpurSoundClips.Length > 0)
			SFXManager.PlaySound(SFXManager.Use.Player, new SFXManager.PlayData()
			{
				Clip = SpurSoundClips[Random.Range(0, SpurSoundClips.Length)],
				Position = position,
				PitchMin = 0.8f,
				PitchMax = 1.2f,
				Volume = 0.3f
			});

		if (FootstepClips.Length > 0)
		{
			SFXManager.PlaySound(UseType, new SFXManager.PlayData()
			{
				Clip = FootstepClips[Random.Range(0, FootstepClips.Length)],
				Position = position,
				PitchMin = 0.8f,
				PitchMax = 1.2f,
				Volume = 0.3f
			});
		}
	}

	void Death(Vector3 position)
	{
		if (DeathClips.Length == 0)
			return;

		SFXManager.PlaySound(UseType, new SFXManager.PlayData()
		{
			Clip = DeathClips[Random.Range(0, DeathClips.Length)],
			Position = position
		});
	}
}
