using UnityEngine;
using UnityEngine.UI;

public class TimelineTrackUI : MonoBehaviour
{
    [SerializeField] private RectTransform trackRect;
    [SerializeField] private GameObject blockPrefab;
    [SerializeField] private float blockHeight = 18f;

    public void CreateBlock(TimelineActionData actionData, float totalTimelineDuration)
    {
        if (trackRect == null || blockPrefab == null || totalTimelineDuration <= 0f)
            return;

        GameObject block = Instantiate(blockPrefab, trackRect);
        RectTransform blockRect = block.GetComponent<RectTransform>();
        Image image = block.GetComponent<Image>();

        if (blockRect == null || image == null)
            return;

        float trackWidth = trackRect.rect.width;

        float normalizedStart = Mathf.Clamp01(actionData.startTime / totalTimelineDuration);
        float normalizedDuration = Mathf.Clamp01(actionData.duration / totalTimelineDuration);

        float x = normalizedStart * trackWidth;
        float width = Mathf.Max(4f, normalizedDuration * trackWidth);

        blockRect.anchorMin = new Vector2(0f, 0.5f);
        blockRect.anchorMax = new Vector2(0f, 0.5f);
        blockRect.pivot = new Vector2(0f, 0.5f);

        blockRect.anchoredPosition = new Vector2(x, 0f);
        blockRect.sizeDelta = new Vector2(width, blockHeight);

        image.color = actionData.color;
    }
}
