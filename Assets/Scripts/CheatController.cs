using UnityEngine;
using UnityEngine.InputSystem;

public class CheatController : MonoBehaviour
{
    private PlayerInventory playerInventory;
    private ItemSpriteManager itemSpriteManager;
    private bool yKeyWasPressed = false;
    
    void Start()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerInventory = player.GetComponent<PlayerInventory>();
        }
        
        // Find ItemSpriteManager
        itemSpriteManager = FindObjectOfType<ItemSpriteManager>();
    }
    
    void Update()
    {
        if (Keyboard.current == null) return;
        
        // Check Y key with proper debouncing
        bool yKeyIsPressed = Keyboard.current.yKey.isPressed;
        
        if (yKeyIsPressed && !yKeyWasPressed)
        {
            // Key was just pressed down
            AddBulbToInventory();
        }
        
        yKeyWasPressed = yKeyIsPressed;
    }
    
    private void AddBulbToInventory()
    {
        Debug.LogWarning("[CheatController] Attempting to add Bulb to inventory");
        
        if (playerInventory == null)
        {
            Debug.LogError("[CheatController] PlayerInventory is null!");
            return;
        }
        
        if (itemSpriteManager == null)
        {
            Debug.LogError("[CheatController] ItemSpriteManager is null!");
            return;
        }
        
        // Create bulb item
        Item bulb = new Item("Bulb", 1, ItemType.Bulb);
        bulb.icon = itemSpriteManager.BulbSprite;
        bulb.color = Color.yellow;
        
        Debug.LogWarning($"[CheatController] Created item: Name={bulb.itemName}, Type={bulb.itemType}, Amount={bulb.amount}, HasIcon={bulb.icon != null}");
        
        if (playerInventory.AddItem(bulb))
        {
            Debug.LogWarning("[CheatController] Successfully added Bulb to inventory");
            PlayerMessageDisplay messageDisplay = PlayerMessageDisplay.Instance;
            if (messageDisplay != null)
            {
                messageDisplay.ShowMessage("Cheat: Added Bulb to inventory!", Color.magenta, 1.5f);
            }
        }
        else
        {
            Debug.LogError("[CheatController] Failed to add Bulb to inventory");
        }
    }
}