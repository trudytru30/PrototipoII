using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class TimelineActions : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayableDirector director;
    [SerializeField] private TimelineTrackUI moveTrackUI;
    [SerializeField] private TimelineTrackUI shootTrackUI;

    [Header("Colors")]
    // AÑADIDO: colores configurables desde el Inspector en lugar de hardcodeados
    [SerializeField] private Color moveColor = Color.blue;
    [SerializeField] private Color shootColor = Color.red;

    private readonly List<TimelineActionData> registeredActions = new();

    // AÑADIDO: punto de entrada público para movimiento
    // Recibe distancia y velocidad y calcula la duración internamente
    // Antes TryQueueAction recibía duración ya calculada y era llamado directamente desde fuera
    public bool TryQueueMove(float distance, float speed, out string failReason)
    {
        if (speed <= 0f)
        {
            failReason = "Velocidad inválida.";
            return false;
        }

        float duration = distance / speed;
        return TryQueueAction(TimelineActionType.Move, duration, moveColor, out failReason);
    }

    // AÑADIDO: punto de entrada público para disparo
    // Recibe duración fija y delega en TryQueueAction
    public bool TryQueueShoot(float duration, out string failReason)
    {
        return TryQueueAction(TimelineActionType.Shoot, duration, shootColor, out failReason);
    }

    private bool TryQueueAction(TimelineActionType type, float duration, Color color, out string failReason)
    {
        failReason = string.Empty;

        if (director == null)
        {
            failReason = "No hay PlayableDirector asignado.";
            return false;
        }

        if (duration <= 0f)
        {
            failReason = "Duración inválida.";
            return false;
        }

        float startTime = (float)director.time;

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
        return true;
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
        float totalDuration = (float)director.duration;

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
}