using UnityEngine;
using UnityEngine.InputSystem;

public class ExchangeStation : MonoBehaviour
{
    [Header("Settings")]
    public float interactionRange = 3f;
    
    private Transform player;
    private PlayerMessageDisplay messageDisplay;
    private ExchangeMenuController menuController;
    private bool isInRange = false;
    private bool menuOpen = false;
    
    void Start()
    {
        GameObject p = GameObject.Find("Player");
        if (p != null)
        {
            player = p.transform;
        }
        
        messageDisplay = PlayerMessageDisplay.Instance;
        menuController = GetComponent<ExchangeMenuController>();
    }
    
    void Update()
    {
        if (player == null || Keyboard.current == null) return;
        
        float dist = Vector2.Distance(transform.position, player.position);
        bool wasInRange = isInRange;
        isInRange = dist <= interactionRange;
        
        // Show prompt when entering range
        if (isInRange && !wasInRange && messageDisplay != null)
        {
            messageDisplay.ShowInteraction("Press E to open Exchange Station");
        }
        
        if (isInRange && Keyboard.current.eKey.wasPressedThisFrame)
        {
            menuOpen = !menuOpen;
            if (menuOpen)
            {
                if (menuController != null)
                    menuController.ShowMenu();
            }
            else
            {
                if (menuController != null)
                    menuController.CloseMenu();
            }
        }
    }
    
    public void SetMenuOpen(bool isOpen)
    {
        menuOpen = isOpen;
    }
    
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, interactionRange);
    }
}