using System.Collections;
using UnityEngine;

public class FramerateController: MonoBehaviour
{
    private FramerateView _view;
    private float _fpsCount;

    public float FpsCount => _fpsCount;

    private void Awake()
    {
        _view = GetComponent<FramerateView>();
    }

    private void Start()
    {
        StartCoroutine(UpdateFps());
    }

    private IEnumerator UpdateFps()
    {
        while (true)
        {
            _fpsCount = 1f / Time.unscaledDeltaTime;
            _view.UpdateFpsText(FpsCount);
            _view.UpdateFpsColor(FpsCount);
            yield return new WaitForSeconds(0.1f);
        }
    }
}
