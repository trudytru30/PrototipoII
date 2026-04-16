using System;
using UnityEngine;
using UnityEngine.UI;

public class TimelineController : MonoBehaviour
{
      public bool IsPaused { get; private set; }
    [SerializeField] GameObject pausePanel;
    
    [Header("Timer Settings")]
    [SerializeField] Image timerBar;
    [SerializeField] float maxPauseTime = 8f;
    
    [SerializeField] float drainRate = 1f; 
    [SerializeField] float regenRate = 0.5f; 
    
    private float currentPauseTime; 

    private void Start()
    {
        Time.timeScale = 1f;
        IsPaused = false;
        pausePanel.SetActive(false);
        
        currentPauseTime = maxPauseTime;
        UpdateTimer();
    }

    private void Update()
    {
        
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (IsPaused)
            {
                if (AudioManager.Instance)
                {
                    AudioManager.Instance.Play("hud_button");
                }
                ResumeTimeline();
            }
            else
            {
                if (AudioManager.Instance)
                {
                    AudioManager.Instance.Play("hud_button");
                }
                PauseTimeline();
            }

        }
        
        
        if (IsPaused)
        {
            currentPauseTime -= drainRate * Time.unscaledDeltaTime;
            if (currentPauseTime <= 0)
            {
                currentPauseTime = 0;
                ResumeTimeline(); 
            }
            
            UpdateTimer();
        }
        //si no esta lleno ni pausado
        else if (currentPauseTime < maxPauseTime)
        {
            currentPauseTime += regenRate * Time.deltaTime;
            
            if (currentPauseTime > maxPauseTime)
                currentPauseTime = maxPauseTime;
            
            UpdateTimer();
        }
    }

    public void PauseTimeline()
    {
        if (IsPaused) return;
        
        if (currentPauseTime <= 0)
        {
            Debug.Log("No hay tiempo de pausa disponible");
            return;
        }

        if (AudioManager.Instance)
        {
            AudioManager.Instance.Play("pause");
        }
        
        IsPaused = true;
        Time.timeScale = 0f;
        pausePanel.SetActive(true);
    }

    public void ResumeTimeline()
    {
        if (!IsPaused) return;
        
        if (AudioManager.Instance)
        {
            AudioManager.Instance.Play("resume");
        }
        
        IsPaused = false;
        Time.timeScale = 1f;
        pausePanel.SetActive(false);
    }
    
    private void UpdateTimer()
    {
        if (timerBar != null)
        {
            float fillPercentage = currentPauseTime / maxPauseTime;
            timerBar.fillAmount = fillPercentage;
        }
    }

    private void OnDisable()
    {
        Time.timeScale = 1f;
    }
}
