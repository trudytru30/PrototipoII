using System;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

public class Movement : MonoBehaviour
{
    [SerializeField] private float speed = 10;
    [SerializeField] private InputAction moveAction;
    
    private NavMeshAgent navMeshAgent;
    private Vector3 destinyPosition;

    private bool isMoving = false;

    private void Awake()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        navMeshAgent.speed = speed; // Velocidad máxima del agente NavMesh
    }

    // Activar y desactivar el input de movimiento
    private void OnEnable()
    {
        moveAction.Enable();
        moveAction.performed += OnMove();
    }
    
    private void OnDisable()
    {
        moveAction.performed -= OnMove();
        moveAction.Disable();
    }
    
    // Guardar el punto como punto al que moverse y moverse hacia él
    private Action<InputAction.CallbackContext> OnMove()
    {
        MoveToPoint();
        return null;    // TODO: Comprobar que hay que poner aqui
    }
    
    private void MoveToPoint()
    {
        navMeshAgent.SetDestination(destinyPosition);
        isMoving = true;
    }

    // ===== Getters y setters =====
    public bool IsMoving()
    {
        return isMoving;
    }
    
    public Vector3 GetDestinyPosition()
    {
        return destinyPosition;
    }
    
    public void SetDestinyPosition(Vector3 position)
    {
        destinyPosition = position;
    }
}