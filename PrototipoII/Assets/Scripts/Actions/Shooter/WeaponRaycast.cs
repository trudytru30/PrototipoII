using UnityEngine;
/*
 * Logica de calculo de direccion,
 * aplicar dispersion,
 * raycast tanto para player como enemigo
 * aplicar daño
 * devolver info del disparo
 *
 * NO TOCAR(Fire())
 */
public class WeaponRaycast : MonoBehaviour
{
    [Header("Disparo")]
    [SerializeField] private float damage = 50f;
    [SerializeField] private float range = 100f;
    [SerializeField] private LayerMask shootMask = ~0;

    [Header("Dispersión")]
    [SerializeField] private bool useSpread = true;//player false
    [SerializeField] private float spreadAngle = 2f;
    
    public ShootResult Fire(Transform muzzle, Vector3 targetPoint, Transform ownerRoot)
    {
        ShootResult result = new ShootResult();//

        if (muzzle == null)//punto de salida del arma(prefab)
            return result;

        Vector3 origin = muzzle.position;

        // click con solo direccion , no el final de disparo
        Vector3 direction = (targetPoint - origin).normalized;
        direction = ApplySpread(direction);//dipersion en caso de activarla

        result.finalDirection = direction;

        if (Physics.Raycast(origin, direction, out RaycastHit hit, range, shootMask, QueryTriggerInteraction.Ignore))
        {
            //no daña a quien dispara(p o e)
            if (ownerRoot != null && hit.collider.transform.root == ownerRoot)
            {
                result.hit = false;
                result.finalPoint = origin + direction * range;
                result.hitInfo = hit;
                return result;
            }
            //el disparo impacta con algo
            result.hit = true;
            result.hitInfo = hit;
            result.finalPoint = hit.point;

            //si tiene health le aplica daño
            Health health = hit.collider.GetComponentInParent<Health>();
            if (health != null)
            {
                health.TakeDamage(damage);
            }

            return result;
        }
        //si no impacta con nada
        result.hit = false;
        result.finalPoint = origin + direction * range;
        return result;
    }

    private Vector3 ApplySpread(Vector3 baseDirection)
    {
        if (!useSpread || spreadAngle <= 0f)
        {
            return baseDirection;
        }

        Quaternion yaw = Quaternion.AngleAxis(Random.Range(-spreadAngle, spreadAngle), Vector3.up);
        Quaternion pitch = Quaternion.AngleAxis(Random.Range(-spreadAngle, spreadAngle), Vector3.right);

        return (yaw * pitch * baseDirection).normalized;//aplica la dispersion a la direccion base
    }

    public float GetDamage() => damage;
    public float GetRange() => range;
    public float GetSpreadAngle() => spreadAngle;
}