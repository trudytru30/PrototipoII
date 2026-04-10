using UnityEngine;
using UnityEngine.Playables;

public class TimelineController : MonoBehaviour
{
    [SerializeField] private PlayableDirector director;

    public bool IsPaused { get; private set; }

    private void Start()
    {
        Time.timeScale = 1f;
        IsPaused = false;

        if (director != null)
            director.Play();
        else
            Debug.LogWarning("TimelineController: no hay PlayableDirector asignado.");
    }

    public void PauseTimeline()
    {
        if (director == null || IsPaused) return;

        IsPaused = true;
        // MODIFICADO: eliminado Time.timeScale = 0f
        // En tiempo real congelar timeScale bloquea NavMesh, animaciones y físicas
        director.Pause();
    }

    public void ResumeTimeline()
    {
        if (director == null || !IsPaused) return;

        IsPaused = false;
        // MODIFICADO: eliminado Time.timeScale = 1f por el mismo motivo
        director.Resume();
    }

    // MODIFICADO: OnDisable mantiene Time.timeScale = 1f como seguridad
    // por si quedara en 0 de alguna versión anterior
    private void OnDisable()
    {
        Time.timeScale = 1f;
    }
}