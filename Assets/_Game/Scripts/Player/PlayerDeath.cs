using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Health))]
public class PlayerDeath : MonoBehaviour
{
    [Header("Death")]
    [SerializeField] private float restartDelay = 2f;

    private Health health;
    private PlayerMovement playerMovement;
    private PlayerCombat playerCombat;

    private void Awake()
    {
        health = GetComponent<Health>();
        playerMovement = GetComponent<PlayerMovement>();
        playerCombat = GetComponent<PlayerCombat>();
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
        if (playerMovement != null)
            playerMovement.enabled = false;

        if (playerCombat != null)
            playerCombat.enabled = false;

        StartCoroutine(RestartScene());
    }

    private IEnumerator RestartScene()
    {
        yield return new WaitForSeconds(restartDelay);

        Scene currentScene = SceneManager.GetActiveScene();

        SceneManager.LoadScene(currentScene.buildIndex);
    }
}