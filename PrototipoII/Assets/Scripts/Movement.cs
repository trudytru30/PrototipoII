using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;

public class Movement : MonoBehaviour
{
    [SerializeField] private float walkSpeed = 2f;
    [SerializeField] private float normalSpeed = 5f;
    [SerializeField] private float runSpeed = 8f;

    private NavMeshAgent navMeshAgent;
    private Queue<Vector3> destinationQueue = new Queue<Vector3>();
    
    private void Awake()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
    }

    private void Update()
    {
        // Si el agente ha llegado al destino y hay más destinos en la cola, procesar el siguiente
        if (HasReachedDestination() && destinationQueue.Count > 0)
        {
            Vector3 nextDestination = destinationQueue.Dequeue();
            navMeshAgent.SetDestination(nextDestination);
        }
    }

    private bool HasReachedDestination()
    {
        return !navMeshAgent.pathPending && navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance &&
               (!navMeshAgent.hasPath || navMeshAgent.velocity.sqrMagnitude == 0f);
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
            case MovementType.Walk:
                navMeshAgent.speed = walkSpeed;
                break;
            case MovementType.Normal:
                navMeshAgent.speed = normalSpeed;
                break;
            case MovementType.Run:
                navMeshAgent.speed = runSpeed;
                break;
        }
        
        navMeshAgent.SetDestination(destination);
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
}