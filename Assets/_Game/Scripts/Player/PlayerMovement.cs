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

    [Header("References")]
    [SerializeField] private Transform cameraTransform;

    private CharacterController controller;
    private Vector3 currentVelocity;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();

        if (cameraTransform == null && Camera.main != null)
        {
            cameraTransform = Camera.main.transform;
        }
    }

    private void Update()
    {
        Vector2 input = ReadInput();

        Vector3 moveDirection = GetCameraRelativeDirection(input);

        Vector3 targetVelocity = moveDirection * moveSpeed;

        float changeRate = moveDirection.sqrMagnitude > 0.01f
            ? acceleration
            : deceleration;

        currentVelocity = Vector3.MoveTowards(
            currentVelocity,
            targetVelocity,
            changeRate * Time.deltaTime
        );

        controller.Move(currentVelocity * Time.deltaTime);

        RotatePlayer();
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

    private Vector3 GetCameraRelativeDirection(Vector2 input)
    {
        if (cameraTransform == null)
        {
            return new Vector3(input.x, 0f, input.y);
        }

        Vector3 cameraForward = cameraTransform.forward;
        Vector3 cameraRight = cameraTransform.right;

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
            new Vector3(currentVelocity.x, 0f, currentVelocity.z);

        if (facingDirection.sqrMagnitude < 0.01f)
            return;

        Quaternion targetRotation =
            Quaternion.LookRotation(facingDirection.normalized);

        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            targetRotation,
            rotationSpeed * Time.deltaTime
        );
    }
}