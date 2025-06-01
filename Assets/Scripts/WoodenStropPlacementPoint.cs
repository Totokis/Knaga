using UnityEngine;
using UnityEngine.InputSystem;

public class WoodenStropPlacementPoint : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float interactionDistance = 3f;
    [SerializeField] public bool hasStropInstalled = false;
    [SerializeField] private float emptyAlpha = 0.14f;
    [SerializeField] private float installedAlpha = 1f;
    
    private SpriteRenderer spriteRenderer;
    private PlayerInventory playerInventory;
    private GameObject player;
    private PlayerMessageDisplay messageDisplay;
    private bool playerInRange = false;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        playerInventory = player?.GetComponent<PlayerInventory>();
        messageDisplay = PlayerMessageDisplay.Instance;
        spriteRenderer = GetComponent<SpriteRenderer>();
        
        UpdateStropState();
    }

    void Update()
    {
        if (player == null) return;

        float horizontalDistance = Mathf.Abs(transform.position.x - player.transform.position.x);
        bool wasInRange = playerInRange;
        playerInRange = horizontalDistance <= interactionDistance;

        if (!hasStropInstalled && playerInRange && !wasInRange)
        {
            if (playerInventory != null && playerInventory.HasItem("Wooden Strop") && messageDisplay != null)
            {
                messageDisplay.ShowPickupSprite(); // Use existing sprite
            }
        }

        if (playerInRange && !hasStropInstalled && Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame)
        {
            if (playerInventory != null && playerInventory.HasItem("Wooden Strop"))
            {
                InstallStrop();
            }
        }
    }

    private void InstallStrop()
    {
        if (playerInventory.RemoveItem("Wooden Strop", 1))
        {
            hasStropInstalled = true;
            UpdateStropState();
        }
    }

    private void UpdateStropState()
    {
        if (spriteRenderer != null)
        {
            Color color = spriteRenderer.color;
            color.a = hasStropInstalled ? installedAlpha : emptyAlpha;
            spriteRenderer.color = color;
        }
    }
}