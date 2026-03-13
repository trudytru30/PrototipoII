using Unity.Mathematics;

public class RotationCommand : ICommand
{
    private readonly Character _character;
    private quaternion _originRotation;
    private readonly quaternion _targetRotation;

    public RotationCommand(Character character, quaternion targetRotation)
    {
        _character = character;
        _targetRotation = targetRotation;
    }

    public void ExecuteAction()
    {
        _originRotation = _character.transform.rotation;
        _character.Rotate(_targetRotation);
    }

    public void UndoAction()
    {
        _character.transform.rotation = _originRotation;
    }
}