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
	UIItemListBox[] m_requireItemBoxs;
	FormulaData m_formula;


	void Awake()
	{
		m_requireItemBoxs = GetComponentsInChildren<UIItemListBox>();
	}
	protected override void OnOpen()
	{
		var elements = Helpers.AdjustElements<UIFormulaElement>(uiFormulaRoot.transform, GameManager.Instance.formulas.Count, uiFormulaElement);

		for (int i = 0; i < elements.Length; i++)
		{
			UIFormulaElement element = elements[i].GetComponent<UIFormulaElement>();
			FormulaData formula = GameManager.Instance.formulas[i];
			element.SetFormula(formula);
			element.GetComponent<Toggle>().group = uiFormulaRoot;
			element.GetComponent<Toggle>().onValueChanged.AddListener((value) => { if (value) UpdateInfo(formula); });
		}
		if (m_formula != null) UpdateInfo(m_formula);
	}

	public void UpdateInfo(FormulaData formula)
	{
		m_formula = formula;
		product_Name.text = formula.product.ItemName;
		product_Desc.text = formula.product.Description;
		for (int i = 0; i < m_requireItemBoxs.Length; i++)
		{
			if (formula.requireList.Count > i) m_requireItemBoxs[i].SetRequiredInfo(formula.requireList[i].Key, formula.requireList[i].Value);
			else m_requireItemBoxs[i].gameObject.SetActive(false);
		}
	}

	public void Craft()
	{
		m_formula.Craft(GameManager.CurHero.Inventory);
	}
}
