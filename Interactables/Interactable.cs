using UnityEngine;

public class Interactable : MonoBehaviour
{
    [Tooltip("Optional UI object that shows the 'Press E' prompt")]
    public GameObject promptUI;
    public string interactionPrompt = "Press E";

    private bool playerInRange = false;

    protected virtual void Start()
    {
        if (promptUI != null)
            promptUI.SetActive(false);
    }

    void Update()
    {
        if (playerInRange && Input.GetKeyDown(KeyCode.E))
        {
            Interact();
        }
    }

    protected virtual void Interact()
    {
        Debug.Log($"Interacted with: {gameObject.name}");
        // To be overridden
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;

            if (promptUI != null)
                promptUI.SetActive(true);
            else
                Debug.LogWarning($"{gameObject.name}: No promptUI assigned.");
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;

            if (promptUI != null)
                promptUI.SetActive(false);
        }
    }
}
