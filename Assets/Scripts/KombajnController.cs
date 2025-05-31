using UnityEngine;

public class KombajnController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float stopDistance = 0.1f;
    public bool autoMove = true;
    
    private Rigidbody2D rb;
    private bool isMoving = false;
    private bool shouldMove = false;
    private float stopTimer = 0f;
    private const float STOP_DELAY = 0.5f;
    
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.gravityScale = 0f;
            rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        }
        
        if (autoMove)
        {
            StartMoving();
        }
    }
    
    void FixedUpdate()
    {
        if (shouldMove && !isMoving)
        {
            stopTimer += Time.fixedDeltaTime;
            if (stopTimer >= STOP_DELAY)
            {
                isMoving = true;
                stopTimer = 0f;
            }
        }
        
        if (isMoving && rb != null)
        {
            rb.linearVelocity = new Vector2(moveSpeed, rb.linearVelocity.y);
        }
        else if (rb != null)
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
        }
    }
    
    public void StopMoving()
    {
        Debug.Log("Kombajn: Stopping movement");
        isMoving = false;
        shouldMove = false;
        stopTimer = 0f;
    }
    
    public void StartMoving()
    {
        Debug.Log("Kombajn: Starting movement (with delay)");
        shouldMove = true;
    }
    
    public bool IsStopped()
    {
        return !isMoving && rb != null && Mathf.Abs(rb.linearVelocity.x) < stopDistance;
    }
    
    public bool IsMoving()
    {
        return isMoving;
    }
}