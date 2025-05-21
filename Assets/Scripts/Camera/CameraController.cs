using System;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private GameObject _target;
    [Range(0, 50f)]
    [SerializeField] private float _rotationSpeed;
    private bool _isRotating;

    private void Update()
    {
        if (_isRotating && _target)
            _target.transform.Rotate(Vector3.up, _rotationSpeed * Time.deltaTime);
    }

    public void RotateTarget() => _isRotating = true;
    public void StopRotating() => _isRotating = false;
}

