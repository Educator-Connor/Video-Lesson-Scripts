using UnityEngine;

public class Points : MonoBehaviour
{
    private LevelManager levelManager;

    private void Start()
    {
        levelManager = FindObjectOfType<LevelManager>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (levelManager != null)
            {
                levelManager.CollectGem();
            }
            Destroy(gameObject);
        }
    }
}
