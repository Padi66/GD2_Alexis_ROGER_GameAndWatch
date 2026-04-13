using UnityEngine;

public enum HackDifficulty { Easy, Normal, Hard }

/// <summary>
/// Gère la difficulté du mini-jeu de hack :
/// modifie le temps minimum par grille et la vie du boss.
/// </summary>
public class HackDifficultyManager : MonoBehaviour
{
    [Header("Référence")]
    [SerializeField] private hackGame _hackGame;

    [Header("Temps minimum par difficulté (secondes)")]
    [SerializeField] private float _easyMinTime   = 12f;
    [SerializeField] private float _normalMinTime  = 7f;
    [SerializeField] private float _hardMinTime    = 3f;

    [Header("Vie du boss par difficulté")]
    [SerializeField] private int _easyBossHealth   = 300;
    [SerializeField] private int _normalBossHealth  = 500;
    [SerializeField] private int _hardBossHealth    = 800;

    /// <summary>Sélectionne la difficulté Facile et lance le jeu.</summary>
    public void SelectEasyAndStart()
    {
        ApplyDifficulty(HackDifficulty.Easy);
        _hackGame?.OnStartButtonPressed();
    }

    /// <summary>Sélectionne la difficulté Normale et lance le jeu.</summary>
    public void SelectNormalAndStart()
    {
        ApplyDifficulty(HackDifficulty.Normal);
        _hackGame?.OnStartButtonPressed();
    }

    /// <summary>Sélectionne la difficulté Difficile et lance le jeu.</summary>
    public void SelectHardAndStart()
    {
        ApplyDifficulty(HackDifficulty.Hard);
        _hackGame?.OnStartButtonPressed();
    }

    private void ApplyDifficulty(HackDifficulty difficulty)
    {
        if (_hackGame == null)
        {
            Debug.LogError("HackDifficultyManager : la référence à hackGame est manquante !");
            return;
        }

        switch (difficulty)
        {
            case HackDifficulty.Easy:
                _hackGame.minMaxTime = _easyMinTime;
                _hackGame.SetEnemyHealth(_easyBossHealth);
                break;
            case HackDifficulty.Hard:
                _hackGame.minMaxTime = _hardMinTime;
                _hackGame.SetEnemyHealth(_hardBossHealth);
                break;
            default:
                _hackGame.minMaxTime = _normalMinTime;
                _hackGame.SetEnemyHealth(_normalBossHealth);
                break;
        }

        Debug.Log($"Difficulté : {difficulty} | Temps min : {_hackGame.minMaxTime}s | Vie boss : {_hackGame.enemyHealth}");
    }
}
