using UnityEngine;
using UnityEngine.Playables;
using TMPro;

public class TimelineUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayableDirector director;
    // MODIFICADO: eliminado cursorRect — el cursor es un elemento fijo en la UI, no se mueve por código
    // barRect es ahora el panel de contenido (bloques) que se desplaza hacia la izquierda
    [SerializeField] private RectTransform barRect;
    [SerializeField] private TextMeshProUGUI timeText;
    [SerializeField] private TextMeshProUGUI stateText;

    private void Update()
    {
        if (director == null || barRect == null)
            return;

        UpdateBar();
        UpdateTexts();
    }

    // MODIFICADO: renombrado de UpdateCursor a UpdateBar y lógica invertida
    // Antes se movía cursorRect hacia la derecha sobre una barra estática
    // Ahora el cursor verde es fijo en la UI y se desplaza barRect hacia la izquierda
    // Requiere que barRect tenga anchorMin/Max = (0, 0.5) y pivot = (0, 0.5) en el Inspector
    private void UpdateBar()
    {
        if (director.duration <= 0) return;

        float normalizedTime = Mathf.Clamp01((float)(director.time / director.duration));
        float totalWidth = barRect.rect.width;

        Vector2 pos = barRect.anchoredPosition;
        pos.x = -normalizedTime * totalWidth;
        barRect.anchoredPosition = pos;
    }

    private void UpdateTexts()
    {
        if (timeText != null)
            timeText.text = $"Tiempo: {director.time:F1}s";

        if (stateText != null)
            stateText.text = director.state == PlayState.Playing ? "Estado: En marcha" : "Estado: Pausado";
    }
}