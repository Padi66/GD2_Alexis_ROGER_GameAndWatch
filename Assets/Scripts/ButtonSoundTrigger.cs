using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// Joue le son ButtonClick dès l'appui sur le bouton (pointer down),
/// plus tôt que onClick qui se déclenche au relâchement.
/// </summary>
[RequireComponent(typeof(Canvas))]
public class ButtonSoundTrigger : MonoBehaviour
{
    [SerializeField] private AudioEventDispatcher _audioEventDispatcher;

    private void Start()
    {
        foreach (Button button in GetComponentsInChildren<Button>(includeInactive: true))
        {
            ButtonSoundHandler handler = button.gameObject.GetComponent<ButtonSoundHandler>();
            if (handler == null)
                handler = button.gameObject.AddComponent<ButtonSoundHandler>();

            handler.Initialize(_audioEventDispatcher);
        }
    }
}