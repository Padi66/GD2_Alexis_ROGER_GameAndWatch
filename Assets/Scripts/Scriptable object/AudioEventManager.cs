using UnityEngine;

public class AudioEventManager : MonoBehaviour
{
    [SerializeField] private AudioEventDispatcher _audioEventDispatcher;
    [SerializeField] private AudioSource _fxAudioSource;
    [SerializeField] private AudioSource _musicAudioSource;

    private void OnEnable()
    {
        _audioEventDispatcher.OnAudioEvent += PlayAudioFX;
        _audioEventDispatcher.OnMusicEvent += PlayMusic;
    }

    private void OnDisable()
    {
        _audioEventDispatcher.OnAudioEvent -= PlayAudioFX;
        _audioEventDispatcher.OnMusicEvent -= PlayMusic;
    }

    private void PlayAudioFX(AudioClip clip, float volume)
    {
        _fxAudioSource.PlayOneShot(clip, volume);
    }

    private void PlayMusic(AudioClip clip)
    {
        if (_musicAudioSource.clip == clip && _musicAudioSource.isPlaying)
            return;

        _musicAudioSource.clip = clip;
        _musicAudioSource.loop = true;
        _musicAudioSource.Play();
    }
}