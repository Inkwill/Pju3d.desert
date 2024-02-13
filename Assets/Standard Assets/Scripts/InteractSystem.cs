using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CreatorKitCode;

public class InteractSystem : HighlightableObject
{
	RoleControl m_role;
	public List<EffectData> DeadEffects;
	void Start()
	{
		m_role = GetComponent<RoleControl>();
		m_role.eventSender.events.AddListener(OnRoleEvent);
	}

	void OnRoleEvent(GameObject obj, string eventName)
	{
		if (eventName == "roleEvent_OnState_DEAD")
		{
			foreach (var eff in DeadEffects)
			{
				eff.Take(m_role.Data);
			}
			Helpers.RecursiveLayerChange(transform, LayerMask.NameToLayer("EnemyCorpse"));
			StartCoroutine(DestroyCorpse());
		}
	}

	IEnumerator DestroyCorpse()
	{
		yield return new WaitForSeconds(1.0f);
		VFXManager.PlayVFX(VFXType.Death, transform.position);
		Destroy(gameObject);
	}
}
