using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.InputSystem;

public class LampPlacementPoint : MonoBehaviour
{
    [Header("Placement Settings")]
    [SerializeField] private float interactionDistance = 3f;
    [SerializeField] public bool hasLampInstalled = false;
    
    [Header("Visual Settings")]
    [SerializeField] private float emptyPlaceholderAlpha = 0.5f;
    [SerializeField] private float installedAlpha = 1f;
    
    [Header("References")]
    private GameObject lampObject;
    private SpriteRenderer spriteRenderer;
    private Light2D lampLight;
    private PlayerInventory playerInventory;
    private GameObject player;
    private PlayerMessageDisplay messageDisplay;
    
    private bool playerInRange = false;

    void Start()
    {
        Debug.LogWarning($"[LampPlacementPoint] Starting initialization for {gameObject.name}");
        
        player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)
        {
            Debug.LogError("[LampPlacementPoint] No GameObject with tag 'Player' found!");
        }
        
        playerInventory = player?.GetComponent<PlayerInventory>();
        messageDisplay = PlayerMessageDisplay.Instance;
        
        // Find components
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            Debug.LogError($"[LampPlacementPoint] No SpriteRenderer found on {gameObject.name}! This component is required!");
        }
        
        lampLight = GetComponentInChildren<Light2D>();
        if (lampLight == null)
        {
            Debug.LogWarning($"[LampPlacementPoint] No Light2D found in children of {gameObject.name}");
        }
        
        // Find lamp object (first child if exists)
        if (transform.childCount > 0)
        {
            lampObject = transform.GetChild(0).gameObject;
            Debug.LogWarning($"[LampPlacementPoint] Found child object: {lampObject.name}");
        }
        else
        {
            Debug.LogWarning($"[LampPlacementPoint] No child objects found on {gameObject.name}");
        }
        
        // Set initial state based on whether lamp is installed
        UpdateLampState();
        
        Debug.LogWarning($"[LampPlacementPoint] {gameObject.name} initialized. Has lamp: {hasLampInstalled}, Alpha: {(spriteRenderer != null ? spriteRenderer.color.a : -1)}, Light enabled: {(lampLight != null ? lampLight.enabled : false)}");
    }

    void Update()
    {
        if (player == null) return;

        // Check only horizontal distance (ignore Y difference)
        float horizontalDistance = Mathf.Abs(transform.position.x - player.transform.position.x);
        bool wasInRange = playerInRange;
        playerInRange = horizontalDistance <= interactionDistance;
        
        // Debug distance check
        if (!hasLampInstalled && horizontalDistance <= interactionDistance * 2)
        {
            Debug.LogWarning($"[LampPlacementPoint] {gameObject.name} - Player horizontal distance: {horizontalDistance:F2}, In range: {playerInRange}");
        }

        // Handle showing/hiding interaction prompt
        if (!hasLampInstalled && playerInRange && !wasInRange)
        {
            Debug.LogWarning($"[LampPlacementPoint] Player entered range of {gameObject.name}");
            // Check if player has a bulb
            if (playerInventory != null && playerInventory.HasItem("Bulb") && messageDisplay != null)
            {
                Debug.LogWarning($"[LampPlacementPoint] Player has Bulb, showing interaction prompt");
                messageDisplay.ShowInteraction("Press E to install lamp");
            }
            else
            {
                Debug.LogWarning($"[LampPlacementPoint] Player doesn't have Bulb or missing components. HasBulb={playerInventory?.HasItem("Bulb")}, MessageDisplay={messageDisplay != null}");
            }
        }
        else if (!playerInRange && wasInRange)
        {
            // Message will disappear automatically after duration
        }

        // Handle interaction
        if (playerInRange && !hasLampInstalled && Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame)
        {
            if (playerInventory != null && playerInventory.HasItem("Bulb"))
            {
                InstallLamp();
            }
        }
    }

    private void InstallLamp()
    {
        // Remove bulb from inventory
        if (playerInventory.RemoveItem("Bulb", 1))
        {
            hasLampInstalled = true;
            UpdateLampState();
            
            if (messageDisplay != null)
            {
                messageDisplay.ShowMessage("Lamp installed!", Color.green, 2f);
            }
            
            // Play sound effect if available
            AudioSource audioSource = GetComponent<AudioSource>();
            if (audioSource != null)
            {
                audioSource.Play();
            }
        }
    }

    private void UpdateLampState()
    {
        Debug.LogWarning($"[LampPlacementPoint] UpdateLampState called for {gameObject.name}. hasLampInstalled={hasLampInstalled}");
        
        if (hasLampInstalled)
        {
            // Lamp is installed - full opacity and light on
            if (spriteRenderer != null)
            {
                Color color = spriteRenderer.color;
                color.a = installedAlpha;
                spriteRenderer.color = color;
                Debug.LogWarning($"[LampPlacementPoint] Set {gameObject.name} to INSTALLED state. Alpha={installedAlpha}");
            }
            
            if (lampLight != null)
            {
                lampLight.enabled = true;
            }
        }
        else
        {
            // No lamp installed - half transparency and light off
            if (spriteRenderer != null)
            {
                Color color = spriteRenderer.color;
                color.a = emptyPlaceholderAlpha;
                spriteRenderer.color = color;
                Debug.LogWarning($"[LampPlacementPoint] Set {gameObject.name} to PLACEHOLDER state. Alpha={emptyPlaceholderAlpha}");
            }
            
            if (lampLight != null)
            {
                lampLight.enabled = false;
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        // Draw interaction range in editor
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactionDistance);
    }
}