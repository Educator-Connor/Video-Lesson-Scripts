using UnityEngine;

public class Increasehealth : MonoBehaviour
{
    [Tooltip("Amount of health to increase when the player collides with this object")]
    public int healthIncreaseAmount = 1;

    [Tooltip("Should this object be destroyed after being picked up?")]
    public bool destroyOnPickup = true;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.IncreaseHealth(healthIncreaseAmount);

                if (destroyOnPickup)
                {
                    Destroy(gameObject);
                }
            }
        }
    }
}
