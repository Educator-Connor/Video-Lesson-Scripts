using UnityEngine;

public class DamageZone : MonoBehaviour
{
    public int damage = 1;
    public float knockbackForce = 10f;
    public Vector2 knockbackDirection = new Vector2(-1f, 1f); // default direction

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
        Rigidbody2D playerRb = other.GetComponent<Rigidbody2D>();

        if (playerHealth != null)
        {
            playerHealth.TakeDamage(damage);
            Debug.Log("DamageZone dealt damage to the player!");

            if (playerRb != null)
            {
                // Flip direction if enemy is to the right or left of player
                Vector2 direction = (other.transform.position.x < transform.position.x)
                    ? new Vector2(-1, 1)
                    : new Vector2(1, 1);

                direction.Normalize();
                playerRb.velocity = Vector2.zero; // Reset first to prevent stacking
                playerRb.AddForce(direction * knockbackForce, ForceMode2D.Impulse);
            }
        }
    }
}
