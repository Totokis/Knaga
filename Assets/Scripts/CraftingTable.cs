using UnityEngine;
using UnityEngine.InputSystem;

public class CraftingTable : MonoBehaviour
{
    [Header("Settings")]
    public float interactionRange = 3f;

    public Item CurrentItem;
    private Transform player;
    private PlayerMessageDisplay messageDisplay;
    private bool isInRange = false;

    void Start()
    {
        GameObject p = GameObject.Find("Player");
        if (p != null)
        {
            player = p.transform;
        }

        messageDisplay = PlayerMessageDisplay.Instance;
        // menuController = GetComponent<ExchangeMenuController>();
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
            messageDisplay.ShowInteraction("Press E to put item to fusion");
        }

        if (isInRange && Keyboard.current.eKey.wasPressedThisFrame)
        {
            // TODO: Fix this - GetCurrentItem() method doesn't exist in PlayerInventory
            // Item newItem = player.GetComponent<PlayerInventory>().GetCurrentItem();
            // if(newItem != null)
            // {
            //     if (CurrentItem != null)
            //     {
            //         // CRAFT
            //     }
            //     else
            //         CurrentItem = newItem;
            // }
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, interactionRange);
    }
}
