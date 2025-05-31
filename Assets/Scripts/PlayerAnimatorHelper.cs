using UnityEngine;

public class PlayerAnimatorHelper : MonoBehaviour
{
    private Animator animator;
    
    void Awake()
    {
        animator = GetComponent<Animator>();
        
        if (animator != null)
        {
            // Logowanie informacji o parametrach animatora
            Debug.Log("Player Animator Helper: Checking animator parameters...");
            
            // Sprawdź czy parametr IsTaking jest dostępny
            // W runtime nie możemy dodać parametru, ale możemy sprawdzić jego istnienie
            #if UNITY_EDITOR
            var controller = animator.runtimeAnimatorController as UnityEditor.Animations.AnimatorController;
            if (controller != null)
            {
                bool hasIsTaking = false;
                foreach (var param in controller.parameters)
                {
                    if (param.name == "IsTaking")
                    {
                        hasIsTaking = true;
                        break;
                    }
                }
                
                if (!hasIsTaking)
                {
                    Debug.LogWarning("IsTaking parameter not found in animator! Please add it manually in the Animator window.");
                }
                else
                {
                    Debug.Log("IsTaking parameter found in animator.");
                }
            }
            #endif
        }
    }
    
    // Pomocnicze metody do ustawiania animacji
    public void SetTakingAnimation(bool isTaking)
    {
        if (animator != null)
        {
            animator.SetBool("IsTaking", isTaking);
        }
    }
    
    public void TriggerTakeAnimation()
    {
        StartCoroutine(PlayTakeAnimation());
    }
    
    private System.Collections.IEnumerator PlayTakeAnimation()
    {
        SetTakingAnimation(true);
        yield return new WaitForSeconds(0.5f);
        SetTakingAnimation(false);
    }
}