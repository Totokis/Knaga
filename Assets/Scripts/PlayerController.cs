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
    
    private float currentVelocity = 0f;
    private float targetVelocity = 0f;
    private float moveInput = 0f;
    private Rigidbody2D rb;
    
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            // Upewnij się że gracz nie obraca się i może poruszać się tylko w X
            rb.constraints = RigidbodyConstraints2D.FreezeRotation | RigidbodyConstraints2D.FreezePositionY;
            rb.gravityScale = 0; // Wyłącz grawitację bo gracz ma być na stałej wysokości
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
            // Calculate new position
            Vector2 newPosition = rb.position + Vector2.right * currentVelocity * Time.fixedDeltaTime;
            
            // Apply bounds
            newPosition.x = Mathf.Clamp(newPosition.x, leftBound, rightBound);
            
            // Apply the movement
            rb.MovePosition(newPosition);
        }
    }
}