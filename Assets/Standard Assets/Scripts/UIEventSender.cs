using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class UIEventSender : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler
//, IPointerEnterHandler, IPointerExitHandler,IBeginDragHandler, IDragHandler, IEndDragHandler
{
	public float longPressDuration = 1.0f;
	public bool canClick { get { return m_canClick; } set { m_canClick = value; } }
	bool m_canClick = true;
	float pressTime;
	bool m_longpress;

	public UnityEvent OnClick;
	public UnityEvent OnLonePress;
	private void Update()
	{
		if (pressTime > 0)
		{
			pressTime -= Time.deltaTime;

			if (pressTime <= 0)
			{
				// 执行长按操作
				OnLonePress?.Invoke();
				m_longpress = true;
				pressTime = 0; // 重置长按持续时间
				return;
			}
		}
		OnUpdate();
	}

	protected virtual void OnUpdate() { }
	public void OnPointerClick(PointerEventData eventData)
	{
		if (!m_longpress && m_canClick) OnClick?.Invoke();
	}

	public void OnPointerDown(PointerEventData eventData)
	{
		pressTime = longPressDuration;
		m_longpress = false;
	}

	public void OnPointerUp(PointerEventData eventData)
	{
		pressTime = 0;
	}
	// public void OnPointerEnter(PointerEventData eventData)
	// {
	// 	Debug.Log("OnPointerEnter!");
	// }

	// public void OnPointerExit(PointerEventData eventData)
	// {
	// 	Debug.Log("OnPointerExit!");
	// }
	// public void OnBeginDrag(PointerEventData eventData)
	// {
	// 	Debug.Log("OnBeginDrag!");
	// }

	// public void OnDrag(PointerEventData eventData)
	// {
	// 	Debug.Log("OnDrag!");
	// }
	// public void OnEndDrag(PointerEventData eventData)
	// {
	// 	Debug.Log("OnEndDrag!");
	// }
}
