using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class ButtonView : MonoBehaviour
{
    [SerializeField] private Button _controlsButton;
    [SerializeField] private GameObject _controlsPanel;

    private bool _buttonClicked;

    private void OnEnable()
    {
        _controlsButton.onClick.AddListener(ToggleControlPanel);
    }

    private void OnDisable()
    {
        _controlsButton.onClick.RemoveListener(ToggleControlPanel);
    }

    private void ToggleControlPanel()
    {
        _buttonClicked = !_buttonClicked;
        _controlsPanel.SetActive(_buttonClicked);
    }


}
