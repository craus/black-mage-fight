using UnityEngine;
using System.Collections;
using RSG;
using System;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class InstantButton : MonoBehaviour, IPointerDownHandler
{
	public UnityEvent onMouseDown;

	public void OnPointerDown(PointerEventData eventData)
	{
		onMouseDown.Invoke();
	}
}