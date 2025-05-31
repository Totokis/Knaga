using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class ItemPickup : MonoBehaviour
{
    [Header("Item Settings")]
    public Item itemData = new Item("Ore", 1);
    
    [Header("Pickup Settings")]
    public float pickupRange = 2f;
    
    private Transform player;
    private bool isInRange = false;
    private SpriteRenderer spriteRenderer;
    
    void Start()
    {
        GameObject playerObj = GameObject.Find("Player");
        if (playerObj != null)
            player = playerObj.transform;
        
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
        }
        
        // Load sprite
        Sprite whiteSquare = Resources.Load<Sprite>("whitesquare");
        if (whiteSquare == null)
        {
            // Try to load from assets
            UnityEngine.Object[] sprites = UnityEditor.AssetDatabase.LoadAllAssetsAtPath("Assets/Sprites/whitesquare.png");
            if (sprites.Length > 0)
            {
                foreach (var s in sprites)
                {
                    if (s is Sprite)
                    {
                        whiteSquare = s as Sprite;
                        break;
                    }
                }
            }
        }
        
        if (whiteSquare != null)
            spriteRenderer.sprite = whiteSquare;
        
        spriteRenderer.color = itemData.color;
        
        CircleCollider2D col = GetComponent<CircleCollider2D>();
        if (col == null)
        {
            col = gameObject.AddComponent<CircleCollider2D>();
            col.radius = 0.3f;
            col.isTrigger = true;
        }
    }
    
    void Update()
    {
        if (player == null) return;
        
        float distance = Vector2.Distance(transform.position, player.position);
        bool wasInRange = isInRange;
        isInRange = distance <= pickupRange;
        
        if (isInRange != wasInRange)
        {
            if (isInRange)
                Debug.Log($"Press E to pick up {itemData.itemName}");
        }
        
        if (isInRange && Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame)
        {
            TryPickup();
        }
    }
    
    void TryPickup()
    {
        PlayerInventory inventory = PlayerInventory.Instance;
        if (inventory != null)
        {
            if (inventory.AddItem(itemData))
            {
                Debug.Log($"Picked up {itemData.amount} {itemData.itemName}");
                Destroy(gameObject);
            }
            else
            {
                Debug.Log("Inventory full or incompatible item!");
            }
        }
    }
    
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, pickupRange);
    }
}