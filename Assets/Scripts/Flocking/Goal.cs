using UnityEngine;
using UnityEngine.InputSystem;

public class Goal : MonoBehaviour
{
    private Camera _camera;
    private Bounds _bounds;

    private void Awake()
    {
        _camera = Camera.main;
        _bounds = new Bounds();
    }

    void Update()
    {
        UpdatePosition();
    }

    private void UpdatePosition()
    {
        Vector2 mouseScreenPos = Mouse.current.position.ReadValue();
        float zDistance = Mathf.Abs(_camera.transform.position.z - transform.position.z);
        Vector3 screenPosition = new Vector3(mouseScreenPos.x, mouseScreenPos.y, zDistance);
        Vector3 worldPosition = _camera.ScreenToWorldPoint(screenPosition);
        transform.position = worldPosition;
    }
}
