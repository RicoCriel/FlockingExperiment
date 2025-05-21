using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonController : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public Action OnButtonHeld;
    public Action OnButtonReleased;

    public void OnPointerDown(PointerEventData eventData)
    {
        OnButtonHeld?.Invoke();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        OnButtonReleased?.Invoke();
    }
}
