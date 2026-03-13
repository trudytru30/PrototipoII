using System.Collections;
using Unity.Mathematics;
using UnityEngine;

public class Character : MonoBehaviour
{
    [SerializeField] private MovementController movementController;
    [SerializeField] private CommandManager commandManager;
    [SerializeField] private GameObject weapon;

    private bool _canAttack = true;
    
    private void Awake()
    {
        if (movementController == null)
            movementController = GetComponent<MovementController>();
    }
    
    public void MoveCharacter(Vector3 direction)
    {
        ICommand command = new MoveCommand(this, direction);
        commandManager.Execute(command);
    }
    
    public void RotateCharacter(quaternion direction)
    {
        ICommand command = new RotationCommand(this, direction);
        commandManager.Execute(command);
    }

    public void Attack()
    {
        if(!_canAttack)
            return;
        _canAttack = false;
        StartCoroutine(AttackCooldown());
        ICommand command = new AttackCommand(weapon.GetComponentInParent<Transform>().gameObject, weapon, this);
        commandManager.Execute(command);
    }

    private IEnumerator AttackCooldown()
    {
        yield return new WaitForSeconds(1.5f);
        _canAttack = true;
    }

    public void Move(Vector3 direction)
    {
        movementController.Move(direction);
    }

    public void Rotate(quaternion direction)
    {
        movementController.Rotate(direction);
    }
}