using UnityEngine;

namespace CreatorKitCodeInternal
{
	/// <summary>
	/// Need to be added to the GameObject that have the AnimatorController. This will receive the Event defined in the
	/// import of the animations and can dispatch them to some receivers. Used by step event and attack frame event on
	/// characters.
	/// </summary>
	public class AnimationControllerDispatcher : MonoBehaviour
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

		public MonoBehaviour AttackFrameReceiver;
		public MonoBehaviour FootstepFrameReceiver;
		public MonoBehaviour SkillstepFrameReceiver;

		IAttackFrameReceiver m_AttackReceiver;
		IFootstepFrameReceiver m_FootstepFrameReceiver;
		ISkillstepFrameReceiver m_SkillstepFrameReceiver;

		void Awake()
		{
			if (AttackFrameReceiver != null)
			{
				m_AttackReceiver = AttackFrameReceiver as IAttackFrameReceiver;

				if (m_AttackReceiver == null)
				{
					Debug.LogError("The Monobehaviour set as Attack Frame Receiver don't implement the IAttackFrameReceiver interface!", AttackFrameReceiver);
				}
			}

			if (FootstepFrameReceiver)
			{
				m_FootstepFrameReceiver = FootstepFrameReceiver as IFootstepFrameReceiver;

				if (m_FootstepFrameReceiver == null)
				{
					Debug.LogError("The Monobehaviour set as Footstep Frame Receiver don't implement the IFootstepFrameReceiver interface!", FootstepFrameReceiver);
				}
			}
			if (SkillstepFrameReceiver)
			{
				m_SkillstepFrameReceiver = SkillstepFrameReceiver as ISkillstepFrameReceiver;

				if (m_AttackReceiver == null)
				{
					Debug.LogError("The Monobehaviour set as Skillstep Frame Receiver don't implement the ISkillstepFrameReceiver interface!", SkillstepFrameReceiver);
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
}