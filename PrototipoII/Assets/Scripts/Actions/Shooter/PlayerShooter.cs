using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using UnityEngine.Rendering;

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
    
    [Header("FX")]
    [SerializeField] private string sfxID;
    [SerializeField] private float fireDuration = 0.1f;
    [SerializeField] private AmmoVisual ammoVis;

    
    [SerializeField] private TrailRenderer BulletTrail;
    public float BulletSpeed;
    private Vector3 currentAimPoint;//punto de mira actual(raton)
    private Coroutine shootingCoroutine;//controla el tiempo de disparo
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

        isAiming = true;
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

        //solo si ya está apuntando o en estado de disparo puede disparar
        bool canShoot = actionController.GetUpperBodyState() == UpperBodyState.Aiming
                     || actionController.GetUpperBodyState() == UpperBodyState.Shooting;

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

        Vector3 aimHeightCorrection = currentAimPoint;
        aimHeightCorrection.y = muzzle.position.y;
        
        ammoVis.RemoveAmmo(); // Removes piece from gun
        ShootResult result = weaponRaycast.Fire(muzzle, aimHeightCorrection, transform.root);

       /* Debug.DrawLine(
            muzzle.position,
            result.finalPoint,
            result.hit ? Color.red : Color.yellow,
            1f
        );
        */
        Debug.DrawRay(muzzle.position, result.finalDirection * weaponRaycast.GetRange(), Color.green, 1f);
        //TODO: efectos visuales
        //-sonido de disparo
        //usar hit,hitInfoy finalPoint para efectos de impacto (struct deShootResult)
        if (AudioManager.Instance)
            AudioManager.Instance.PlayAtPoint(sfxID, muzzle.position);
        
        //Estructura provisional para que se escuche "bien"

        if (VFXManager.Instance)
            VFXManager.Instance.CallGlitchFX(
              0.1f,
               0.07f,
            0.07f,
                 0.1f,
            0.08f,
               true);
        
        
        if (result.hit)
        {
            Debug.Log("Result:" + result.hitInfo.collider.gameObject.name);
            if (result.hitInfo.collider.name == "EnemyModel") //Mira si el gameobject tiene el EnemyModel
            {
                //audio
                if (AudioManager.Instance)
                    AudioManager.Instance.Play("hitBlood");
                
                //Vfx
                var direction = this.transform.position - result.hitInfo.point;
            
                var rotation = Quaternion.LookRotation(-direction, Vector3.up);
                
                if (VFXManager.Instance)
                    VFXManager.Instance.PlayVFXPrefab("blood", result.hitInfo.point, direction, rotation);
            }
            else
            {
                if (AudioManager.Instance)
                    AudioManager.Instance.Play("hitWall");
            }
        }
        
        

        //cfx
        
        var directionCfx = this.transform.position - result.finalPoint;

        Debug.Log(directionCfx);
        
        if (CFXManager.Instance)
        {
            CFXManager.GenerateImpulse(new Vector3(-2 * directionCfx.normalized.x,
                -2 * directionCfx.normalized.z, 
                1));
        }
        
        TrailRenderer trail = Instantiate(BulletTrail, muzzle.position, Quaternion.identity);
        StartCoroutine(SpawnTrail(trail, result.finalPoint));

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
    
    private IEnumerator SpawnTrail(TrailRenderer Trail, Vector3 HitPoint)
    {
        Vector3 startPosition = Trail.transform.position;
        float distance = Vector3.Distance(startPosition, HitPoint);
        float remainingDistance = distance;

        while (remainingDistance > 0)
        {
            Trail.transform.position = Vector3.Lerp(startPosition, HitPoint, 1 - (remainingDistance / distance));

            remainingDistance -= BulletSpeed * Time.deltaTime;

            yield return null;
        }
        Destroy(Trail.gameObject, Trail.time);
    }
}