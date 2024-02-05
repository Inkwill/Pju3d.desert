using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Need to be added to the GameObject that have the AnimatorController. This will receive the Event defined in the
/// import of the animations and can dispatch them to some receivers. Used by step event and attack frame event on
/// characters.
/// </summary>
public class AnimationDispatcher : MonoBehaviour
{
	public UnityEvent AttackStep;
	public UnityEvent FootStep;
	public UnityEvent SkillStep;

	void AttackEvent()
	{
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
