using UnityEngine;
using UnityEngine.UI;

public class ButtonView : MonoBehaviour
{
    [SerializeField] private Button _settingsButton;
    [SerializeField] private Button _diverSettingsButton;
    [SerializeField] private GameObject _settingsPanel;
    [SerializeField] private GameObject _diverSettingsPanel;

    private void OnEnable()
    {
        _settingsButton.onClick.AddListener(ToggleSettingsPanel);
        _diverSettingsButton.onClick.AddListener(ToggleDiverSettingsPanel);
    }

    private void OnDisable()
    {
        _settingsButton.onClick.RemoveListener(ToggleSettingsPanel);
        _diverSettingsButton.onClick.RemoveListener(ToggleDiverSettingsPanel);
    }

    private void ToggleSettingsPanel()
    {
        bool shouldOpen = !_settingsPanel.activeSelf;

        _settingsPanel.SetActive(shouldOpen);
        _diverSettingsPanel.SetActive(false); 
    }

    private void ToggleDiverSettingsPanel()
    {
        bool shouldOpen = !_diverSettingsPanel.activeSelf;

        _diverSettingsPanel.SetActive(shouldOpen);
        _settingsPanel.SetActive(false); 
    }


}
