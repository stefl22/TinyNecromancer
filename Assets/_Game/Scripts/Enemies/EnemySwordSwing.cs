using System.Collections;
using UnityEngine;

[RequireComponent(typeof(EnemyAI))]
public class EnemySwordSwing : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform rightHandPoint;

    [Header("Swing")]
    [SerializeField]
    private Vector3 windupEuler =
        new Vector3(0f, 0f, -70f);

    [SerializeField]
    private Vector3 strikeEuler =
        new Vector3(0f, 0f, 70f);

    [SerializeField] private float windupDuration = 0.4f;
    [SerializeField] private float strikeDuration = 0.12f;
    [SerializeField] private float returnDuration = 0.18f;

    private EnemyAI enemyAI;

    private Quaternion normalRotation;
    private Quaternion windupRotation;
    private Quaternion strikeRotation;

    private Coroutine swingRoutine;

    private void Awake()
    {
        enemyAI = GetComponent<EnemyAI>();

        if (rightHandPoint != null)
        {
            normalRotation =
                rightHandPoint.localRotation;

            windupRotation =
                normalRotation *
                Quaternion.Euler(windupEuler);

            strikeRotation =
                normalRotation *
                Quaternion.Euler(strikeEuler);
        }
    }

    private void OnEnable()
    {
        enemyAI.AttackStarted +=
            HandleAttackStarted;
    }

    private void OnDisable()
    {
        enemyAI.AttackStarted -=
            HandleAttackStarted;
    }

    private void HandleAttackStarted()
    {
        if (rightHandPoint == null)
            return;

        if (swingRoutine != null)
        {
            StopCoroutine(swingRoutine);

            rightHandPoint.localRotation =
                normalRotation;
        }

        swingRoutine =
            StartCoroutine(SwingRoutine());
    }

    private IEnumerator SwingRoutine()
    {
        yield return RotateOverTime(
            normalRotation,
            windupRotation,
            windupDuration
        );

        yield return RotateOverTime(
            windupRotation,
            strikeRotation,
            strikeDuration
        );

        yield return RotateOverTime(
            strikeRotation,
            normalRotation,
            returnDuration
        );

        rightHandPoint.localRotation =
            normalRotation;

        swingRoutine = null;
    }

    private IEnumerator RotateOverTime(
        Quaternion from,
        Quaternion to,
        float duration)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;

            float progress =
                Mathf.Clamp01(
                    elapsed / duration
                );

            rightHandPoint.localRotation =
                Quaternion.Slerp(
                    from,
                    to,
                    progress
                );

            yield return null;
        }

        rightHandPoint.localRotation = to;
    }
}