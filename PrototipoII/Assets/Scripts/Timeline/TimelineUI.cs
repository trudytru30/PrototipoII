using UnityEngine;
using TMPro;

public class TimelineUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private TimelineClock clock;
    [SerializeField] private RectTransform barRect;
    [SerializeField] private TextMeshProUGUI timeText;
    [SerializeField] private TextMeshProUGUI stateText;

    private void Update()
    {
        if (clock == null || barRect == null)
            return;

        UpdateBar();
        UpdateTexts();
    }

    private void UpdateBar()
    {
        if (clock.TotalDuration <= 0f) return;

        float normalizedTime = Mathf.Clamp01(clock.CurrentTime / clock.TotalDuration);
        float totalWidth = barRect.rect.width;

        Vector2 pos = barRect.anchoredPosition;
        pos.x = -normalizedTime * totalWidth;
        barRect.anchoredPosition = pos;
    }

    private void UpdateTexts()
    {
        if (timeText != null)
            timeText.text = $"Tiempo: {clock.CurrentTime:F1}s";

        if (stateText != null)
            stateText.text = Time.timeScale > 0f ? "Estado: En marcha" : "Estado: Pausado";
    }
}
