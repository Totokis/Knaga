using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] public float moveSpeed = 5f;
    [SerializeField] public float acceleration = 10f;
    [SerializeField] public float deceleration = 10f;
    
    [Header("Movement Bounds")]
    [SerializeField] public float leftBound = -4f;
    [SerializeField] public float rightBound = 40f;
    
    [Header("Collision Settings")]
    [SerializeField] public float collisionCheckDistance = 0.1f;
    
    private float currentVelocity = 0f;
    private float targetVelocity = 0f;
    private float moveInput = 0f;
    private Rigidbody2D rb;
    private BoxCollider2D boxCollider;
    
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        boxCollider = GetComponent<BoxCollider2D>();
        
        if (rb != null)
        {
            // Upewnij się że gracz nie obraca się i może poruszać się tylko w X
            rb.constraints = RigidbodyConstraints2D.FreezeRotation | RigidbodyConstraints2D.FreezePositionY;
            rb.gravityScale = 0; // Wyłącz grawitację bo gracz ma być na stałej wysokości
            rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous; // Lepsza detekcja kolizji
        }
        
        if (boxCollider != null)
        {
            boxCollider.isTrigger = false; // Upewnij się że collider nie jest triggerem
        }
    }
    
    void Update()
    {
        // Get input from keyboard
        moveInput = 0f;
        
        if (Keyboard.current != null)
        {
            if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed)
                moveInput = -1f;
            else if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed)
                moveInput = 1f;
        }
        
        // Also check gamepad if available
        if (Gamepad.current != null)
        {
            float gamepadInput = Gamepad.current.leftStick.x.ReadValue();
            if (Mathf.Abs(gamepadInput) > 0.1f)
                moveInput = gamepadInput;
        }
        
        // Set target velocity based on input
        targetVelocity = moveInput * moveSpeed;
        
        // Smooth velocity changes
        float velocityChange = targetVelocity != 0 ? acceleration : deceleration;
        currentVelocity = Mathf.MoveTowards(currentVelocity, targetVelocity, velocityChange * Time.deltaTime);
    }
    
    void FixedUpdate()
    {
        if (rb != null)
        {
            // Calculate desired movement
            Vector2 movement = Vector2.right * currentVelocity * Time.fixedDeltaTime;
            Vector2 newPosition = rb.position + movement;
            
            // Check for collisions before moving
            if (currentVelocity != 0 && boxCollider != null)
            {
                // Cast a box in the direction of movement
                float direction = Mathf.Sign(currentVelocity);
                Vector2 boxSize = boxCollider.size * 0.9f; // Slightly smaller to avoid edge cases
                Vector2 boxCenter = rb.position + (Vector2)boxCollider.offset;
                
                RaycastHit2D hit = Physics2D.BoxCast(
                    boxCenter, 
                    boxSize, 
                    0f, 
                    Vector2.right * direction, 
                    Mathf.Abs(movement.x) + collisionCheckDistance,
                    LayerMask.GetMask("Default")
                );
                
                if (hit.collider != null && hit.collider.CompareTag("WallSegment"))
                {
                    // Stop at the wall
                    float distanceToWall = hit.distance - collisionCheckDistance;
                    if (distanceToWall > 0)
                    {
                        movement = Vector2.right * direction * distanceToWall;
                        newPosition = rb.position + movement;
                    }
                    else
                    {
                        // Already at the wall, don't move
                        newPosition = rb.position;
                    }
                    currentVelocity = 0f; // Stop velocity when hitting wall
                }
            }
            
            // Apply bounds
            newPosition.x = Mathf.Clamp(newPosition.x, leftBound, rightBound);
            
            // Apply the movement
            rb.MovePosition(newPosition);
        }
    }
    
    void OnDrawGizmos()
    {
        // Visualize collision check in editor
        if (boxCollider != null && Application.isPlaying)
        {
            Gizmos.color = Color.yellow;
            Vector3 boxCenter = transform.position + (Vector3)boxCollider.offset;
            Vector3 boxSize = boxCollider.size * 0.9f;
            
            if (currentVelocity > 0)
            {
                Gizmos.DrawWireCube(boxCenter + Vector3.right * collisionCheckDistance, boxSize);
            }
            else if (currentVelocity < 0)
            {
                Gizmos.DrawWireCube(boxCenter - Vector3.right * collisionCheckDistance, boxSize);
            }
        }
    }
}