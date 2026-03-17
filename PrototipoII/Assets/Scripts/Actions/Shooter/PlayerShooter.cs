using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
public class PlayerShooter : MonoBehaviour
{
    //De momwnto prueba inicial con raycast, luego se implementará con proyectiles,etc..
    [Header("Referencias")]
    [SerializeField] private Camera cam;
    [SerializeField] private Transform weaponArm;
    [SerializeField] private ActionController actionController;
    
    [SerializeField] private float rotationSpeed = 720f;
    [SerializeField] private LayerMask aimMask = ~0;//Capa para el raycast, por defecto TODO:capa específica para el raycast
    
    [Header("Shoot")]
    [SerializeField] private float damage = 50f;
    [SerializeField] private float range = 100f;
    [SerializeField] private float fireDuration = 0.1f;
    [SerializeField] private LayerMask shootMask = ~0;//(..)
    
    private Vector3 currentAimPoint;
    private Coroutine shootingCoroutine;
    private bool isAiming;
    
    
    private void Update()
    {
        UpdateAimPoint();

        if (isAiming)
        {
            RotateTowardsAimPoint();
        }
    }
    
    private void UpdateAimPoint()
    {
        if (cam == null) return;

        Ray ray = cam.ScreenPointToRay(Mouse.current.position.ReadValue());

        if (Physics.Raycast(ray, out RaycastHit hit, 500f, aimMask))
        {
            currentAimPoint = hit.point;
        }
        else
        {
            currentAimPoint = ray.origin + ray.direction * 50f;
        }
    }
    
    private void RotateTowardsAimPoint()
    {
        Vector3 flatDirection = currentAimPoint - transform.position;
        flatDirection.y = 0f;

        if (flatDirection.sqrMagnitude < 0.001f)
            return;

        Quaternion targetRotation = Quaternion.LookRotation(flatDirection);
        transform.rotation = Quaternion.RotateTowards(
            transform.rotation,
            targetRotation,
            rotationSpeed * Time.deltaTime
        );
    }

    
    public void StartAim()
    {
        if (actionController == null) return;

        if (!actionController.CanUseUpperBody())
            return;

        isAiming = true;
        actionController.SetUpperBodyState(UpperBodyState.Aiming);
    }

    public void StopAim()
    {
        if (actionController == null) return;

        isAiming = false;

        if (actionController.GetUpperBodyState() == UpperBodyState.Aiming)
        {
            actionController.SetUpperBodyState(UpperBodyState.None);
        }
    }
    
    public void Shoot()
    {
        if (actionController == null) return;

        bool canShoot =
            actionController.CanUseUpperBody() ||
            actionController.GetUpperBodyState() == UpperBodyState.Aiming;

        if (!canShoot)
            return;

        if (shootingCoroutine != null)
            StopCoroutine(shootingCoroutine);

        shootingCoroutine = StartCoroutine(ShootRoutine());
    }
    
    private IEnumerator ShootRoutine()
    {
        isAiming = false;
        actionController.SetUpperBodyState(UpperBodyState.Shooting);

        Vector3 origin = weaponArm != null
            ? weaponArm.position
            : transform.position + Vector3.up * 1.2f;

        Vector3 direction = (currentAimPoint - origin).normalized;

        if (Physics.Raycast(origin, direction, out RaycastHit hit, range, shootMask))
        {
            Debug.DrawLine(origin, hit.point, Color.red, 1f);

            Health health = hit.collider.GetComponentInParent<Health>();
            if (health != null)
            {
                health.TakeDamage(damage);
            }
            //TODO:todo lo visual , particulas, sonido,etc
            
        }
        else
        {
            Debug.DrawRay(origin, direction * range, Color.red, 1f);
        }

        yield return new WaitForSeconds(fireDuration);

        actionController.SetUpperBodyState(UpperBodyState.None);
        shootingCoroutine = null;
    }

    public Vector3 GetAimPoint()
    {
        return currentAimPoint;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(currentAimPoint, 0.15f);

        if (weaponArm != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(weaponArm.position, currentAimPoint);
        }
    }
    
    
    
    
    
    
    
}
