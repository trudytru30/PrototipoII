using System.Collections.Generic;
using UnityEngine;

public class CommandManager : MonoBehaviour
{
    private Stack<ICommand> _undoStack = new Stack<ICommand>();
    private Stack<ICommand> _redoStack = new Stack<ICommand>();

    public void Execute(ICommand command)
    {
        command.ExecuteAction();
        _undoStack.Push(command);
        _redoStack.Clear();
    }

    public void UndoCommandAction()
    {
        if (_undoStack.Count > 0)
        {
            ICommand currentCommand = _undoStack.Pop();
            currentCommand.UndoAction();
            _redoStack.Push(currentCommand);
        }
    }

    public void RedoCommandAction()
    {
        if (_redoStack.Count > 0)
        {
            ICommand currentCommand = _redoStack.Pop();
            currentCommand.ExecuteAction();
            _undoStack.Push(currentCommand);
        }
    }
}