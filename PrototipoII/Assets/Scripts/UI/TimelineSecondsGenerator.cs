using UnityEngine;
using UnityEngine.UI;
using TMPro;


// Genera visualmente las marcas de segundos de la timeline.
// IMPORTANTE:
// - El segundo 0 se coloca en x = 0 dentro de secondsLayer
// - NO mueve el content
// - El scroll lo hará otro script moviendo el RectTransform de Content
// - Ahora mismo no coincide con el 0 de la timeline, 
//   porque se hace en el script grande la UITimelineController (float centerX=viewportWidt..


public class TimelineSecondsGenerator : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private RectTransform content;
    [SerializeField] private RectTransform secondsLayer;

    [Header("Configuración temporal")]
    [SerializeField] private int totalSeconds = 60;
    [SerializeField] private float pixelsPerSecond = 100f;
    [SerializeField] private int subdivisionsPerSecond = 4; // 4 = marcas cada 0.25s

    [Header("Estilo líneas")]
    [SerializeField] private float majorLineHeight = 24f;
    [SerializeField] private float majorLineWidth = 2f;
    [SerializeField] private Color majorLineColor = new Color(1f, 1f, 1f, 0.55f);

    [SerializeField] private float minorLineHeight = 12f;
    [SerializeField] private float minorLineWidth = 1f;
    [SerializeField] private Color minorLineColor = new Color(1f, 1f, 1f, 0.20f);

    [Header("Estilo texto")]
    [SerializeField] private float labelFontSize = 16f;
    [SerializeField] private Color labelColor = Color.white;
    [SerializeField] private Vector2 labelOffset = new Vector2(4f, -10f);
    [SerializeField] private Vector2 labelSize = new Vector2(40f, 20f);

    [Header("Opciones")]
    [SerializeField] private bool generateOnStart = true;
    [SerializeField] private bool clearPreviousChildren = true;
    [SerializeField] private bool resizeContentToFit = true;

    private void Start()
    {
        if (generateOnStart)
        {
            Generate();
        }
    }


    // Genera líneas de segundo, subdivisiones y etiquetas.
    public void Generate()
    {
        if (secondsLayer == null)
        {
            Debug.LogError("TimelineSecondsGenerator: secondsLayer no está asignado.");
            return;
        }

        if (clearPreviousChildren)
        {
            ClearChildren(secondsLayer);
        }

        if (resizeContentToFit && content != null)
        {
            float totalWidth = totalSeconds * pixelsPerSecond;
            content.sizeDelta = new Vector2(totalWidth + 500f, content.sizeDelta.y);
        }

        for (int second = 0; second <= totalSeconds; second++)
        {
            float secondX = second * pixelsPerSecond;

            // Línea principal del segundo
            CreateLine(
                parent: secondsLayer,
                name: $"SecondLine_{second}",
                anchoredX: secondX,
                width: majorLineWidth,
                height: majorLineHeight,
                color: majorLineColor
            );

            // Etiqueta numérica
            CreateLabel(
                parent: secondsLayer,
                textValue: second.ToString(),
                anchoredPosition: new Vector2(secondX + labelOffset.x, labelOffset.y)
            );

            // Subdivisiones entre este segundo y el siguiente
            if (second < totalSeconds && subdivisionsPerSecond > 1)
            {
                for (int sub = 1; sub < subdivisionsPerSecond; sub++)
                {
                    float t = sub / (float)subdivisionsPerSecond;
                    float subX = secondX + t * pixelsPerSecond;

                    CreateLine(
                        parent: secondsLayer,
                        name: $"MinorLine_{second}_{sub}",
                        anchoredX: subX,
                        width: minorLineWidth,
                        height: minorLineHeight,
                        color: minorLineColor
                    );
                }
            }
        }
    }

    private void CreateLine(RectTransform parent, string name, float anchoredX, float width, float height, Color color)
    {
        GameObject lineObj = new GameObject(name, typeof(RectTransform), typeof(Image));
        lineObj.transform.SetParent(parent, false);

        Image image = lineObj.GetComponent<Image>();
        image.color = color;

        RectTransform rect = lineObj.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0f, 0.5f);
        rect.anchorMax = new Vector2(0f, 0.5f);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.sizeDelta = new Vector2(width, height);
        rect.anchoredPosition = new Vector2(anchoredX, 0f);
    }

    private void CreateLabel(RectTransform parent, string textValue, Vector2 anchoredPosition)
    {
        GameObject textObj = new GameObject($"Label_{textValue}", typeof(RectTransform));
        textObj.transform.SetParent(parent, false);

        TextMeshProUGUI text = textObj.AddComponent<TextMeshProUGUI>();
        text.text = textValue;
        text.fontSize = labelFontSize;
        text.color = labelColor;
        text.alignment = TextAlignmentOptions.TopLeft;
        text.raycastTarget = false;

        RectTransform rect = textObj.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0f, 0.5f);
        rect.anchorMax = new Vector2(0f, 0.5f);
        rect.pivot = new Vector2(0f, 1f);
        rect.sizeDelta = labelSize;
        rect.anchoredPosition = anchoredPosition;
    }

    private void ClearChildren(RectTransform parent)
    {
        for (int i = parent.childCount - 1; i >= 0; i--)
        {
            Destroy(parent.GetChild(i).gameObject);
        }
    }
}