using UnityEngine;
using UnityEngine.InputSystem;

public class CheatController : MonoBehaviour
{
    private PlayerInventory playerInventory;
    private ItemSpriteManager itemSpriteManager;
    private bool yKeyWasPressed = false;
    private bool uKeyWasPressed = false;
    
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
        
        // Check U key for Wooden Strop
        bool uKeyIsPressed = Keyboard.current.uKey.isPressed;
        
        if (uKeyIsPressed && !uKeyWasPressed)
        {
            AddWoodenStropToInventory();
        }
        
        uKeyWasPressed = uKeyIsPressed;
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
        bulb.icon = itemSpriteManager.Bulbitemsprite;
        bulb.color = Color.yellow;
        
        Debug.LogWarning($"[CheatController] Created item: Name={bulb.itemName}, Type={bulb.itemType}, Amount={bulb.amount}, HasIcon={bulb.icon != null}");
        
        if (playerInventory.AddItem(bulb))
        {
            Debug.LogWarning("[CheatController] Successfully added Bulb to inventory");
            PlayerMessageDisplay messageDisplay = PlayerMessageDisplay.Instance;
        }
        else
        {
            Debug.LogError("[CheatController] Failed to add Bulb to inventory");
        }
    }
    
    private void AddWoodenStropToInventory()
    {
        Debug.LogWarning("[CheatController] Attempting to add Wooden Strop to inventory");
        
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
        
        // Create wooden strop item
        Item strop = new Item("Wooden Strop", 1, ItemType.WoodenStrop);
        strop.icon = itemSpriteManager.WoodenStropSprite;
        strop.color = new Color(0.6f, 0.4f, 0.2f);
        
        Debug.LogWarning($"[CheatController] Created item: Name={strop.itemName}, Type={strop.itemType}, Amount={strop.amount}, HasIcon={strop.icon != null}");
        
        if (playerInventory.AddItem(strop))
        {
            Debug.LogWarning("[CheatController] Successfully added Wooden Strop to inventory");
            PlayerMessageDisplay messageDisplay = PlayerMessageDisplay.Instance;
            if (messageDisplay != null)
            {
                messageDisplay.ShowPickupSprite();
            }
        }
        else
        {
            Debug.LogError("[CheatController] Failed to add Wooden Strop to inventory");
        }
    }
}