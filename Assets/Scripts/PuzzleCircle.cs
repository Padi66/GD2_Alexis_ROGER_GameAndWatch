using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PuzzleCircle : MonoBehaviour
{
    [Header("Boutons du puzzle (8 boutons)")]
    public Button[] buttons;

    [Header("Lumières du puzzle (8 images)")]
    public Image[] lights;

    [Header("Couleurs")]
    public Color activeColor = Color.yellow;
    public Color inactiveColor = Color.gray;

    [Header("Clignotement de victoire")]
    [SerializeField] private Color blinkColor = Color.white;
    [SerializeField] private float blinkInterval = 0.07f;
    [SerializeField] private int blinkCycles = 6;

    [Header("Panels")]
    [SerializeField] private GameObject rulesPanel;
    [SerializeField] private GameObject difficultyPanel;
    [SerializeField] private GameObject victoryPanel;
    [SerializeField] private GameObject gameOverPanel;

    [Header("Timer UI")]
    [SerializeField] private GameObject timerObject;
    [SerializeField] private TextMeshProUGUI timerText;

    [Header("Score Save")]
    [SerializeField] private SO_ScoreDatas _scoreDatas;

    private string _currentDifficulty = "Normal";

    private const float TimerEasy   = -1f;
    private const float TimerNormal = 60f;
    private const float TimerHard   = 30f;

    private int[]     buttonToLight;
    private int       currentTarget   = -1;
    private int       lightsActivated = 0;
    private bool      isPuzzleSolved  = false;
    private bool      isGameStarted   = false;
    private float     remainingTime   = -1f;
    private bool      isTimerRunning  = false;
    private Coroutine _blinkCoroutine;

    // ─── Initialisation ────────────────────────────────────────────────────────

    private void Start()
    {
        GenerateRandomMapping();
        ResetLights();
        AssignButtonEvents();

        if (timerObject != null) timerObject.SetActive(false);
        if (rulesPanel  != null) rulesPanel.SetActive(true);
    }

    private void Update()
    {
        if (!isTimerRunning || remainingTime < 0f)
            return;

        remainingTime -= Time.deltaTime;

        if (remainingTime <= 0f)
        {
            remainingTime  = 0f;
            isTimerRunning = false;
            UpdateTimerDisplay();
            TriggerGameOver();
            return;
        }

        UpdateTimerDisplay();
    }

    // ─── Flow des panels ───────────────────────────────────────────────────────

    /// <summary>Ouvre le panel de sélection de difficulté.</summary>
    public void OpenDifficultyPanel()
    {
        if (rulesPanel      != null) rulesPanel.SetActive(false);
        if (difficultyPanel != null) difficultyPanel.SetActive(true);
    }

    /// <summary>Démarre la partie en mode Facile (temps infini).</summary>
    public void StartEasy()
    {
        _currentDifficulty = "Facile";
        StartGame(TimerEasy);
    }

    /// <summary>Démarre la partie en mode Normal (60 secondes).</summary>
    public void StartNormal()
    {
        _currentDifficulty = "Normal";
        StartGame(TimerNormal);
    }

    /// <summary>Démarre la partie en mode Difficile (30 secondes).</summary>
    public void StartHard()
    {
        _currentDifficulty = "Difficile";
        StartGame(TimerHard);
    }

    private void StartGame(float duration)
    {
        if (difficultyPanel != null) difficultyPanel.SetActive(false);

        remainingTime  = duration;
        isGameStarted  = true;
        isTimerRunning = duration > 0f;

        if (timerObject != null) timerObject.SetActive(true);

        UpdateTimerDisplay();
    }

    // ─── Score Save ────────────────────────────────────────────────────────────

    private void SaveJeu2Score()
    {
        if (_scoreDatas == null) return;
        // En mode Facile remainingTime est -1, on passe 0 pour indiquer "infini"
        float timeToSave = remainingTime < 0f ? float.MaxValue : remainingTime;
        _scoreDatas.SetJeu2BestTime(timeToSave, _currentDifficulty);
    }

    // ─── Game Over ─────────────────────────────────────────────────────────────

    private void TriggerGameOver()
    {
        isGameStarted = false;

        if (gameOverPanel != null)
            gameOverPanel.SetActive(true);
    }

    // ─── Puzzle ────────────────────────────────────────────────────────────────

    private void GenerateRandomMapping()
    {
        buttonToLight = new int[8];

        for (int i = 0; i < 8; i++)
            buttonToLight[i] = i;

        for (int i = 0; i < 8; i++)
        {
            int rand            = Random.Range(i, 8);
            int temp            = buttonToLight[i];
            buttonToLight[i]    = buttonToLight[rand];
            buttonToLight[rand] = temp;
        }
    }

    private void ResetLights()
    {
        if (_blinkCoroutine != null)
        {
            StopCoroutine(_blinkCoroutine);
            _blinkCoroutine = null;
        }

        foreach (var light in lights)
            light.color = inactiveColor;

        currentTarget   = -1;
        lightsActivated = 0;
        isPuzzleSolved  = false;
    }

    private void AssignButtonEvents()
    {
        for (int i = 0; i < buttons.Length; i++)
        {
            int index = i;
            buttons[i].onClick.AddListener(() => OnButtonPressed(index));
        }
    }

    private void OnButtonPressed(int buttonIndex)
    {
        if (!isGameStarted || isPuzzleSolved)
            return;

        int lightIndex = buttonToLight[buttonIndex];

        if (currentTarget == -1)
        {
            currentTarget = lightIndex;
            lights[currentTarget].color = activeColor;
            lightsActivated = 1;
            return;
        }

        int nextTarget = (currentTarget + 1) % 8;

        if (lightIndex == nextTarget)
        {
            currentTarget = nextTarget;
            lights[currentTarget].color = activeColor;
            lightsActivated++;

            if (lightsActivated >= 8)
            {
                isPuzzleSolved = true;
                isTimerRunning = false;
                SaveJeu2Score();
                _blinkCoroutine = StartCoroutine(BlinkVictory());
            }
        }
        else
        {
            ResetLights();
        }
    }

    // ─── Timer ─────────────────────────────────────────────────────────────────

    private void UpdateTimerDisplay()
    {
        if (timerText == null) return;

        if (remainingTime < 0f)
        {
            timerText.text = "∞";
            return;
        }

        int minutes = Mathf.FloorToInt(remainingTime / 60f);
        int seconds = Mathf.FloorToInt(remainingTime % 60f);
        timerText.text = $"{minutes:00}:{seconds:00}";
    }

    // ─── Victoire ──────────────────────────────────────────────────────────────

    private IEnumerator BlinkVictory()
    {
        for (int cycle = 0; cycle < blinkCycles; cycle++)
        {
            for (int i = 0; i < lights.Length; i++)
            {
                lights[i].color = blinkColor;
                yield return new WaitForSeconds(blinkInterval);
            }

            for (int i = 0; i < lights.Length; i++)
            {
                lights[i].color = activeColor;
                yield return new WaitForSeconds(blinkInterval);
            }
        }

        foreach (var light in lights)
            light.color = activeColor;

        if (victoryPanel != null)
            victoryPanel.SetActive(true);
    }
}
