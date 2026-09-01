using System.Collections;
using UnityEngine;

[RequireComponent(typeof(EnemyAI))]
public class EnemyAttackFeedback : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform visual;

    [Header("Windup")]
    [SerializeField] private float windupDuration = 0.4f;
    [SerializeField] private float scaleAmount = 1.15f;

    private EnemyAI enemyAI;
    private Vector3 originalScale;

    private Coroutine feedbackRoutine;

    private void Awake()
    {
        enemyAI = GetComponent<EnemyAI>();

        if (visual != null)
        {
            originalScale = visual.localScale;
        }
    }

    private void OnEnable()
    {
        enemyAI.AttackStarted += HandleAttackStarted;
    }

    private void OnDisable()
    {
        enemyAI.AttackStarted -= HandleAttackStarted;
    }

    private void HandleAttackStarted()
    {
        if (visual == null)
            return;

        if (feedbackRoutine != null)
        {
            StopCoroutine(feedbackRoutine);
            visual.localScale = originalScale;
        }

        feedbackRoutine =
            StartCoroutine(WindupRoutine());
    }

    private IEnumerator WindupRoutine()
    {
        Vector3 targetScale =
            originalScale * scaleAmount;

        float elapsed = 0f;

        while (elapsed < windupDuration)
        {
            elapsed += Time.deltaTime;

            float progress =
                Mathf.Clamp01(
                    elapsed / windupDuration
                );

            visual.localScale =
                Vector3.Lerp(
                    originalScale,
                    targetScale,
                    progress
                );

            yield return null;
        }

        visual.localScale = originalScale;

        feedbackRoutine = null;
    }
}