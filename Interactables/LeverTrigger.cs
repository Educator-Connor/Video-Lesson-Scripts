using UnityEngine;

public class LeverTrigger : MonoBehaviour
{
    public MovingPlatform platform;        // Reference to the moving platform
    public Sprite crankUpSprite;           // Sprite when the lever is cranked up
    public Sprite crankDownSprite;         // Sprite when the lever is cranked down

    private bool playerInRange = false;
    private SpriteRenderer spriteRenderer;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        // Set initial sprite based on platform state
        UpdateLeverSprite();
    }

    void Update()
    {
        if (playerInRange && Input.GetKeyDown(KeyCode.E))
        {
            platform.isActive = !platform.isActive;
            UpdateLeverSprite();
        }
    }

    void UpdateLeverSprite()
    {
        if (spriteRenderer == null) return;

        spriteRenderer.sprite = platform.isActive ? crankDownSprite : crankUpSprite;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
        }
    }
}
