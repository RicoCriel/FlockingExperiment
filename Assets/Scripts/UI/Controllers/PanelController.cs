using System.Collections.Generic;
using UnityEngine;

public class PanelController : MonoBehaviour
{
    [SerializeField] private GameObject _unlockPanel;
    [SerializeField] private GameObject _controlDiverPanel;
    [SerializeField] private GameObject _icons;

    [SerializeField] private GameObject _simulationSettingsPanel;
    [SerializeField] private GameObject _diverSettingsPanel;

    private List<GameObject> _settingPanels;

    private void Awake()
    {
        _settingPanels = new List<GameObject>();
    }

    private void OnEnable()
    {
        _settingPanels.Add(_simulationSettingsPanel);
        _settingPanels.Add(_diverSettingsPanel);
    }

    private void OnDisable()
    {
        _settingPanels.Clear();
    }

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

    public void HideIcons()
    {
        _icons.SetActive(false);
    }

    public void ShowIcons()
    {
        _icons.SetActive(true);
    }

    public void HideActiveSettingPanels()
    {
        foreach(var panel in _settingPanels)
        {
            if(panel.activeInHierarchy)
            {
                panel.SetActive(false);
            }
        }
    }
}
