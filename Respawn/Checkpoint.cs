using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    private static readonly List<Checkpoint> allCheckpoints = new List<Checkpoint>();

    private SpriteRenderer sr;
    private bool isActivated;                       // prevents double-trigger sound

    [Header("Sound FX")]
    [Tooltip("Optional unique clip to play when this checkpoint is activated.")]
    public AudioClip checkpointSfx;                 // leave empty to use index fallback

    [Tooltip("If no unique clip assigned, use this index in AudioManager.sfxClips.")]
    public int sfxIndex = 3;                        // default index for checkpoint sound

    private void Awake()
    {
        allCheckpoints.Add(this);
        sr = GetComponent<SpriteRenderer>();
    }

    private void OnDestroy()
    {
        allCheckpoints.Remove(this);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isActivated) return;                    // already triggered once
        if (!other.CompareTag("Player")) return;    // ignore non-player

        PlayerRespawn respawn = other.GetComponent<PlayerRespawn>();
        if (respawn != null)
        {
            respawn.SetRespawnPoint(transform.position);
            UpdateCheckpointVisuals(this);
            PlayCheckpointSound();
            isActivated = true;
        }
    }

    // Plays either the unique clip or the indexed clip
    private void PlayCheckpointSound()
    {
        if (checkpointSfx != null)
        {
            AudioManager.Instance.PlaySFX(checkpointSfx);
        }
        else
        {
            AudioManager.Instance.PlaySFXByIndex(sfxIndex);
        }
    }

    // Static method to update visuals of all checkpoints
    private static void UpdateCheckpointVisuals(Checkpoint active)
    {
        foreach (Checkpoint cp in allCheckpoints)
        {
            if (cp != null && cp.sr != null)
            {
                cp.sr.enabled = cp != active;   // hide the active checkpoint
            }
        }
    }
}
