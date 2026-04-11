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
        if (director == null || IsPaused)
            return;

        IsPaused = true;
        Time.timeScale = 0f;
        director.Pause();
    }

    public void ResumeTimeline()
    {
        if (director == null || !IsPaused)
            return;

        IsPaused = false;
        Time.timeScale = 1f;
        director.Resume();
    }

    private void OnDisable()
    {
        Time.timeScale = 1f;
    }
}