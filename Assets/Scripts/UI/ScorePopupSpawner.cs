using UnityEngine;

/// <summary>
/// Listens to ScoreManager.OnScoreAdded and spawns a ScorePopup when
/// the player earns exactly 10 points, flying from the player's screen
/// position toward the score HUD element in the top-left corner.
/// </summary>
public class ScorePopupSpawner : MonoBehaviour
{
    private const int TriggerAmount = 10;

    [Header("References")]
    [SerializeField] private ScoreManager _scoreManager;
    [SerializeField] private ScorePopup _popupPrefab;
    [SerializeField] private RectTransform _scoreHudTarget;

    private Canvas _canvas;
    private RectTransform _canvasRect;
    private Transform _playerTransform;

    private void Awake()
    {
        _canvas = GetComponentInParent<Canvas>();
        _canvasRect = _canvas != null ? _canvas.GetComponent<RectTransform>() : null;
    }

    private void Start()
    {
        // Resolve ScoreManager
        if (_scoreManager == null)
            _scoreManager = FindObjectOfType<ScoreManager>();

        if (_scoreManager != null)
            _scoreManager.OnScoreAdded += HandleScoreAdded;
        else
            Debug.LogWarning("[ScorePopupSpawner] ScoreManager introuvable.");

        // Resolve Player transform via tag
        GameObject player = GameObject.FindWithTag("Player");
        if (player != null)
            _playerTransform = player.transform;
        else
            Debug.LogWarning("[ScorePopupSpawner] Aucun GameObject taggé 'Player' trouvé.");
    }

    private void OnDestroy()
    {
        if (_scoreManager != null)
            _scoreManager.OnScoreAdded -= HandleScoreAdded;
    }

    private void HandleScoreAdded(int points)
    {
        if (points != TriggerAmount) return;
        SpawnPopup(points);
    }

    private void SpawnPopup(int points)
    {
        if (_popupPrefab == null || _scoreHudTarget == null || _canvas == null) return;

        Vector2 spawnAnchoredPos = ComputePlayerAnchoredPosition();

        // Convert the ScoreText world position to canvas local space so the
        // target is correct regardless of the ScoreText's anchor or scale.
        Vector2 targetAnchoredPos = _canvasRect.InverseTransformPoint(_scoreHudTarget.position);

        ScorePopup popup = Instantiate(_popupPrefab, _canvas.transform);
        popup.Play(points, spawnAnchoredPos, targetAnchoredPos);
    }

    /// <summary>
    /// Converts the player's world position to anchored coordinates in the Canvas.
    /// Falls back to canvas centre if the player transform is unavailable.
    /// </summary>
    private Vector2 ComputePlayerAnchoredPosition()
    {
        Camera cam = Camera.main;

        if (_playerTransform != null && cam != null && _canvasRect != null)
        {
            Vector3 screenPos = cam.WorldToScreenPoint(_playerTransform.position);
            Camera uiCam = _canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : cam;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                _canvasRect,
                screenPos,
                uiCam,
                out Vector2 localPoint);
            return localPoint;
        }

        return Vector2.zero;
    }
}
