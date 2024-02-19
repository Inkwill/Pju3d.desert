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
	public AudioClip[] DeathClips;
	public SFXManager.Use UseType;
	RoleControl m_role;
	void Start()
	{
		m_role = GetComponent<RoleControl>();
		if (m_role) m_role.eventSender.events.AddListener(OnRoleEvent);
	}

	void OnRoleEvent(GameObject obj, string eventName)
	{
		Vector3 position = obj.transform.position;
		if (eventName == "roleEvent_OnDamage") Hit(position);
		if (eventName == "roleEvent_OnState_DEAD") Death(position);
		if (eventName == "roleEvent_OnFootStep") Step(position);
		if (eventName == "roleEvent_OnState_ATTACKING")
		{
			Attack(position);
			SFXManager.PlaySound(UseType, new SFXManager.PlayData()
			{
				Clip = m_role.Data.Equipment.Weapon.GetSwingSound(),
				Position = position
			});
		}
		if (eventName == "roleEvent_OnPursuing")
		{
			if (SpottedAudioClip.Length != 0)
			{
				SFXManager.PlaySound(SFXManager.Use.Enemies, new SFXManager.PlayData()
				{
					Clip = SpottedAudioClip[Random.Range(0, SpottedAudioClip.Length)],
					Position = position
				});
			}
		}
	}
	public void Attack(Vector3 position)
	{
		if (VocalAttack.Length == 0)
			return;

		SFXManager.PlaySound(UseType, new SFXManager.PlayData()
		{
			Clip = VocalAttack[Random.Range(0, VocalAttack.Length)],
			Position = position,
		});
	}

	void Hit(Vector3 position)
	{
		if (VocalHit.Length == 0)
			return;

		SFXManager.PlaySound(UseType, new SFXManager.PlayData()
		{
			Clip = VocalHit[Random.Range(0, VocalHit.Length)],
			Position = position,
		});
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
