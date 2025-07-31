using UnityEngine;

public class EnemyAttackZone : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        // 1. Get player's Y position and Rigidbody
        float playerY = other.transform.position.y;
        float enemyY = transform.position.y;

        // 2. Stomp Check
        PlayerMovement playerMovement = other.GetComponent<PlayerMovement>();
        if (playerMovement != null && playerMovement.IsStomping())
        {
            return; // Ignore if stomping
        }

        // 3. Positional check
        if (playerY > enemyY + 0.2f)
        {
            return; // Player is above — assume stomping
        }

     
    }
}
