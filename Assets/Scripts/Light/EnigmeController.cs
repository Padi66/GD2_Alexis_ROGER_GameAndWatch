using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering.Universal;

public class EnigmeController : MonoBehaviour
{
    [SerializeField] private EnigmeDatas enigmeDatas;

    [Header("Point Images")]
    [SerializeField] private Image[] pointImages;
    [SerializeField] private Color activeColor = Color.green;
    [SerializeField] private Color inactiveColor = Color.gray;
    [SerializeField] private Color blinkColor = Color.white;

    [Header("Win Light")]
    [SerializeField] private Light2D winLight;

    [Header("Victory Blink Settings")]
    [SerializeField] private float blinkInterval = 0.08f;
    [SerializeField] private int blinkCycles = 5;

    private Coroutine _blinkCoroutine;

    private void Start()
    {
        RefreshPoints();

        if (enigmeDatas.IsAllActivated())
        {
            TriggerVictoryBlink();
        }
        else if (winLight != null)
        {
            winLight.enabled = false;
        }
    }

    /// <summary>
    /// Refreshes all point image colors from EnigmeDatas and triggers the victory blink if all are activated.
    /// </summary>
    public void RefreshPoints()
    {
        for (int i = 0; i < pointImages.Length; i++)
        {
            pointImages[i].color = enigmeDatas.LampsActivated[i] ? activeColor : inactiveColor;
        }

        if (enigmeDatas.IsAllActivated())
        {
            TriggerVictoryBlink();
        }
    }

    /// <summary>
    /// Starts the sequential blink effect on all point images.
    /// </summary>
    public void TriggerVictoryBlink()
    {
        if (winLight != null)
        {
            winLight.enabled = true;
        }

        if (_blinkCoroutine != null)
        {
            StopCoroutine(_blinkCoroutine);
        }

        _blinkCoroutine = StartCoroutine(BlinkSequentially());
    }

    private IEnumerator BlinkSequentially()
    {
        for (int cycle = 0; cycle < blinkCycles; cycle++)
        {
            // Flash each point to blinkColor one by one
            for (int i = 0; i < pointImages.Length; i++)
            {
                pointImages[i].color = blinkColor;
                yield return new WaitForSeconds(blinkInterval);
                pointImages[i].color = activeColor;
            }
        }

        // Ensure all points end on activeColor
        for (int i = 0; i < pointImages.Length; i++)
        {
            pointImages[i].color = activeColor;
        }
    }
}
