using UnityEngine;
using UnityEngine.Playables;

public class TimelineController : MonoBehaviour
{
    [SerializeField] private PlayableDirector director;
    //de momento va a lo burro asi, probablemente se quite por el boton de UI
    private KeyCode pauseKey = KeyCode.Escape;

    public bool IsPaused { get; private set; }

    private void Start()
    {
        Time.timeScale = 1f;

        if (director != null)
            director.Play();
    }

    private void Update()
    {
        if (Input.GetKeyDown(pauseKey))
            TogglePause();
    }

    public void TogglePause()
    {
        SetPaused(!IsPaused);
    }

    public void SetPaused(bool paused)
    {
        IsPaused = paused;

        Time.timeScale = paused ? 0f : 1f;

        if (director == null) return;

        if (paused)
            director.Pause();
        else
            director.Resume();
    }
}
