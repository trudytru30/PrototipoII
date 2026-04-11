using UnityEngine;
using UnityEngine.AI;
using TMPro;

public class PlayerController2 : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private TimelineActions timelineActions;
    [SerializeField] private Transform shootOrigin;
    [SerializeField] private TextMeshProUGUI messageText;

    [Header("Timeline Colors")]
    [SerializeField] private Color moveColor = Color.green;
    [SerializeField] private Color shootColor = Color.red;

    [Header("Shoot Settings")]
    [SerializeField] private float projectileSpeed = 30f;
    [SerializeField] private float maxShootDistance = 100f;

    private Camera mainCamera;

    private void Awake()
    {
        mainCamera = Camera.main;
    }

    private void Update()
    {
        HandleMovementInput();
        HandleShootInput();
    }

    private void HandleMovementInput()
    {
        if (!Input.GetMouseButtonDown(1))
            return;

        if (mainCamera == null)
            return;

        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

        if (!Physics.Raycast(ray, out RaycastHit hit))
            return;

        TryMoveTo(hit.point);
    }

    private void HandleShootInput()
    {
        if (!Input.GetMouseButtonDown(0))
            return;

        Vector3 direction = transform.forward;

        if (mainCamera != null)
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            direction = ray.direction.normalized;
        }

        TryShoot(direction);
    }

    public void TryMoveTo(Vector3 destination)
    {
        if (agent == null)
        {
            ShowMessage("No hay NavMeshAgent asignado.");
            return;
        }

        if (timelineActions == null)
        {
            ShowMessage("No hay TimelineActions asignado.");
            return;
        }

        NavMeshPath path = new NavMeshPath();
        bool hasPath = agent.CalculatePath(destination, path);

        if (!hasPath || path.corners == null || path.corners.Length < 2)
        {
            ShowMessage("No hay ruta válida.");
            return;
        }

        float pathLength = CalculatePathLength(path);
        float duration = pathLength / Mathf.Max(agent.speed, 0.01f);

        bool accepted = timelineActions.TryQueueAction(
            TimelineActionType.Move,
            duration,
            moveColor,
            out string failReason
        );

        if (!accepted)
        {
            ShowMessage(failReason);
            return;
        }

        ClearMessage();
        agent.SetDestination(destination);
    }

    public void TryShoot(Vector3 direction)
    {
        if (timelineActions == null)
        {
            ShowMessage("No hay TimelineActions asignado.");
            return;
        }

        if (shootOrigin == null)
        {
            ShowMessage("No hay punto de disparo asignado.");
            return;
        }

        float distance = maxShootDistance;

        if (Physics.Raycast(shootOrigin.position, direction, out RaycastHit hit, maxShootDistance))
        {
            distance = hit.distance;
        }

        float duration = distance / Mathf.Max(projectileSpeed, 0.01f);

        bool accepted = timelineActions.TryQueueAction(
            TimelineActionType.Shoot,
            duration,
            shootColor,
            out string failReason
        );

        if (!accepted)
        {
            ShowMessage(failReason);
            return;
        }

        ClearMessage();
        ExecuteShoot(direction);
    }

    private void ExecuteShoot(Vector3 direction)
    {
        Debug.DrawRay(shootOrigin.position, direction.normalized * maxShootDistance, Color.red, 1f);
        Debug.Log("Disparo ejecutado.");
    }

    private float CalculatePathLength(NavMeshPath path)
    {
        float totalLength = 0f;

        for (int i = 1; i < path.corners.Length; i++)
        {
            totalLength += Vector3.Distance(path.corners[i - 1], path.corners[i]);
        }

        return totalLength;
    }

    private void ShowMessage(string message)
    {
        if (messageText != null)
            messageText.text = message;

        Debug.Log(message);
    }

    private void ClearMessage()
    {
        if (messageText != null)
            messageText.text = string.Empty;
    }
}