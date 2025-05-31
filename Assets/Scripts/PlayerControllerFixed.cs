using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerControllerFixed : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] public float moveSpeed = 7f;
    [SerializeField] public float acceleration = 50f;
    [SerializeField] public float deceleration = 50f;
    
    [Header("Movement Bounds")]
    [SerializeField] public float leftBound = -4f;
    // Removed rightBound - let colliders handle the right side
    
    [Header("Ground Settings")]
    [SerializeField] public float groundY = -2f;
    
    [Header("Animation Settings")]
    [SerializeField] public string walkAnimationParameter = "IsWalking";
    [SerializeField] public string speedAnimationParameter = "Speed";
    [SerializeField] public float minSpeedForAnimation = 0.1f;
    
    private float currentVelocity = 0f;
    private float targetVelocity = 0f;
    private float moveInput = 0f;
    private Rigidbody2D rb;
    private BoxCollider2D boxCollider;
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private bool facingRight = true;
    
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        boxCollider = GetComponent<BoxCollider2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        
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
        
        Vector3 pos = transform.position;
        pos.y = groundY;
        transform.position = pos;
    }
    
    void Update()
    {
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
                moveInput = gamepadInput;
        }
        
        if (moveInput == 0f)
        {
            currentVelocity = Mathf.MoveTowards(currentVelocity, 0f, deceleration * Time.deltaTime);
        }
        else
        {
            targetVelocity = moveInput * moveSpeed;
            currentVelocity = Mathf.MoveTowards(currentVelocity, targetVelocity, acceleration * Time.deltaTime);
        }
        
        // Obracanie postaci w kierunku ruchu
        if (spriteRenderer != null)
        {
            if (currentVelocity > 0 && !facingRight)
            {
                Flip();
            }
            else if (currentVelocity < 0 && facingRight)
            {
                Flip();
            }
        }
        
        // Aktualizacja animacji
        UpdateAnimation();
    }
    
    void FixedUpdate()
    {
        if (rb != null)
        {
            Vector2 movement = Vector2.right * currentVelocity * Time.fixedDeltaTime;
            Vector2 newPosition = rb.position + movement;
            
            // Keep Y position locked
            newPosition.y = groundY;
            
            // Only apply left bound
            if (newPosition.x < leftBound)
            {
                newPosition.x = leftBound;
                currentVelocity = 0f;
            }
            
            rb.MovePosition(newPosition);
        }
    }
    
    void Flip()
    {
        facingRight = !facingRight;
        
        // Metoda 1: Flip przez SpriteRenderer (zalecana)
        if (spriteRenderer != null)
        {
            spriteRenderer.flipX = !facingRight;
        }
        
        // Alternatywa - Metoda 2: Flip przez skalę (zakomentowana)
        // Vector3 scale = transform.localScale;
        // scale.x *= -1;
        // transform.localScale = scale;
    }
    
    void UpdateAnimation()
    {
        if (animator == null) return;
        
        bool isMoving = Mathf.Abs(currentVelocity) > minSpeedForAnimation;
        
        // Jeśli animator ma parametr bool "IsWalking"
        if (!string.IsNullOrEmpty(walkAnimationParameter))
        {
            if (HasParameter(walkAnimationParameter))
            {
                animator.SetBool(walkAnimationParameter, isMoving);
            }
        }
        
        // Jeśli animator ma parametr float "Speed" 
        if (!string.IsNullOrEmpty(speedAnimationParameter))
        {
            if (HasParameter(speedAnimationParameter))
            {
                animator.SetFloat(speedAnimationParameter, Mathf.Abs(currentVelocity));
            }
        }
    }
    
    bool HasParameter(string parameterName)
    {
        foreach (AnimatorControllerParameter param in animator.parameters)
        {
            if (param.name == parameterName)
                return true;
        }
        return false;
    }
    
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(new Vector3(leftBound, -5, 0), new Vector3(leftBound, 5, 0));
        
        Gizmos.color = Color.green;
        Gizmos.DrawLine(new Vector3(-10, groundY, 0), new Vector3(50, groundY, 0));
    }
}