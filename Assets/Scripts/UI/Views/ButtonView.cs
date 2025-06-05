using UnityEngine;
using UnityEngine.EventSystems;
using System;

public class ButtonView : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler,IPointerClickHandler
{
    [SerializeField] private string _enterMessage;

    private ToolTipController _toolTipController;

    public void Initialize(ToolTipController controller)
    {
        _toolTipController = controller;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        _toolTipController.HideToolTips();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        _toolTipController.ShowToolTips();
        _toolTipController.DisplayMessage(_enterMessage);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _toolTipController.HideToolTips();
    }


}
