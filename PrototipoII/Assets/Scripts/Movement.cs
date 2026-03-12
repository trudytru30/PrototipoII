using UnityEngine;
using UnityEngine.AI;

public class Movement : MonoBehaviour
{
    [SerializeField] private float speed = 10;
    
    private NavMeshAgent navMeshAgent;
    private bool isMoving = false;

    private void Awake()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        navMeshAgent.speed = speed; // Velocidad máxima del agente NavMesh
    }
    
    public void MoveToPoint(Vector3 destination)
    {
        navMeshAgent.SetDestination(destination);
        isMoving = true;
    }

    // ===== Getters y setters =====
    public bool GetIsMoving()
    {
        return isMoving;
    }
}