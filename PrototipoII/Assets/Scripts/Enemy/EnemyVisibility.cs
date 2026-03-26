using UnityEngine;

public class EnemyVisibility : MonoBehaviour
{
    [SerializeField] private GameObject enemyModel;
    
    private void Awake()
    {
        SetVisibility(false);
    }

    private void Start()
    {
        if (FogManager.Instance != null)
        {
            FogManager.Instance.RegisterEnemy(this);
        }
        else
        {
            Debug.LogError("FogManager not found");
        }
    }
    
    private void OnDisable()
    {
        FogManager.Instance.UnregisterEnemy(this);
    }

    public void SetVisibility(bool visible)
    {
        if(enemyModel.activeSelf == visible) return;
        enemyModel.SetActive(visible);
    }
}