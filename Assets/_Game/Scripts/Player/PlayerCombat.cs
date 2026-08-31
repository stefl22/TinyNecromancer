using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCombat : MonoBehaviour
{
    [Header("Basic Attack")]
    [SerializeField] private float attackDamage = 25f;
    [SerializeField] private float attackRange = 1.25f;
    [SerializeField] private float attackForwardOffset = 1.25f;
    [SerializeField] private float attackCooldown = 0.4f;

    private float nextAttackTime;

    private void Update()
    {
        if (Mouse.current == null)
            return;

        if (Mouse.current.leftButton.wasPressedThisFrame &&
            Time.time >= nextAttackTime)
        {
            PerformAttack();

            nextAttackTime = Time.time + attackCooldown;
        }
    }

    private void PerformAttack()
    {
        Vector3 attackCenter =
            transform.position +
            transform.forward * attackForwardOffset;

        Collider[] hitColliders =
            Physics.OverlapSphere(attackCenter, attackRange);

        HashSet<Health> damagedTargets = new HashSet<Health>();

        foreach (Collider hitCollider in hitColliders)
        {
            Health targetHealth =
                hitCollider.GetComponentInParent<Health>();

            if (targetHealth == null)
                continue;

            if (targetHealth.gameObject == gameObject)
                continue;

            if (!damagedTargets.Add(targetHealth))
                continue;

            targetHealth.TakeDamage(attackDamage);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Vector3 attackCenter =
            transform.position +
            transform.forward * attackForwardOffset;

        Gizmos.DrawWireSphere(
            attackCenter,
            attackRange
        );
    }
}