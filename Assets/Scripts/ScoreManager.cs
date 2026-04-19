using UnityEngine;
using System;
using System.Collections.Generic;

[System.Serializable]
public class ScoreMilestone
{
    public int scoreThreshold;
    public string message;
}

public class ScoreManager : MonoBehaviour
{
    [SerializeField] private int _currentScore = 0;
    [SerializeField] private List<ScoreMilestone> _milestones = new List<ScoreMilestone>();
    [SerializeField] private AudioEventDispatcher _audioEventDispatcher;

    [Header("Score Save")]
    [SerializeField] private SO_ScoreDatas _scoreDatas;
    [SerializeField] private string _difficulty = "Normal";

    public event Action<int> OnScoreChanged;
    public event Action<string> OnMilestoneReached;

    public int CurrentScore => _currentScore;

    private readonly HashSet<int> _triggeredMilestones = new HashSet<int>();

    private void Start()
    {
        _currentScore = 0;
        OnScoreChanged?.Invoke(_currentScore);
    }
    
    public void AddScore(int points)
    {
        _currentScore += points;
        Debug.Log($"Score ajouté: +{points}. Score total: {_currentScore}");
        _audioEventDispatcher?.PlayAudio(AudioType.TouchObject);

        OnScoreChanged?.Invoke(_currentScore);
        CheckMilestones();
    }
    
    private void OnDestroy()
    {
        SaveScore();
    }

    private void OnApplicationQuit()
    {
        SaveScore();
    }

    public void ResetScore()
    {
        _currentScore = 0;
        _triggeredMilestones.Clear();
        OnScoreChanged?.Invoke(_currentScore);
    }

    /// <summary>Sauvegarde le score courant dans SO_ScoreDatas (appeler en fin de partie).</summary>
    public void SaveScore()
    {
        if (_scoreDatas != null)
            _scoreDatas.SetJeu1Score(_currentScore, _difficulty);
    }

    /// <summary>Définit la difficulté courante (pour la sauvegarde du score).</summary>
    public void SetDifficulty(string difficulty)
    {
        _difficulty = difficulty;
    }

    private void CheckMilestones()
    {
        foreach (ScoreMilestone milestone in _milestones)
        {
            if (_currentScore >= milestone.scoreThreshold && !_triggeredMilestones.Contains(milestone.scoreThreshold))
            {
                _triggeredMilestones.Add(milestone.scoreThreshold);
                

                OnMilestoneReached?.Invoke(milestone.message);
            }
        }
    }
}
