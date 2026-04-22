using UnityEngine;

public class DificultyManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private TimeManager _timeManager;
    [SerializeField] private Spawner _spawner;
    [SerializeField] private ScoreManager _scoreManager;

    [Header("Time Step Settings")]
    [SerializeField] private float _initialTimeStep = 5f;
    [SerializeField] private float _minTimeStep = 0.5f;
    [SerializeField] private float _timeStepDecreaseAmount = 0.2f;

    [Header("Spawn Delay Settings")]
    [SerializeField] private int _initialSpawnDelay = 3;
    [SerializeField] private int _minSpawnDelay = 1;

    [Header("Difficulty Ramp")]
    [SerializeField] private float _difficultyInterval = 10f;

    private float _difficultyTimer = 0f;
    private int _difficultyLevel = 0;


    private float _speedMultiplier = 1f;
    private float _spawnMultiplier = 1f;

    private void Start()
    {
        ApplyDifficulty();
    }

    private void Update()
    {
        _difficultyTimer += Time.deltaTime;

        if (_difficultyTimer >= _difficultyInterval)
        {
            _difficultyTimer = 0f;
            IncreaseDifficulty();
        }
    }

    private void IncreaseDifficulty()
    {
        _difficultyLevel++;
        ApplyDifficulty();
    }

    private void ApplyDifficulty()
    {

        float autoTimeStep = Mathf.Max(_initialTimeStep - (_timeStepDecreaseAmount * _difficultyLevel), _minTimeStep);
        int autoSpawnDelay = Mathf.Max(_initialSpawnDelay - (_difficultyLevel / 2), _minSpawnDelay);

 
        float finalTimeStep = autoTimeStep / _speedMultiplier;
        int finalSpawnDelay = Mathf.RoundToInt(autoSpawnDelay / _spawnMultiplier);


        _timeManager.SetTimeStep(finalTimeStep);
        _spawner.SpawnDelayDuration = finalSpawnDelay;

        Debug.Log($"Lvl: {_difficultyLevel} | TimeStep: {finalTimeStep:F2} | SpawnDelay: {finalSpawnDelay}");
    }

    public void SetEasy()
    {
        _speedMultiplier = 0.8f;
        _spawnMultiplier = 0.8f;
        _scoreManager?.SetDifficulty("Facile");
        ApplyDifficulty();
    }

    public void SetNormal()
    {
        _speedMultiplier = 1f;
        _spawnMultiplier = 1f;
        _scoreManager?.SetDifficulty("Normal");
        ApplyDifficulty();
    }

    public void SetHard()
    {
        _speedMultiplier = 1.5f;
        _spawnMultiplier = 1.5f;
        _scoreManager?.SetDifficulty("Difficile");
        ApplyDifficulty();
    }
}
