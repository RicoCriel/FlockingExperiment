using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SliderView : MonoBehaviour
{
    [Header("Simulation settings UI text")]
    [SerializeField] private TextMeshProUGUI _fishCount;
    [SerializeField] private TextMeshProUGUI _alignmentFactor;
    [SerializeField] private TextMeshProUGUI _cohesionFactor;
    [SerializeField] private TextMeshProUGUI _separationFactor;
    [SerializeField] private TextMeshProUGUI _diverAttractionFactor;
    [Header("Diver setting UI text")]
    [SerializeField] private TextMeshProUGUI _diverMoveSpeed;
    [SerializeField] private TextMeshProUGUI _mouseSensitivity;
    [Header("Simulation setting sliders")]
    [SerializeField] private Slider _fishSlider;
    [SerializeField] private Slider _alignmentSlider;
    [SerializeField] private Slider _cohesionSlider;
    [SerializeField] private Slider _separationSlider;
    [SerializeField] private Slider _diverAttractionSlider;
    [Header("Movement setting sliders")]
    [SerializeField] private Slider _moveSpeedSlider;
    [SerializeField] private Slider _mouseSensitivitySlider;

    // Dirty but didnt want to overengineer 
    [SerializeField] private FlockingManager _manager;
    [SerializeField] private PlayerController _controller;

    private void Start()
    {
        // Initialize with inspector values
        UpdateFishCountDisplay(_manager.MaxPopulation);
        UpdateAlignmentDisplay(_manager.AlignmentWeight);
        UpdateCohesionDisplay(_manager.CohesionWeight);
        UpdateSeparationDisplay(_manager.SeparationWeight);
        UpdateDiverAttractionDisplay(_manager.GoalAttractionStrength);

        UpdateDiverMoveSpeedDisplay(_controller.MoveSpeed);
        UpdateMouseSensitivityDisplay(_controller.MouseSensitivity);

        // Add listeners
        _fishSlider.onValueChanged.AddListener(value => {
            UpdateFishCountDisplay(Mathf.RoundToInt(value));
        });
        _alignmentSlider.onValueChanged.AddListener(UpdateAlignmentDisplay);
        _cohesionSlider.onValueChanged.AddListener(UpdateCohesionDisplay);
        _separationSlider.onValueChanged.AddListener(UpdateSeparationDisplay);
        _diverAttractionSlider.onValueChanged.AddListener(UpdateDiverAttractionDisplay);

        _moveSpeedSlider.onValueChanged.AddListener(UpdateDiverMoveSpeedDisplay);
        _mouseSensitivitySlider.onValueChanged.AddListener(UpdateMouseSensitivityDisplay);

    }

    private void OnDisable()
    {
        // Clean up listeners
        _fishSlider.onValueChanged.RemoveListener(value => {
            UpdateFishCountDisplay(Mathf.RoundToInt(value));
        });
        _alignmentSlider.onValueChanged.RemoveListener(UpdateAlignmentDisplay);
        _cohesionSlider.onValueChanged.RemoveListener(UpdateCohesionDisplay);
        _separationSlider.onValueChanged.RemoveListener(UpdateSeparationDisplay);
        _diverAttractionSlider.onValueChanged.RemoveListener(UpdateDiverAttractionDisplay);

        _moveSpeedSlider.onValueChanged.RemoveListener(UpdateDiverMoveSpeedDisplay);
        _mouseSensitivitySlider.onValueChanged.RemoveListener(UpdateMouseSensitivityDisplay);
    }

    public void UpdateFishCountDisplay(float value)
    {
        // Cast to int for display
        _fishCount.text = Mathf.RoundToInt(value).ToString("N0");
    }
    public void UpdateAlignmentDisplay(float value) => _alignmentFactor.text = value.ToString("F1");
    public void UpdateCohesionDisplay(float value) => _cohesionFactor.text = value.ToString("F1");
    public void UpdateSeparationDisplay(float value) => _separationFactor.text = value.ToString("F1");
    public void UpdateDiverAttractionDisplay(float value) => _diverAttractionFactor.text = value.ToString("F1");

    public void UpdateDiverMoveSpeedDisplay(float value) => _diverMoveSpeed.text = value.ToString("F1");
    public void UpdateMouseSensitivityDisplay(float value) => _mouseSensitivity.text = value.ToString("F1");
}