using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public int maxHealth = 1;
    private int currentHealth;

    public int basePoints = 10;  // Points awarded for killing this enemy

    [Tooltip("Optional VFX prefab to spawn on enemy death.")]
    public GameObject deathVFXPrefab;

    private bool isDead = false;

    private void Awake()
    {
        currentHealth = Mathf.Max(1, maxHealth);
    }

    public void TakeDamage(int amount)
    {
        if (isDead) return;

        currentHealth -= amount;

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        if (isDead) return;
        isDead = true;

        // Try to play sound
        try
        {
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlaySFXByIndexWithPitch(1, 1.5f);
            }
            else
            {
                Debug.LogWarning("EnemyHealth: AudioManager.Instance is missing!");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"EnemyHealth: Error playing sound - {e.Message}");
        }

        // Try to update level state
        try
        {
            LevelManager levelManager = FindObjectOfType<LevelManager>();
            if (levelManager != null)
            {
                levelManager.RegisterHit();
                levelManager.AddScore(basePoints);
            }
            else
            {
                Debug.LogWarning("EnemyHealth: LevelManager not found in scene.");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"EnemyHealth: Error updating LevelManager - {e.Message}");
        }

        // Try to spawn VFX
        try
        {
            if (deathVFXPrefab != null)
            {
                Instantiate(deathVFXPrefab, transform.position, Quaternion.identity);
            }
            else
            {
                Debug.LogWarning("EnemyHealth: No death VFX prefab assigned.");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"EnemyHealth: Error spawning VFX - {e.Message}");
        }

        // Destroy enemy regardless
        Destroy(gameObject);
    }
}
