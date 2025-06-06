using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonController : MonoBehaviour
{
    [Header("UI Buttons")]
    [SerializeField] private Button _settingsButton;
    [SerializeField] private Button _diverSettingsButton;
    [SerializeField] private Button _cameraSwitchButton;
    [Header("UI Panels")]
    [SerializeField] private GameObject _settingsPanel;
    [SerializeField] private GameObject _diverSettingsPanel;
    [SerializeField] private GameObject _fpsCounterPanel;

    public UnityEvent OnCameraButtonClicked;

    private void OnEnable()
    {
        _settingsButton.onClick.AddListener(ToggleSettingsPanel);
        _diverSettingsButton.onClick.AddListener(ToggleDiverSettingsPanel);
        _cameraSwitchButton.onClick.AddListener(ToggleCameraPov);
    }

    private void OnDisable()
    {
        _settingsButton.onClick.RemoveListener(ToggleSettingsPanel);
        _diverSettingsButton.onClick.RemoveListener(ToggleDiverSettingsPanel);
        _cameraSwitchButton.onClick.RemoveListener(ToggleCameraPov);
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

    private void ToggleCameraPov()
    {
        OnCameraButtonClicked?.Invoke();
    }

    public void ToggleFpsCounter()
    {
        bool shouldOpen = !_fpsCounterPanel.activeSelf;

        _fpsCounterPanel.SetActive(shouldOpen);
    }
}
