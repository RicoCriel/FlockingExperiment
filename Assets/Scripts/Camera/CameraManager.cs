using Unity.Cinemachine;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    [SerializeField] private CinemachineCamera _fixedCamera;
    [SerializeField] private CinemachineCamera _freelookCamera;

    private bool _isFreeLookActive = false;

    private void Awake()
    {
        InitializeCameras();
    }

    private void InitializeCameras()
    {
        _freelookCamera.Priority = 10; 
        _fixedCamera.Priority = 30;    
    }

    public void SwitchCamera()
    {
        _isFreeLookActive = !_isFreeLookActive;

        if (_isFreeLookActive)
        {
            _freelookCamera.Priority = 30; // Make active
            _fixedCamera.Priority = 10;    // Make inactive
        }
        else
        {
            _fixedCamera.Priority = 30;    
            _freelookCamera.Priority = 10; 
        }
    }
}
