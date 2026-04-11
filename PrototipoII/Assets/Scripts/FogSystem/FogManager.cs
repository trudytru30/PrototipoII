using UnityEngine;
using System.Collections.Generic;

public class FogManager : MonoBehaviour
{
    public static FogManager Instance;
    private static readonly int origin = Shader.PropertyToID("_MapOrigin");
    private static readonly int size = Shader.PropertyToID("_MapSize");
    private static readonly int texture = Shader.PropertyToID("_FogTexture");
    private static readonly int fogTextureSize = Shader.PropertyToID("_FogTextureSize");

    [Header("Map Settings")]
    [SerializeField] private LayerMask obstaclesMask;
    [SerializeField] private Transform mapTransform;
    [SerializeField] private float cellSize = 1f;
    [SerializeField] private Material fogMaterial;
    private Vector3 mapOrigin;
    private int mapWidth, mapHeight;
    private Texture2D fogTexture;
    
    [SerializeField] private int rayCount = 120;
    private HashSet<Vector2Int> visibleCells = new();
    private List<VisionSource> activeSources = new();
    private List<EnemyVisibility> enemiesVisibles = new();
    private FogState[,] fogGrid;
    
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
    
    // Inicializar el grid de niebla en base al tamanio del mapa
    private void InitializeGrid()
    {
        Renderer mapRenderer = mapTransform.GetComponent<Renderer>();
        Vector3 mapSize = mapRenderer.bounds.size;
        
        mapWidth = Mathf.CeilToInt(mapSize.x / cellSize);
        mapHeight = Mathf.CeilToInt(mapSize.z / cellSize);
        mapOrigin = mapTransform.position - mapRenderer.bounds.extents;
        
        fogGrid = new FogState[mapWidth, mapHeight];
        fogTexture = new Texture2D(mapWidth, mapHeight);
        fogTexture.filterMode = FilterMode.Point;
        fogTexture.wrapMode = TextureWrapMode.Clamp;
        
        for (int x = 0; x < mapWidth; x++)
        {
            for (int y = 0; y < mapHeight; y++)
            {
                fogGrid[x, y] = FogState.Total;
            }
        }
        Shader.SetGlobalVector(origin, new Vector2(mapOrigin.x, mapOrigin.z));
        Shader.SetGlobalVector(size, new Vector2(mapWidth * cellSize, mapHeight * cellSize));
        Shader.SetGlobalVector(fogTextureSize, new Vector2(mapWidth, mapHeight));
    }
    
    // Manejar textura de la niebla
    private void UpdateFogTexture()
    {
        for (int x = 0; x < mapWidth; x++)
        {
            for (int y = 0; y < mapHeight; y++)
            {
                Color color = new Color(0, 0, 0, 1f);   // Inicializar en negro
                switch (fogGrid[x, y])
                {
                    case FogState.None:
                        color = new Color(0, 0, 0, 0f); // Transparente
                        break;
                    case FogState.Partial:
                        color = new Color(0, 0, 0, 0.4f);   // Gris
                        break;
                    case FogState.Total:
                        color = new Color(0, 0, 0, 1f); // Negro
                        break;
                }
                fogTexture.SetPixel(x, y, color);
            }
        }
        fogTexture.Apply();
    }
    
    // Registrar una fuente de vision
    public void RegisterVisionSource(VisionSource source)
    {
        activeSources.Add(source);
    }

    private void LateUpdate()
    {
        ResetVisible();

        foreach (var source in activeSources)
        {
            RevealArea(source.transform.position, source.GetVisionRange());
        }
        
        UpdateEnemyVisibility();
        activeSources.Clear();
        UpdateFogTexture();
        fogMaterial.SetTexture(texture, fogTexture);
    }
    
    // Actualiza la visibilidad de los enemigos
    private void UpdateEnemyVisibility()
    {
        foreach (var enemy in enemiesVisibles)
        {
            FogState state = GetFogState(enemy.transform.position);
            bool visible = state == FogState.None;
            enemy.SetVisibility(visible);
        }
    }
    
    // Registrar a un enemigo para hacerlo visible
    public void RegisterEnemy(EnemyVisibility enemy)
    {
        if(!enemiesVisibles.Contains(enemy))
            enemiesVisibles.Add(enemy);
        Debug.Log("Enemy registered");
    }
    
    // Desregistrar a un enemigo para ocultarlo
    public void UnregisterEnemy(EnemyVisibility enemy)
    {
        enemiesVisibles.Remove(enemy);
        Debug.Log("Enemy unregistered");
    }

    // Actualiza a niebla parcial si ya se ha dejado de ver pero se han visto antes
    public void ResetVisible()
    {
        foreach (var cell in visibleCells)
        {
            if(fogGrid[cell.x, cell.y] == FogState.None)
                fogGrid[cell.x, cell.y] = FogState.Partial;
        }
        visibleCells.Clear();
    }
    
    // Revela un area de celdas alrededor de la fuente de vision
    public void RevealArea(Vector3 origin, float radius)
    {
        float angleStep = 360f / rayCount;

        for (int i = 0; i < rayCount; i++)
        {
            float angle = angleStep * i * Mathf.Deg2Rad;

            Vector3 dir = new Vector3(Mathf.Cos(angle), 0f, Mathf.Sin(angle));

            CastVisionRay(origin, dir, radius);
        }
    }
    
    // Revela una linea de celdas
    private void CastVisionRay(Vector3 origin, Vector3 direction, float radius)
    {
        Vector3 start = origin + Vector3.up * 1.05f;

        RaycastHit hit;
        Debug.DrawRay(start, direction * radius, Color.red);
        if (Physics.Raycast(start, direction, out hit, radius, obstaclesMask))
        {
            RevealLine(origin, hit.point);
        }
        else
        {
            RevealLine(origin, origin + direction * radius);
        }
    }
    
    // Revela una linea de celdas
    private void RevealLine(Vector3 start, Vector3 end)
    {
        float distance = Vector3.Distance(start, end);
        int steps = Mathf.CeilToInt(distance/(cellSize*0.5f));

        for (int i = 0; i <= steps; i++)
        {
            Vector3 pos = Vector3.Lerp(start, end, i / (float)steps);
            Vector2Int cell = WorldToGrid(pos);
            
            if (InsideGrid(cell))
            {
                Vector3 cellCenter = GridToWorld(cell);
                fogGrid[cell.x, cell.y] = FogState.None;
                visibleCells.Add(cell);
            }
        }
    }
    
    // Convertir world a celda
    private Vector2Int WorldToGrid(Vector3 worldPos)
    {
        int x = Mathf.FloorToInt((worldPos.x - mapOrigin.x) / cellSize);
        int y = Mathf.FloorToInt((worldPos.z - mapOrigin.z) / cellSize);
        return new Vector2Int(x, y);
    }
    
    // Convertir celda a world (centro de celda)
    private Vector3 GridToWorld(Vector2Int cell)
    {
        float x = mapOrigin.x + cell.x * cellSize + cellSize * 0.5f;
        float z = mapOrigin.z + cell.y * cellSize + cellSize * 0.5f;
        return new Vector3(x, 0f, z);
    }
    
    // Verificar si una celda esta dentro del grid
    private bool InsideGrid(Vector2Int cell)
    {
        return cell.x >= 0 && cell.x < mapWidth && cell.y >= 0 && cell.y < mapHeight;
    }
    
    // Devuelve el estado de la niebla de una celda
    public FogState GetFogState(Vector3 worldPos)
    {
       Vector2Int cell = WorldToGrid(worldPos);

        if(!InsideGrid(cell)) return FogState.Total;
        
        return fogGrid[cell.x, cell.y];
    }
    
    void OnDrawGizmos()
    {
        if (fogGrid == null) return;

        for (int x = 0; x < mapWidth; x++)
        {
            for (int y = 0; y < mapHeight; y++)
            {
                if (fogGrid[x, y] == FogState.None)
                {
                    Vector3 pos = GridToWorld(new Vector2Int(x, y));

                    pos.y += 1.6f; // levantar un poco

                    Gizmos.color = new Color(0, 1, 0, 0.5f);
                    Gizmos.DrawCube(pos, Vector3.one * cellSize);
                }
            }
        }
    }
}