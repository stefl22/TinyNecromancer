using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Health))]
public class DamageFlash : MonoBehaviour
{
    [Header("Flash")]
    [SerializeField] private Color flashColor = Color.white;
    [SerializeField] private float flashDuration = 0.08f;

    private Health health;

    private Renderer[] renderers;
    private Material[][] materials;
    private Color[][] originalColors;

    private Coroutine flashRoutine;

    private void Awake()
    {
        health = GetComponent<Health>();

        renderers = GetComponentsInChildren<Renderer>();

        materials = new Material[renderers.Length][];
        originalColors = new Color[renderers.Length][];

        for (int i = 0; i < renderers.Length; i++)
        {
            materials[i] = renderers[i].materials;

            originalColors[i] =
                new Color[materials[i].Length];

            for (int j = 0; j < materials[i].Length; j++)
            {
                originalColors[i][j] =
                    materials[i][j].color;
            }
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
        if (flashRoutine != null)
        {
            StopCoroutine(flashRoutine);
        }

        flashRoutine = StartCoroutine(
            FlashRoutine()
        );
    }

    private IEnumerator FlashRoutine()
    {
        SetColor(flashColor);

        yield return new WaitForSeconds(
            flashDuration
        );

        RestoreOriginalColors();

        flashRoutine = null;
    }

    private void SetColor(Color color)
    {
        for (int i = 0; i < materials.Length; i++)
        {
            for (int j = 0; j < materials[i].Length; j++)
            {
                materials[i][j].color = color;
            }
        }
    }

    private void RestoreOriginalColors()
    {
        for (int i = 0; i < materials.Length; i++)
        {
            for (int j = 0; j < materials[i].Length; j++)
            {
                materials[i][j].color =
                    originalColors[i][j];
            }
        }
    }
}