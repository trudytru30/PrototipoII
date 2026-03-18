using UnityEngine;

public class EnemyBase : MonoBehaviour
{
    [SerializeField] private float damage;
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("TriggerPlayer");
            other.gameObject.GetComponent<Health>().TakeDamage(damage);
        }
    }
}
