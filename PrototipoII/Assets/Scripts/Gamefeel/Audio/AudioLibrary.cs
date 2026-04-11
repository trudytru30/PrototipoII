using UnityEngine;

[System.Serializable]
public class AudioLibrary
{
    public AudioClip[] clips;
    [Range(0f, 1f)] public float volume = 1f;
    [Range(0.1f, 3f)] public float pitch = 1f;
    public bool hasRandomPitch = false;
    public bool hasRandomVolume = false;
    public Vector2 randomPitchRange = new Vector2(0.95f, 1.05f);
    public Vector2 randomVolumeRange = new Vector2(0.8f, 1f);
}

[System.Serializable]
public class SoundType
{
    public string id;
    public Color debugColor = Color.white;
}

[System.Serializable]
public class AudioEntry
{
    public SoundType typeConfig;
    [SerializeField] AudioSource _source;
    [SerializeField] AudioLibrary _library;

    public string id => typeConfig != null ? typeConfig.id : "NULL_ID"; 
    public AudioSource source => _source;
    public AudioLibrary library => _library;
}
