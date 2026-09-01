using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(Health))]
[RequireComponent(typeof(EnemyPerception))]
public class EnemyAI : MonoBehaviour
{
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
    private EnemyPerception perception;

    private bool isAttacking;
    private float nextAttackTime;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        health = GetComponent<Health>();
        perception = GetComponent<EnemyPerception>();
    }

    private void Start()
    {
        if (perception.Target != null)
        {
            targetHealth =
                perception.Target.GetComponent<Health>();
        }
    }

    private void Update()
    {
        if (health.IsDead)
            return;

        Transform target = perception.Target;

        if (target == null)
            return;

        if (targetHealth == null)
        {
            targetHealth =
                target.GetComponent<Health>();
        }

        if (targetHealth != null &&
            targetHealth.IsDead)
        {
            return;
        }

        if (isAttacking)
            return;

        if (!perception.CanSeeTarget)
            return;

        Vector3 direction =
            target.position - transform.position;

        direction.y = 0f;

        float distance =
            direction.magnitude;

        if (direction.sqrMagnitude > 0.01f)
        {
            RotateTowards(direction);
        }

        if (distance <= attackRange &&
            Time.time >= nextAttackTime)
        {
            StartCoroutine(
                AttackRoutine()
            );

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

        yield return new WaitForSeconds(
            attackWindup
        );

        Transform target =
            perception.Target;

        if (target != null)
        {
            float distanceToTarget =
                Vector3.Distance(
                    transform.position,
                    target.position
                );

            if (perception.CanSeeTarget &&
                distanceToTarget <= attackRange &&
                targetHealth != null &&
                !targetHealth.IsDead)
            {
                targetHealth.TakeDamage(
                    attackDamage
                );

                AttackHit?.Invoke();
            }
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
                rotationSpeed *
                Time.deltaTime
            );
    }
}