using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.VFX;
using URPGlitch;
using Random = UnityEngine.Random;

public class VFXManager : MonoBehaviour
{
    public static VFXManager Instance { get; private set; }

    [Header("VFX Library")] 
    [SerializeField] public VFXEntry[] vfxEntries;
    
    [Header("Decal Management")]
    public List<GameObject> bloodDecalPrefabs;
    public List<GameObject> bulletHoleDecalPrefabs;
    public List<GameObject> scratchDecalPrefabs;
    public int maxDecalsInScene = 75;
    public float decalLifetime = 10f;
    public LayerMask noDecalLayers;
    
    [Header("PostProcessingFX")]
    [SerializeField] Volume globalVolume;
    [SerializeField] AnalogGlitchVolume analogGlitch;
    [SerializeField] DigitalGlitchVolume digitalGlitch;
    
    [Header("Glitch")] 
    private float defaultScanLineJitter;
    private float defaultVerticalJump;
    private float defaultHorizontalShake;
    private float defaultColorDrift;
    private float defaultIntensityOfDigital;
    
    
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

    private void Start()
    {
        if (globalVolume.profile.TryGet(out DigitalGlitchVolume dgFX))
        {
            digitalGlitch = dgFX;
            defaultIntensityOfDigital = digitalGlitch.intensity.value;
        }

        if (globalVolume.profile.TryGet(out AnalogGlitchVolume aFX))
        {
            analogGlitch = aFX;
            defaultScanLineJitter = analogGlitch.scanLineJitter.value;
            defaultVerticalJump = analogGlitch.verticalJump.value;
            defaultHorizontalShake = analogGlitch.horizontalShake.value;
            defaultColorDrift = analogGlitch.colorDrift.value;
        }
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

    // PARA PONER UN VFX, SE LLAMA A LA INSTANCE DE VFXMANAGER Y SE HACE EL PLAYVFX, INDICANDO SIEMPRE
    // EL VFX ID (EL NOMBRE DEL VFXASSET DADO EN LA LIBRERÍA QUE SE QUIERE USAR)
    
    // Play VFX 
    public void PlayVFXAsset(string vfxID, Vector3 position)
    {
        VFXEntry entry = System.Array.Find(vfxEntries, e => e.id == vfxID);

        GameObject go = new GameObject();
        go.transform.position = position;
        go.transform.rotation = Quaternion.LookRotation(position);

        VisualEffect ve = go.AddComponent<VisualEffect>();
        ve.visualEffectAsset = entry.library.vfxAssets[Random.Range(0, entry.library.vfxAssets.Length)];
        ve.Play();

        Destroy(go, 2f);
    }
    
    public void PlayVFXPrefab(string vfxID, Vector3 position, Vector3 direction, Quaternion rotation)
    {
        VFXEntry entry = System.Array.Find(vfxEntries, e => e.id == vfxID);
        
        GameObject go = Instantiate(entry.library.vfxPrefab[Random.Range(0, entry.library.vfxPrefab.Length)], position, rotation);
        
        go.GetComponent<ParticleSystem>().Play();

        Destroy(go, 2f);
    }
    
    // PARA PONER UN DECAL, SE LLAMA A LA INSTANCE DE VFXMANAGER Y SE HACE EL SPAWNDECAL, INDICANDO SIEMPRE
    // EL DECALTYPE (EL NOMBRE DEL DECAL DADO EN LA LISTA QUE SE QUIERE USAR)
    // Y LOS DATOS DEL TRANSFORM (POSICION Y NORMAL)
    
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
        
        PlayDecalSFX(decalType, position);

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

    //If the decal has an sfx associated
    private void PlayDecalSFX(string decalID, Vector3 position)
    {
        if (decalID != "Blood") return;
        
        //AudioManager.Instance.PlayAtPoint("blood", position);
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
    
    //Post Processing

    public void CallGlitchFX(float scanLineJitter, float verticalJump, float horizontalShake, float colorDrift,
        float intensityOfDigital, bool isTemporary, float timeToReturn= 0.4f)
    {
        Debug.Log(digitalGlitch.intensity);
        
        digitalGlitch.intensity.Override(intensityOfDigital);
        analogGlitch.colorDrift.Override(colorDrift);
        analogGlitch.scanLineJitter.Override(scanLineJitter);
        analogGlitch.verticalJump.Override(verticalJump);
        analogGlitch.horizontalShake.Override(horizontalShake);

        Debug.Log(digitalGlitch.intensity);
        if (isTemporary)
        {
            StartCoroutine(TempGlitchFX(timeToReturn));
        }
    }

    private IEnumerator TempGlitchFX(float timeToReturn)
    {
        //Se intentará hacer que progresivamente cambie de uno a otro más adelante.
        
        yield return new WaitForSeconds(timeToReturn);
        
        digitalGlitch.intensity.Override(defaultIntensityOfDigital);
        analogGlitch.colorDrift.Override(defaultColorDrift);
        analogGlitch.scanLineJitter.Override(defaultScanLineJitter);
        analogGlitch.verticalJump.Override(defaultVerticalJump);
        analogGlitch.horizontalShake.Override(defaultHorizontalShake);
    }
    
}
