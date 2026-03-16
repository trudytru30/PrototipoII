using UnityEngine;

public class FogManager : MonoBehaviour
{
    public static FogManager Instance;

    [SerializeField] private Transform mapTransform;
    [SerializeField] private int mapWidth = 100;
    [SerializeField] private int mapHeight = 100;
    [SerializeField] private float cellSize = 1f;
    
    private FogState[,] fogGrid;
    private Vector3 mapOrigin;
    
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        InitializeGrid();
    }
    
    // Inicializar el grid de niebla
    private void InitializeGrid()
    {
        Renderer renderer = mapTransform.GetComponent<Renderer>();
        
        Vector3 mapSize = renderer.bounds.size;
        
        mapWidth = Mathf.CeilToInt(mapSize.x / cellSize);
        mapHeight = Mathf.CeilToInt(mapSize.z / cellSize);
        mapOrigin = renderer.bounds.min;
        
        fogGrid = new FogState[mapWidth, mapHeight];

        for (int x = 0; x < mapWidth; x++)
        {
            for (int y = 0; y < mapHeight; y++)
            {
                fogGrid[x, y] = FogState.Total;
            }
        }
    }

    // Actualizar a niebla parcial si ya se ha explorado
    public void ResetVisibility()
    {
        for (int x = 0; x < mapWidth; x++)
        {
            for (int y = 0; y < mapHeight; y++)
            {
                if(fogGrid[x, y] == FogState.None)
                    fogGrid[x, y] = FogState.Partial;
            }
        }
    }
    
    // Revela las celdas que ve el player
    public void RevealCell(Vector3 worldPos)
    {
        int x = Mathf.FloorToInt((worldPos.x - mapOrigin.x) / cellSize);
        int y = Mathf.FloorToInt((worldPos.z - mapOrigin.z) / cellSize);
        
        if (x < 0 || x >= mapWidth || y < 0 || y >= mapHeight)
            return;
        
        fogGrid[x, y] = FogState.None;
    }

    // Revela una linea de celdas que ve el player
    public void RevealLine(Vector3 start, Vector3 end)
    {
        float distance = Vector3.Distance(start, end);
        
        int steps = Mathf.CeilToInt(distance / cellSize);

        for (int i = 0; i <= steps; i++)
        {
            Vector3 pos = Vector3.Lerp(start, end, (float)i / steps);
            RevealCell(pos);
        }
    }
    
    // Devuelve el estado de la niebla de una celda
    public FogState GetFogState(Vector3 worldPos)
    {
        int x = Mathf.FloorToInt((worldPos.x - mapOrigin.x) / cellSize);
        int y = Mathf.FloorToInt((worldPos.z - mapOrigin.z) / cellSize);

        if (x < 0 || x >= mapWidth || y < 0 || y >= mapHeight)
            return FogState.Total;
        
        return fogGrid[x, y];
    }
}