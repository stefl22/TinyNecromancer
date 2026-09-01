using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(Health))]
public class EnemyAI : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private Transform target;

    [Header("Detection")]
    [SerializeField] private float detectionRange = 8f;

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 2.5f;
    [SerializeField] private float stoppingDistance = 1.5f;
    [SerializeField] private float rotationSpeed = 10f;

    [Header("Attack")]
    [SerializeField] private float attackRange = 1.8f;
    [SerializeField] private float attackDamage = 10f;
    [SerializeField] private float attackWindup = 0.4f;
    [SerializeField] private float attackCooldown = 1f;

    public event Action AttackStarted;
    public event Action AttackHit;

    private CharacterController controller;
    private Health health;
    private Health targetHealth;

    private bool isAttacking;
    private float nextAttackTime;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        health = GetComponent<Health>();
    }

    private void Start()
    {
        if (target != null)
        {
            targetHealth = target.GetComponent<Health>();
        }
    }

    private void Update()
    {
        if (health.IsDead || target == null)
            return;

        if (targetHealth != null && targetHealth.IsDead)
            return;

        Vector3 direction =
            target.position - transform.position;

        direction.y = 0f;

        float distance = direction.magnitude;

        if (distance > detectionRange)
            return;

        if (direction.sqrMagnitude > 0.01f)
        {
            RotateTowards(direction);
        }

        // While preparing an attack,
        // the enemy stops moving.
        if (isAttacking)
            return;

        if (distance <= attackRange &&
            Time.time >= nextAttackTime)
        {
            StartCoroutine(AttackRoutine());
            return;
        }

        if (distance > stoppingDistance)
        {
            MoveTowards(direction);
        }
    }

    private IEnumerator AttackRoutine()
    {
        isAttacking = true;

        AttackStarted?.Invoke();

        // Gives the player time to react.
        yield return new WaitForSeconds(
            attackWindup
        );

        float distanceToTarget =
            Vector3.Distance(
                transform.position,
                target.position
            );

        // Only hit if the player is still close enough.
        if (distanceToTarget <= attackRange &&
            targetHealth != null &&
            !targetHealth.IsDead)
        {
            targetHealth.TakeDamage(
                attackDamage
            );

            AttackHit?.Invoke();
        }

        nextAttackTime =
            Time.time + attackCooldown;

        isAttacking = false;
    }

    private void MoveTowards(Vector3 direction)
    {
        Vector3 movement =
            direction.normalized *
            moveSpeed *
            Time.deltaTime;

        controller.Move(movement);
    }

    private void RotateTowards(Vector3 direction)
    {
        Quaternion targetRotation =
            Quaternion.LookRotation(
                direction.normalized
            );

        transform.rotation =
            Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                rotationSpeed * Time.deltaTime
            );
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(
            transform.position,
            detectionRange
        );
    }
}