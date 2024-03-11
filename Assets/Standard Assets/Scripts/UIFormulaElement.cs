using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIFormulaElement : MonoBehaviour
{
	[SerializeField]
	Image product_icon;
	FormulaData m_formula;

	public void SetFormula(FormulaData data)
	{
		m_formula = data;
		product_icon.sprite = data.product.ItemSprite;
	}
}
