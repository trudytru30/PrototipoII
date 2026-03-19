using UnityEngine;

public class EnemyVisionSource : MonoBehaviour
{
    [SerializeField] private float visionRange = 12f;
    [SerializeField] private float visionAngle = 60f;
    [SerializeField] private Transform player;
    [SerializeField] private LayerMask obstacleMask;
    
    private bool playerSpotted;
    private bool lastState;

    private void Update()
    {
        playerSpotted = false;
        if(!player) return;
        
        Vector3 origin = transform.position + Vector3.up * 1.5f;
        
        Vector3 direction = player.position - origin;
        float distance = direction.magnitude;
        
        // Comprobar distancia
        if(distance > visionRange) return;
        direction.Normalize();
        
        // Comprobar ángulo
        float angle = Vector3.Angle(transform.forward, direction);

        if (angle < visionAngle / 2f)
        {
            if (!Physics.Raycast(transform.position, direction, distance, obstacleMask))
            {
                playerSpotted = true;
                Debug.DrawRay(transform.position, direction * distance, Color.green);
            } else
            {
                Debug.DrawRay(transform.position, direction * distance, Color.red);
            }
        }

        if (playerSpotted && !lastState)
        {
            Debug.Log("Player spotted");
        }
        lastState = playerSpotted;
    }
    
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = playerSpotted ? Color.red : Color.yellow;

        int steps = 30;
        float halfAngle = visionAngle / 2f;

        Vector3 origin = transform.position;
        Vector3 prevPoint = origin;

        for (int i = 0; i <= steps; i++)
        {
            float t = i / (float)steps;
            float angle = Mathf.Lerp(-halfAngle, halfAngle, t);

            Vector3 dir = Quaternion.Euler(0, angle, 0) * transform.forward;
            Vector3 point = origin + dir * visionRange;

            Gizmos.DrawLine(origin, point);

            if (i > 0)
                Gizmos.DrawLine(prevPoint, point);

            prevPoint = point;
        }
    }
    
    public bool GetPlayerSpotted()
    {
        return playerSpotted;
    }
    
    public Transform GetPlayerTransform()
    {
        return player;
    }
}