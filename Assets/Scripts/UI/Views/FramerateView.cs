using System.Collections;
using UnityEngine;
using TMPro;

public class FramerateView : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _fpsText;
    private Color _lowFrameRateColor = Color.red;
    private Color _highFrameRateColor = Color.green;

    public void UpdateFpsText(float fpsCount)
    {
        _fpsText.text = Mathf.Round(fpsCount).ToString();
    }

    public void UpdateFpsColor(float fpsCount)
    {
        float t = Mathf.InverseLerp(15f, 60f, fpsCount);
        _fpsText.color = Color.Lerp(_lowFrameRateColor, _highFrameRateColor, t);
    }
}
