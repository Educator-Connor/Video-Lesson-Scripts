using UnityEngine;

public class EnemyStompZone : MonoBehaviour
{
    public EnemyHealth enemyHealth;
    public float bounceForce = 12f;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        Rigidbody2D playerRb = other.GetComponent<Rigidbody2D>();
        if (playerRb == null) return;

        // Damage enemy
        if (enemyHealth != null)
        {
            enemyHealth.TakeDamage(1);
        }

        // Bounce the player upward
        playerRb.velocity = new Vector2(playerRb.velocity.x, bounceForce);

        Debug.Log("Enemy stomped and took damage!");
    }
}
