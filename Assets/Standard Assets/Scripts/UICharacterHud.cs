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
		if (data && textName) textName.text = data.name;
		if (sliderPg) sliderPg.gameObject.SetActive(false);
	}

	private void Update()
	{
		this.transform.forward = Camera.main.transform.forward;
		if (data && sliderHp)
		{
			sliderHp.value = data.Stats.CurrentHealth / (float)data.Stats.stats.health;
		}
	}

	public void SetProgressSlider(bool active)
	{
		if (sliderPg) sliderPg.gameObject.SetActive(active);
	}
}