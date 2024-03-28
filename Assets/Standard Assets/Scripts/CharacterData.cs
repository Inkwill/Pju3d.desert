using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CharacterData", menuName = "Data/CharacterData", order = 800)]
public class CharacterData : ScriptableObject
{
	public string CharacterId;
	public string CharacterName;
	public Sprite headIcon;

}
