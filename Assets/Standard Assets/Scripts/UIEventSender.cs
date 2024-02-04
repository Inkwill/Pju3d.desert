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
			}
		}
	}
	public void OnPointerClick(PointerEventData eventData)
	{
		if (!m_longpress) OnClick?.Invoke();
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
