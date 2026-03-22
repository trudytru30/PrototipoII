using System.Collections.Generic;
using UnityEngine;

public class BloodSplatter : MonoBehaviour
{
    [Header("Particle System")]
    public new ParticleSystem particleSystem;
    public string decalType = "Blood"; // Set this in inspector
    
    private List<ParticleCollisionEvent> collisionEvents = new List<ParticleCollisionEvent>();
    
    void Start()
    {
        if (particleSystem == null)
            particleSystem = GetComponent<ParticleSystem>();
    }
    
    void OnParticleCollision(GameObject other)
    {
        int numCollisionEvents = particleSystem.GetCollisionEvents(other, collisionEvents);
        
        for (int i = 0; i < numCollisionEvents; i++)
        {
            // Spawn decal with specified type
            if (VFXManager.Instance != null)
            {
                VFXManager.Instance.SpawnDecal(
                    decalType,
                    collisionEvents[i].intersection,
                    collisionEvents[i].normal,
                    other.transform
                );
            }
        }
    }

    private void CleanupAndDestroy()
    {
        Invoke("DestroySelf", 0.1f);
    }
    
    void DestroySelf()
    {
        Destroy(gameObject);
    }
    
    void Update()
    {
        if (particleSystem != null && !particleSystem.IsAlive())
        {
            CleanupAndDestroy();
        }
    }
}
