using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerDefense : MonoBehaviour, IDamageInterceptor
{
    [Header("Shield")]
    [SerializeField] private Transform leftHandPoint;

    [Header("Block")]
    [SerializeField] private float blockAngle = 120f;

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

    public event Action<DamageInfo> Blocked;

    private void Awake()
    {
        playerMovement =
            GetComponent<PlayerMovement>();

        playerCombat =
            GetComponent<PlayerCombat>();

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

    public bool TryInterceptDamage(
        DamageInfo damageInfo)
    {
        if (!IsBlocking)
            return false;

        if (!damageInfo.CanBeBlocked)
            return false;

        if (damageInfo.Category !=
            DamageCategory.Physical)
        {
            return false;
        }

        Vector3 attackDirection =
            damageInfo.AttackDirection;

        attackDirection.y = 0f;

        if (attackDirection.sqrMagnitude < 0.001f)
            return false;

        attackDirection.Normalize();

        Vector3 directionToAttacker =
            -attackDirection;

        float angle =
            Vector3.Angle(
                transform.forward,
                directionToAttacker
            );

        float halfBlockAngle =
            blockAngle * 0.5f;

        if (angle > halfBlockAngle)
            return false;

        Blocked?.Invoke(damageInfo);

        return true;
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