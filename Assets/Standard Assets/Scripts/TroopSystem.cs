using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TroopSystem
{
	public enum TrooperType
	{
		Leader,
		Forward,
		Middle,
		Rear
	}

	public enum TroopDeploy
	{
		Forward,
		Middle,
		Rear
	}
	public class Troop
	{
		public Character leader;
		public TroopDeploy leaderDeploy = TroopDeploy.Forward;
		public Dictionary<TroopDeploy, List<Character>> Troops;
		public Action<TrooperType> changeAction;
		public Action<TroopDeploy, int> adjustAction;

		public Troop()
		{
			Troops = new Dictionary<TroopDeploy, List<Character>>();
			Troops.Add(TroopDeploy.Forward, new List<Character>());
			Troops.Add(TroopDeploy.Middle, new List<Character>());
			Troops.Add(TroopDeploy.Rear, new List<Character>());
		}

		public Character GetTrooper(TrooperType type)
		{
			switch (type)
			{
				case TrooperType.Leader:
					return leader;
				case TrooperType.Forward:
					if (Troops[TroopDeploy.Forward].Count > 0) return Troops[TroopDeploy.Forward][0];
					else return null;
				case TrooperType.Middle:
					if (Troops[TroopDeploy.Middle].Count > 0) return Troops[TroopDeploy.Middle][0];
					else return null;
				case TrooperType.Rear:
					if (Troops[TroopDeploy.Rear].Count > 0) return Troops[TroopDeploy.Rear][0];
					else return null;
				default:
					return null;
			}
		}

	}
}
