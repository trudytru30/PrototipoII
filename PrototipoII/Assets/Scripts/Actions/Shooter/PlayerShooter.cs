using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
/*
 * Logica de apuntado y disparo del PLAYER
 *
 * NO TOCAR currentAimPoint directamente desde otro script
 * En Inspector:
 * AimPlaneHeight: 0
 *      altura del plano de apuntado, por defecto a la altura del player
 *      ,se puede ajustar para apuntar a diferentes alturas
 * ShootMask(todas menos player)
 * Use Spread :false
 * SpreadAngle: 0
 */
public class PlayerShooter : MonoBehaviour
{
    //De momwnto prueba inicial con raycast, luego se implementará con proyectiles,etc..
    [Header("Referencias")]
    [SerializeField] private Camera cam;
    [SerializeField] private Transform weaponArm;
    [SerializeField] private ActionController actionController;
    [SerializeField] private WeaponRaycast weaponRaycast;
    
    [Header("Aim")]
    [SerializeField] private float rotationSpeed = 720f;
    [SerializeField] private float aimPlaneHeight = ~0;//Capa para el raycast, por defecto TODO:capa específica para el raycast
    
    [Header("Visual")]
    [SerializeField] private float fireDuration = 0.1f;
    
    private Vector3 currentAimPoint;//punto de mira actual(raton)
    private Coroutine shootingCoroutine;//controla el tiempo de disparo
    private bool isAiming;
    
    
    private void Update()
    {
        UpdateAimPoint();
        if (isAiming && actionController.GetLowerBodyState() == LowerBodyState.Idle)
        //if (isAiming) //lo ideal pero el player se mueve solo en idle
        {
            RotateTowardsAimPoint();
        }
    }
    
    private void UpdateAimPoint()
    {
        if (cam == null) return;

        Ray ray = cam.ScreenPointToRay(Mouse.current.position.ReadValue());//crea un rayo desde la camara hacia el punto del mouse
        //plano horizontal a la altura del player
        Plane groundPlane = new Plane(Vector3.up, new Vector3(0f, aimPlaneHeight, 0f));

        if (groundPlane.Raycast(ray, out float enter))
        {
            currentAimPoint = ray.GetPoint(enter);
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

    //Apunta (tren superior libre)
    public void StartAim()
    {
        if (actionController == null) return;

        if (!actionController.CanUseUpperBody()) return;

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
        if (actionController == null || weaponRaycast == null) return;

        //solo si usa el cuerpo superior o si ya esta apuntando, puede disparar
        bool canShoot = actionController.CanUseUpperBody() || actionController.GetUpperBodyState() == UpperBodyState.Aiming;

        if (!canShoot) return;

        if (shootingCoroutine != null)
        {
            StopCoroutine(shootingCoroutine);
        }

        shootingCoroutine = StartCoroutine(ShootRoutine());//inicia la rutina de disparo
    }
    
    private IEnumerator ShootRoutine()
    {
        isAiming = false;
        actionController.SetUpperBodyState(UpperBodyState.Shooting);

        Transform muzzle = weaponArm != null ? weaponArm : transform;

        ShootResult result = weaponRaycast.Fire(muzzle, currentAimPoint, transform.root);

        Debug.DrawLine(
            muzzle.position,
            result.finalPoint,
            result.hit ? Color.red : Color.yellow,
            1f
        );
        //TODO: efectos visuales
        //-sonido de disparo
        //usar hit,hitInfoy finalPoint para efectos de impacto (struct deShootResult)
       

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
        Gizmos.color = Color.yellow;//en editor
        Gizmos.DrawSphere(currentAimPoint, 0.15f);

        if (weaponArm != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(weaponArm.position, currentAimPoint);
        }
    }
    
    
    
    
    
    
    
}
