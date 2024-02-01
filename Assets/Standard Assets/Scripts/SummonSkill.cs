using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CreatorKitCode;
using CreatorKitCodeInternal;

[CreateAssetMenu(fileName = "SummonSkill", menuName = "Data/SummonSkill", order = 1)]
public class SummonSkill : Skill
{
	public string summerName;
	public float digDeep;
	public override void Implement(RoleControl user)
	{
		GameObject pbObj = Resources.Load(summerName) as GameObject;
		Instantiate(pbObj, user.DigTool.transform.position, Quaternion.Euler(0, 180, 0));
	}

	public override void Operating(RoleControl user)
	{
		//if (digDeep > 0) GameManager.Instance.TerrainTool.LowerTerrain(user.DigTool.transform.position, digDeep * 0.01f, 5, 5);
	}
	public override void StepEffect(RoleControl user)
	{
		var Effectpos = user.DigTool.transform.position;
		user.AudioPlayer.Attack(Effectpos);
		VFXManager.PlayVFX(VFXType.SmokePoof, Effectpos);
		if (digDeep > 0) GameManager.Instance.TerrainTool.LowerTerrain(user.DigTool.transform.position, digDeep * 0.00025f, 5, 10);
	}
}
