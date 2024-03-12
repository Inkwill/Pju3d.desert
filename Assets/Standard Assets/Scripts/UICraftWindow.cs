using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.UI;
using TMPro;
using CreatorKitCode;

public class UICraftWindow : UIWindow
{
	public GameObject uiFormulaElement;
	public ToggleGroup uiFormulaRoot;
	[SerializeField]
	TMP_Text product_Name;
	[SerializeField]
	Text product_Desc;
	[SerializeField]
	Button bt_Craft;
	UISliderHandle slider_crafting;
	UIItemListBox[] m_requireItemBoxs;
	FormulaData m_formula;
	float m_craftingTime;
	const float craftingDuring = 3.0f;


	void Awake()
	{
		m_requireItemBoxs = GetComponentsInChildren<UIItemListBox>();
		slider_crafting = GetComponentInChildren<UISliderHandle>();
		GameManager.CurHero.Inventory.ItemAction += (item, eventName, num) => { if (m_formula) UpdateInfo(m_formula); };
	}
	protected override void OnOpen()
	{
		var elements = Helpers.AdjustElements<UIElementBase>(uiFormulaRoot.transform, GameManager.Instance.formulas.Count, uiFormulaElement);

		for (int i = 0; i < elements.Length; i++)
		{
			UIElementBase element = elements[i].GetComponent<UIElementBase>();
			FormulaData formula = GameManager.Instance.formulas[i];
			element.icon.sprite = formula.product.ItemSprite;
			element.toggle.group = uiFormulaRoot;
			element.toggle.onValueChanged.AddListener((value) => { if (value) UpdateInfo(formula); });
		}
		if (m_formula != null) UpdateInfo(m_formula);
		slider_crafting?.Init(craftingDuring, m_craftingTime, "crafting...");
	}

	protected override void OnClose()
	{
		base.OnClose();
	}

	public void UpdateInfo(FormulaData formula)
	{
		m_formula = formula;
		product_Name.text = formula.product.ItemName;
		product_Desc.text = formula.product.Description;
		bt_Craft.interactable = formula.canCraft(GameManager.CurHero.Inventory);
		for (int i = 0; i < m_requireItemBoxs.Length; i++)
		{
			if (formula.requireList.Count > i) m_requireItemBoxs[i].SetRequiredInfo(formula.requireList[i].Key, formula.requireList[i].Value);
			else m_requireItemBoxs[i].gameObject.SetActive(false);
		}
	}
	public void Craft()
	{
		GameManager.CurHero.InteractWith(GameManager.CurHero);
		bt_Craft.interactable = false;
		m_craftingTime = craftingDuring;
	}
	void Update()
	{
		if (m_craftingTime > 0)
		{
			m_craftingTime -= Time.deltaTime;
			slider_crafting?.SetValue(craftingDuring, craftingDuring - m_craftingTime, "crafting...");
			if (m_craftingTime <= 0)
			{
				GameManager.CurHero.InteractWith(null);
				GameManager.StartWaitAction(0.1f, () => { Close(); m_formula.Craft(GameManager.CurHero.Inventory); VFXManager.PlayVFX(VFXType.SmokePoof, GameManager.CurHero.transform.position); });
			};
		}
	}
}
