using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class PlayerControllerFixed : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] public float moveSpeed = 7f;
    [SerializeField] public float acceleration = 50f;
    [SerializeField] public float deceleration = 50f;

    [Header("Movement Bounds")]
    [SerializeField] public float leftBound = -4f;

    [Header("Ground Settings")]
    [SerializeField] public float groundY = -2f;

    [Header("Animation Settings")]
    [SerializeField] public AnimationClip idleAnimationClip;
    [SerializeField] public AnimationClip walkAnimationClip;
    [SerializeField] public float minSpeedForAnimation = 0.1f;

    public float CurrentVelocity { get { return currentVelocity; } }
    public float MoveInput { get { return moveInput; } }
    public bool IsMoving { get { return moveInput != 0 && Mathf.Abs(currentVelocity) > minSpeedForAnimation; } }

    private float currentVelocity = 0f;
    private float targetVelocity = 0f;
    private float moveInput = 0f;
    private Rigidbody2D rb;
    private BoxCollider2D boxCollider;
    private Animator animator;
    private AnimatorOverrideController overrideController;
    private SpriteRenderer spriteRenderer;
    private bool facingRight = true;
    private bool wasMoving = false;

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

        if (animator != null && animator.runtimeAnimatorController != null)
        {
            overrideController = new AnimatorOverrideController(animator.runtimeAnimatorController);
            animator.runtimeAnimatorController = overrideController;
        }
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

        UpdateAnimation();
    }

    void FixedUpdate()
    {
        if (rb != null)
        {
            Vector2 movement = Vector2.right * currentVelocity * Time.fixedDeltaTime;
            Vector2 newPosition = rb.position + movement;

            newPosition.y = groundY;

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

        if (spriteRenderer != null)
        {
            spriteRenderer.flipX = !facingRight;
        }
    }

    void UpdateAnimation()
    {
        if (animator == null || overrideController == null) return;

        bool isMoving = IsMoving;

        if (isMoving != wasMoving)
        {
            wasMoving = isMoving;

            animator.SetBool("IsWalking", isMoving);
        }
    }


    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(new Vector3(leftBound, -5, 0), new Vector3(leftBound, 5, 0));

        Gizmos.color = Color.green;
        Gizmos.DrawLine(new Vector3(-10, groundY, 0), new Vector3(50, groundY, 0));
    }
}