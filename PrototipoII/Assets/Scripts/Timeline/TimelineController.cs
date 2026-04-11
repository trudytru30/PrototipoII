using UnityEngine;

public class TimelineController : MonoBehaviour
{
    public bool IsPaused { get; private set; }

    private void Start()
    {
        Time.timeScale = 1f;
        IsPaused = false;
    }

    public void PauseTimeline()
    {
        if (IsPaused) return;
        IsPaused = true;
        Time.timeScale = 0f;
    }

    public void ResumeTimeline()
    {
        if (!IsPaused) return;
        IsPaused = false;
        Time.timeScale = 1f;
    }

    private void OnDisable()
    {
        Time.timeScale = 1f;
    }
}
