using System.Collections;
using TMPro;
using UnityEngine;

/// <summary>
/// Animates a floating score label (+10) from a spawn position toward a target HUD position,
/// then destroys itself. Driven by ScorePopupSpawner.
/// </summary>
public class ScorePopup : MonoBehaviour
{
    private const float TravelDuration = 0.7f;
    private const float FadeInDuration = 0.1f;
    private const float ScaleUpAmount = 1.4f;
    private const float ScaleUpDuration = 0.1f;

    [SerializeField] private TextMeshProUGUI _label;

    private RectTransform _rectTransform;

    private void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
    }

    /// <summary>
    /// Initialises the popup text and starts the fly-to-HUD animation.
    /// </summary>
    /// <param name="points">Points to display (e.g. 10).</param>
    /// <param name="startAnchoredPos">Spawn position in Canvas local space.</param>
    /// <param name="targetAnchoredPos">HUD score element position in Canvas local space.</param>
    public void Play(int points, Vector2 startAnchoredPos, Vector2 targetAnchoredPos)
    {
        _label.text = $"+{points}";
        _rectTransform.anchoredPosition = startAnchoredPos;
        StartCoroutine(AnimateRoutine(startAnchoredPos, targetAnchoredPos));
    }

    private IEnumerator AnimateRoutine(Vector2 from, Vector2 to)
    {
        // Fade in
        Color color = _label.color;
        color.a = 0f;
        _label.color = color;

        float elapsed = 0f;
        while (elapsed < FadeInDuration)
        {
            elapsed += Time.deltaTime;
            color.a = Mathf.Clamp01(elapsed / FadeInDuration);
            _label.color = color;
            yield return null;
        }

        // Scale punch
        Vector3 originalScale = _rectTransform.localScale;
        Vector3 bigScale = originalScale * ScaleUpAmount;
        elapsed = 0f;
        while (elapsed < ScaleUpDuration)
        {
            elapsed += Time.deltaTime;
            _rectTransform.localScale = Vector3.Lerp(originalScale, bigScale, elapsed / ScaleUpDuration);
            yield return null;
        }
        elapsed = 0f;
        while (elapsed < ScaleUpDuration)
        {
            elapsed += Time.deltaTime;
            _rectTransform.localScale = Vector3.Lerp(bigScale, originalScale, elapsed / ScaleUpDuration);
            yield return null;
        }
        _rectTransform.localScale = originalScale;

        // Fly toward HUD with ease-in
        elapsed = 0f;
        while (elapsed < TravelDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.SmoothStep(0f, 1f, elapsed / TravelDuration);
            _rectTransform.anchoredPosition = Vector2.Lerp(from, to, t);

            // Fade out in the last 30 %
            float fadeT = Mathf.InverseLerp(TravelDuration * 0.7f, TravelDuration, elapsed);
            color.a = Mathf.Lerp(1f, 0f, fadeT);
            _label.color = color;

            yield return null;
        }

        Destroy(gameObject);
    }
}
