using UnityEngine;

public class FinishTrigger : MonoBehaviour
{
    [Header("Sound FX")]
    [Tooltip("Optional unique clip to play when the level is finished.")]
    public AudioClip finishSfx;

    [Tooltip("If no unique clip assigned, use this index in AudioManager.sfxClips.")]
    public int sfxIndex = 4;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;

        if (finishSfx != null)
            AudioManager.Instance.PlaySFX(finishSfx);
        else
            AudioManager.Instance.PlaySFXByIndex(sfxIndex);

        GameManager.Instance?.GetCurrentLevelManager()?.SetPlayerAtFinish();
    }
}
