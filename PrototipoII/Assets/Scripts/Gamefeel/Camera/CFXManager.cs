using System;
using Unity.Cinemachine;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;


public class CFXManager : MonoBehaviour
{
    static CinemachineImpulseSource impulseSource;
    
    public static CFXManager Instance { get; private set; }
    
    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        
        Instance = this;
    }
    
    void Start()
    {
        transform.SetParent(null);
        impulseSource = GetComponent<CinemachineImpulseSource>();
    }

    public static void GenerateImpulse(Vector3 impulse)
    {
        impulseSource.GenerateImpulseWithVelocity(impulse);
    }
}
