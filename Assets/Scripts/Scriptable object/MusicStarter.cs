using UnityEngine;

/// <summary>Déclenche la musique de fond au démarrage de la scène via l'AudioEventDispatcher.</summary>
public class MusicStarter : MonoBehaviour
{
    [SerializeField] private AudioEventDispatcher _audioEventDispatcher;

    private void Start()
    {
        _audioEventDispatcher?.PlayAudio(AudioType.BackgroundMusic);
    }
}
