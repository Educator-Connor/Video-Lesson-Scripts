using UnityEngine;

public class BoxReward : MonoBehaviour
{
    [Tooltip("Prefab to spawn when this box is hit by the player.")]
    public GameObject rewardPrefab;

    [Tooltip("Prefab for the VFX to play after box is destroyed.")]
    public GameObject vfxPrefab;

    [Tooltip("Offset for spawn position (e.g. Vector3.up for above box).")]
    public Vector3 spawnOffset = Vector3.up;

    [Header("Sound FX")]
    [Tooltip("Optional unique clip to play on box destruction.")]
    public AudioClip destructionSfx;

    [Tooltip("If no unique clip assigned, use this index in AudioManager.sfxClips.")]
    public int sfxIndex = 2;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!collision.gameObject.CompareTag("Player")) return;

        // Safe audio playback
        if (AudioManager.Instance != null)
        {
            if (destructionSfx != null)
            {
                AudioManager.Instance.PlaySFX(destructionSfx);
            }
            else
            {
                AudioManager.Instance.PlaySFXByIndex(sfxIndex);
            }
        }
        else
        {
            Debug.LogWarning("BoxReward: AudioManager.Instance is null — sound will not play.");
        }

        SpawnReward();
        SpawnVFX();
        Destroy(gameObject);
    }

    private void SpawnReward()
    {
        if (rewardPrefab != null)
        {
            Instantiate(rewardPrefab, transform.position + spawnOffset, Quaternion.identity);
        }
        else
        {
            Debug.LogWarning("BoxReward: No rewardPrefab assigned.");
        }
    }

    private void SpawnVFX()
    {
        if (vfxPrefab != null)
        {
            Instantiate(vfxPrefab, transform.position, Quaternion.identity);
        }
        else
        {
            Debug.LogWarning("BoxReward: No vfxPrefab assigned.");
        }
    }
}
