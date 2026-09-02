using System;
using System.Collections.Generic;
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

    private readonly List<IDamageInterceptor>
        damageInterceptors =
            new List<IDamageInterceptor>();

    private void Awake()
    {
        currentHealth = maxHealth;

        MonoBehaviour[] behaviours =
            GetComponents<MonoBehaviour>();

        foreach (MonoBehaviour behaviour in behaviours)
        {
            if (behaviour is IDamageInterceptor interceptor)
            {
                damageInterceptors.Add(interceptor);
            }
        }
    }

    public void TakeDamage(float damage)
    {
        DamageInfo damageInfo =
            new DamageInfo(
                damage,
                DamageCategory.Physical,
                DamageType.None,
                null,
                transform.position,
                Vector3.zero
            );

        TakeDamage(damageInfo);
    }

    public void TakeDamage(DamageInfo damageInfo)
    {
        if (IsDead ||
            damageInfo.Amount <= 0f)
        {
            return;
        }

        if (IsInvulnerable)
            return;

        foreach (
            IDamageInterceptor interceptor
            in damageInterceptors)
        {
            if (interceptor.TryInterceptDamage(
                damageInfo))
            {
                return;
            }
        }

        float previousHealth =
            currentHealth;

        currentHealth =
            Mathf.Max(
                currentHealth -
                damageInfo.Amount,
                0f
            );

        float actualDamage =
            previousHealth -
            currentHealth;

        if (actualDamage > 0f)
        {
            Damaged?.Invoke(
                actualDamage
            );
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
        if (IsDead ||
            amount <= 0f)
        {
            return;
        }

        currentHealth =
            Mathf.Min(
                currentHealth + amount,
                maxHealth
            );

        HealthChanged?.Invoke(
            currentHealth,
            maxHealth
        );
    }

    public void SetInvulnerable(
        bool invulnerable)
    {
        IsInvulnerable =
            invulnerable;
    }

    private void Die()
    {
        IsInvulnerable = false;

        Died?.Invoke();
    }
}