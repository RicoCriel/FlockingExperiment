using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class SliderController : MonoBehaviour
{
    [System.Serializable]
    public class FloatEvent : UnityEvent<float> { }

    public FloatEvent OnFishCountChanged = new FloatEvent();
    public FloatEvent OnAlignmentChanged = new FloatEvent();
    public FloatEvent OnCohesionChanged = new FloatEvent();
    public FloatEvent OnSeparationChanged = new FloatEvent();
    public FloatEvent OnDiverAttractionChanged = new FloatEvent();

    [SerializeField] private Slider _fishSlider;
    [SerializeField] private Slider _alignmentSlider;
    [SerializeField] private Slider _cohesionSlider;
    [SerializeField] private Slider _separationSlider;
    [SerializeField] private Slider _diverAttraction;
    [SerializeField] private FlockingManager _manager;

    private void Start()
    {
        // Setup slider ranges and initial values
        InitializeSlider(_fishSlider, _manager.FishCountRange, _manager.MaxPopulation);
        InitializeSlider(_alignmentSlider, _manager.AlignmentRange, _manager.AlignmentWeight);
        InitializeSlider(_cohesionSlider, _manager.CohesionRange, _manager.CohesionWeight);
        InitializeSlider(_separationSlider, _manager.SeparationRange, _manager.SeparationWeight);
        InitializeSlider(_diverAttraction, _manager.DiverAttractionRange, _manager.GoalAttractionStrength);

        // Add listeners after initialization
        _fishSlider.onValueChanged.AddListener(HandleFishCountChanged);
        _alignmentSlider.onValueChanged.AddListener(HandleAlignmentChanged);
        _cohesionSlider.onValueChanged.AddListener(HandleCohesionChanged);
        _separationSlider.onValueChanged.AddListener(HandleSeparationChanged);
        _diverAttraction.onValueChanged.AddListener(HandleDiverAttractionChanged);
    }

    private void InitializeSlider(Slider slider, (float min, float max) range, float value)
    {
        slider.minValue = range.min;
        slider.maxValue = range.max;
        slider.SetValueWithoutNotify(value);
    }

    private void HandleFishCountChanged(float value) => OnFishCountChanged?.Invoke(value);
    private void HandleAlignmentChanged(float value) => OnAlignmentChanged?.Invoke(value);
    private void HandleCohesionChanged(float value) => OnCohesionChanged?.Invoke(value);
    private void HandleSeparationChanged(float value) => OnSeparationChanged?.Invoke(value);
    private void HandleDiverAttractionChanged(float value) => OnDiverAttractionChanged?.Invoke(value);
}