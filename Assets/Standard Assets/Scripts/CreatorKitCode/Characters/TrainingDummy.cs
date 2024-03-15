using System.Collections;
using System.Collections.Generic;
using CreatorKitCode;
using UnityEngine;

namespace CreatorKitCodeInternal
{
	public class TrainingDummy : MonoBehaviour
	{
		Character m_CharData;

		float m_HealTimer = 0.0f;

		// Start is called before the first frame update
		void Start()
		{
			m_CharData = GetComponent<Character>();

			m_CharData.DamageEvent.AddListener((damage) =>
			{
				m_HealTimer = 3.0f;
			});
		}

		// Update is called once per frame
		void Update()
		{
			if (m_HealTimer > 0.0f)
				m_HealTimer -= Time.deltaTime;
			else if (m_CharData.Stats.CurrentHealth != m_CharData.Stats.stats.health)
			{
			}
		}
	}
}