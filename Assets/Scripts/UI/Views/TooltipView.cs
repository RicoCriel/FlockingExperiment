using UnityEngine;
using TMPro;

public class TooltipView: MonoBehaviour
{
    [Header("Tooltip UI components")]
    [SerializeField] private TMP_Text _toolTip;
    [SerializeField] private GameObject _toolTipPanel;

    public void DisplayToolTip(string message)
    {
        _toolTipPanel.SetActive(true);
        _toolTip.text = message;
    }

    public void HideToolTip()
    {
        _toolTipPanel.SetActive(false);
    }
}
