using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>À placer sur le DialoguePanel. Transmet les clics/appuis au DialogueSystem.</summary>
public class DialogueClickForwarder : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private DialogueSystem _dialogueSystem;

    public void OnPointerClick(PointerEventData eventData)
    {
        _dialogueSystem?.OnPointerClick(eventData);
    }
}
