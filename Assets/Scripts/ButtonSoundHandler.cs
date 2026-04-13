using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// Composant ajouté dynamiquement sur chaque bouton.
/// Joue le son dès le PointerDown, avant le relâchement.
/// </summary>
public class ButtonSoundHandler : MonoBehaviour, IPointerDownHandler
{
    private AudioEventDispatcher _audioEventDispatcher;

    /// <summary>Initialise la référence au dispatcher audio.</summary>
    public void Initialize(AudioEventDispatcher dispatcher)
    {
        _audioEventDispatcher = dispatcher;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        _audioEventDispatcher?.PlayAudio(AudioType.ButtonClick);
    }
}