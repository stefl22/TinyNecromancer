using System.Collections;
using UnityEngine;

[RequireComponent(typeof(PlayerCombat))]
public class PlayerAttackFeedback : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform visual;

    [Header("Attack Motion")]
    [SerializeField] private float lungeDistance = 0.2f;
    [SerializeField] private float lungeDuration = 0.06f;
    [SerializeField] private float returnDuration = 0.09f;

    private PlayerCombat playerCombat;
    private Vector3 originalLocalPosition;
    private Coroutine attackRoutine;

    private void Awake()
    {
        playerCombat = GetComponent<PlayerCombat>();

        if (visual != null)
        {
            originalLocalPosition = visual.localPosition;
        }
    }

    private void OnEnable()
    {
        playerCombat.AttackStarted += HandleAttackStarted;
    }

    private void OnDisable()
    {
        playerCombat.AttackStarted -= HandleAttackStarted;
    }

    private void HandleAttackStarted()
    {
        if (visual == null)
            return;

        if (attackRoutine != null)
        {
            StopCoroutine(attackRoutine);
            visual.localPosition = originalLocalPosition;
        }

        attackRoutine = StartCoroutine(AttackMotion());
    }

    private IEnumerator AttackMotion()
    {
        Vector3 forwardPosition =
            originalLocalPosition +
            Vector3.forward * lungeDistance;

        float elapsed = 0f;

        while (elapsed < lungeDuration)
        {
            elapsed += Time.deltaTime;

            float progress =
                Mathf.Clamp01(
                    elapsed / lungeDuration
                );

            visual.localPosition =
                Vector3.Lerp(
                    originalLocalPosition,
                    forwardPosition,
                    progress
                );

            yield return null;
        }

        elapsed = 0f;

        while (elapsed < returnDuration)
        {
            elapsed += Time.deltaTime;

            float progress =
                Mathf.Clamp01(
                    elapsed / returnDuration
                );

            visual.localPosition =
                Vector3.Lerp(
                    forwardPosition,
                    originalLocalPosition,
                    progress
                );

            yield return null;
        }

        visual.localPosition =
            originalLocalPosition;

        attackRoutine = null;
    }
}