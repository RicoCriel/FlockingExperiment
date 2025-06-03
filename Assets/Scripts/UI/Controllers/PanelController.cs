using UnityEngine;

public class PanelController : MonoBehaviour
{
    [SerializeField] private GameObject _unlockPanel;
    [SerializeField] private GameObject _controlDiverPanel;
    [SerializeField] private GameObject[] _allUiElements; 

    public void ShowUnlockPanel()
    {
        _unlockPanel.SetActive(true);
    }

    public void HideUnlockPanel()
    {
        _unlockPanel.SetActive(false);
    }

    public void ShowControlDiverPanel()
    {
        _controlDiverPanel.SetActive(true);
    }

    public void HideControlDiverPanel()
    {
        _controlDiverPanel.SetActive(false);
    }
}
