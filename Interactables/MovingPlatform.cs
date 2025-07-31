using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    public Transform pointA;
    public Transform pointB;
    public float moveSpeed = 2f;
    public bool isActive = true; 

    private Transform targetPoint;

    void Start()
    {
        targetPoint = pointB;
    }

    void FixedUpdate()
    {
        if (!isActive) return; // <- only move if active

        transform.position = Vector2.MoveTowards(transform.position, targetPoint.position, moveSpeed * Time.fixedDeltaTime);

        if (Vector2.Distance(transform.position, targetPoint.position) < 0.05f)
        {
            targetPoint = (targetPoint == pointA) ? pointB : pointA;
        }
    }
}
