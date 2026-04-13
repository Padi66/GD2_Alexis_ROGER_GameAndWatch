using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

/// <summary>
/// Gère les effets visuels de victoire :
/// explosions autour du boss, chute du boss avec texte,
/// chute du premier texte, apparition d'un second texte,
/// chute du second texte puis chargement de la prochaine scène.
/// </summary>
public class BossVictoryEffect : MonoBehaviour
{
    [Header("Boss")]
    [Tooltip("RectTransform de l'image du boss dans le VictoryPanel.")]
    [SerializeField] private RectTransform _bossRect;

    [Header("Explosions")]
    [SerializeField] private Sprite _explosionSprite;
    [SerializeField] private int _explosionCount = 6;
    [SerializeField] private float _explosionRadius = 200f;
    [SerializeField] private float _explosionDuration = 0.4f;
    [SerializeField] private float _explosionSize = 120f;
    [SerializeField] private float _explosionInterval = 0.5f;

    [Header("Chute du boss")]
    [SerializeField] private float _fallDistance = 800f;
    [SerializeField] private float _fallDuration = 1.2f;
    [SerializeField] private float _fallRotation = -30f;

    [Header("Premier texte")]
    [Tooltip("Texte à afficher quand le boss tombe (ex: \"VICTOIRE !\").")]
    [SerializeField] private string _victoryText = "VICTOIRE !";
    [SerializeField] private float _textFontSize = 120f;
    [SerializeField] private Color _textColor = Color.yellow;
    [Tooltip("Délai entre l'apparition du premier texte et sa chute (secondes).")]
    [SerializeField] private float _textHoldDuration = 1.5f;
    [SerializeField] private float _textFallDistance = 600f;
    [SerializeField] private float _textFallDuration = 0.8f;

    [Header("Second texte")]
    [Tooltip("Texte à afficher après la chute du premier (ex: \"Niveau suivant...\").")]
    [SerializeField] private string _secondText = "Niveau suivant...";
    [SerializeField] private float _secondTextFontSize = 80f;
    [SerializeField] private Color _secondTextColor = Color.white;
    [Tooltip("Délai entre la chute du premier texte et l'apparition du second (secondes).")]
    [SerializeField] private float _secondTextAppearDelay = 0.3f;
    [Tooltip("Durée d'affichage du second texte avant sa chute (secondes).")]
    [SerializeField] private float _secondTextHoldDuration = 1.2f;
    [SerializeField] private float _secondTextFallDistance = 600f;
    [SerializeField] private float _secondTextFallDuration = 0.8f;

    [Header("Scène suivante")]
    [SerializeField] private string _nextSceneName = "";
    [Tooltip("Délai après la chute du second texte avant de charger la scène (secondes).")]
    [SerializeField] private float _sceneLoadDelay = 0.3f;
    
    [Header("Audio")]
    [SerializeField] private AudioEventDispatcher _audioDispatcher;

    private Coroutine _explosionLoop;

    private void OnEnable()
    {
        StartExplosions();
    }

    private void OnDisable()
    {
        StopExplosions();
    }

    /// <summary>Lance la boucle d'explosions.</summary>
    public void StartExplosions()
    {
        StopExplosions();
        if (_explosionSprite == null || _bossRect == null) return;
        _explosionLoop = StartCoroutine(ExplosionLoop());
    }

    /// <summary>Stoppe les explosions.</summary>
    public void StopExplosions()
    {
        if (_explosionLoop != null)
        {
            StopCoroutine(_explosionLoop);
            _explosionLoop = null;
        }
    }

    /// <summary>
    /// Déclenche la chute du boss, le premier texte, puis le second,
    /// puis charge la scène suivante.
    /// </summary>
    public void TriggerBossFall()
    {
        StopExplosions();
        StartCoroutine(BossFallSequence());
    }

    // ── Coroutines ──────────────────────────────────────────────────────────

    private IEnumerator BossFallSequence()
    {
        // 1 — Chute du boss + apparition du premier texte en simultané
        RectTransform firstTextRect = CreateText(
            "VictoryText", _victoryText, _textFontSize, _textColor, verticalOffset: 0f);
        yield return StartCoroutine(BossFallCoroutine());

        // 2 — Premier texte tenu à l'écran
        yield return new WaitForSeconds(_textHoldDuration);

        // 3 — Chute du premier texte
        yield return StartCoroutine(TextFallCoroutine(firstTextRect, _textColor, _textFallDistance, _textFallDuration));

        // 4 — Délai puis apparition du second texte
        yield return new WaitForSeconds(_secondTextAppearDelay);

        RectTransform secondTextRect = CreateText(
            "VictoryText2", _secondText, _secondTextFontSize, _secondTextColor, verticalOffset: 0f);
        yield return StartCoroutine(WaitForTextAppear(secondTextRect));

        // 5 — Second texte tenu à l'écran
        yield return new WaitForSeconds(_secondTextHoldDuration);

        // 6 — Chute du second texte
        yield return StartCoroutine(TextFallCoroutine(secondTextRect, _secondTextColor, _secondTextFallDistance, _secondTextFallDuration));

        // 7 — Chargement de la scène
        yield return new WaitForSeconds(_sceneLoadDelay);

        if (!string.IsNullOrEmpty(_nextSceneName))
            SceneManager.LoadScene(_nextSceneName);
    }

    // ── Helpers ─────────────────────────────────────────────────────────────

    private RectTransform CreateText(string goName, string text, float fontSize, Color color, float verticalOffset)
    {
        GameObject go = new GameObject(goName,
            typeof(RectTransform), typeof(CanvasRenderer), typeof(TextMeshProUGUI));
        go.transform.SetParent(_bossRect.parent, false);
        go.transform.SetAsLastSibling();

        RectTransform rect = go.GetComponent<RectTransform>();
        rect.anchorMin = rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = new Vector2(0f, verticalOffset);
        rect.sizeDelta = new Vector2(900f, 220f);
        rect.localScale = Vector3.one * 0.5f;

        TextMeshProUGUI tmp = go.GetComponent<TextMeshProUGUI>();
        tmp.text = text;
        tmp.fontSize = fontSize;
        tmp.color = new Color(color.r, color.g, color.b, 0f);
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.fontStyle = FontStyles.Bold;
        tmp.raycastTarget = false;

        StartCoroutine(TextAppearCoroutine(rect, tmp, color));

        return rect;
    }

    private IEnumerator WaitForTextAppear(RectTransform rect)
    {
        const float AppearDuration = 0.4f;
        yield return new WaitForSeconds(AppearDuration);
    }

    private IEnumerator TextAppearCoroutine(RectTransform rect, TextMeshProUGUI tmp, Color targetColor)
    {
        const float AppearDuration = 0.4f;
        float elapsed = 0f;

        while (elapsed < AppearDuration)
        {
            float t = elapsed / AppearDuration;
            float eased = 1f - Mathf.Pow(1f - t, 3f); // ease-out cubic
            rect.localScale = Vector3.one * Mathf.Lerp(0.5f, 1f, eased);
            tmp.color = new Color(targetColor.r, targetColor.g, targetColor.b, eased);
            elapsed += Time.deltaTime;
            yield return null;
        }

        rect.localScale = Vector3.one;
        tmp.color = targetColor;
    }

    private IEnumerator BossFallCoroutine()
    {
        if (_bossRect == null) yield break;
        
        _audioDispatcher?.PlayAudio(AudioType.BossFall);

        Vector2 startPos = _bossRect.anchoredPosition;
        Vector2 endPos = startPos + Vector2.down * _fallDistance;
        Quaternion startRot = _bossRect.localRotation;
        Quaternion endRot = Quaternion.Euler(0f, 0f, _fallRotation);

        float elapsed = 0f;
        while (elapsed < _fallDuration)
        {
            float t = elapsed / _fallDuration;
            float eased = t * t;
            _bossRect.anchoredPosition = Vector2.Lerp(startPos, endPos, eased);
            _bossRect.localRotation = Quaternion.Lerp(startRot, endRot, eased);
            elapsed += Time.deltaTime;
            yield return null;
        }

        _bossRect.anchoredPosition = endPos;
        _bossRect.localRotation = endRot;
    }

    private IEnumerator TextFallCoroutine(RectTransform textRect, Color baseColor, float fallDistance, float fallDuration)
    {
        if (textRect == null) yield break;

        TextMeshProUGUI tmp = textRect.GetComponent<TextMeshProUGUI>();
        Vector2 startPos = textRect.anchoredPosition;
        Vector2 endPos = startPos + Vector2.down * fallDistance;

        float elapsed = 0f;
        while (elapsed < fallDuration)
        {
            float t = elapsed / fallDuration;
            float eased = t * t;
            textRect.anchoredPosition = Vector2.Lerp(startPos, endPos, eased);
            if (tmp != null)
                tmp.color = new Color(baseColor.r, baseColor.g, baseColor.b, Mathf.Lerp(1f, 0f, t));
            elapsed += Time.deltaTime;
            yield return null;
        }

        Destroy(textRect.gameObject);
    }

    // ── Explosions ──────────────────────────────────────────────────────────

    private IEnumerator ExplosionLoop()
    {
        while (true)
        {
            SpawnExplosionBurst();
            yield return new WaitForSeconds(_explosionInterval);
        }
    }

    private void SpawnExplosionBurst()
    {
        _audioDispatcher?.PlayAudio(AudioType.BossExplosion);
        for (int i = 0; i < _explosionCount; i++)
        {
            Vector2 offset = Random.insideUnitCircle.normalized
                             * Random.Range(_explosionRadius * 0.3f, _explosionRadius);
            StartCoroutine(AnimateExplosion(_bossRect.anchoredPosition + offset));
            
        }
    }

    private IEnumerator AnimateExplosion(Vector2 anchoredPos)
    {
        GameObject go = new GameObject("Explosion",
            typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        go.transform.SetParent(_bossRect.parent, false);
        go.transform.SetAsLastSibling();

        RectTransform rect = go.GetComponent<RectTransform>();
        rect.anchorMin = rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = anchoredPos;
        rect.sizeDelta = Vector2.one * _explosionSize;

        Image img = go.GetComponent<Image>();
        img.sprite = _explosionSprite;
        img.raycastTarget = false;

        float elapsed = 0f;
        while (elapsed < _explosionDuration)
        {
            float t = elapsed / _explosionDuration;
            rect.localScale = Vector3.one * Mathf.Lerp(0.2f, 1.5f, t);
            img.color = new Color(img.color.r, img.color.g, img.color.b, Mathf.Lerp(1f, 0f, t));
            elapsed += Time.deltaTime;
            yield return null;
        }

        Destroy(go);
    }
}
