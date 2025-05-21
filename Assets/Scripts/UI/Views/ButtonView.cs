using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class ButtonView : MonoBehaviour
{
    [SerializeField] private ButtonController _orbit;

    [SerializeField] private Button _controlsButton;
    [SerializeField] private GameObject _controlsPanel;
    [SerializeField] private CameraController _cameraController;

    private bool _buttonClicked;

    private void OnEnable()
    {
        _orbit.OnCursorEnter += _cameraController.RotateTarget;
        _orbit.OnCursorExit += _cameraController.StopRotating;
        _controlsButton.onClick.AddListener(ToggleControlPanel);
    }

    private void OnDisable()
    {
        _orbit.OnCursorEnter -= _cameraController.RotateTarget;
        _orbit.OnCursorExit -= _cameraController.StopRotating;
        _controlsButton.onClick.RemoveListener(ToggleControlPanel);
    }

    private void ToggleControlPanel()
    {
        _buttonClicked = !_buttonClicked;
        _controlsPanel.SetActive(_buttonClicked);
    }


}
