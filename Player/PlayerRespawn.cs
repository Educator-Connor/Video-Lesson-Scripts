using UnityEngine;
using System.Collections;

public class PlayerRespawn : MonoBehaviour
{
    private Vector3 respawnPoint;
    private CharacterController2D controller;
    private Animator animator;
    private Rigidbody2D rb;
    private bool isPaused = false;

    [Header("Respawn Animation")]
    public GameObject respawnAnimationPrefab;
    public float delayBeforeRespawn = 0.5f;

    private void Start()
    {
        respawnPoint = transform.position;
        controller = GetComponent<CharacterController2D>();
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
    }

    public void SetRespawnPoint(Vector3 newPoint)
    {
        respawnPoint = newPoint;
    }

    public void Respawn()
    {
        StartCoroutine(RespawnRoutine());
    }

    private IEnumerator RespawnRoutine()
    {
        SetPaused(true);

        // Play respawn sound BEFORE animation and teleport
        AudioManager.Instance?.PlayRespawn();

        // Play Respawn Start animation at respawn point
        if (respawnAnimationPrefab != null)
        {
            GameObject animObj = Instantiate(respawnAnimationPrefab, respawnPoint, Quaternion.identity);
            RespawnAnimationController rac = animObj.GetComponent<RespawnAnimationController>();
            if (rac != null)
                yield return StartCoroutine(rac.PlayRespawnStart());
        }

        // Teleport player
        transform.position = respawnPoint;

        if (rb != null)
            rb.velocity = Vector2.zero;

        yield return new WaitForSeconds(delayBeforeRespawn);

        SetPaused(false);

        // Play Respawn End animation at player position
        if (respawnAnimationPrefab != null)
        {
            GameObject animObj = Instantiate(respawnAnimationPrefab, transform.position, Quaternion.identity);
            RespawnAnimationController rac = animObj.GetComponent<RespawnAnimationController>();
            if (rac != null)
                yield return StartCoroutine(rac.PlayRespawnEnd());
        }
    }

    public void SetPaused(bool paused)
    {
        isPaused = paused;

        if (controller != null)
            controller.enabled = !paused;

        if (animator != null)
            animator.speed = paused ? 0f : 1f;
    }
}
