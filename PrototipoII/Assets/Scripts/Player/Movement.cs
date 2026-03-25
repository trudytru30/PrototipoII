using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;

public class Movement : MonoBehaviour
{
    [SerializeField] private float crouchSpeed = 2f;
    [SerializeField] private float walkSpeed = 5f;
    [SerializeField] private float runSpeed = 8f;

    [Header("Visual Path")]
    [SerializeField] private bool showPath;
    [SerializeField] private float yOffset = 0.15f;
    
    private NavMeshAgent navMeshAgent;
    private LineRenderer pathLine;
    private Queue<Vector3> destinationQueue = new Queue<Vector3>();
    
    [SerializeField] private float stepSoundTimer = 0.6f;
    private float _tempStepSoundTimer;
    
    
    private void Awake()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        pathLine = GetComponent<LineRenderer>();
        
        pathLine.useWorldSpace = true;
        pathLine.positionCount = 0;
    }
    
    private void Update()
    {
        // Si el agente ha llegado al destino y hay más destinos en la cola, procesar el siguiente
        if (HasReachedDestination() && destinationQueue.Count > 0)
        {
            Vector3 nextDestination = destinationQueue.Dequeue();
            navMeshAgent.SetDestination(nextDestination);
        }
        
        if (navMeshAgent.velocity.sqrMagnitude > 0f)
        {
            PlaySteppingSounds();
        }
        UpdatePathVisual();
    }

    // Moverse al punto especificado con el raton
    public void MoveToPoint(MovementType type, Vector3 destination)
    {
        destinationQueue.Clear();
        navMeshAgent.ResetPath();
        
        navMeshAgent.isStopped = false; // Permitir movimiento

        // Moverse en base al tipo de movimiento seleccionado
        switch (type)
        {
            case MovementType.Crouch:
                navMeshAgent.speed = crouchSpeed;
                break;
            case MovementType.Walk:
                navMeshAgent.speed = walkSpeed;
                break;
            case MovementType.Run:
                navMeshAgent.speed = runSpeed;
                break;
        }
        
        navMeshAgent.SetDestination(destination);
    }

    // Comprobar si el agente ha llegado al destino
    public bool HasReachedDestination()
    {
        return !navMeshAgent.pathPending && navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance &&
               (!navMeshAgent.hasPath || navMeshAgent.velocity.sqrMagnitude == 0f);
    }
    
    // Detener al player
    public void StopMoving()
    {
        destinationQueue.Clear(); // Limpiar cola de destinos

        if (navMeshAgent.hasPath) // Si el agente tiene un camino, limpiarlo
            navMeshAgent.ResetPath();
        navMeshAgent.isStopped = true; // Bloquear movimiento
        
        Debug.Log("Movement stopped");
    }

    // ===== Getters y Setters =====
    public void SetSpeed(float speed)
    {
        navMeshAgent.speed = speed;
    }
    
    public float GetSpeed()
    {
        return navMeshAgent.speed;
    }

    public bool IsStopped()
    {
        return navMeshAgent.isStopped;
    }
    
    private void PlaySteppingSounds()
    {
        _tempStepSoundTimer -= Time.deltaTime;
        if (_tempStepSoundTimer <= 0)
        {
            Debug.Log("Stepping sound");
            AudioManager.Instance.Play("steps");
            _tempStepSoundTimer = stepSoundTimer;
        }
    }

    private void UpdatePathVisual()
    {
        if (!showPath || pathLine == null) return;

        if (navMeshAgent.pathPending || !navMeshAgent.hasPath || navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance)
        {
            pathLine.positionCount = 0;
            return;
        }

        Vector3[] corners = navMeshAgent.path.corners;
        pathLine.positionCount = corners.Length;

        for (int i = 0; i < corners.Length; i++)
        {
            Vector3 point = corners[i];
            point.y += yOffset;
            pathLine.SetPosition(i, point);
        }
    }
}