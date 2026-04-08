using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
/*
 * TODO: Meterle el weaponarm(crearlo)
 * 
 * Pasarle el weaponshooter
 * aim at chest :true
 * chest height:1.2f
 *
 * en WeaponRaycast:
 *  Use spread (true)?
 *  spread angle:
 *  Shoot mask: todas menos enemy
 */
public class EnemyController : MonoBehaviour
{
    [Header("Patrol Settings")]
    [SerializeField] private List<Transform> patrolPoints;
    [SerializeField] private float stopTime = 3f;
    [SerializeField] private float speed = 3.5f;
    [SerializeField] private float maxTargetAngle = 40f;
    [SerializeField] private float rotationSpeed = 50f;
    private int currentPatrolPointIndex;
    private float waitTimer;
    

    [SerializeField] private EnemyVisionSource vision;

    [Header("Disparo")]
    [SerializeField] private EnemyShooter shooter;
    
    private NavMeshAgent agent;
    private EnemyState currentState;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    private void Start()
    {
        currentState = EnemyState.Patrol;
        currentPatrolPointIndex = 0;
        agent.speed = speed;
        
        if (patrolPoints.Count > 0)
        {
            agent.SetDestination(patrolPoints[currentPatrolPointIndex].position);
        }
    }

    // Función para rotar hacia un punto
    public void RotateEnemy(Vector3 point)
    {
        Vector3 direction = point - transform.position;
        direction.y = 0f;
        Quaternion rotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, rotation, rotationSpeed * Time.deltaTime);
    }
    
    private void Update()
    {
        // Prioridad a la visión
        if (vision && vision.GetPlayerSpotted())
        {
            currentState = EnemyState.Alert;
        } 
        else if (currentState == EnemyState.Alert)
        {
            currentState = EnemyState.Patrol;
            
            if(patrolPoints.Count > 0)
                agent.SetDestination(patrolPoints[currentPatrolPointIndex].position);
        }

        switch (currentState)
        {
            case EnemyState.Patrol:
                HandlePatrol();
                break;
            case EnemyState.Wait:
                HandleWait();
                break;
            case EnemyState.Alert:
                HandleAlert();
                break;
        }
    }

    // ===== PATRULLA =====
    private void HandlePatrol()
    {
        if (patrolPoints.Count == 0) return;
        
        // Moverse hacia el siguiente punto de patrulla después de haber rotado
        Vector3 target = agent.steeringTarget;
        
        RotateEnemy(target);
        
        if (!IsFacingtarget(target))
        {
            agent.isStopped = true;
        }
        else
        {
            agent.isStopped = false;
        }
        
        // Si ha llegado al destino
        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        {
            currentState = EnemyState.Wait;
            waitTimer = stopTime;
        }
    }
    
    // ===== ESPERA =====
    private void HandleWait()
    {
        waitTimer -= Time.deltaTime;

        if (waitTimer <= 0f)
        {
            NextPatrolPoint();
            currentState = EnemyState.Patrol;
        }
    }

    private void NextPatrolPoint()
    {
        currentPatrolPointIndex = (currentPatrolPointIndex + 1) % patrolPoints.Count;
        agent.SetDestination(patrolPoints[currentPatrolPointIndex].position);
    }

    public void Stop()
    {
        agent.isStopped = true;
    }
    
    // ===== ALERTA =====
    private void HandleAlert()
    {
        agent.isStopped = true;  // Detener movimiento

        if (!vision || !vision.GetPlayerSpotted()) return;
        
        Transform player = vision.GetPlayerTransform();
        //cambiado , no pillaba el player
        RotateEnemy(player.position);  // Rotar hacia el player para dispararle

        // TODO: Disparar al player
        if (shooter)
        {
            shooter.SetTarget(player);

            if (shooter.CanShoot())
            {
                shooter.TryShoot();
            }
        }
    }

    // Método que comprueba si el enemigo está mirando en dirección a
    // su siguiente punto de patrulla antes de empezar a moverse
    private bool IsFacingtarget(Vector3 target)
    {
        Vector3 dir = target - transform.position;
        dir.y = 0f;
        float angle = Vector3.Angle(transform.forward, dir);
        return angle < maxTargetAngle;
    }
}