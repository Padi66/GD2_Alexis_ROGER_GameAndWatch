using UnityEngine;

/// <summary>
/// ScriptableObject central pour les scores des 3 jeux.
/// Les scores sont liés au pseudo du joueur : chaque pseudo a son propre fichier de sauvegarde.
/// </summary>
[CreateAssetMenu(fileName = "SO_ScoreDatas", menuName = "Scriptable Objects/SO_ScoreDatas")]
public class SO_ScoreDatas : ScriptableObject
{
    // Jeu 1
    public int    Jeu1LastScore   = 0;
    public string Jeu1Difficulty  = "-";

    // Jeu 2
    public float  Jeu2BestTime   = -1f;
    public string Jeu2Difficulty  = "-";

    // Jeu 3
    public int    Jeu3Moves      = -1;
    public string Jeu3Difficulty  = "-";

    private ScoreSaveController _saveController;
    private string _currentPseudo = "";

    /// <summary>
    /// Charge les scores du pseudo donné.
    /// Si le pseudo change, les scores du nouveau pseudo sont chargés (vierges si nouveau joueur).
    /// </summary>
    public void LoadScores(string pseudo)
    {
        _currentPseudo = pseudo ?? "";
        EnsureSaveController();
        ApplyData(_saveController.Load(_currentPseudo));
    }

    /// <summary>Sauvegarde les scores pour le pseudo courant.</summary>
    public void SaveScores()
    {
        if (string.IsNullOrWhiteSpace(_currentPseudo)) return;
        EnsureSaveController();
        _saveController.Save(BuildData(), _currentPseudo);
    }

    /// <summary>Met à jour le score de Jeu1 et sauvegarde.</summary>
    public void SetJeu1Score(int score, string difficulty)
    {
        Jeu1LastScore  = score;
        Jeu1Difficulty = difficulty;
        SaveScores();
    }

    /// <summary>Met à jour le meilleur temps de Jeu2 si meilleur, et sauvegarde.</summary>
    public void SetJeu2BestTime(float timeRemaining, string difficulty)
    {
        if (Jeu2BestTime < 0f || timeRemaining > Jeu2BestTime)
        {
            Jeu2BestTime   = timeRemaining;
            Jeu2Difficulty = difficulty;
        }
        SaveScores();
    }

    /// <summary>Met à jour le nombre de coups de Jeu3 si meilleur (moins = meilleur), et sauvegarde.</summary>
    public void SetJeu3Moves(int moves, string difficulty)
    {
        if (Jeu3Moves < 0 || moves < Jeu3Moves)
        {
            Jeu3Moves      = moves;
            Jeu3Difficulty = difficulty;
        }
        SaveScores();
    }

    /// <summary>Définit le pseudo courant sans recharger les scores (utiliser LoadScores pour recharger).</summary>
    public void SetCurrentPseudo(string pseudo)
    {
        _currentPseudo = pseudo ?? "";
    }

    private void ApplyData(ScoreSaveData data)
    {
        Jeu1LastScore  = data.Jeu1LastScore;
        Jeu1Difficulty = data.Jeu1Difficulty;
        Jeu2BestTime   = data.Jeu2BestTime;
        Jeu2Difficulty = data.Jeu2Difficulty;
        Jeu3Moves      = data.Jeu3Moves;
        Jeu3Difficulty = data.Jeu3Difficulty;
    }

    private ScoreSaveData BuildData() => new ScoreSaveData
    {
        Jeu1LastScore  = Jeu1LastScore,
        Jeu1Difficulty = Jeu1Difficulty,
        Jeu2BestTime   = Jeu2BestTime,
        Jeu2Difficulty = Jeu2Difficulty,
        Jeu3Moves      = Jeu3Moves,
        Jeu3Difficulty = Jeu3Difficulty
    };

    private void EnsureSaveController()
    {
        if (_saveController == null)
            _saveController = new ScoreSaveController();
    }
}
