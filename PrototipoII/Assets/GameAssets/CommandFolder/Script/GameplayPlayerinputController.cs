using UnityEngine;
using UnityEngine.InputSystem;

public class GameplayPlayerInputController : MonoBehaviour
{
    [SerializeField] private Character character;
    [SerializeField] private CommandManager commandManager;
    [SerializeField] private InputActionProperty moveAction;
    [SerializeField] private InputActionProperty rotateLAction;
    [SerializeField] private InputActionProperty rotateRAction;
    [SerializeField] private InputActionProperty redoAction;
    [SerializeField] private InputActionProperty undoAction;    
    [SerializeField] private InputActionProperty attackAction;
    [SerializeField] private InputActionProperty changeColorAction;

    //Inicializar acciones
    private void Awake()
    {
        moveAction.action.started += OnMove;
        rotateLAction.action.started += OnLeftRotate;
        rotateRAction.action.started += OnRightRotate;
        redoAction.action.started += OnRedo;
        undoAction.action.started += OnUndo;
        attackAction.action.started += OnAttack;
        changeColorAction.action.started += OnChangeColor;
    }

    private void OnMove(InputAction.CallbackContext context)
    {
        Vector2 input = context.ReadValue<Vector2>();
        Vector3 direction = new Vector3(input.x, 0, input.y);
        character.MoveCharacter(direction);
    }
    
    private void OnUndo(InputAction.CallbackContext context)
    {
       commandManager.UndoCommandAction();
    }

    private void OnRedo(InputAction.CallbackContext context)
    {
        commandManager.RedoCommandAction();
    }
    private void OnLeftRotate(InputAction.CallbackContext context)
    {
        Quaternion current = character.transform.rotation;
        Quaternion delta = Quaternion.Euler(0, -15, 0);
        Quaternion newRotation = current * delta;

        character.RotateCharacter(newRotation);
    }

    private void OnRightRotate(InputAction.CallbackContext context)
    {
        Quaternion current = character.transform.rotation;
        Quaternion delta = Quaternion.Euler(0, 15, 0);
        Quaternion newRotation = current * delta;

        character.RotateCharacter(newRotation);
    }
    
    private void OnAttack(InputAction.CallbackContext context)
    {
        character.Attack();
    }

    private void OnChangeColor(InputAction.CallbackContext context)
    {
        ICommand command = new ChangeColorCommand(character.gameObject);
        commandManager.Execute(command);
    }
}