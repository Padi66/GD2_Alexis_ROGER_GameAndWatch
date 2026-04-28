using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

[Serializable]
public struct DialogueLine
{
    [TextArea(2, 5)]
    public string text;
    public string speakerName;
    public Sprite speakerPortrait;
}

public class DialogueSystem : MonoBehaviour, IPointerClickHandler
{
    [Header("UI")]
    public GameObject dialoguePanel;
    public TextMeshProUGUI speakerNameText;
    public TextMeshProUGUI dialogueText;
    public Image speakerPortraitImage;

    [Header("Timing")]
    [Tooltip("Durée d'affichage de chaque ligne (secondes).")]
    public float displayDuration = 3f;
    [Tooltip("Délai avant le premier dialogue.")]
    public float initialDelay = 0.5f;
    [Tooltip("Vitesse de frappe (caractères par seconde).")]
    public float typingSpeed = 40f;

    public event Action OnSequenceComplete;

    private Coroutine _sequenceCoroutine;
    private bool _skipRequested = false;
    private bool _isTyping      = false;
    private string _currentFullText = string.Empty;

    private void Awake()
    {
        if (dialoguePanel != null)
            dialoguePanel.SetActive(false);
    }

    /// <summary>Appelé par le système d'événements Unity quand l'utilisateur clique/touche le panel.</summary>
    public void OnPointerClick(PointerEventData eventData)
    {
        _skipRequested = true;
    }

    /// <summary>Démarre une séquence de dialogues. Déclenche OnSequenceComplete et onComplete à la fin.</summary>
    public void StartDialogue(DialogueLine[] lines, Action onComplete = null)
    {
        if (lines == null || lines.Length == 0)
        {
            onComplete?.Invoke();
            OnSequenceComplete?.Invoke();
            return;
        }

        if (_sequenceCoroutine != null)
            StopCoroutine(_sequenceCoroutine);

        _sequenceCoroutine = StartCoroutine(RunSequence(lines, onComplete));
    }

    /// <summary>Arrête le dialogue en cours et ferme le panel.</summary>
    public void StopDialogue()
    {
        if (_sequenceCoroutine != null)
        {
            StopCoroutine(_sequenceCoroutine);
            _sequenceCoroutine = null;
        }

        CloseDialogue();
    }

    private IEnumerator RunSequence(DialogueLine[] lines, Action onComplete)
    {
        if (speakerNameText != null) speakerNameText.text = string.Empty;
        if (dialogueText    != null) dialogueText.text    = string.Empty;
        if (dialoguePanel   != null) dialoguePanel.SetActive(true);

        yield return new WaitForSeconds(initialDelay);

        foreach (DialogueLine line in lines)
        {
            _skipRequested = false;

            yield return StartCoroutine(DisplayLine(line));

            // Si le typewriter n'est pas encore terminé, afficher le texte complet
            if (_isTyping)
            {
                _isTyping = false;
                if (dialogueText != null)
                    dialogueText.text = _currentFullText;
                _skipRequested = false;

                // Attendre un second appui pour passer à la ligne suivante
                yield return new WaitUntil(() => _skipRequested);
            }
            else
            {
                // Attendre soit la durée automatique soit un appui
                float elapsed = 0f;
                while (elapsed < displayDuration && !_skipRequested)
                {
                    elapsed += Time.deltaTime;
                    yield return null;
                }
            }

            _skipRequested = false;
        }

        CloseDialogue();
        onComplete?.Invoke();
        OnSequenceComplete?.Invoke();
    }

    private IEnumerator DisplayLine(DialogueLine line)
    {
        if (speakerNameText != null)
            speakerNameText.text = line.speakerName;

        if (speakerPortraitImage != null)
        {
            speakerPortraitImage.sprite = line.speakerPortrait;
            speakerPortraitImage.gameObject.SetActive(line.speakerPortrait != null);
        }

        if (dialogueText != null)
        {
            _currentFullText     = line.text;
            dialogueText.text    = string.Empty;
            float delay          = 1f / typingSpeed;
            _isTyping            = true;

            foreach (char c in line.text)
            {
                if (_skipRequested)
                {
                    _isTyping = false;
                    yield break;
                }

                dialogueText.text += c;
                yield return new WaitForSeconds(delay);
            }

            _isTyping = false;
        }
    }

    private void CloseDialogue()
    {
        if (dialoguePanel != null)
            dialoguePanel.SetActive(false);
    }
}
