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

    [Header("Post Attack")]
    [SerializeField] private float recoilDistance = 0.6f;
    [SerializeField] private float recoilSpeed = 4f;
    [SerializeField] private float recoveryTime = 0.2f;

    [Header("Investigation")]
    [SerializeField] private float memoryDuration = 6f;
    [SerializeField] private float investigateStopDistance = 0.4f;
    [SerializeField] private float searchDuration = 2f;
    [SerializeField] private float searchTurnSpeed = 180f;

    public event Action AttackStarted;
    public event Action AttackHit;

    private CharacterController controller;
    private Health health;
    private Health targetHealth;
    private EnemyPerception perception;

    private bool isAttacking;
    private bool isSearching;

    private float nextAttackTime;
    private float searchEndTime;

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

        if (perception.CanSeeTarget)
        {
            isSearching = false;

            HandleVisibleTarget(target);

            return;
        }

        HandleLostTarget();
    }

    private void HandleVisibleTarget(Transform target)
    {
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

    private void HandleLostTarget()
    {
        if (!perception.HasTargetMemory)
            return;

        if (perception.TimeSinceLastPerceived >
            memoryDuration)
        {
            perception.ForgetTarget();
            isSearching = false;
            return;
        }

        Vector3 direction =
            perception.LastKnownPosition -
            transform.position;

        direction.y = 0f;

        float distance =
            direction.magnitude;

        if (!isSearching)
        {
            if (distance > investigateStopDistance)
            {
                RotateTowards(direction);
                MoveTowards(direction);

                return;
            }

            isSearching = true;

            searchEndTime =
                Time.time + searchDuration;
        }

        SearchForTarget();
    }

    private void SearchForTarget()
    {
        if (!isSearching)
            return;

        if (Time.time >= searchEndTime)
        {
            isSearching = false;
            perception.ForgetTarget();

            return;
        }

        transform.Rotate(
            0f,
            searchTurnSpeed * Time.deltaTime,
            0f
        );
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

        bool attackLanded = false;

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

                attackLanded = true;
            }
        }

        if (attackLanded)
        {
            yield return StartCoroutine(
                PostAttackRecoil()
            );
        }

        nextAttackTime =
            Time.time + attackCooldown;

        isAttacking = false;
    }

    private IEnumerator PostAttackRecoil()
    {
        Vector3 recoilDirection =
            -transform.forward;

        recoilDirection.y = 0f;

        if (recoilDirection.sqrMagnitude < 0.001f)
            yield break;

        recoilDirection.Normalize();

        float traveledDistance = 0f;

        while (traveledDistance < recoilDistance)
        {
            if (health.IsDead)
                yield break;

            if (controller == null ||
                !controller.enabled ||
                !controller.gameObject.activeInHierarchy)
            {
                yield break;
            }

            float step =
                recoilSpeed *
                Time.deltaTime;

            float remainingDistance =
                recoilDistance -
                traveledDistance;

            step =
                Mathf.Min(
                    step,
                    remainingDistance
                );

            controller.Move(
                recoilDirection *
                step
            );

            traveledDistance += step;

            yield return null;
        }

        if (health.IsDead)
            yield break;

        yield return new WaitForSeconds(
            recoveryTime
        );
    }

    private void MoveTowards(Vector3 direction)
    {
        if (direction.sqrMagnitude < 0.001f)
            return;

        controller.Move(
            direction.normalized *
            moveSpeed *
            Time.deltaTime
        );
    }

    private void RotateTowards(Vector3 direction)
    {
        if (direction.sqrMagnitude < 0.001f)
            return;

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