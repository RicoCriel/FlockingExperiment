using UnityEngine;
using UnityEngine.EventSystems;
using System;

public class ButtonView : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private string _enterMessage;
    [SerializeField] private string _exitMessage;
    [SerializeField] private bool _shouldBeHighlighted;

    public Action OnCursorEnter;
    public Action OnCursorExit;

    private void Awake()
    {
        if(_shouldBeHighlighted)
        {

        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        //To Do: Show ToolTip
        //Debug.Log(_enterMessage);
        OnCursorEnter?.Invoke();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        //To Do: Hide ToolTip
        //Debug.Log(_exitMessage);
        OnCursorExit?.Invoke();
    }


}
