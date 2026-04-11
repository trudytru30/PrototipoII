    using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class TimelineActions : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayableDirector director;
    [SerializeField] private TimelineTrackUI moveTrackUI;
    [SerializeField] private TimelineTrackUI shootTrackUI;

    private readonly List<TimelineActionData> registeredActions = new();

    public bool TryQueueAction(TimelineActionType type, float duration, Color color, out string failReason)
    {
        failReason = string.Empty;

        if (director == null)
        {
            failReason = "No hay PlayableDirector asignado.";
            return false;
        }

        if (duration <= 0f)
        {
            failReason = "La duración de la acción no es válida.";
            return false;
        }

        float startTime = (float)director.time;

        TimelineActionData newAction = new TimelineActionData
        {
            actionType = type,
            startTime = startTime,
            duration = duration,
            endTime = startTime + duration,
            color = color
        };

        if (HasOverlap(newAction))
        {
            failReason = "La acción no cabe en la timeline ahora mismo.";
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

            bool overlaps = candidate.startTime < current.endTime &&
                            candidate.endTime > current.startTime;

            if (overlaps)
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
                if (moveTrackUI != null)
                    moveTrackUI.CreateBlock(action, totalDuration);
                break;

            case TimelineActionType.Shoot:
                if (shootTrackUI != null)
                    shootTrackUI.CreateBlock(action, totalDuration);
                break;
        }
    }
}
