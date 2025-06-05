using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class SliderController : MonoBehaviour
{
    [Header("Sliders")]
    [SerializeField] private Slider _fishSlider;
    [SerializeField] private Slider _alignmentSlider;
    [SerializeField] private Slider _cohesionSlider;
    [SerializeField] private Slider _separationSlider;
    [SerializeField] private Slider _diverAttraction;
    [SerializeField] private Slider _diverSpeedSlider;
    [SerializeField] private Slider _mouseSensitivitySlider;
    [Header("Behaviour Scripts References")]
    [SerializeField] private PlayerController _controller;
    [SerializeField] private FlockingManager _manager;

    [System.Serializable]
    public class FloatEvent : UnityEvent<float> { }

    public FloatEvent OnFishCountChanged = new FloatEvent();
    public FloatEvent OnAlignmentChanged = new FloatEvent();
    public FloatEvent OnCohesionChanged = new FloatEvent();
    public FloatEvent OnSeparationChanged = new FloatEvent();
    public FloatEvent OnDiverAttractionChanged = new FloatEvent();

    public FloatEvent OnDiverSpeedChanged = new FloatEvent();
    public FloatEvent OnMouseSensitivityChanged = new FloatEvent();

    private void Start()
    {
        // Setup slider ranges and initial values
        InitializeFishSlider();
        InitializeSlider(_alignmentSlider, _manager.AlignmentRange, _manager.AlignmentWeight);
        InitializeSlider(_cohesionSlider, _manager.CohesionRange, _manager.CohesionWeight);
        InitializeSlider(_separationSlider, _manager.SeparationRange, _manager.SeparationWeight);
        InitializeSlider(_diverAttraction, _manager.DiverAttractionRange, _manager.GoalAttractionStrength);

        InitializeSlider(_diverSpeedSlider, _controller.MoveSpeedRange, _controller.MoveSpeed);
        InitializeSlider(_mouseSensitivitySlider, _controller.MouseSensitivityRange, _controller.MouseSensitivity);

        // Add listeners after initialization
        _fishSlider.onValueChanged.AddListener(HandleFishCountChanged);
        _alignmentSlider.onValueChanged.AddListener(HandleAlignmentChanged);
        _cohesionSlider.onValueChanged.AddListener(HandleCohesionChanged);
        _separationSlider.onValueChanged.AddListener(HandleSeparationChanged);
        _diverAttraction.onValueChanged.AddListener(HandleDiverAttractionChanged);

        _diverSpeedSlider.onValueChanged.AddListener(HandleDiverSpeedChanged);
        _mouseSensitivitySlider.onValueChanged.AddListener(HandleMouseSensititvyChanged);
    }

    private void InitializeSlider(Slider slider, (float min, float max) range, float value)
    {
        slider.minValue = range.min;
        slider.maxValue = range.max;
        slider.SetValueWithoutNotify(value);
    }

    private void InitializeFishSlider()
    {
        _fishSlider.minValue = 0;
        _fishSlider.maxValue = 10000;
        _fishSlider.wholeNumbers = true;
        _fishSlider.SetValueWithoutNotify(_manager.MaxPopulation);
        _fishSlider.onValueChanged.AddListener(HandleFishCountChanged);
    }

    private void HandleFishCountChanged(float value)
    {
        // Round to nearest integer for fish count
        int intValue = Mathf.RoundToInt(value);
        OnFishCountChanged?.Invoke(intValue);
    }
    private void HandleAlignmentChanged(float value) => OnAlignmentChanged?.Invoke(value);
    private void HandleCohesionChanged(float value) => OnCohesionChanged?.Invoke(value);
    private void HandleSeparationChanged(float value) => OnSeparationChanged?.Invoke(value);
    private void HandleDiverAttractionChanged(float value) => OnDiverAttractionChanged?.Invoke(value);
    private void HandleDiverSpeedChanged(float value) => OnDiverSpeedChanged?.Invoke(value);
    private void HandleMouseSensititvyChanged(float value) => OnMouseSensitivityChanged?.Invoke(value);
}