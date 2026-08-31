using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Health))]
public class EnemyDeath : MonoBehaviour
{
    [SerializeField] private float disappearDuration = 0.5f;

    private Health health;
    private Collider[] colliders;

    private void Awake()
    {
        health = GetComponent<Health>();
        colliders = GetComponentsInChildren<Collider>();
    }

    private void OnEnable()
    {
        health.Died += HandleDeath;
    }

    private void OnDisable()
    {
        health.Died -= HandleDeath;
    }

    private void HandleDeath()
    {
        foreach (Collider enemyCollider in colliders)
        {
            enemyCollider.enabled = false;
        }

        StartCoroutine(Disappear());
    }

    private IEnumerator Disappear()
    {
        Vector3 startingScale = transform.localScale;
        float elapsedTime = 0f;

        while (elapsedTime < disappearDuration)
        {
            elapsedTime += Time.deltaTime;

            float progress = elapsedTime / disappearDuration;

            transform.localScale = Vector3.Lerp(
                startingScale,
                Vector3.zero,
                progress
            );

            yield return null;
        }

        Destroy(gameObject);
    }
}