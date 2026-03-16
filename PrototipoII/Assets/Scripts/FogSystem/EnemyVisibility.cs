using UnityEngine;

public class EnemyVisibility : MonoBehaviour
{
    [SerializeField] private GameObject enemyModel;

    private void Update()
    {
        FogState state = FogManager.Instance.GetFogState(transform.position);
        
        enemyModel.SetActive(state == FogState.None);
    }
    
    // ===== Getters y Setters =====
    public void SetVisibility(bool visible)
    {
        enemyModel.SetActive(visible);
    }
}