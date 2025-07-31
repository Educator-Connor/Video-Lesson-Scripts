using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private int maxHealth = 3;
    public float knockbackForce = 5f;
    public float invincibilityTime = 1f;
    public float hurtAnimationDuration = 0.5f;

    [Header("UI")]
    public HealthHeartsUI healthHeartsUI;

    private int health;
    private bool isInvincible = false;
    private bool isDead = false;
    private Rigidbody2D rb;
    private PlayerRespawn respawn;
    private Animator animator;
    private PlayerMovement movement;
    private LevelManager levelManager;

    private void OnEnable() => SceneManager.sceneLoaded += OnSceneLoaded;
    private void OnDisable() => SceneManager.sceneLoaded -= OnSceneLoaded;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        respawn = GetComponent<PlayerRespawn>();
        animator = GetComponent<Animator>();
        movement = GetComponent<PlayerMovement>();
        levelManager = FindObjectOfType<LevelManager>();

        if (rb == null) Debug.LogWarning("PlayerHealth: Rigidbody2D not found!");
        if (respawn == null) Debug.LogWarning("PlayerHealth: PlayerRespawn not found!");
        if (animator == null) Debug.LogWarning("PlayerHealth: Animator not found!");
        if (movement == null) Debug.LogWarning("PlayerHealth: PlayerMovement not found!");
        if (levelManager == null) Debug.LogWarning("PlayerHealth: LevelManager not found!");

        health = maxHealth;

        healthHeartsUI ??= FindObjectOfType<HealthHeartsUI>();
        if (healthHeartsUI == null)
        {
            Debug.LogWarning("PlayerHealth: HealthHeartsUI not assigned or found!");
        }
        else
        {
            healthHeartsUI.SetHealth(health);
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        healthHeartsUI = FindObjectOfType<HealthHeartsUI>();
        if (healthHeartsUI == null)
        {
            Debug.LogWarning("PlayerHealth: HealthHeartsUI not found on scene load!");
        }
        else
        {
            healthHeartsUI.SetHealth(health);
        }
    }

    public void TakeDamage(int amount, Vector2 hitDirection = default)
    {
        if (isInvincible || isDead) return;

        health = Mathf.Max(health - amount, 0);
        healthHeartsUI?.SetHealth(health);
        levelManager?.RegisterInjury();

        if (health > 0)
        {
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlayHurt();
            }
            else
            {
                Debug.LogWarning("PlayerHealth: AudioManager.Instance is null - can't play hurt sound.");
            }

            if (animator != null)
            {
                animator.SetBool("isHurt", true);
            }

            if (movement != null)
            {
                movement.isHurt = true;
            }

            if (rb != null)
            {
                rb.velocity = Vector2.zero;
                rb.AddForce(hitDirection.normalized * knockbackForce, ForceMode2D.Impulse);
            }

            StartCoroutine(InvincibilityCoroutine());
            StartCoroutine(HurtCoroutine());
        }
        else if (!isDead)
        {
            isDead = true;
            StartCoroutine(DeathCoroutine());
        }
    }

    public void IncreaseHealth(int amount)
    {
        if (isDead) return;

        health = Mathf.Min(health + amount, maxHealth);
        healthHeartsUI?.SetHealth(health);
        Debug.Log("Health increased by " + amount + ". Current health: " + health);
    }

    private IEnumerator InvincibilityCoroutine()
    {
        isInvincible = true;
        yield return new WaitForSeconds(invincibilityTime);
        isInvincible = false;
    }

    private IEnumerator HurtCoroutine()
    {
        yield return new WaitForSeconds(hurtAnimationDuration);
        if (animator != null)
        {
            animator.SetBool("isHurt", false);
        }

        if (movement != null)
        {
            movement.isHurt = false;
        }
    }

    private IEnumerator DeathCoroutine()
    {
        yield return new WaitForSeconds(1f);
        levelManager?.RegisterDeath();

        if (respawn != null)
        {
            respawn.Respawn();
            health = maxHealth;
            isDead = false;
            isInvincible = false;
            healthHeartsUI?.SetHealth(health);
        }
        else
        {
            Debug.LogWarning("PlayerHealth: No PlayerRespawn component found. Cannot respawn.");
        }
    }
}
