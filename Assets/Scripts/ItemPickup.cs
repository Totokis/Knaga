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

        // Użyj sprite'a z itemData.icon jeśli jest dostępny
        if (itemData.icon != null)
        {
            spriteRenderer.sprite = itemData.icon;
            Debug.Log($"Using itemData.icon for {itemData.itemName}: {itemData.icon.name}");
        }

        if (itemData.icon == null)
        {
            spriteRenderer.color = itemData.color;
        }

        transform.localScale = new Vector3(0.4f, 0.4f, 0.4f);

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
                messageDisplay.ShowPickupSprite();
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
                    messageDisplay.ShowPickupSprite();
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
                    //messageDisplay.ShowInventoryFull();
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