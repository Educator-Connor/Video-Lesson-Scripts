using UnityEngine;

public class GemCollect : MonoBehaviour
{
    private LevelManager levelManager;

    [Header("FX")]
    public GameObject collectVFXPrefab;   // Assign your VFX prefab in Inspector

    [Tooltip("Leave empty to use AudioManager.PlaySFXByIndex instead.")]
    public AudioClip collectSfx;          // Optional: drag a unique clip here

    [Tooltip("If Collect Sfx is empty, use this index into AudioManager.sfxClips.")]
    public int sfxIndex = 0;              // Default to first clip in AudioManager

    private void Start()
    {
        levelManager = FindObjectOfType<LevelManager>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        // Safely play collect sound effect if AudioManager exists
        if (AudioManager.Instance != null)
        {
            if (collectSfx != null)
            {
                AudioManager.Instance.PlaySFX(collectSfx);
            }
            else
            {
                AudioManager.Instance.PlaySFXByIndex(sfxIndex);
            }
        }
        else
        {
            Debug.LogWarning("GemCollect: AudioManager.Instance is null — no sound will play.");
        }

        // Spawn collect VFX at gem position (if assigned)
        if (collectVFXPrefab != null)
        {
            Instantiate(collectVFXPrefab, transform.position, Quaternion.identity);
        }

        // Notify level manager that a gem was collected
        if (levelManager != null)
        {
            levelManager.CollectGem();
        }
        else
        {
            Debug.LogWarning("GemCollect: LevelManager reference is missing.");
        }

        // Destroy the gem GameObject after collection
        Destroy(gameObject);
    }

}
