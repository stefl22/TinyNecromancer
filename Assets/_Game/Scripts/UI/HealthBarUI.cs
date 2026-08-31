using UnityEngine;
using UnityEngine.UI;

public class HealthBarUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Health health;
    [SerializeField] private Image fillImage;

    private void OnEnable()
    {
        if (health != null)
        {
            health.HealthChanged += UpdateHealthBar;
        }
    }

    private void Start()
    {
        Refresh();
    }

    private void OnDisable()
    {
        if (health != null)
        {
            health.HealthChanged -= UpdateHealthBar;
        }
    }

    private void UpdateHealthBar(float currentHealth, float maxHealth)
    {
        if (fillImage == null)
            return;

        fillImage.fillAmount =
            maxHealth > 0f
                ? currentHealth / maxHealth
                : 0f;
    }

    private void Refresh()
    {
        if (health == null || fillImage == null)
            return;

        UpdateHealthBar(
            health.CurrentHealth,
            health.MaxHealth
        );
    }
}