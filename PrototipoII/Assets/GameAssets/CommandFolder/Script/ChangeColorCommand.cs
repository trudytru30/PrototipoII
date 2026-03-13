using UnityEngine;

public class ChangeColorCommand : ICommand
{
    private readonly GameObject _player;
    private readonly Color _oldColor;
    private readonly Color _newColor;

    public ChangeColorCommand(GameObject player)
    {
        this._player = player;
        _oldColor = player.GetComponent<Renderer>().material.color;
        _newColor = new Color(Random.value, Random.value, Random.value);
    }
    
    public void ExecuteAction()
    {
        _player.GetComponent<Renderer>().material.color = _newColor;
    }

    public void UndoAction()
    {
        _player.GetComponent<Renderer>().material.color = _oldColor;
    }
}