using UnityEngine;
using UnityEngine.UI;
using CreatorKitCode;
using TMPro;

public class UICharacterHud : MonoBehaviour
{
	public TMP_Text textName;
	public Slider sliderHp;
	public Slider sliderPg;

	public CharacterData data;

	private void Start()
	{
		if (sliderPg) sliderPg.gameObject.SetActive(false);
		if (data) Show(data);
	}

	public void Show(CharacterData targetData)
	{
		data = targetData;
		if (data)
		{
			if (textName) textName.text = data.CharacterName;
			if (sliderHp)
			{
				sliderHp.maxValue = (float)data.Stats.stats.health;
				sliderHp.value = data.Stats.CurrentHealth;
			}
			gameObject.SetActive(true);
		}
		else
		{
			gameObject.SetActive(false);
		}
	}

	private void Update()
	{
		this.transform.forward = Camera.main.transform.forward;
		if (data && sliderHp)
		{
			sliderHp.value = data.Stats.CurrentHealth;
		}
	}

	// public void SetProgressSlider(bool active)
	// {
	// 	if (sliderPg) sliderPg.gameObject.SetActive(active);
	// }
}