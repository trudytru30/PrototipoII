using UnityEngine;
using UnityEngine.VFX;

[System.Serializable]
public class VFXLibrary
{
    public VisualEffectAsset[] vfxAssets;
    public GameObject[] vfxPrefab;
}

[System.Serializable]
public class VFXType
{
    public string id;
    public Color debugColor = Color.white;
}

[System.Serializable]
public class VFXEntry
{
    public VFXType typeConfig;
    [SerializeField] VisualEffect _vfx;
    [SerializeField] VFXLibrary _library;

    public string id => typeConfig != null ? typeConfig.id : "NULL_ID"; 
    public VisualEffect source => _vfx;
    public VFXLibrary library => _library;
    
    
}