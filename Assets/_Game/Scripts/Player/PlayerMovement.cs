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

    private CharacterController controller;
    private Vector3 currentVelocity;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
    }

    private void Update()
    {
        Vector2 input = ReadInput();

        Vector3 inputDirection = new Vector3(input.x, 0f, input.y);

        if (inputDirection.sqrMagnitude > 1f)
        {
            inputDirection.Normalize();
        }

        Vector3 targetVelocity = inputDirection * moveSpeed;

        float changeRate = inputDirection.sqrMagnitude > 0.01f
            ? acceleration
            : deceleration;

        currentVelocity = Vector3.MoveTowards(
            currentVelocity,
            targetVelocity,
            changeRate * Time.deltaTime
        );

        controller.Move(currentVelocity * Time.deltaTime);

        Vector3 facingDirection =
            new Vector3(currentVelocity.x, 0f, currentVelocity.z);

        if (facingDirection.sqrMagnitude > 0.01f)
        {
            Quaternion targetRotation =
                Quaternion.LookRotation(facingDirection.normalized);

            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                rotationSpeed * Time.deltaTime
            );
        }
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

        return input;
    }
}