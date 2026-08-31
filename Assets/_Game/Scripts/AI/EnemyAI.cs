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
    [SerializeField] private float attackCooldown = 1f;

    private CharacterController controller;
    private Health health;
    private Health targetHealth;

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

        Vector3 direction = target.position - transform.position;
        direction.y = 0f;

        float distance = direction.magnitude;

        if (distance > detectionRange)
            return;

        if (direction.sqrMagnitude > 0.01f)
        {
            RotateTowards(direction);
        }

        if (distance > stoppingDistance)
        {
            MoveTowards(direction);
        }

        if (distance <= attackRange)
        {
            TryAttack();
        }
    }

    private void MoveTowards(Vector3 direction)
    {
        Vector3 movement =
            direction.normalized * moveSpeed * Time.deltaTime;

        controller.Move(movement);
    }

    private void RotateTowards(Vector3 direction)
    {
        Quaternion targetRotation =
            Quaternion.LookRotation(direction.normalized);

        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            targetRotation,
            rotationSpeed * Time.deltaTime
        );
    }

    private void TryAttack()
    {
        if (Time.time < nextAttackTime)
            return;

        if (targetHealth == null || targetHealth.IsDead)
            return;

        targetHealth.TakeDamage(attackDamage);

        nextAttackTime = Time.time + attackCooldown;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(
            transform.position,
            detectionRange
        );
    }
}