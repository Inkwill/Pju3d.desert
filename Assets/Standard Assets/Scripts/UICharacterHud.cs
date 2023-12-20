using UnityEngine;
using UnityEngine.UI;
using CreatorKitCode;
using TMPro;

public class UICharacterHud : MonoBehaviour
{
	public TMP_Text textName;
	public Slider sliderHp;

	public CharacterData data;

	private void Start()
	{
		if (data && textName) textName.text = data.name;
	}

	private void Update()
	{
		this.transform.forward = Camera.main.transform.forward;
		if (data && sliderHp)
		{
			sliderHp.value = data.Stats.CurrentHealth / (float)data.Stats.stats.health;
		}
	}
}