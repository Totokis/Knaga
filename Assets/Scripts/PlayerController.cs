using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float acceleration = 10f;
    public float deceleration = 10f;
    
    [Header("Movement Bounds")]
    public float leftBound = -10f;
    public float rightBound = 20f;
    
    private float currentVelocity = 0f;
    private float targetVelocity = 0f;
    private float moveInput = 0f;
    private Rigidbody2D rb;
    
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
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
        // Calculate new position
        Vector2 newPosition = rb.position + Vector2.right * currentVelocity * Time.fixedDeltaTime;
        
        // Apply bounds
        newPosition.x = Mathf.Clamp(newPosition.x, leftBound, rightBound);
        
        // Apply the movement
        rb.MovePosition(newPosition);
    }
}