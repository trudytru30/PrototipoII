using UnityEngine;
using UnityEngine.UI;

public class TimelineTrackUI : MonoBehaviour
{
    [SerializeField] private Transform blockContainer;
    [SerializeField] private TimelineMovement referenceMovement;
    [SerializeField] private GameObject blockPrefab;
    [SerializeField] private float blockHeight = 18f;
    [SerializeField] private float spawnX = 0f;
    [SerializeField] private float spawnY = 0f;

    public void CreateBlock(TimelineActionData actionData, float totalTimelineDuration)
    {
        if (blockPrefab == null)
            return;

        Transform parent = blockContainer != null ? blockContainer : this.transform;
        GameObject block = Instantiate(blockPrefab, parent);
        RectTransform blockRect = block.GetComponent<RectTransform>();
        Image image = block.GetComponent<Image>();

        if (blockRect == null || image == null)
            return;

        float speed = referenceMovement != null ? referenceMovement.Speed : 1f;
        float width = Mathf.Max(4f, actionData.duration * speed);

        blockRect.anchorMin = new Vector2(0f, 0.5f);
        blockRect.anchorMax = new Vector2(0f, 0.5f);
        blockRect.pivot     = new Vector2(0f, 0.5f);

        blockRect.anchoredPosition = new Vector2(spawnX, spawnY);
        blockRect.sizeDelta        = new Vector2(width, blockHeight);

        image.color = actionData.color;

        if (referenceMovement != null)
            block.AddComponent<TimelineMovement>().SetSpeed(speed);
    }
}
