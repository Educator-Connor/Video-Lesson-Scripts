using UnityEngine;

public class Spike : MonoBehaviour
{
    public int damageAmount = 1;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerHealth health = other.GetComponent<PlayerHealth>();
            if (health != null)
            {
                Vector2 knockbackDir = (other.transform.position - transform.position).normalized;
                health.TakeDamage(damageAmount, knockbackDir);
            }
        }
    }
}
