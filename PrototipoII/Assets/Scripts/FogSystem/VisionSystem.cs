using UnityEngine;

public class VisionSystem : MonoBehaviour
{
    [SerializeField] private float visionRange = 20f;
    [SerializeField] private int rayCount = 40; // Numero de raycast alrededor del player
    [SerializeField] private LayerMask obstacles;

    private float timer;
    
    // Comprobar cada 0.15 segundos la vision
    private void Update()
    {
        timer += Time.deltaTime;
        
        if (timer >= 0.15f)
        {
            timer = 0f;
            UpdateVision();
        }
    }
    
    private void UpdateVision()
    {
        FogManager.Instance.ResetVisibility();
        
        Vector3 origin = transform.position;

        for (int i = 0; i < rayCount; i++)
        {
            float angle = (360f / rayCount) * i;
            
            Vector3 dir = new Vector3(Mathf.Sin(angle * Mathf.Deg2Rad),
                0f, Mathf.Cos(angle * Mathf.Deg2Rad));

            RaycastHit hit;

            if (Physics.Raycast(origin, dir, out hit, visionRange, obstacles))
            {
                FogManager.Instance.RevealLine(origin, hit.point);
                Debug.DrawLine(origin, hit.point, Color.red, 0.1f);
            }
            else
            {
                Vector3 end = origin + dir * visionRange;
                FogManager.Instance.RevealLine(origin, end);
                Debug.DrawLine(origin, end, Color.green, 0.1f);
            }
        }
    }
}