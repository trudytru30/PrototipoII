using System;
using System.Collections.Generic;
using UnityEngine;

public class TimelineActions : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private TimelineClock clock;
    [SerializeField] private TimelineTrackUI moveTrackUI;
    [SerializeField] private TimelineTrackUI shootTrackUI;

    [Header("Colors")]
    [SerializeField] private Color moveColor = Color.blue;
    [SerializeField] private Color shootColor = Color.red;
    
    private Dictionary<TimelineActionType, Action> pendingExecutions = new();

    private readonly List<TimelineActionData> registeredActions = new();
    
    public bool TryQueueMove(float distance, float speed,Action onExecute , out string failReason)
    {
        if (speed <= 0f)
        {
            failReason = "Velocidad inválida.";
            return false;
        }

        float duration = distance / speed;
        return TryQueueAction(TimelineActionType.Move, duration, moveColor,onExecute , out failReason);
    }

    public bool TryQueueShoot(float duration,Action onExecute , out string failReason)
    {
        return TryQueueAction(TimelineActionType.Shoot, duration, shootColor,onExecute , out failReason);
    }

    private bool TryQueueAction(TimelineActionType type, float duration, Color color,Action onExecute ,out string failReason)
    {
        failReason = string.Empty;

        if (clock == null)
        {
            failReason = "No hay TimelineClock asignado.";
            return false;
        }

        if (duration <= 0f)
        {
            failReason = "Duración inválida.";
            return false;
        }

        float startTime = clock.CurrentTime;

        TimelineActionData newAction = new TimelineActionData
        {
            actionType = type,
            startTime  = startTime,
            duration   = duration,
            endTime    = startTime + duration,
            color      = color
        };

        if (HasOverlap(newAction))
        {
            failReason = $"Ya hay una acción de tipo {type} en ese tramo.";
            return false;
        }

        registeredActions.Add(newAction);
        CreateVisualBlock(newAction);
        
        if (onExecute != null)
        {
            pendingExecutions[type] = onExecute; 
        }
        
        return true;
    }
    
    private void Update()
    {
        if (Time.timeScale > 0f && pendingExecutions.Count > 0)
        {
            List<Action> toExecute = new List<Action>(pendingExecutions.Values);
            pendingExecutions.Clear();
            foreach (var execution in toExecute)
            {
                execution?.Invoke();
            }
        }
    }

    public void CancelLastAction(TimelineActionType type)
    {
        for (int i = registeredActions.Count - 1; i >= 0; i--)
        {
            if (registeredActions[i].actionType == type)
            {
                registeredActions.RemoveAt(i);
                return;
            }
        }
    }

    private bool HasOverlap(TimelineActionData candidate)
    {
        foreach (TimelineActionData current in registeredActions)
        {
            if (current.actionType != candidate.actionType)
                continue;

            if (candidate.startTime < current.endTime && candidate.endTime > current.startTime)
                return true;
        }

        return false;
    }

    private void CreateVisualBlock(TimelineActionData action)
    {
        float totalDuration = clock.TotalDuration;

        switch (action.actionType)
        {
            case TimelineActionType.Move:
                moveTrackUI?.CreateBlock(action, totalDuration);
                break;

            case TimelineActionType.Shoot:
                shootTrackUI?.CreateBlock(action, totalDuration);
                break;
        }
    }
    
    public void ClearSpecificExecution(TimelineActionType type)
    {
        if (pendingExecutions.ContainsKey(type))
        {
            pendingExecutions.Remove(type);
        }
    }
}
