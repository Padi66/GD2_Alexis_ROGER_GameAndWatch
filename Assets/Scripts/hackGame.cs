using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;
using TouchPhase = UnityEngine.InputSystem.TouchPhase;

public enum NodeType { Normal, Bonus, Malus, Goal }

public class hackGame : MonoBehaviour
{
    [System.Serializable]
    public class HackNode
    {
        public string name;
        public NodeType type;
        public RectTransform uiTransform;
        public List<int> neighbors = new List<int>();
    }

    private const int GridRows = 4;
    private const int GridCols = 4;

    [Header("Grille de nœuds")]
    public HackNode[] nodes;

    [Header("Curseur")]
    public RectTransform cursor;
    public int startNodeIndex = 0;

    [Header("Timer")]
    public float maxTime = 20f;
    private float currentTime;

    [Header("Bonus / Malus")]
    public float bonusTime = 2f;
    public float malusTime = 2f;

    [Header("Combat")]
    public int damagePerHack = 20;
    public int enemyHealth = 100;
    private int _enemyMaxHealth;

    [Header("Accélération")]
    [Tooltip("Réduction du temps max à chaque nouvelle grille (en secondes).")]
    public float timeReductionPerGrid = 2f;
    [Tooltip("Temps minimum auquel le timer peut descendre.")]
    public float minMaxTime = 5f;
    private float _currentMaxTime;
    private int _gridCount = 0;

    [Header("Swipe")]
    public float swipeThreshold = 50f;

    [Header("Génération")]
    public int bonusNodeCount = 2;
    public int malusNodeCount = 2;
    public int minGoalDistance = 3;

    [Header("Sprites des nœuds")]
    public Sprite spriteNormal;
    public Sprite spriteBonus;
    public Sprite spriteMalus;
    public Sprite spriteGoal;
    public Sprite spriteCurrent;

    [Header("UI")]
    public Slider timerSlider;
    public TMPro.TextMeshProUGUI timerText;
    public Slider enemyHealthSlider;
    public GameObject victoryPanel;
    public GameObject startPanel;
    [Tooltip("Les GameObjects à cacher quand le joueur clique sur Start.")]
    public GameObject[] objectsToHideOnStart;

    [Header("Boss")]
    public RectTransform bossImage;

    [Header("Tremblement")]
    [Tooltip("Durée totale du tremblement en secondes.")]
    public float shakeDuration = 0.4f;
    [Tooltip("Amplitude du déplacement en pixels.")]
    public float shakeMagnitude = 12f;

    [Header("Score Save")]
    public SO_ScoreDatas scoreDatas;
    public string currentDifficulty = "Normal";

    [Header("Audio")]
    public AudioEventDispatcher audioDispatcher;

    [Header("Dialogue")]
    [Tooltip("Référence au GameplayDialogueTrigger qui gère les 3 séquences.")]
    public GameplayDialogueTrigger dialogueTrigger;

    private int currentNodeIndex;
    private bool hackActive = false;
    private Vector2 _startTouchPos;
    private Coroutine _shakeHealthBarCoroutine;
    private Coroutine _shakeBossCoroutine;

    private void OnEnable()
    {
        EnhancedTouchSupport.Enable();
    }

    private void OnDisable()
    {
        EnhancedTouchSupport.Disable();
    }

    private void Start()
    {
        _enemyMaxHealth = enemyHealth;
        _currentMaxTime = maxTime;
        InitEnemyHealthSlider();

        if (startPanel != null)
            startPanel.SetActive(true);

        if (victoryPanel != null)
            victoryPanel.SetActive(false);

        BuildGridNeighbors();
    }

    /// <summary>Cache les objets choisis dans l'Inspector et lance le dialogue d'intro.</summary>
    public void OnStartButtonPressed()
    {
        foreach (GameObject obj in objectsToHideOnStart)
        {
            if (obj != null)
                obj.SetActive(false);
        }

        if (dialogueTrigger != null)
            dialogueTrigger.OnGameStart();
        else
            LaunchGame();
    }

    /// <summary>Cache le StartPanel et lance la grille. Appelé automatiquement après la fin du dialogue d'intro.</summary>
    public void LaunchGame()
    {
        if (startPanel != null)
            startPanel.SetActive(false);

        StartNewGrid();
    }

    /// <summary>Initialise le slider de vie ennemie avec les valeurs de départ.</summary>
    private void InitEnemyHealthSlider()
    {
        if (enemyHealthSlider == null) return;

        enemyHealthSlider.minValue = 0;
        enemyHealthSlider.maxValue = _enemyMaxHealth;
        enemyHealthSlider.value = _enemyMaxHealth;

        Image fill = enemyHealthSlider.fillRect?.GetComponent<Image>();
        if (fill != null)
            fill.color = Color.red;
    }

    /// <summary>Génère automatiquement les voisins pour une grille 4x4.</summary>
    private void BuildGridNeighbors()
    {
        for (int i = 0; i < nodes.Length; i++)
        {
            nodes[i].neighbors.Clear();

            int row = i / GridCols;
            int col = i % GridCols;

            if (row > 0)            nodes[i].neighbors.Add(i - GridCols);
            if (row < GridRows - 1) nodes[i].neighbors.Add(i + GridCols);
            if (col > 0)            nodes[i].neighbors.Add(i - 1);
            if (col < GridCols - 1) nodes[i].neighbors.Add(i + 1);
        }
    }

    /// <summary>Randomise les types de nœuds à chaque nouvelle grille.</summary>
    private void RandomizeNodeTypes()
    {
        for (int i = 0; i < nodes.Length; i++)
            nodes[i].type = NodeType.Normal;

        nodes[startNodeIndex].type = NodeType.Normal;

        List<int> available = new List<int>();
        for (int i = 0; i < nodes.Length; i++)
            if (i != startNodeIndex) available.Add(i);

        int startRow = startNodeIndex / GridCols;
        int startCol = startNodeIndex % GridCols;

        List<int> farNodes = available.FindAll(i =>
            Mathf.Abs(i / GridCols - startRow) +
            Mathf.Abs(i % GridCols - startCol) >= minGoalDistance);

        if (farNodes.Count == 0) farNodes = available;

        int goalIndex = farNodes[Random.Range(0, farNodes.Count)];
        nodes[goalIndex].type = NodeType.Goal;
        available.Remove(goalIndex);

        for (int i = 0; i < bonusNodeCount && available.Count > 0; i++)
        {
            int pick = available[Random.Range(0, available.Count)];
            nodes[pick].type = NodeType.Bonus;
            available.Remove(pick);
        }

        for (int i = 0; i < malusNodeCount && available.Count > 0; i++)
        {
            int pick = available[Random.Range(0, available.Count)];
            nodes[pick].type = NodeType.Malus;
            available.Remove(pick);
        }
    }

    /// <summary>Met à jour le sprite de chaque nœud selon son type.</summary>
    private void RefreshNodeVisuals()
    {
        for (int i = 0; i < nodes.Length; i++)
        {
            Image img = nodes[i].uiTransform.GetComponent<Image>();
            if (img == null) continue;

            if (i == currentNodeIndex && spriteCurrent != null)
            {
                img.sprite = spriteCurrent;
                continue;
            }

            img.sprite = nodes[i].type switch
            {
                NodeType.Bonus  => spriteBonus  != null ? spriteBonus  : spriteNormal,
                NodeType.Malus  => spriteMalus  != null ? spriteMalus  : spriteNormal,
                NodeType.Goal   => spriteGoal   != null ? spriteGoal   : spriteNormal,
                _               => spriteNormal
            };
        }
    }

    private void Update()
    {
        if (!hackActive) return;

        HandleSwipe();
        HandleArrowKeys();

        currentTime -= Time.deltaTime;
        if (currentTime <= 0)
        {
            Debug.Log("Temps écoulé → Échec du hack !");
            StartNewGrid();
        }

        UpdateTimerUI();
    }

    /// <summary>Gère les inputs des touches fléchées du clavier.</summary>
    private void HandleArrowKeys()
    {
        Keyboard keyboard = Keyboard.current;
        if (keyboard == null) return;

        if (keyboard.upArrowKey.wasPressedThisFrame)
            TryMoveDirection(Vector2.up);
        else if (keyboard.downArrowKey.wasPressedThisFrame)
            TryMoveDirection(Vector2.down);
        else if (keyboard.leftArrowKey.wasPressedThisFrame)
            TryMoveDirection(Vector2.left);
        else if (keyboard.rightArrowKey.wasPressedThisFrame)
            TryMoveDirection(Vector2.right);
    }

    private void HandleSwipe()
    {
        if (Touch.activeTouches.Count == 0) return;

        Touch touch = Touch.activeTouches[0];

        if (touch.phase == TouchPhase.Began)
        {
            _startTouchPos = touch.screenPosition;
        }
        else if (touch.phase == TouchPhase.Ended)
        {
            Vector2 swipe = touch.screenPosition - _startTouchPos;

            if (swipe.magnitude < swipeThreshold) return;

            if (Mathf.Abs(swipe.x) > Mathf.Abs(swipe.y))
                TryMoveDirection(swipe.x > 0 ? Vector2.right : Vector2.left);
            else
                TryMoveDirection(swipe.y > 0 ? Vector2.up : Vector2.down);
        }
    }

    private void TryMoveDirection(Vector2 dir)
    {
        int bestNode = -1;
        float bestDot = 0.7f;

        foreach (int n in nodes[currentNodeIndex].neighbors)
        {
            Vector2 toNeighbor = (nodes[n].uiTransform.anchoredPosition - nodes[currentNodeIndex].uiTransform.anchoredPosition).normalized;
            float dot = Vector2.Dot(toNeighbor, dir);

            if (dot > bestDot)
            {
                bestDot = dot;
                bestNode = n;
            }
        }

        if (bestNode != -1)
            MoveCursor(bestNode);
    }

    private void SnapCursorToNode(int nodeIndex)
    {
        cursor.anchorMin = nodes[nodeIndex].uiTransform.anchorMin;
        cursor.anchorMax = nodes[nodeIndex].uiTransform.anchorMax;
        cursor.anchoredPosition = nodes[nodeIndex].uiTransform.anchoredPosition;
    }

    /// <summary>Lance une nouvelle grille en réduisant le temps disponible selon le nombre de grilles jouées.</summary>
    private void StartNewGrid()
    {
        if (_gridCount > 0)
            _currentMaxTime = Mathf.Max(_currentMaxTime - timeReductionPerGrid, minMaxTime);

        _gridCount++;
        RandomizeNodeTypes();
        currentNodeIndex = startNodeIndex;
        SnapCursorToNode(currentNodeIndex);
        currentTime = _currentMaxTime;
        hackActive = true;
        UpdateTimerUI();
        RefreshNodeVisuals();
    }

    private void UpdateTimerUI()
    {
        float ratio = Mathf.Clamp01(currentTime / _currentMaxTime);

        if (timerSlider != null)
            timerSlider.value = ratio;

        if (timerText != null)
            timerText.text = Mathf.CeilToInt(Mathf.Max(currentTime, 0)).ToString();

        if (timerSlider != null)
        {
            Image fill = timerSlider.fillRect?.GetComponent<Image>();
            if (fill != null)
                fill.color = Color.Lerp(new Color(0.9f, 0.15f, 0.1f), new Color(0.0f, 0.85f, 0.35f), ratio);
        }
    }

    /// <summary>Met à jour la valeur du slider de vie et déclenche les tremblements.</summary>
    private void UpdateEnemyHealthUI()
    {
        if (enemyHealthSlider == null) return;

        enemyHealthSlider.value = enemyHealth;

        if (_shakeHealthBarCoroutine != null)
            StopCoroutine(_shakeHealthBarCoroutine);
        _shakeHealthBarCoroutine = StartCoroutine(ShakeRect(enemyHealthSlider.GetComponent<RectTransform>()));

        if (bossImage != null)
        {
            if (_shakeBossCoroutine != null)
                StopCoroutine(_shakeBossCoroutine);
            _shakeBossCoroutine = StartCoroutine(ShakeRect(bossImage));
        }
    }

    /// <summary>Fait trembler un RectTransform pendant shakeDuration avec une amplitude décroissante.</summary>
    private IEnumerator ShakeRect(RectTransform rect)
    {
        Vector2 originalPosition = rect.anchoredPosition;
        float elapsed = 0f;

        while (elapsed < shakeDuration)
        {
            float strength = Mathf.Lerp(shakeMagnitude, 0f, elapsed / shakeDuration);
            rect.anchoredPosition = originalPosition + Random.insideUnitCircle * strength;
            elapsed += Time.deltaTime;
            yield return null;
        }

        rect.anchoredPosition = originalPosition;
    }

    /// <summary>Joue le son correspondant au type de nœud atterri.</summary>
    private void PlayNodeSound(NodeType type)
    {
        if (audioDispatcher == null) return;

        AudioType audioType = type switch
        {
            NodeType.Bonus  => AudioType.HackNodeBonus,
            NodeType.Malus  => AudioType.HackNodeMalus,
            NodeType.Goal   => AudioType.HackNodeGoal,
            _               => AudioType.HackNodeNormal
        };

        audioDispatcher.PlayAudio(audioType);
    }

    /// <summary>Déplace le curseur vers un nœud voisin donné.</summary>
    public void MoveCursor(int targetNodeIndex)
    {
        if (!hackActive) return;

        if (!nodes[currentNodeIndex].neighbors.Contains(targetNodeIndex)) return;

        currentNodeIndex = targetNodeIndex;
        SnapCursorToNode(currentNodeIndex);
        PlayNodeSound(nodes[currentNodeIndex].type);
        ApplyNodeEffect(nodes[currentNodeIndex]);
        RefreshNodeVisuals();
    }

    private void ApplyNodeEffect(HackNode node)
    {
        switch (node.type)
        {
            case NodeType.Normal:
                break;
            case NodeType.Bonus:
                currentTime = Mathf.Min(currentTime + bonusTime, _currentMaxTime);
                break;
            case NodeType.Malus:
                currentTime -= malusTime;
                break;
            case NodeType.Goal:
                OnHackSuccess();
                break;
        }
    }

    private void OnHackSuccess()
    {
        hackActive = false;
        enemyHealth -= damagePerHack;

        UpdateEnemyHealthUI();
        dialogueTrigger?.OnEnemyHealthChanged(enemyHealth, _enemyMaxHealth);

        if (enemyHealth <= 0)
        {
            OpenVictoryPanel();
            return;
        }

        StartNewGrid();
    }

    /// <summary>Ouvre le panel de victoire, déclenche le dialogue de fin et stoppe le jeu.</summary>
    private void OpenVictoryPanel()
    {
        hackActive = false;
        dialogueTrigger?.OnBossDefeated();

        if (scoreDatas != null)
            scoreDatas.SetJeu3Moves(_gridCount, currentDifficulty);

        if (victoryPanel != null)
            victoryPanel.SetActive(true);
    }
    /// <summary>Définit la vie maximale du boss et met à jour le slider. À appeler avant de lancer le jeu.</summary>
    public void SetEnemyHealth(int health)
    {
        enemyHealth = health;
        _enemyMaxHealth = health;
        InitEnemyHealthSlider();
    }

}
