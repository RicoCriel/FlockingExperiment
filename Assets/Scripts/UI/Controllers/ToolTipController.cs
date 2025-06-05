using UnityEngine;
using UnityEngine.InputSystem;

public class ToolTipController : MonoBehaviour
{
    [SerializeField] private TooltipView _tooltipView;
    [SerializeField] private GameObject _toolTipElement;
    private bool _isEnabled = false;

    public void SetEnabled(bool enabled) => _isEnabled = enabled;

    public void ShowToolTips()
    {
        _toolTipElement.SetActive(true);
    }

    public void HideToolTips()
    {
        _toolTipElement.SetActive(false);
        _tooltipView.HideToolTip();
    }

    public void UpdatePosition()
    {
        if (!_isEnabled) return;
        transform.position = Input.mousePosition;
    }

    public void DisplayMessage(string message)
    {
        if (!_isEnabled) return;
        _tooltipView.DisplayToolTip(message);
    }
}
