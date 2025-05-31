using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerControllerSnappy : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] public float moveSpeed = 8f;
    
    [Header("Movement Bounds")]
    [SerializeField] public float leftBound = -4f;
    [SerializeField] public float rightBound = 40f;
    
    [Header("Collision Settings")]
    [SerializeField] public float collisionCheckDistance = 0.05f;
    
    private float moveInput = 0f;
    private Rigidbody2D rb;
    private BoxCollider2D boxCollider;
    
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        boxCollider = GetComponent<BoxCollider2D>();
        
        if (rb != null)
        {
            rb.constraints = RigidbodyConstraints2D.FreezeRotation | RigidbodyConstraints2D.FreezePositionY;
            rb.gravityScale = 0;
            rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
            rb.interpolation = RigidbodyInterpolation2D.Interpolate;
        }
        
        if (boxCollider != null)
        {
            boxCollider.isTrigger = false;
        }
        
        Debug.Log("PlayerControllerSnappy initialized. MoveSpeed: " + moveSpeed);
    }
    
    void Update()
    {
        // Natychmiastowy input - bez żadnej interpolacji
        moveInput = 0f;
        
        if (Keyboard.current != null)
        {
            if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed)
                moveInput = -1f;
            else if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed)
                moveInput = 1f;
        }
        
        if (Gamepad.current != null)
        {
            float gamepadInput = Gamepad.current.leftStick.x.ReadValue();
            if (Mathf.Abs(gamepadInput) > 0.1f)
                moveInput = Mathf.Sign(gamepadInput); // Konwertuj na -1 lub 1
        }
    }
    
    void FixedUpdate()
    {
        if (rb == null) return;
        
        // Natychmiastowy ruch bez akceleracji
        float velocity = moveInput * moveSpeed;
        Vector2 movement = Vector2.right * velocity * Time.fixedDeltaTime;
        Vector2 newPosition = rb.position + movement;
        
        // Sprawdzanie kolizji tylko gdy się poruszamy
        if (moveInput != 0 && boxCollider != null)
        {
            Vector2 boxSize = boxCollider.size * 0.9f; // Trochę mniejsze dla bezpieczeństwa
            Vector2 boxCenter = rb.position + (Vector2)boxCollider.offset;
            
            // Sprawdź wszystkie warstwy oprócz gracza
            int layerMask = ~(1 << gameObject.layer);
            
            // Raycast w kierunku ruchu
            RaycastHit2D hit = Physics2D.BoxCast(
                boxCenter, 
                boxSize, 
                0f, 
                Vector2.right * moveInput, 
                Mathf.Abs(movement.x) + collisionCheckDistance,
                layerMask
            );
            
            if (hit.collider != null && !hit.collider.isTrigger)
            {
                // Sprawdź czy to ściana lub szyb
                if (hit.collider.CompareTag("WallSegment") || hit.collider.name == "SzybKopalniany")
                {
                    // Zatrzymaj się tuż przed przeszkodą
                    float maxDistance = Mathf.Max(0, hit.distance - collisionCheckDistance);
                    newPosition = rb.position + Vector2.right * moveInput * maxDistance;
                }
            }
        }
        
        // Granice poziome
        newPosition.x = Mathf.Clamp(newPosition.x, leftBound, rightBound);
        
        // Zastosuj pozycję
        rb.MovePosition(newPosition);
    }
    
    void OnDrawGizmosSelected()
    {
        // Wizualizacja granic ruchu
        Gizmos.color = Color.red;
        Gizmos.DrawLine(new Vector3(leftBound, -5, 0), new Vector3(leftBound, 5, 0));
        Gizmos.DrawLine(new Vector3(rightBound, -5, 0), new Vector3(rightBound, 5, 0));
        
        // Pokaż kierunek ruchu
        if (Application.isPlaying && moveInput != 0)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawRay(transform.position, Vector3.right * moveInput * 2f);
        }
    }
}