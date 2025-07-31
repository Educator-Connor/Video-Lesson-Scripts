using UnityEngine;

public class EnemyStompable : MonoBehaviour
{
    public int stompDamage = 1;
    public float bounceForce = 10f;

    private EnemyHealth enemyHealth;

    private void Awake()
    {
        enemyHealth = GetComponentInParent<EnemyHealth>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        Rigidbody2D playerRb = other.GetComponent<Rigidbody2D>();
        PlayerMovement playerMovement = other.GetComponent<PlayerMovement>();

        if (playerRb == null || playerMovement == null) return;

        // Only trigger if falling
        if (playerRb.velocity.y < -1f)
        {
            enemyHealth.TakeDamage(stompDamage);

            // Bounce up
            playerRb.velocity = new Vector2(playerRb.velocity.x, bounceForce);

            Debug.Log("Enemy stomped and killed!");
        }
    }
}
