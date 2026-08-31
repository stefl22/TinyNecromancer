using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCombat : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform attackPoint;

    [Header("Basic Attack")]
    [SerializeField] private float attackDamage = 25f;
    [SerializeField] private float attackRadius = 1f;
    [SerializeField] private float attackCooldown = 0.45f;
    [SerializeField] private float attackWindup = 0.1f;

    private bool isAttacking;

    private void Update()
    {
        if (Mouse.current == null)
            return;

        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            TryAttack();
        }
    }

    private void TryAttack()
    {
        if (isAttacking)
            return;

        if (attackPoint == null)
        {
            Debug.LogWarning("PlayerCombat has no AttackPoint assigned.");
            return;
        }

        StartCoroutine(AttackRoutine());
    }

    private IEnumerator AttackRoutine()
    {
        isAttacking = true;

        // Small delay before the hit connects.
        // Later this will synchronize with the attack animation.
        yield return new WaitForSeconds(attackWindup);

        ApplyAttackDamage();

        float remainingCooldown =
            Mathf.Max(attackCooldown - attackWindup, 0f);

        yield return new WaitForSeconds(remainingCooldown);

        isAttacking = false;
    }

    private void ApplyAttackDamage()
    {
        Collider[] hitColliders =
            Physics.OverlapSphere(
                attackPoint.position,
                attackRadius
            );

        HashSet<Health> damagedTargets =
            new HashSet<Health>();

        foreach (Collider hitCollider in hitColliders)
        {
            Health targetHealth =
                hitCollider.GetComponentInParent<Health>();

            if (targetHealth == null)
                continue;

            if (targetHealth.gameObject == gameObject)
                continue;

            if (targetHealth.IsDead)
                continue;

            if (!damagedTargets.Add(targetHealth))
                continue;

            targetHealth.TakeDamage(attackDamage);
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null)
            return;

        Gizmos.DrawWireSphere(
            attackPoint.position,
            attackRadius
        );
    }
}