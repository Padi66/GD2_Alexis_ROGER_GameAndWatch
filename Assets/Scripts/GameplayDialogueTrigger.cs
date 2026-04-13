using UnityEngine;

/// <summary>
/// Orchestre les 3 séquences de dialogue : intro après Start,
/// phrases déclenchées par seuils de vie du boss, et dialogue quand le boss est vaincu.
/// </summary>
public class GameplayDialogueTrigger : MonoBehaviour
{
    [Header("Références")]
    [SerializeField] private DialogueSystem _dialogueSystem;
    [SerializeField] private hackGame _hackGame;
    [SerializeField] private BossVictoryEffect _bossVictoryEffect;

    [Header("Séquence 1 — Intro (après Start)")]
    public DialogueLine[] introLines;

    [Header("Séquence 2 — Pendant le combat (seuils de vie du boss)")]
    [Tooltip("Déclenché quand le boss a perdu 20% de sa vie (80% restant).")]
    public DialogueLine[] phase1Lines;
    [Tooltip("Déclenché quand le boss est à 50% de sa vie.")]
    public DialogueLine[] phase2Lines;
    [Tooltip("Déclenché quand le boss a perdu 85% de sa vie (15% restant).")]
    public DialogueLine[] phase3Lines;

    [Header("Séquence 3 — Boss vaincu")]
    public DialogueLine[] bossDefeatedLines;

    private const float Phase1Threshold = 0.80f;
    private const float Phase2Threshold = 0.50f;
    private const float Phase3Threshold = 0.15f;

    private bool _phase1Triggered = false;
    private bool _phase2Triggered = false;
    private bool _phase3Triggered = false;

    /// <summary>Appelé par hackGame quand le joueur clique Start.</summary>
    public void OnGameStart()
    {
        _phase1Triggered = false;
        _phase2Triggered = false;
        _phase3Triggered = false;

        _dialogueSystem.StartDialogue(introLines, () =>
        {
            _hackGame.LaunchGame();
        });
    }

    /// <summary>Appelé par hackGame après chaque dégât sur le boss.</summary>
    public void OnEnemyHealthChanged(int currentHealth, int maxHealth)
    {
        float ratio = (float)currentHealth / maxHealth;

        if (!_phase1Triggered && ratio <= Phase1Threshold)
        {
            _phase1Triggered = true;
            _dialogueSystem.StartDialogue(phase1Lines);
            return;
        }

        if (!_phase2Triggered && ratio <= Phase2Threshold)
        {
            _phase2Triggered = true;
            _dialogueSystem.StartDialogue(phase2Lines);
            return;
        }

        if (!_phase3Triggered && ratio <= Phase3Threshold)
        {
            _phase3Triggered = true;
            _dialogueSystem.StartDialogue(phase3Lines);
        }
    }

    /// <summary>Appelé par hackGame quand le boss est vaincu.</summary>
    public void OnBossDefeated()
    {
        _dialogueSystem.StopDialogue();

        if (bossDefeatedLines == null || bossDefeatedLines.Length == 0)
        {
            _bossVictoryEffect?.TriggerBossFall();
            return;
        }

        // Joue toutes les lignes sauf la dernière normalement,
        // puis déclenche la chute au début de la dernière ligne.
        if (bossDefeatedLines.Length == 1)
        {
            _dialogueSystem.StartDialogue(bossDefeatedLines, () =>
            {
                _bossVictoryEffect?.TriggerBossFall();
            });
            return;
        }

        // Lignes d'intro du dialogue de défaite (toutes sauf la dernière)
        DialogueLine[] allButLast = new DialogueLine[bossDefeatedLines.Length - 1];
        System.Array.Copy(bossDefeatedLines, allButLast, allButLast.Length);

        DialogueLine[] lastLine = new DialogueLine[] { bossDefeatedLines[bossDefeatedLines.Length - 1] };

        _dialogueSystem.StartDialogue(allButLast, () =>
        {
            // Déclenche la chute au début de la dernière phrase
            _bossVictoryEffect?.TriggerBossFall();
            _dialogueSystem.StartDialogue(lastLine);
        });
    }
}
