using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Health))]
public class HitReaction : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform visual;

    [Header("Hit Reaction")]
    [SerializeField] private float recoilDistance = 0.18f;
    [SerializeField] private float recoilDuration = 0.06f;
    [SerializeField] private float returnDuration = 0.08f;

    private Health health;
    private Vector3 originalLocalPosition;
    private Coroutine reactionRoutine;

    private void Awake()
    {
        health = GetComponent<Health>();

        if (visual != null)
        {
            originalLocalPosition = visual.localPosition;
        }
    }

    private void OnEnable()
    {
        health.Damaged += HandleDamage;
    }

    private void OnDisable()
    {
        health.Damaged -= HandleDamage;
    }

    private void HandleDamage(float damage)
    {
        if (health.IsDead || visual == null)
            return;

        if (reactionRoutine != null)
        {
            StopCoroutine(reactionRoutine);
            visual.localPosition = originalLocalPosition;
        }

        reactionRoutine = StartCoroutine(HitRoutine());
    }

    private IEnumerator HitRoutine()
    {
        Vector3 recoilPosition =
            originalLocalPosition +
            Vector3.back * recoilDistance;

        float elapsed = 0f;

        while (elapsed < recoilDuration)
        {
            elapsed += Time.deltaTime;

            float progress =
                Mathf.Clamp01(elapsed / recoilDuration);

            visual.localPosition = Vector3.Lerp(
                originalLocalPosition,
                recoilPosition,
                progress
            );

            yield return null;
        }

        elapsed = 0f;

        while (elapsed < returnDuration)
        {
            elapsed += Time.deltaTime;

            float progress =
                Mathf.Clamp01(elapsed / returnDuration);

            visual.localPosition = Vector3.Lerp(
                recoilPosition,
                originalLocalPosition,
                progress
            );

            yield return null;
        }

        visual.localPosition = originalLocalPosition;
        reactionRoutine = null;
    }
}