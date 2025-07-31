using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerDust : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private ParticleSystem dustPrefab;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundMask;

    [Header("Settings")]
    [SerializeField] private float groundCheckRadius = 0.2f;
    [SerializeField] private float emitInterval = 0.15f;
    [SerializeField] private float runThreshold = 0.1f;

    private Rigidbody2D rb;
    private float timer;
    private ParticleSystem currentDust;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        if (dustPrefab != null)
        {
            currentDust = Instantiate(dustPrefab, groundCheck.position, Quaternion.identity);
            currentDust.Stop();
        }
    }

    void Update()
    {
        timer += Time.deltaTime;

        bool grounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundMask);
        bool running = Mathf.Abs(rb.velocity.x) > runThreshold;

        if (grounded && running && timer >= emitInterval)
        {
            EmitDust(rb.velocity.x > 0 ? 1 : -1);
            timer = 0f;
        }
    }

    void EmitDust(int direction)
    {
        if (currentDust == null) return;

        currentDust.transform.position = groundCheck.position;

        // Point opposite to movement direction
        currentDust.transform.rotation = Quaternion.Euler(0, 0, direction > 0 ? 180 : 0);

        currentDust.Emit(4);
    }
}
