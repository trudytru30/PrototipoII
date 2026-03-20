using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using Random = UnityEngine.Random;

public class VFXManager : MonoBehaviour
{
    public static VFXManager Instance { get; private set; }

    [Header("Decal Management")]
    public List<GameObject> bloodDecalPrefabs;
    public List<GameObject> bulletHoleDecalPrefabs;
    public List<GameObject> scratchDecalPrefabs;
    public int maxDecalsInScene = 75;
    public float decalLifetime = 10f;

    [Header("Decal Prevention")]
    public LayerMask noDecalLayers; // Set this in inspector to include "Bouncer" layer

    private  Queue<GameObject> _activeDecals = new Queue<GameObject>();
    private  Dictionary<string, List<GameObject>> _decalPools = new Dictionary<string, List<GameObject>>();

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        
        Instance = this;
        DontDestroyOnLoad(gameObject);
        
        InitializeDecalPools();
    }

    void InitializeDecalPools()
    {
        _decalPools["Blood"] = bloodDecalPrefabs;
        _decalPools["BulletHole"] = bulletHoleDecalPrefabs;
        _decalPools["Scratch"] = scratchDecalPrefabs;
    }

    void Update()
    {
        CleanupOldDecals();
    }

    // Main decal spawning method with prevention logic
    public void SpawnDecal(string decalType, Vector3 position, Vector3 normal, Transform parent = null)
    {
        // Check if parent is in a no-decal layer
        if (parent != null && IsObjectInNoDecalLayer(parent))
        {
            return; // Don't spawn decal on forbidden objects
        }

        if (!_decalPools.ContainsKey(decalType) || _decalPools[decalType] == null || _decalPools[decalType].Count == 0)
        {
            Debug.LogWarning($"No decal prefabs found for type: {decalType}");
            return;
        }

        // Check if we need to remove old decals before spawning new one
        if (_activeDecals.Count >= maxDecalsInScene)
        {
            RemoveOldestDecal();
        }

        // Get random prefab from the specified type
        List<GameObject> availablePrefabs = _decalPools[decalType];
        GameObject selectedPrefab = availablePrefabs[Random.Range(0, availablePrefabs.Count)];

        if (selectedPrefab == null)
        {
            Debug.LogWarning($"Null prefab found in {decalType} decal list!");
            return;
        }

        // Calculate rotation
        Quaternion rotation = CalculateDecalRotation(normal);

        // Instantiate decal
        GameObject decal = Instantiate(selectedPrefab, position, rotation);
        //
        // Vector3 scale = decal.transform.localScale;
        // decal.transform.parent = null;
        // decal.transform.localScale = scale;
        decal.transform.parent = parent;
        
        PlaySFX(decalType, position);

        // Random rotation variation
        float randomAngle = Random.Range(0f, 360f);
        decal.transform.rotation = Quaternion.AngleAxis(randomAngle, normal) * decal.transform.rotation;

        // Random scale variation
        float randomScale = Random.Range(0.8f, 1.2f);

        decal.GetComponent<DecalProjector>().size *= randomScale;

        // Register the decal
        _activeDecals.Enqueue(decal);

        // Set up automatic destruction after lifetime
        Destroy(decal, decalLifetime);
    }

    // Check if an object is in a no-decal layer
    private bool IsObjectInNoDecalLayer(Transform obj)
    {
        return ((1 << obj.gameObject.layer) & noDecalLayers) != 0;
    }

    private Quaternion CalculateDecalRotation(Vector3 normal)
    {
        Vector3 projectedUp = Vector3.up;
        
        if (Mathf.Abs(Vector3.Dot(normal.normalized, Vector3.up)) > 0.99f)
        {
            projectedUp = Vector3.forward;
        }
        
        Vector3 right = Vector3.Cross(projectedUp, normal).normalized;
        if (right.magnitude < 0.1f)
        {
            right = Vector3.Cross(Vector3.forward, normal).normalized;
        }
        
        Vector3 up = Vector3.Cross(normal, right).normalized;
        return Quaternion.LookRotation(-normal, up);
    }

    private void RemoveOldestDecal()
    {
        if (_activeDecals.Count > 0)
        {
            GameObject oldestDecal = _activeDecals.Dequeue();
            if (oldestDecal != null)
            {
                Destroy(oldestDecal, decalLifetime/3);
            }
        }
    }

    private void CleanupOldDecals()
    {
        while (_activeDecals.Count > 0 && _activeDecals.Peek() == null)
        {
            _activeDecals.Dequeue();
        }

        while (_activeDecals.Count > maxDecalsInScene)
        {
            RemoveOldestDecal();
        }
    }

    private void PlaySFX(string decalID, Vector3 position)
    {
        //Por ahora solo hay sfx de sangre.
        
        if (decalID != "Blood") return;
        
        AudioManager.Instance.PlayAtPoint("blood", position);
    }

    // Add new decal type at runtime
    public void RegisterDecalType(string decalType, List<GameObject> prefabs)
    {
        if (_decalPools.ContainsKey(decalType))
        {
            _decalPools[decalType].AddRange(prefabs);
        }
        else
        {
            _decalPools[decalType] = new List<GameObject>(prefabs);
        }
    }
    
    public void ForceCleanupAllDecals()
    {
        foreach (GameObject decal in _activeDecals)
        {
            if (decal != null)
            {
                Destroy(decal);
            }
        }
        _activeDecals.Clear();
    }

    public int GetActiveDecalCount()
    {
        return _activeDecals.Count;
    }

    public void SetMaxDecals(int newMax)
    {
        maxDecalsInScene = Mathf.Max(1, newMax);
        while (_activeDecals.Count > maxDecalsInScene)
        {
            RemoveOldestDecal();
        }
    }
    
}
