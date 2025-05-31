using UnityEngine;

public class PlayerAnimationController : MonoBehaviour
{
    [Header("Animation Clips")]
    [SerializeField] private AnimationClip idleAnimation;
    [SerializeField] private AnimationClip walkAnimation;
    
    [Header("Animation Settings")]
    [SerializeField] private float minSpeedForWalk = 0.1f;
    [SerializeField] private string currentAnimationDebug = "None";
    
    private Animator animator;
    private PlayerControllerFixed playerController;
    private Animation legacyAnimation;
    private bool isUsingLegacyAnimation = false;
    
    private enum AnimationState
    {
        Idle,
        Walk
    }
    
    private AnimationState currentState = AnimationState.Idle;
    
    void Start()
    {
        animator = GetComponent<Animator>();
        playerController = GetComponent<PlayerControllerFixed>();
        legacyAnimation = GetComponent<Animation>();
        
        if (animator == null && legacyAnimation == null)
        {
            legacyAnimation = gameObject.AddComponent<Animation>();
            isUsingLegacyAnimation = true;
        }
        else if (legacyAnimation != null)
        {
            isUsingLegacyAnimation = true;
        }
        
        if (isUsingLegacyAnimation && legacyAnimation != null)
        {
            if (idleAnimation != null)
            {
                legacyAnimation.AddClip(idleAnimation, "Idle");
            }
            if (walkAnimation != null)
            {
                legacyAnimation.AddClip(walkAnimation, "Walk");
            }
            
            PlayIdleAnimation();
        }
    }
    
    void Update()
    {
        if (playerController == null) return;
        
        // Używamy publicznych właściwości z PlayerControllerFixed
        bool isMoving = playerController.IsMoving;
        
        AnimationState targetState = isMoving ? AnimationState.Walk : AnimationState.Idle;
        
        if (targetState != currentState)
        {
            currentState = targetState;
            
            if (currentState == AnimationState.Idle)
            {
                PlayIdleAnimation();
            }
            else
            {
                PlayWalkAnimation();
            }
        }
    }
    
    void PlayIdleAnimation()
    {
        currentAnimationDebug = "Idle";
        
        if (isUsingLegacyAnimation && legacyAnimation != null)
        {
            if (idleAnimation != null)
            {
                legacyAnimation.CrossFade("Idle", 0.2f);
            }
        }
        else if (animator != null)
        {
            if (HasParameter("IsWalking"))
            {
                animator.SetBool("IsWalking", false);
            }
            else if (HasParameter("Speed"))
            {
                animator.SetFloat("Speed", 0f);
            }
            else
            {
                animator.Play("Idle", 0, 0f);
            }
        }
    }
    
    void PlayWalkAnimation()
    {
        currentAnimationDebug = "Walk";
        
        if (isUsingLegacyAnimation && legacyAnimation != null)
        {
            if (walkAnimation != null)
            {
                legacyAnimation.CrossFade("Walk", 0.2f);
            }
        }
        else if (animator != null)
        {
            if (HasParameter("IsWalking"))
            {
                animator.SetBool("IsWalking", true);
            }
            else if (HasParameter("Speed"))
            {
                animator.SetFloat("Speed", 1f);
            }
            else
            {
                animator.Play("Walk", 0, 0f);
            }
        }
    }
    
    bool HasParameter(string parameterName)
    {
        if (animator == null) return false;
        
        foreach (AnimatorControllerParameter param in animator.parameters)
        {
            if (param.name == parameterName)
                return true;
        }
        return false;
    }
}