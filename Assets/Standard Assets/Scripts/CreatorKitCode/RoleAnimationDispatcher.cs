using UnityEngine;

/// <summary>
/// Need to be added to the GameObject that have the AnimatorController. This will receive the Event defined in the
/// import of the animations and can dispatch them to some receivers. Used by step event and attack frame event on
/// characters.
/// </summary>
public class RoleAnimationDispatcher : MonoBehaviour
{
	public interface IAttackFrameReceiver
	{
		void AttackFrame();
	}

	public interface IFootstepFrameReceiver
	{
		void FootstepFrame();
	}

	public interface ISkillstepFrameReceiver
	{
		void SkillstepFrame();
	}

	RoleControl m_role;

	IAttackFrameReceiver m_AttackReceiver;
	IFootstepFrameReceiver m_FootstepFrameReceiver;
	ISkillstepFrameReceiver m_SkillstepFrameReceiver;

	public void Init(RoleControl role)
	{
		m_role = role;
		if (m_role != null)
		{
			m_AttackReceiver = m_role as IAttackFrameReceiver;
			m_FootstepFrameReceiver = m_role as IFootstepFrameReceiver;
			m_SkillstepFrameReceiver = m_role as ISkillstepFrameReceiver;

			if (m_AttackReceiver == null)
			{
				Debug.LogError("The Monobehaviour set as Attack Frame Receiver don't implement the IAttackFrameReceiver interface!", m_role);
			}
			if (m_FootstepFrameReceiver == null)
			{
				Debug.LogError("The Monobehaviour set as Footstep Frame Receiver don't implement the IFootstepFrameReceiver interface!", m_role);
			}
			if (m_AttackReceiver == null)
			{
				Debug.LogError("The Monobehaviour set as Skillstep Frame Receiver don't implement the ISkillstepFrameReceiver interface!", m_role);
			}
		}
	}

	void AttackEvent()
	{
		m_AttackReceiver?.AttackFrame();
	}

	void FootstepEvent()
	{
		m_FootstepFrameReceiver?.FootstepFrame();
	}
	void SkillstepEvent()
	{
		m_SkillstepFrameReceiver?.SkillstepFrame();
	}
}
