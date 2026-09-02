using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerDefense : MonoBehaviour
{
    [Header("Shield")]
    [SerializeField] private Transform leftHandPoint;

    [Header("Block Pose")]
    [SerializeField]
    private Vector3 blockLocalPosition =
        new Vector3(-0.18f, 0.90f, 0.45f);

    [SerializeField]
    private Vector3 blockLocalEulerAngles =
        new Vector3(0f, 0f, -20f);

    [SerializeField] private float poseSpeed = 12f;

    private Vector3 normalLocalPosition;
    private Quaternion normalLocalRotation;

    private PlayerMovement playerMovement;
    private PlayerCombat playerCombat;

    public bool IsBlocking { get; private set; }

    private void Awake()
    {
        playerMovement = GetComponent<PlayerMovement>();
        playerCombat = GetComponent<PlayerCombat>();

        if (leftHandPoint != null)
        {
            normalLocalPosition =
                leftHandPoint.localPosition;

            normalLocalRotation =
                leftHandPoint.localRotation;
        }
    }

    private void Update()
    {
        ReadBlockInput();
        UpdateBlockPose();
    }

    private void ReadBlockInput()
    {
        if (Mouse.current == null)
        {
            IsBlocking = false;
            return;
        }

        bool wantsToBlock =
            Mouse.current.rightButton.isPressed;

        if (playerMovement != null &&
            playerMovement.IsDodging)
        {
            wantsToBlock = false;
        }

        if (playerCombat != null &&
            playerCombat.IsAttacking)
        {
            wantsToBlock = false;
        }

        IsBlocking = wantsToBlock;
    }

    private void UpdateBlockPose()
    {
        if (leftHandPoint == null)
            return;

        Vector3 targetPosition =
            IsBlocking
                ? blockLocalPosition
                : normalLocalPosition;

        Quaternion targetRotation =
            IsBlocking
                ? Quaternion.Euler(
                    blockLocalEulerAngles
                )
                : normalLocalRotation;

        leftHandPoint.localPosition =
            Vector3.Lerp(
                leftHandPoint.localPosition,
                targetPosition,
                poseSpeed * Time.deltaTime
            );

        leftHandPoint.localRotation =
            Quaternion.Slerp(
                leftHandPoint.localRotation,
                targetRotation,
                poseSpeed * Time.deltaTime
            );
    }
}