using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TroopSystem
{
	public class Troop
	{
		public enum TroopDeploy
		{
			Forward,
			Middle,
			Rear
		}
		public Character leader;
		public TroopDeploy leaderDeploy;
		public Dictionary<TroopDeploy, Dictionary<Character, int>> Troops;

	}
}
