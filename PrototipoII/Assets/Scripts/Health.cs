 using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
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
        
        _currentHealth = maxHealth;
        if (!isPlayer) return;
        if (globalVolume.profile.TryGet(out _vignette))
            _vignette.intensity.value = 0f;
    }


    public void TakeDamage(float damage)
    {
        Debug.Log("Health Taken");
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
                0.9f,
                0.9f,
                0.9f,
                0.9f,
                0.9f,
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
            // Player death logic
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
}
