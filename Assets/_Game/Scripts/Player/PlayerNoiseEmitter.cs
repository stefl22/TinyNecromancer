using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerNoiseEmitter : MonoBehaviour
{
    [Header("Movement Noise")]
    [SerializeField] private float movementNoiseRadius = 4.5f;
    [SerializeField] private float movementNoiseInterval = 0.45f;

    [Header("Action Noise")]
    [SerializeField] private float dodgeNoiseRadius = 6f;
    [SerializeField] private float attackNoiseRadius = 7f;

    private CharacterController controller;
    private PlayerMovement playerMovement;
    private PlayerCombat playerCombat;

    private float nextMovementNoiseTime;
    private bool wasDodging;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        playerMovement = GetComponent<PlayerMovement>();
        playerCombat = GetComponent<PlayerCombat>();
    }

    private void OnEnable()
    {
        if (playerCombat != null)
        {
            playerCombat.AttackStarted += HandleAttackStarted;
        }
    }

    private void OnDisable()
    {
        if (playerCombat != null)
        {
            playerCombat.AttackStarted -= HandleAttackStarted;
        }
    }

    private void Update()
    {
        HandleDodgeNoise();

        if (playerMovement != null &&
            playerMovement.IsDodging)
        {
            return;
        }

        HandleMovementNoise();
    }

    private void HandleMovementNoise()
    {
        Vector3 horizontalVelocity =
            new Vector3(
                controller.velocity.x,
                0f,
                controller.velocity.z
            );

        if (horizontalVelocity.sqrMagnitude < 0.1f)
            return;

        if (Time.time < nextMovementNoiseTime)
            return;

        EmitNoise(movementNoiseRadius);

        nextMovementNoiseTime =
            Time.time + movementNoiseInterval;
    }

    private void HandleDodgeNoise()
    {
        if (playerMovement == null)
            return;

        bool isDodging =
            playerMovement.IsDodging;

        if (isDodging && !wasDodging)
        {
            EmitNoise(dodgeNoiseRadius);
        }

        wasDodging = isDodging;
    }

    private void HandleAttackStarted()
    {
        EmitNoise(attackNoiseRadius);
    }

    private void EmitNoise(float radius)
    {
        NoiseSystem.EmitNoise(
            transform.position,
            radius,
            gameObject
        );
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(
            transform.position,
            movementNoiseRadius
        );
    }
}