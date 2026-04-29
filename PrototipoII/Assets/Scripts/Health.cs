 using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;

public class Health : MonoBehaviour
{

    [SerializeField] private float maxHealth = 100f;
    private float _currentHealth;

    [Header("Vignette Settings")]
    [SerializeField] private Volume globalVolume;
    [SerializeField] private float maxVignetteIntensity = 0.5f;
    [SerializeField] bool isPlayer;
    

    private Vignette _vignette;
    [SerializeField] private MeshRenderer meshRenderer;
    private EnemyController enemyController;

    

    private void Awake()
    {
        enemyController = gameObject.GetComponent<EnemyController>();
        
        if (!isPlayer) return;
        globalVolume.profile.TryGet(out _vignette);
    }

    private void Start()
    {
        _currentHealth = maxHealth;
        if (!isPlayer) return;
        _vignette.intensity.value = 0f;
        UpdateVignette();
    }


    public void TakeDamage(float damage)
    {
        _currentHealth -= damage;

        _currentHealth = Mathf.Clamp(_currentHealth, 0, maxHealth);
        if (isPlayer)
            UpdateVignette();

        if (_currentHealth <= 0)
        {
            Die();
        }

        if (VFXManager.Instance)
            VFXManager.Instance.CallGlitchFX(
                0.1f,
                0.07f,
                0.07f,
                0.1f,
                0.08f,
                true);
    }

    private void UpdateVignette()
    {
        if (_vignette == null) return;

        float missingHealthPercent = 1 - (_currentHealth / maxHealth);

        float targetIntensity = missingHealthPercent * maxVignetteIntensity;

        _vignette.intensity.value = targetIntensity;
    }

    private void Die()
    {
        if (isPlayer)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        else
        {
            // Enemy death logic
            enemyController.Stop();
            enemyController.enabled = false;
        }

        // Destroy(gameObject);
        DieVisual();
        
    }

    public float GetMaxHealth() => maxHealth;
    public float GetCurrentHealth() => _currentHealth;

    // Visual de la muerte

    private void DieVisual()
    {
        meshRenderer.material.color = Color.grey;
        // Spawnea part�culas de sangre
    }
    
    private void OnDestroy()
    {
        // Solo si es el jugador, reseteamos el valor del Asset de Post-procesado
        if (isPlayer && _vignette != null)
        {
            _vignette.intensity.value = 0f;
        }
    }
}
