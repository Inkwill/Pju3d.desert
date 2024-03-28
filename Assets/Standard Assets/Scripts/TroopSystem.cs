using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TroopSystem
{
	public enum TeamerType
	{
		Leader,
		Forward,
		Middle,
		Rear
	}

	public enum TeamDeploy
	{
		Forward,
		Middle,
		Rear
	}
	public class Team
	{
		public Character leader;
		public TeamDeploy leaderDeploy = TeamDeploy.Forward;
		public Dictionary<TeamDeploy, List<Character>> Troops;
		public Action<TeamerType> changeAction;
		public Action<TeamDeploy, int> adjustAction;

		public Team()
		{
			Troops = new Dictionary<TeamDeploy, List<Character>>();
			Troops.Add(TeamDeploy.Forward, new List<Character>());
			Troops.Add(TeamDeploy.Middle, new List<Character>());
			Troops.Add(TeamDeploy.Rear, new List<Character>());
		}

		public Character GetTeamer(TeamerType type)
		{
			switch (type)
			{
				case TeamerType.Leader:
					return leader;
				case TeamerType.Forward:
					if (Troops[TeamDeploy.Forward].Count > 0) return Troops[TeamDeploy.Forward][0];
					else return null;
				case TeamerType.Middle:
					if (Troops[TeamDeploy.Middle].Count > 0) return Troops[TeamDeploy.Middle][0];
					else return null;
				case TeamerType.Rear:
					if (Troops[TeamDeploy.Rear].Count > 0) return Troops[TeamDeploy.Rear][0];
					else return null;
				default:
					return null;
			}
		}

	}
}
