using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour, PlayerActions.IGameplayActions
{
    [SerializeField] private Camera cam;
    [SerializeField] private Movement movement;
    [SerializeField] private PlayerShooter shooter;
    
    // AÑADIDO: referencia a TimelineActions para consultar la timeline antes de ejecutar acciones
    [SerializeField] private TimelineActions timelineActions;
    [SerializeField] private float shootDuration = 0.5f; // duración fija de un disparo en segundos
    
    private ActionController actionController;
    private PlayerActions inputs;
    
    private void Awake()
    {
        inputs = new PlayerActions();
        inputs.Gameplay.SetCallbacks(this);
        actionController = GetComponent<ActionController>();
    }

    // Activar/desactivar los inputs
    private void OnEnable()
    {
       inputs.Gameplay.Enable();
    }

    private void OnDisable()
    {
        inputs.Gameplay.Disable();
    }

    // Consultar si el personaje esta en movimiento
    private void Update()
    {
        if (actionController.GetLowerBodyState() != LowerBodyState.Idle && movement.HasReachedDestination())
        {
            actionController.SetLowerBodyState(LowerBodyState.Idle);
        }
    }

    // Accion de moverse (normal)
    public void OnMove(InputAction.CallbackContext context)
    {
        if (!context.performed) return;
        if (actionController.GetFullBodyState() != FullBodyState.None) return;
        
        bool slowWalk = Keyboard.current[Key.LeftShift].isPressed;
        
        MovePlayer(slowWalk ? MovementType.Crouch : MovementType.Walk);
        
        Debug.Log("Move");
    }
    
    // Accion de moverse (corriendo)
    public void OnRun(InputAction.CallbackContext context)
    {
        if (!context.performed) return;
        if (actionController.GetFullBodyState() != FullBodyState.None) return;
        
        MovePlayer(MovementType.Run);
        
        Debug.Log("Run");
    }
    
    // Movimiento
    public void MovePlayer(MovementType type)
    {
        if (Time.timeScale <= 0f)
        {
            timelineActions?.CancelLastAction(TimelineActionType.Move);
            timelineActions?.ClearSpecificExecution(TimelineActionType.Move);
        }
        
        // Si ya está moviéndose, cancelar el movimiento actual y redirigir al nuevo destino
        if (actionController.GetLowerBodyState() != LowerBodyState.Idle)
        {
            timelineActions?.CancelLastAction(TimelineActionType.Move);
            actionController.SetLowerBodyState(LowerBodyState.Idle);
        }

        Ray ray = cam.ScreenPointToRay(Mouse.current.position.ReadValue());

        if (!Physics.Raycast(ray, out RaycastHit hit)) return;

        // MODIFICADO: distancia calculada con CalculatePathDistance en lugar de Vector3.Distance
        // Vector3.Distance devuelve distancia en línea recta, pero el NavMeshAgent
        // sigue el path real que puede ser más largo si hay obstáculos en el NavMesh.
        // CalculatePathDistance suma los corners del path sin mover al agente.
        float distance = movement.CalculatePathDistance(hit.point);
        float speed = movement.GetSpeedForType(type);

        // Si el path no es válido (destino inaccesible) se aborta sin consultar la timeline
        if (distance < 0f)
        {
            Debug.LogWarning("[Timeline] Destino inaccesible en el NavMesh.");
            return;
        }

        System.Action executeMove = () => {
            if (actionController.GetLowerBodyState() != LowerBodyState.Idle)
            {
                timelineActions?.CancelLastAction(TimelineActionType.Move);
                actionController.SetLowerBodyState(LowerBodyState.Idle);
            }

            switch (type)
            {
                case MovementType.Crouch: actionController.SetLowerBodyState(LowerBodyState.Crouching); break;
                case MovementType.Walk: actionController.SetLowerBodyState(LowerBodyState.Walking); break;
                case MovementType.Run: actionController.SetLowerBodyState(LowerBodyState.Running); break;
            }
            movement.MoveToPoint(type, hit.point);
        };
        
        if (timelineActions != null)
        {
            // consultar la timeline; si rechaza la acción se aborta el movimiento
            if (!timelineActions.TryQueueMove(distance, speed, executeMove, out string reason))
            {
                Debug.LogWarning($"[Timeline] Movimiento rechazado: {reason}");
                return;
            }
        }
            
        if (Time.timeScale <= 0f)
        {
            movement.PreviewPath(hit.point);
        }
        else
        {
            executeMove.Invoke();
        
            timelineActions?.ClearSpecificExecution(TimelineActionType.Move);
        }
    }
    
    // Cancelacion de movimiento
    public void OnCancelMove(InputAction.CallbackContext context)
    {
        if (!context.performed) return;
        // Limpiar la lista de destinos de movimiento y hacer que el player se quede quieto
        movement.StopMoving();
        actionController.SetLowerBodyState(LowerBodyState.Idle);
        // Eliminar la accion de movimiento pendiente en la timeline para que se pueda volver a mover
        timelineActions?.CancelLastAction(TimelineActionType.Move);
    }
    
    // Accion de apuntar
    public void OnAim(InputAction.CallbackContext context)
    {
        if (shooter == null) return;
        bool isPaused = Time.timeScale <= 0f;

        if (context.started)
        {
            // AÑADIDO: comprobar que el upper body está libre antes de continuar
            if (!actionController.CanUseUpperBody()) return;
            
            Vector3 aimPointAtClick = shooter.GetAimPoint();
 
            System.Action executeShoot = () => {
                actionController.SetUpperBodyState(UpperBodyState.Aiming);
                shooter.StartAim();
                actionController.SetUpperBodyState(UpperBodyState.Shooting);
                shooter.ShootAtTarget(aimPointAtClick); 
            };
            
            if (timelineActions != null)
            {
                if (!timelineActions.TryQueueShoot(shootDuration, executeShoot, out string reason)) return;
                if (!isPaused)
                {
                    executeShoot.Invoke();
                    timelineActions.ClearSpecificExecution(TimelineActionType.Shoot);
                }
            }
            else if (!isPaused)
            {
                executeShoot.Invoke();
            }
        }

        if (context.canceled && !isPaused)
        {
            // AÑADIDO: liberar el upper body al soltar el botón de apuntar
            actionController.SetUpperBodyState(UpperBodyState.None);
            
            shooter.StopAim();
            Debug.Log("Aim Cancel");
            
        }
    }
}