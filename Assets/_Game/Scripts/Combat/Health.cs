using System;
using UnityEngine;

public class Health : MonoBehaviour
{
    [Header("Health")]
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private float currentHealth;

    public float MaxHealth => maxHealth;
    public float CurrentHealth => currentHealth;
    public bool IsDead => currentHealth <= 0f;
    public bool IsInvulnerable { get; private set; }

    public event Action<float, float> HealthChanged;
    public event Action<float> Damaged;
    public event Action Died;

    private void Awake()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(float damage)
    {
        if (IsDead || damage <= 0f)
            return;

        if (IsInvulnerable)
            return;

        float previousHealth = currentHealth;

        currentHealth = Mathf.Max(
            currentHealth - damage,
            0f
        );

        float actualDamage =
            previousHealth - currentHealth;

        if (actualDamage > 0f)
        {
            Damaged?.Invoke(actualDamage);
        }

        HealthChanged?.Invoke(
            currentHealth,
            maxHealth
        );

        if (currentHealth <= 0f)
        {
            Die();
        }
    }

    public void Heal(float amount)
    {
        if (IsDead || amount <= 0f)
            return;

        currentHealth = Mathf.Min(
            currentHealth + amount,
            maxHealth
        );

        HealthChanged?.Invoke(
            currentHealth,
            maxHealth
        );
    }

    public void SetInvulnerable(bool invulnerable)
    {
        IsInvulnerable = invulnerable;
    }

    private void Die()
    {
        IsInvulnerable = false;
        Died?.Invoke();
    }
}