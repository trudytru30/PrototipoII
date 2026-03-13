using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;

public class Movement : MonoBehaviour
{
    [SerializeField] private float speed = 10;
    
    private NavMeshAgent navMeshAgent;
    private Queue<Vector3> destinationQueue = new Queue<Vector3>();

    private void Awake()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        navMeshAgent.speed = speed; // Velocidad máxima del agente NavMesh
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
    public void MoveToPoint(Vector3 destination)
    {
        // Si es el primer punto, ir directamente al destino
        if (!navMeshAgent.hasPath && !navMeshAgent.pathPending)
        {
            navMeshAgent.SetDestination(destination);
        }
        // Si ya se está moviendo, poner en cola el siguiente destino
        else
        {
            destinationQueue.Enqueue(destination);
        }
    }

    // ===== Getters y setters =====
    public bool GetIsDestinationQueue()
    {
        return navMeshAgent.hasPath || navMeshAgent.pathPending || destinationQueue.Count > 0;
    }
}