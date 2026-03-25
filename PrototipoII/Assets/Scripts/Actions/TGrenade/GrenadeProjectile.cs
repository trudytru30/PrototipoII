using UnityEngine;

public class GrenadeProjectile : MonoBehaviour
{
    [Header("Grenade")]
    [SerializeField] private float explosionDelay = 2f;
    [SerializeField] private float explosionRadius = 4f;
    [SerializeField] private float damage = 100f;
    [SerializeField] private LayerMask damageMask = ~0;

    [Header("FX")]
    //TODO: poner fx
    
    private bool hasExploded;

    private void Start()
    {
        Invoke(nameof(Explode), explosionDelay);//invoca la explosion despues de un tiempo
    }

    public void Launch(Vector3 velocity)
    {
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = velocity;
        }
    }

    private void Explode()
    {
        if (hasExploded) return;
        hasExploded = true;

        Collider[] hits = Physics.OverlapSphere(
            transform.position,
            explosionRadius,
            damageMask,
            QueryTriggerInteraction.Ignore
        );

        for (int i = 0; i < hits.Length; i++)
        {
            Health health = hits[i].GetComponentInParent<Health>();
            if (health != null)
            {
                health.TakeDamage(damage);
            }
        }

        //TODO: instanciar vfx
        //vfx explision,sonido,etc

        Destroy(gameObject);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}
