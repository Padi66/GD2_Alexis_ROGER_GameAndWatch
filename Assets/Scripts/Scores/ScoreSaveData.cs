using System;

/// <summary>Données de score sérialisées en JSON pour les 3 jeux.</summary>
[Serializable]
public class ScoreSaveData
{
    // Jeu 1 — dernier score + difficulté
    public int   Jeu1LastScore      = 0;
    public string Jeu1Difficulty    = "-";

    // Jeu 2 — meilleur temps restant (en secondes) + difficulté
    public float  Jeu2BestTime      = -1f;   // -1 = jamais joué
    public string Jeu2Difficulty    = "-";

    // Jeu 3 — nombre de coups (grilles complétées) + difficulté
    public int   Jeu3Moves          = -1;    // -1 = jamais joué
    public string Jeu3Difficulty    = "-";
}
