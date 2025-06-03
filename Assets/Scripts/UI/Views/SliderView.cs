using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SliderView : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _fishCount;
    [SerializeField] private TextMeshProUGUI _alignmentFactor;
    [SerializeField] private TextMeshProUGUI _cohesionFactor;
    [SerializeField] private TextMeshProUGUI _separationFactor;
    [SerializeField] private TextMeshProUGUI _diverAttractionFactor;

    [SerializeField] private Slider _fishSlider;
    [SerializeField] private Slider _alignmentSlider;
    [SerializeField] private Slider _cohesionSlider;
    [SerializeField] private Slider _separationSlider;
    [SerializeField] private Slider _diverAttractionSlider;

    [SerializeField] private FlockingManager _manager;

    private void Start()
    {
        // Initialize with manager values (not slider values)
        UpdateFishCountDisplay(_manager.MaxPopulation);
        UpdateAlignmentDisplay(_manager.AlignmentWeight);
        UpdateCohesionDisplay(_manager.CohesionWeight);
        UpdateSeparationDisplay(_manager.SeparationWeight);
        UpdateDiverAttractionDisplay(_manager.GoalAttractionStrength);

        // Add listeners
        _fishSlider.onValueChanged.AddListener(UpdateFishCountDisplay);
        _alignmentSlider.onValueChanged.AddListener(UpdateAlignmentDisplay);
        _cohesionSlider.onValueChanged.AddListener(UpdateCohesionDisplay);
        _separationSlider.onValueChanged.AddListener(UpdateSeparationDisplay);
        _diverAttractionSlider.onValueChanged.AddListener(UpdateDiverAttractionDisplay);
    }

    public void UpdateFishCountDisplay(float value) => _fishCount.text = value.ToString("N0");
    public void UpdateAlignmentDisplay(float value) => _alignmentFactor.text = value.ToString("F1");
    public void UpdateCohesionDisplay(float value) => _cohesionFactor.text = value.ToString("F1");
    public void UpdateSeparationDisplay(float value) => _separationFactor.text = value.ToString("F1");
    public void UpdateDiverAttractionDisplay(float value) => _diverAttractionFactor.text = value.ToString("F1");
}