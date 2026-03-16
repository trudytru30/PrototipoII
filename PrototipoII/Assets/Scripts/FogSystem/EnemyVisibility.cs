using UnityEngine;

public class EnemyVisibility : MonoBehaviour
{
    [SerializeField] private GameObject enemyModel;
    
    private void Awake()
    {
        SetVisibility(false);
    }
    
    private void OnEnable()
    {
        FogManager.Instance.RegisterEnemy(this);
        Debug.Log("Enemy is visible");
    }

    private void OnDisable()
    {
        FogManager.Instance.UnregisterEnemy(this);
        Debug.Log("Enemy is invisible");
    }

    public void SetVisibility(bool visible)
    {
        if(enemyModel.activeSelf == visible) return;
        enemyModel.SetActive(visible);
    }
}