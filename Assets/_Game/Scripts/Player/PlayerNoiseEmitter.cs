using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerNoiseEmitter : MonoBehaviour
{
    [Header("Movement Noise")]
    [SerializeField] private float movementNoiseRadius = 3f;
    [SerializeField] private float movementNoiseInterval = 0.45f;

    private CharacterController controller;
    private float nextNoiseTime;

    private void Awake()
    {
        controller =
            GetComponent<CharacterController>();
    }

    private void Update()
    {
        Vector3 horizontalVelocity =
            new Vector3(
                controller.velocity.x,
                0f,
                controller.velocity.z
            );

        if (horizontalVelocity.sqrMagnitude < 0.1f)
            return;

        if (Time.time < nextNoiseTime)
            return;

        NoiseSystem.EmitNoise(
            transform.position,
            movementNoiseRadius,
            gameObject
        );

        nextNoiseTime =
            Time.time +
            movementNoiseInterval;
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(
            transform.position,
            movementNoiseRadius
        );
    }
}