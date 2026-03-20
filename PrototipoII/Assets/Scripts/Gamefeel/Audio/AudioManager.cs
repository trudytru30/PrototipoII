using UnityEngine;
using UnityEditor;
using System.Collections;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [SerializeField] AudioEntry[] _audioEntries;
    //[SerializeField][Range(0, 1f)] float defaultVolume;
    //[SerializeField][Range(-2, 2f)] private float defaultPitch;

    private void Awake() // SINGLETON
    {
        transform.parent = null;

        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

    }

    // PARA PONER UN SONIDO, SE LLAMA A LA INSTANCE DE AUDIOMANAGER Y SE HACE UNO DE SUS PLAYS, INDICANDO SIEMPRE
    // EL SOUND ID (EL NOMBRE DEL SONIDO DADO EN LA LIBRERÍA QUE SE QUIERE USAR)
    public void Play(string soundID)
    {
        AudioEntry entry = System.Array.Find(_audioEntries, e => e.id == soundID);
        
        if(entry == null)
        {
            //Debug.LogWarning($"Audio key '{soundID}' no encontrado", this);
            return;
        }

        if(entry.library.clips.Length == 0)
        {
            //Debug.LogError($"La librería de audio para '{soundID}' está vacía", this);
            return;
        }

        AudioClip clip = entry.library.clips[Random.Range(0, entry.library.clips.Length)];
        if (entry.library.hasRandomPitch)
        {
            entry.source.pitch = entry.library.pitch *
                     Random.Range(
                         entry.library.randomPitchRange.x,
                         entry.library.randomPitchRange.y
                     );
        }
        else
            entry.source.pitch = entry.library.pitch;
        if (entry.library.hasRandomVolume)
        {
            entry.source.volume = entry.library.volume *
                  Random.Range(
                      entry.library.randomVolumeRange.x,
                      entry.library.randomVolumeRange.y
                  );
        }
        else
            entry.source.volume = entry.library.volume;
        entry.source.PlayOneShot(clip);
    }

    // public void PlayAtSpecificAudioSource(string soundID, AudioSource source)
    // {
    //     AudioEntry entry = System.Array.Find(_audioEntries, e => e.id == soundID);
    //     
    //     if(entry.library.clips.Length == 0)
    //     {
    //         //Debug.LogError($"La librería de audio para '{soundID}' está vacía", this);
    //         return;
    //     }
    //
    //     AudioClip clip = entry.library.clips[Random.Range(0, entry.library.clips.Length)];
    //     if (entry.library.hasRandomPitch)
    //     {
    //         source.pitch = entry.library.pitch *
    //                              Random.Range(
    //                                  entry.library.randomPitchRange.x,
    //                                  entry.library.randomPitchRange.y
    //                              );
    //     }
    //     else
    //         source.pitch = entry.library.pitch;
    //     if (entry.library.hasRandomVolume)
    //     {
    //         source.volume = entry.library.volume *
    //                               Random.Range(
    //                                   entry.library.randomVolumeRange.x,
    //                                   entry.library.randomVolumeRange.y
    //                               );
    //     }
    //     else
    //         source.volume = entry.library.volume;
    //     
    //     source.PlayOneShot(clip);
    // }
    
    public void PlayAtPoint(string soundID, Vector3 position)
    {
        AudioEntry entry = System.Array.Find(_audioEntries, e => e.id == soundID);
        
        if(entry.library.clips.Length == 0)
        {
            //Debug.LogError($"La librería de audio para '{soundID}' está vacía", this);
            return;
        }

        AudioClip clip = entry.library.clips[Random.Range(0, entry.library.clips.Length)];
        float volume;
        
        if (entry.library.hasRandomVolume)
        {
            volume = entry.library.volume *
                            Random.Range(
                                entry.library.randomVolumeRange.x,
                                entry.library.randomVolumeRange.y
                            );
        }
        else
            volume = entry.library.volume;
        
        AudioSource.PlayClipAtPoint(clip, position, volume);
    }

    internal void Stop(float fadeOutDuration, string soundID)
    {
        AudioEntry entry = System.Array.Find(_audioEntries, e => e.id == soundID);

        if (entry == null)
        {
            Debug.LogWarning($"Audio key '{soundID}' no encontrado", this);
            return;
        }

        if (entry.library.clips.Length == 0)
        {
            Debug.LogError($"La librería de audio para '{soundID}' está vacía", this);
            return;
        }

        StartCoroutine(Stop(fadeOutDuration, entry));
    }

    IEnumerator Stop(float duration, AudioEntry entry)
    {
        if (entry.source == null) yield break;

        float startVolume = entry.source.volume;

        while (entry.source.volume > 0f)
        {
            entry.source.volume -= startVolume * Time.deltaTime / duration;
            yield return null;
        }
        entry.source.Stop();
        entry.source.volume = entry.library.volume;
        entry.source.clip = null;
    }
}




#if UNITY_EDITOR

[CustomEditor(typeof(AudioManager))]
public class AudioManagerEditor : Editor
{
    private SerializedProperty _audioEntriesProp;

    private void OnEnable()
    {
        _audioEntriesProp = serializedObject.FindProperty("_audioEntries");
    }
}
#endif
