using UnityEngine;

public class VisionSource : MonoBehaviour
{
    [SerializeField] private float visionRange = 12f;
    
    private void Update()
    {
        FogManager.Instance.RegisterVisionSource(this);
    }
    
    public float GetVisionRange()
    {
        return visionRange;
    }
}
