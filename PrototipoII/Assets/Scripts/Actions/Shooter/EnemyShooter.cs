using UnityEngine;
using System.Collections;
/*
 * Controla disparo de enemigo usando el weaponraycast
 * recibe el target(player)yrota hacia el(EnemyController)
 *
 * Enemy debe tener este script y weapinraycast ademas de EC
 *
 * Modificar fireCooldown y fireDuration para ajustar la cadencia de disparo
 */
public class EnemyShooter : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private Transform weaponArmE;
    [SerializeField] private WeaponRaycast weaponRaycastEnemy;

    [Header("Disparo")]
    [SerializeField] private float fireCooldown = 0.5f;
    [SerializeField] private float fireDuration = 0.1f;
    
    [SerializeField] private bool aimAtChest = true;
    [SerializeField] private float chestHeight = 1.2f;//*para no disparar a los pies

    private Transform currentTarget;//target actual(player)
    private bool canShoot = true;
    private Coroutine shootingCoroutine;//controla el tiempo de disparo
    
    public void SetTarget(Transform target)
    {
        currentTarget = target;//player
    }
    
    //mira si puede disparar
    public bool CanShoot()
    {
        return canShoot && currentTarget != null && weaponRaycastEnemy != null && weaponArmE != null;
    }
    
    public void TryShoot()
    {
        if (!CanShoot()) return;

        if (shootingCoroutine != null)
        {
            StopCoroutine(shootingCoroutine);//si ya esta disparando, reinicia el proceso de disparo
        }

        shootingCoroutine = StartCoroutine(ShootRoutine());//inicia la rutina de disparo
    }
    
    private IEnumerator ShootRoutine()
    {
        canShoot = false;

        //punto al que dispara (mejorar altura para no disparar a los pies)
        Vector3 targetPoint = currentTarget.position;

        if (aimAtChest)
        {
            targetPoint += Vector3.up * chestHeight;//*
        }
        targetPoint.y=weaponArmE.position.y;
        //
        ShootResult result = weaponRaycastEnemy.Fire(
            weaponArmE,
            targetPoint,
            transform.root
        );

        //dibuja la line del disparo, cyan si impacta, azul si no
        Debug.DrawRay(
            weaponArmE.position,
            result.finalDirection * weaponRaycastEnemy.GetRange(),
            Color.cyan,
            1f
        );

        //TODO: efectos visuales
        //-sonido de disparo
        //usar hit,hitInfoy finalPoint para efectos de impacto (struct deShootResult

        yield return new WaitForSeconds(fireDuration);
        yield return new WaitForSeconds(fireCooldown);

        canShoot = true;
        shootingCoroutine = null;
    }
}
