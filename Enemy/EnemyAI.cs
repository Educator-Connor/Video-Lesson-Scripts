using UnityEngine;

public class EnemyPatrol : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 2f;
    public bool startsFacingRight = true;
    public bool canFly = false;

    [Header("Flying Settings")]
    public float flyRadius = 2f;
    public bool flyVertical = false;
    private Vector3 flyCenter;
    private float flyTimer;

    [Header("Detection (Ground Only)")]
    public Transform groundCheck;
    public Transform wallCheck;
    public float groundCheckDistance = 1.2f;
    public float wallCheckDistance = 0.3f;
    public LayerMask groundLayer;

    [Header("Flip Cooldown")]
    public float flipCooldown = 0.2f;

    private Rigidbody2D rb;
    private bool movingRight;
    private float lastFlipTime;
    private Vector3 originalScale;

    [HideInInspector]
    public bool canMove = true;  // Toggle patrol movement externally

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        movingRight = startsFacingRight;
        originalScale = transform.localScale;
        ApplyFacingDirection();

        if (canFly)
        {
            flyCenter = transform.position; // Set the center of flying patrol area
        }
    }

    void Update()
    {
        if (!canMove) return;

        if (canFly)
        {
            FlyPatrol();
        }
        else
        {
            Patrol();
        }
    }

    void Patrol()
    {
        Vector2 direction = movingRight ? Vector2.right : Vector2.left;

        // Check for obstacle just ahead at foot level
        RaycastHit2D obstacleHit = Physics2D.Raycast(wallCheck.position, direction, 0.1f, groundLayer);

        if (obstacleHit.collider != null)
        {
            rb.velocity = new Vector2(0, rb.velocity.y);
        }
        else
        {
            rb.velocity = new Vector2(direction.x * moveSpeed, rb.velocity.y);
        }

        // Check ground and walls
        bool groundAhead = Physics2D.Raycast(groundCheck.position, Vector2.down, groundCheckDistance, groundLayer);
        bool wallAhead = Physics2D.Raycast(wallCheck.position, direction, wallCheckDistance, groundLayer);

        Debug.DrawRay(groundCheck.position, Vector2.down * groundCheckDistance, Color.green);
        Debug.DrawRay(wallCheck.position, direction * wallCheckDistance, Color.red);

        if (Time.time > lastFlipTime + flipCooldown && (!groundAhead || wallAhead))
        {
            Flip();
            lastFlipTime = Time.time;
        }
    }

    void FlyPatrol()
    {
        flyTimer += Time.deltaTime * moveSpeed;

        Vector3 offset = flyVertical
            ? new Vector3(0f, Mathf.Sin(flyTimer) * flyRadius, 0f)
            : new Vector3(Mathf.Sin(flyTimer) * flyRadius, 0f, 0f);

        Vector3 targetPos = flyCenter + offset;
        rb.MovePosition(Vector2.Lerp(transform.position, targetPos, Time.deltaTime * moveSpeed));

        // Face movement direction (only if not vertical)
        if (!flyVertical)
        {
            bool shouldFaceRight = targetPos.x > transform.position.x;
            if (shouldFaceRight != movingRight)
            {
                Flip();
            }
        }
    }

    void Flip()
    {
        movingRight = !movingRight;
        ApplyFacingDirection();
    }

    void ApplyFacingDirection()
    {
        Vector3 scale = originalScale;
        scale.x = originalScale.x * (movingRight ? -1 : 1);
        transform.localScale = scale;
    }

    public void StopPatrol()
    {
        canMove = false;
        rb.velocity = new Vector2(0, rb.velocity.y);
    }

    public void ResumePatrol()
    {
        canMove = true;
    }
}
