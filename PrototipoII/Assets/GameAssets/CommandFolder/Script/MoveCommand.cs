using UnityEngine;

public class MoveCommand : ICommand
{
    private readonly Character _character;
    private readonly Vector3 _target;
    private Vector3 _origin;

    public MoveCommand(Character character, Vector3 direction)
    {
        _character = character;
        _target = direction;
    }

    public void ExecuteAction()
    {
        _origin = _character.transform.position;
        _character.Move(_target);
    }

    public void UndoAction()
    {
        _character.transform.position = _origin;
    }
}