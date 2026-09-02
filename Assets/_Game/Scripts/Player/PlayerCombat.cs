using System;
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
    [SerializeField] private float attackRadius = 1.2f;
    [SerializeField] private float attackCooldown = 0.45f;
    [SerializeField] private float attackWindup = 0.1f;

    public event Action AttackStarted;
    public event Action AttackHit;

    private bool isAttacking;
    public bool IsAttacking => isAttacking;

    private PlayerMovement playerMovement;

    private void Awake()
    {
        playerMovement = GetComponent<PlayerMovement>();
    }

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
        // Don't allow attacks while dodging.
        if (playerMovement != null &&
            playerMovement.IsDodging)
        {
            return;
        }

        if (isAttacking)
            return;

        if (attackPoint == null)
        {
            Debug.LogWarning(
                "PlayerCombat has no AttackPoint assigned."
            );

            return;
        }

        StartCoroutine(AttackRoutine());
    }

    private IEnumerator AttackRoutine()
    {
        isAttacking = true;

        AttackStarted?.Invoke();

        yield return new WaitForSeconds(attackWindup);

        ApplyAttackDamage();

        AttackHit?.Invoke();

        float remainingCooldown =
            Mathf.Max(
                attackCooldown - attackWindup,
                0f
            );

        yield return new WaitForSeconds(
            remainingCooldown
        );

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

            Vector3 attackDirection =
    targetHealth.transform.position -
    transform.position;

            attackDirection.y = 0f;

            DamageInfo damageInfo =
                new DamageInfo(
                    attackDamage,
                    DamageCategory.Physical,
                    DamageType.Slash,
                    gameObject,
                    hitCollider.ClosestPoint(
                        attackPoint.position
                    ),
                    attackDirection,
                    true,
                    true
                );

            targetHealth.TakeDamage(
                damageInfo
            );
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