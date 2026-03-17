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
    
    [Header("Disparo")]
    [SerializeField] private float damage = 50f;
    [SerializeField] private float range = 100f;
    [SerializeField] private float fireDuration = 0.1f;
    [SerializeField] private LayerMask shootMask = ~0;//(..)
    
    private Vector3 currentAimPoint;
    private Coroutine shootingCoroutine;//controla el tiempo de disparo
    private bool isAiming;
    
    
    private void Update()
    {
        UpdateAimPoint();//actualiza el punto de mira cada frame

        if (isAiming)
        {
            RotateTowardsAimPoint();
        }
    }
    
    private void UpdateAimPoint()
    {
        if (cam == null) return;

        Ray ray = cam.ScreenPointToRay(Mouse.current.position.ReadValue());//crea un rayo desde la camara hacia el punto del mouse

        if (Physics.Raycast(ray, out RaycastHit hit, 500f, aimMask))
        {
            currentAimPoint = hit.point;//si el raycast golpea algo, el punto de mira se actualiza al punto de impacto
        }
        else
        {
            currentAimPoint = ray.origin + ray.direction * 50f;//
        }
    }
    
    //Rotacion del personaje hacia el punto de mira
    private void RotateTowardsAimPoint()
    {
        Vector3 flatDirection = currentAimPoint - transform.position;
        flatDirection.y = 0f;

        if (flatDirection.sqrMagnitude < 0.001f)
        {
            return;
        }

        Quaternion targetRotation = Quaternion.LookRotation(flatDirection);
        //rota suave hacia el objetivo
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }

    //Apuntado
    public void StartAim()
    {
        if (actionController == null) return;

        if (!actionController.CanUseUpperBody())
            return;

        isAiming = true;
        actionController.SetUpperBodyState(UpperBodyState.Aiming);
    }

    //Deja de apuntar
    public void StopAim()
    {
        if (actionController == null) return;

        isAiming = false;

        if (actionController.GetUpperBodyState() == UpperBodyState.Aiming)
        {
            actionController.SetUpperBodyState(UpperBodyState.None);
        }
    }
    
    // Disparo
    public void Shoot()
    {
        if (actionController == null) return;

        //solo si usa el cuerpo superior o si ya esta apuntando, puede disparar
        bool canShoot = actionController.CanUseUpperBody() || actionController.GetUpperBodyState() == UpperBodyState.Aiming;

        if (!canShoot)
            return;

        if (shootingCoroutine != null)
            StopCoroutine(shootingCoroutine);

        shootingCoroutine = StartCoroutine(ShootRoutine());//inicia la rutina de disparo
    }
    
    private IEnumerator ShootRoutine()
    {
        isAiming = false;
        actionController.SetUpperBodyState(UpperBodyState.Shooting);

        //weaponarm o si no centro del personaje + altura
        Vector3 origin = weaponArm != null ? weaponArm.position : transform.position + Vector3.up * 1.2f;

        Vector3 direction = (currentAimPoint - origin).normalized;

        if (Physics.Raycast(origin, direction, out RaycastHit hit, range, shootMask))
        {
            Debug.DrawLine(origin, hit.point, Color.red, 1f);//raycast 

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

        yield return new WaitForSeconds(fireDuration);//espera el tiempo de disparo

        actionController.SetUpperBodyState(UpperBodyState.None);//vuelve al estado normal del cuerpo superior
        shootingCoroutine = null;
    }

    public Vector3 GetAimPoint()
    {
        return currentAimPoint;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;//en editor
        Gizmos.DrawSphere(currentAimPoint, 0.15f);

        if (weaponArm != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(weaponArm.position, currentAimPoint);
        }
    }
    
    
    
    
    
    
    
}
