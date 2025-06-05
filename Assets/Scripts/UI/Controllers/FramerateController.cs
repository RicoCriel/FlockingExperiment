using System.Collections;
using UnityEngine;

public class FramerateController: MonoBehaviour
{
    private FramerateView _view;
    private Coroutine _fpsCoroutine;

    private float _fpsCount;

    public float FpsCount => _fpsCount;

    private void Awake()
    {
        _view = GetComponent<FramerateView>();
    }

    private void OnEnable()
    {
        _fpsCoroutine = StartCoroutine(UpdateFps());
    }

    private void OnDisable()
    {
        if (_fpsCoroutine != null)
            StopCoroutine(_fpsCoroutine);
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
