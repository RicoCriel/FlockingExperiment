using UnityEngine;

[RequireComponent(typeof (AudioSource))]
public class AudioManager : MonoBehaviour
{
    [SerializeField] private AudioClip _audioClip;
    private AudioSource _audioSource;

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
        Play();
    }

    private void Play()
    {
        if(_audioClip)
        {
            _audioSource.clip = _audioClip;
            _audioSource.Play();
        }
    }
}
