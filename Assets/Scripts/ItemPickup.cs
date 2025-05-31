using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using System.Collections;

public class ItemPickup : MonoBehaviour
{
    [Header("Item Settings")]
    public Item itemData = new Item("Ore", 1);
    
    [Header("Pickup Settings")]
    public float pickupRange = 2f;
    public float pickupAnimationDuration = 0.5f;
    
    private Transform player;
    private bool isInRange = false;
    private bool isPickingUp = false;
    private SpriteRenderer spriteRenderer;
    private PlayerMessageDisplay messageDisplay;
    private PlayerAnimatorHelper animatorHelper;
    
    void Start()
    {
        GameObject playerObj = GameObject.Find("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
            animatorHelper = playerObj.GetComponent<PlayerAnimatorHelper>();
        }
        
        // Znajdź system komunikatów
        messageDisplay = PlayerMessageDisplay.Instance;
        
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
        if (player == null || isPickingUp) return;
        
        float distance = Vector2.Distance(transform.position, player.position);
        bool wasInRange = isInRange;
        isInRange = distance <= pickupRange;
        
        if (isInRange != wasInRange)
        {
            if (isInRange && messageDisplay != null)
            {
                messageDisplay.ShowPickupPrompt(itemData.itemName);
            }
        }
        
        if (isInRange && Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame)
        {
            StartCoroutine(PickupWithAnimation());
        }
    }
    
    IEnumerator PickupWithAnimation()
    {
        isPickingUp = true;
        
        // Rozpocznij animację podnoszenia
        if (animatorHelper != null)
        {
            animatorHelper.SetTakingAnimation(true);
        }
        
        // Poczekaj na animację
        yield return new WaitForSeconds(pickupAnimationDuration);
        
        // Wykonaj właściwe podniesienie
        TryPickup();
        
        // Zakończ animację podnoszenia
        if (animatorHelper != null)
        {
            animatorHelper.SetTakingAnimation(false);
        }
        
        isPickingUp = false;
    }
    
    void TryPickup()
    {
        PlayerInventory inventory = PlayerInventory.Instance;
        if (inventory != null)
        {
            if (inventory.AddItem(itemData))
            {
                if (messageDisplay != null)
                {
                    messageDisplay.ShowItemPickedUp(itemData.itemName, itemData.amount);
                }
                else
                {
                    Debug.Log($"Picked up {itemData.amount} {itemData.itemName}");
                }
                Destroy(gameObject);
            }
            else
            {
                if (messageDisplay != null)
                {
                    messageDisplay.ShowInventoryFull();
                }
                else
                {
                    Debug.Log("Inventory full or incompatible item!");
                }
            }
        }
    }
    
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, pickupRange);
    }
}