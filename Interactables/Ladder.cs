using UnityEngine;

public class LadderZone : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log($"OnTriggerEnter2D with {other.name}, tag: {other.tag}");
        if (other.CompareTag("Player") && other.TryGetComponent(out PlayerMovement movement))
        {
            Debug.Log("Player entered ladder trigger");
            movement.EnterLadder(transform);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        Debug.Log($"OnTriggerExit2D with {other.name}, tag: {other.tag}");
        if (other.CompareTag("Player") && other.TryGetComponent(out PlayerMovement movement))
        {
            Debug.Log("Player exited ladder trigger");
            movement.ExitLadder();
        }
    }

    // Optional: Visualize ladder zone in scene view for debugging
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        BoxCollider2D box = GetComponent<BoxCollider2D>();
        if (box != null)
            Gizmos.DrawWireCube(box.bounds.center, box.bounds.size);
    }
}
