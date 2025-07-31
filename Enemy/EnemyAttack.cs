using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    [Header("Detection Settings")]
    public float detectionRange = 5f;
    public float attackRange = 1.2f;
    public float moveSpeed = 3f;
    public float attackCooldown = 1f;

    [Header("Ground Check")]
    public Transform ledgeCheck;
    public float ledgeCheckDistance = 0.2f;
    public LayerMask groundLayer;

    [Header("AI Type Settings")]
    public bool isFlyingAI = false;

    private Transform player;
    private float lastAttackTime;
    private Rigidbody2D rb;
    private EnemyPatrol patrol;
    private Animator animator;
    private bool hasIsAttackingParam;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        rb = GetComponent<Rigidbody2D>();
        patrol = GetComponent<EnemyPatrol>();
        animator = GetComponent<Animator>();

        if (player == null)
            Debug.LogError("Player not found! Make sure the Player GameObject is tagged 'Player'.");
        if (patrol == null)
            Debug.LogError("EnemyPatrol component not found on enemy!");
        if (animator == null)
            Debug.LogError("Animator component not found on enemy!");
        if (ledgeCheck == null)
            Debug.LogWarning("LedgeCheck transform is not assigned. Enemy may fall off ledges.");

        hasIsAttackingParam = HasParameter("isAttacking");
    }

    private void Update()
    {
        if (player == null || patrol == null || animator == null) return;

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if (distanceToPlayer <= detectionRange)
        {
            if (patrol.canMove)
                patrol.StopPatrol();

            if (distanceToPlayer > attackRange)
            {
                Vector2 direction = (player.position - transform.position).normalized;

                bool isGroundAhead = true;
                if (ledgeCheck != null)
                {
                    isGroundAhead = Physics2D.Raycast(ledgeCheck.position, Vector2.down, ledgeCheckDistance, groundLayer);
                }

                if (isGroundAhead)
                {
                    rb.velocity = new Vector2(direction.x * moveSpeed, rb.velocity.y);
                    FlipTowards(direction.x);
                }
                else
                {
                    rb.velocity = new Vector2(0, rb.velocity.y);
                }

                if (!isFlyingAI && hasIsAttackingParam)
                    animator.SetBool("isAttacking", true);
            }
            else
            {
                rb.velocity = new Vector2(0, rb.velocity.y);

                if (Time.time >= lastAttackTime + attackCooldown)
                {
                    Debug.Log("Enemy attacked! (No damage applied)");
                    lastAttackTime = Time.time;
                }
            }
        }
        else
        {
            if (!patrol.canMove)
                patrol.ResumePatrol();

            if (!isFlyingAI && hasIsAttackingParam)
                animator.SetBool("isAttacking", false);
        }
    }

    void FlipTowards(float horizontalDirection)
    {
        Vector3 scale = transform.localScale;

        if (horizontalDirection > 0)
            scale.x = -Mathf.Abs(scale.x); // face right
        else if (horizontalDirection < 0)
            scale.x = Mathf.Abs(scale.x);  // face left

        transform.localScale = scale;
    }

    private bool HasParameter(string paramName)
    {
        foreach (AnimatorControllerParameter param in animator.parameters)
        {
            if (param.name == paramName)
                return true;
        }
        return false;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        if (ledgeCheck != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(ledgeCheck.position, ledgeCheck.position + Vector3.down * ledgeCheckDistance);
        }
    }
}
