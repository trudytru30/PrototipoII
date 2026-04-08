using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour, PlayerActions.IGameplayActions
{
    [SerializeField] private Camera cam;
    [SerializeField] private Movement movement;
    
    [SerializeField] private PlayerShooter shooter;
    
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
        if (actionController.GetLowerBodyState() != LowerBodyState.Idle && movement.IsStopped())
        {
            actionController.SetLowerBodyState(LowerBodyState.Idle);
        }
    }

    // Accion de moverse (normal)
    public void OnMove(InputAction.CallbackContext context)
    {
        if (!context.performed) return;
        if (!actionController.CanUseLowerBody()) return;
        
        bool slowWalk = Keyboard.current[Key.LeftShift].isPressed;
        
        MovePlayer(slowWalk ? MovementType.Crouch : MovementType.Walk);
        
        Debug.Log("Move");
    }
    
    // Accion de moverse (corriendo)
    public void OnRun(InputAction.CallbackContext context)
    {
        if (!context.performed) return;
        if (!actionController.CanUseLowerBody()) return;
        
        MovePlayer(MovementType.Run);
        
        Debug.Log("Run");
    }
    
    // Movimiento
    public void MovePlayer(MovementType type)
    {
        Ray ray = cam.ScreenPointToRay(Mouse.current.position.ReadValue());

        if (!Physics.Raycast(ray, out RaycastHit hit)) return;
        
        switch (type)
        {
            case MovementType.Crouch:
                actionController.SetLowerBodyState(LowerBodyState.Crouching);
                break;
            case MovementType.Walk:
                actionController.SetLowerBodyState(LowerBodyState.Walking);
                break;
            case MovementType.Run:
                actionController.SetLowerBodyState(LowerBodyState.Running);
                break;
        }
            
        movement.MoveToPoint(type, hit.point);
    }
    
    // Cancelacion de movimiento
    public void OnCancelMove(InputAction.CallbackContext context)
    {
        if (!context.performed) return;
        // Limpiar la lista de destinos de movimiento y hacer que el player se quede quieto
        movement.StopMoving();
        actionController.SetLowerBodyState(LowerBodyState.Idle);
    }
    
    // Accion de apuntar
    public void OnAim(InputAction.CallbackContext context)
    {
        if (shooter == null) return;

        if (context.started)
        {
            shooter.StartAim();
            Debug.Log("Aim");
        }

        if (context.performed)
        {
            shooter.Shoot();
        }

        if (context.canceled)
        {
            shooter.StopAim();
            Debug.Log("Aim Cancel");
            
        }
    }
}