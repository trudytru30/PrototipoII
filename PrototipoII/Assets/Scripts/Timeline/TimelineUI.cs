using UnityEngine;
using UnityEngine.Playables;
using TMPro;

public class TimelineUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayableDirector director;
    [SerializeField] private RectTransform barRect;
    [SerializeField] private RectTransform cursorRect;
    [SerializeField] private TextMeshProUGUI timeText;
    [SerializeField] private TextMeshProUGUI stateText;

    private void Update()
    {
        if (director == null || barRect == null || cursorRect == null)
            return;

        UpdateCursor();
        UpdateTexts();
    }

    private void UpdateCursor()
    {
        if (director.duration <= 0)
            return;

        float normalizedTime = Mathf.Clamp01((float)(director.time / director.duration));

        float barWidth = barRect.rect.width;
        float x = normalizedTime * barWidth;

        Vector2 anchoredPos = cursorRect.anchoredPosition;
        anchoredPos.x = x;
        cursorRect.anchoredPosition = anchoredPos;
    }

    private void UpdateTexts()
    {
        if (timeText != null)
            timeText.text = $"Tiempo: {director.time:F1}s";

        if (stateText != null)
            stateText.text = director.state == PlayState.Playing ? "Estado: En marcha" : "Estado: Pausado";
    }
}
