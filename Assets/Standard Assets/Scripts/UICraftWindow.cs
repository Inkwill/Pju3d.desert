using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.UI;
using TMPro;

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
	UIItemListBox[] m_requireItemBoxs;
	FormulaData m_formula;


	void Awake()
	{
		m_requireItemBoxs = GetComponentsInChildren<UIItemListBox>();
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
		StartCoroutine(DoCraft());
	}

	IEnumerator DoCraft()
	{
		yield return new WaitForSeconds(3.0f);
		m_formula.Craft(GameManager.CurHero.Inventory);
		GameManager.CurHero.InteractWith(null);
	}
}
