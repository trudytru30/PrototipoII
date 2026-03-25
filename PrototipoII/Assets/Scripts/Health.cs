using System;
using UnityEngine;
using UnityEngine.Rendering;

public class Health : MonoBehaviour
{
    
    [SerializeField] private float maxHealth;
    private float _currentHealth;

    private void Awake()
    {
        _currentHealth = maxHealth;
    }

    public void TakeDamage(float damage)
    {
        Debug.Log("Health Taken");
        _currentHealth -= damage;
        if (_currentHealth <= 0)
        {
            Die();
        }
        
        //FX
        if (VFXManager.Instance)
            VFXManager.Instance.CallGlitchFX(
            0.9f,
            0.9f,
        0.9f,
             0.9f,
        0.9f,
            true);
    }

    
    
    private void Die()
    {
        Destroy(gameObject);
    }

    public float GetMaxHealth()
    {
        return maxHealth;
    }

    public float GetCurrentHealth()
    {
        return _currentHealth;
    }
}
