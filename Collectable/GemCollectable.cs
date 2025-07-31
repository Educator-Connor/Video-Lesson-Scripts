using UnityEngine;

public class DestroyOnPlayerContact : MonoBehaviour
{
    public GameObject collectVFXPrefab; // Assign your VFX animation prefab here

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // Spawn collect VFX at gem's position
            if (collectVFXPrefab != null)
            {
                Instantiate(collectVFXPrefab, transform.position, Quaternion.identity);
            }

            // Then destroy the gem
            Destroy(gameObject);
        }
    }
}
