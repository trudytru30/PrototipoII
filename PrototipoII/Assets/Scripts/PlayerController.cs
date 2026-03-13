using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour, PlayerActions.IGameplayActions
{
    [SerializeField] private Camera cam;
    [SerializeField] private Movement movement;
    
    
    
    private PlayerActions inputs;
    private void Awake()
    {
        inputs = new PlayerActions();
        inputs.Gameplay.SetCallbacks(this);
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
    
    // Accion de moverse (normal)
    public void OnMove(InputAction.CallbackContext context)
    {
        if (!context.performed) return;
        
        Ray ray = cam.ScreenPointToRay(Mouse.current.position.ReadValue());

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            bool slowWalk = Keyboard.current[Key.LeftShift].isPressed;
            movement.MoveToPoint(slowWalk ? MovementType.Walk : MovementType.Normal, hit.point);
        }
        
        Debug.Log("Move");
    }
    
    // Accion de moverse (corriendo)
    public void OnRun(InputAction.CallbackContext context)
    {
        if (!context.performed) return;
        
        Ray ray = cam.ScreenPointToRay(Mouse.current.position.ReadValue());
        
        if(Physics.Raycast(ray, out RaycastHit hit))
            movement.MoveToPoint(MovementType.Run, hit.point);
        
        Debug.Log("Run");
    }
    
    // Cancelacion de movimiento
    public void OnCancelMove(InputAction.CallbackContext context)
    {
        // Limpiar la lista de destinos de movimiento y hacer que el player se quede quieto
        movement.StopMoving();
    }
    
    // Accion de apuntar
    public void OnAim(InputAction.CallbackContext context)
    {
        if(context.started) Debug.Log("Aim");
        if(context.canceled) Debug.Log("Aim Cancel");
    }
}