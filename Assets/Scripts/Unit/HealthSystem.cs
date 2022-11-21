using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthSystem : MonoBehaviour
{


    public event EventHandler OnDeath;
    public event EventHandler OnHealthChanged;


    [SerializeField] int maxHealth = 100;
    private int health;

    private void Awake()
    {
        health = maxHealth;
    }

    public void Damage(int damageAmount)
    {
        health -= damageAmount;

        if (health < 0) health = 0;
        OnHealthChanged?.Invoke(this, EventArgs.Empty);

        if (health == 0) Die();

        
    }

    private void Die()
    {
        OnDeath?.Invoke(this, EventArgs.Empty);
    }

    public float GetHealthNormalized()
    {
        return (float)health/maxHealth;
    }
}
