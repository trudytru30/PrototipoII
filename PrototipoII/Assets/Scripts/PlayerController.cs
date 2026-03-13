using UnityEngine;
using UnityEngine.AI;
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
    
    // Accion de moverse
    public void OnMove(InputAction.CallbackContext context)
    {
        if (!context.performed) return;
        
        Ray ray = cam.ScreenPointToRay(Mouse.current.position.ReadValue());
        
        if(Physics.Raycast(ray, out RaycastHit hit))
            movement.MoveToPoint(hit.point);
        
        Debug.Log("Move");
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