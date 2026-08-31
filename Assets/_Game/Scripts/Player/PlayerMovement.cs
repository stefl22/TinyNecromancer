using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float acceleration = 24f;
    [SerializeField] private float deceleration = 18f;

    [Header("Rotation")]
    [SerializeField] private float rotationSpeed = 12f;

    [Header("Dodge")]
    [SerializeField] private float dodgeSpeed = 10f;
    [SerializeField] private float dodgeDuration = 0.22f;
    [SerializeField] private float dodgeCooldown = 0.65f;

    [Header("References")]
    [SerializeField] private Transform cameraTransform;

    private CharacterController controller;
    private Vector3 currentVelocity;

    private bool isDodging;
    private PlayerCombat playerCombat;
    private float nextDodgeTime;

    public bool IsDodging => isDodging;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        playerCombat = GetComponent<PlayerCombat>();

        if (cameraTransform == null && Camera.main != null)
        {
            cameraTransform = Camera.main.transform;
        }
    }

    private void Update()
    {
        if (isDodging)
            return;

        Vector2 input = ReadInput();

        if (TryStartDodge(input))
            return;

        Vector3 moveDirection =
            GetCameraRelativeDirection(input);

        Vector3 targetVelocity =
            moveDirection * moveSpeed;

        float changeRate =
            moveDirection.sqrMagnitude > 0.01f
                ? acceleration
                : deceleration;

        currentVelocity = Vector3.MoveTowards(
            currentVelocity,
            targetVelocity,
            changeRate * Time.deltaTime
        );

        controller.Move(
            currentVelocity * Time.deltaTime
        );

        RotatePlayer();
    }

    private bool TryStartDodge(Vector2 input)
    {
        if (Keyboard.current == null)
            return false;

        if (playerCombat != null &&
            playerCombat.IsAttacking)
        {
            return false;
        }

        if (!Keyboard.current.spaceKey.wasPressedThisFrame)
            return false;

        if (Time.time < nextDodgeTime)
            return false;

        Vector3 dodgeDirection =
            GetCameraRelativeDirection(input);

        // If no WASD key is pressed, dodge forward.
        if (dodgeDirection.sqrMagnitude < 0.01f)
        {
            dodgeDirection = transform.forward;
        }

        dodgeDirection.Normalize();

        StartCoroutine(
            DodgeRoutine(dodgeDirection)
        );

        return true;
    }

    private IEnumerator DodgeRoutine(
        Vector3 dodgeDirection)
    {
        isDodging = true;

        currentVelocity = Vector3.zero;

        nextDodgeTime =
            Time.time + dodgeCooldown;

        transform.rotation =
            Quaternion.LookRotation(dodgeDirection);

        float elapsedTime = 0f;

        while (elapsedTime < dodgeDuration)
        {
            controller.Move(
                dodgeDirection *
                dodgeSpeed *
                Time.deltaTime
            );

            elapsedTime += Time.deltaTime;

            yield return null;
        }

        isDodging = false;
    }

    private Vector2 ReadInput()
    {
        Vector2 input = Vector2.zero;

        if (Keyboard.current == null)
            return input;

        if (Keyboard.current.wKey.isPressed)
            input.y += 1f;

        if (Keyboard.current.sKey.isPressed)
            input.y -= 1f;

        if (Keyboard.current.dKey.isPressed)
            input.x += 1f;

        if (Keyboard.current.aKey.isPressed)
            input.x -= 1f;

        if (input.sqrMagnitude > 1f)
        {
            input.Normalize();
        }

        return input;
    }

    private Vector3 GetCameraRelativeDirection(
        Vector2 input)
    {
        if (cameraTransform == null)
        {
            return new Vector3(
                input.x,
                0f,
                input.y
            );
        }

        Vector3 cameraForward =
            cameraTransform.forward;

        Vector3 cameraRight =
            cameraTransform.right;

        cameraForward.y = 0f;
        cameraRight.y = 0f;

        cameraForward.Normalize();
        cameraRight.Normalize();

        Vector3 direction =
            cameraForward * input.y +
            cameraRight * input.x;

        if (direction.sqrMagnitude > 1f)
        {
            direction.Normalize();
        }

        return direction;
    }

    private void RotatePlayer()
    {
        Vector3 facingDirection =
            new Vector3(
                currentVelocity.x,
                0f,
                currentVelocity.z
            );

        if (facingDirection.sqrMagnitude < 0.01f)
            return;

        Quaternion targetRotation =
            Quaternion.LookRotation(
                facingDirection.normalized
            );

        transform.rotation =
            Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                rotationSpeed * Time.deltaTime
            );
    }
}