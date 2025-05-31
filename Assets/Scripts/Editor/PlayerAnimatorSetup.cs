using UnityEngine;
using UnityEditor;

#if UNITY_EDITOR
[InitializeOnLoad]
public static class PlayerAnimatorSetup
{
    static PlayerAnimatorSetup()
    {
        EditorApplication.delayCall += SetupPlayerAnimator;
    }
    
    [MenuItem("Tools/Setup Player Animator")]
    static void SetupPlayerAnimator()
    {
        GameObject player = GameObject.Find("Player");
        if (player == null)
        {
            Debug.LogWarning("Player object not found!");
            return;
        }
        
        Animator animator = player.GetComponent<Animator>();
        if (animator == null || animator.runtimeAnimatorController == null)
        {
            Debug.LogWarning("Player has no animator or animator controller!");
            return;
        }
        
        // Pobierz kontroler animatora
        var controller = animator.runtimeAnimatorController as UnityEditor.Animations.AnimatorController;
        if (controller == null)
        {
            Debug.LogWarning("Could not access animator controller!");
            return;
        }
        
        // Sprawdź czy parametr IsTaking już istnieje
        bool hasTakingParam = false;
        foreach (var param in controller.parameters)
        {
            if (param.name == "IsTaking")
            {
                hasTakingParam = true;
                break;
            }
        }
        
        // Dodaj parametr jeśli nie istnieje
        if (!hasTakingParam)
        {
            controller.AddParameter("IsTaking", AnimatorControllerParameterType.Bool);
            Debug.Log("Added IsTaking parameter to player animator");
        }
        else
        {
            Debug.Log("IsTaking parameter already exists");
        }
        
        // Zapisz zmiany
        EditorUtility.SetDirty(controller);
        AssetDatabase.SaveAssets();
        
        Debug.Log("Player animator setup completed!");
    }
}
#endif