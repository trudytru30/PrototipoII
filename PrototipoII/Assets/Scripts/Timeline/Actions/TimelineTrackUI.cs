using UnityEngine;
using UnityEngine.UI;

public class TimelineTrackUI : MonoBehaviour
{
    [SerializeField] private RectTransform trackRect;
    [SerializeField] private RectTransform blockContainer;
    [SerializeField] private GameObject blockPrefab;
    [SerializeField] private float blockHeight = 18f;

    public void CreateBlock(TimelineActionData actionData, float totalTimelineDuration)
    {
        if (trackRect == null || blockPrefab == null || totalTimelineDuration <= 0f)
            return;

        RectTransform parent = blockContainer != null ? blockContainer : trackRect;
        GameObject block = Instantiate(blockPrefab, parent);
        RectTransform blockRect = block.GetComponent<RectTransform>();
        Image image = block.GetComponent<Image>();

        if (blockRect == null || image == null)
            return;

        // AÑADIDO: fallback por si el layout de Canvas aún no se ha calculado en este frame
        // rect.width puede ser 0 si CreateBlock se llama en el mismo frame que el Awake del canvas
        float trackWidth = trackRect.rect.width;
        if (trackWidth <= 0f)
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(trackRect);
            trackWidth = trackRect.rect.width;
        }

        float normalizedStart    = Mathf.Clamp01(actionData.startTime / totalTimelineDuration);
        float normalizedDuration = Mathf.Clamp01(actionData.duration  / totalTimelineDuration);

        float x     = normalizedStart * trackWidth;
        float width = Mathf.Max(4f, normalizedDuration * trackWidth);

        blockRect.anchorMin = new Vector2(0f, 0.5f);
        blockRect.anchorMax = new Vector2(0f, 0.5f);
        blockRect.pivot     = new Vector2(0f, 0.5f);

        blockRect.anchoredPosition = new Vector2(x, 0f);
        blockRect.sizeDelta        = new Vector2(width, blockHeight);

        image.color = actionData.color;
    }
}