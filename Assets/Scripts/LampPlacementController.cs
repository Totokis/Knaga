using UnityEngine;
using System.Collections.Generic;
using UnityEngine.InputSystem;

public class LampPlacementController : MonoBehaviour
{
    [Header("Placement Settings")]
    [SerializeField] private float placementInterval = 5f; // Distance between lamp placement points
    [SerializeField] private float maxPlacementDistance = 3f; // Max distance from player to place lamp
    [SerializeField] private float ceilingCheckDistance = 10f; // How far up to check for ceiling
    [SerializeField] private LayerMask ceilingLayer = -1; // Layer mask for ceiling detection
    
    [Header("Prefabs")]
    [SerializeField] private GameObject lampPlacementPointPrefab; // Prefab with LampPlacementPoint script
    [SerializeField] private GameObject lampPreviewPrefab; // Semi-transparent preview
    
    [Header("Visual Settings")]
    [SerializeField] private Color validPlacementColor = new Color(0, 1, 0, 0.5f);
    [SerializeField] private Color invalidPlacementColor = new Color(1, 0, 0, 0.5f);
    
    private PlayerInventory playerInventory;
    private PlayerMessageDisplay messageDisplay;
    private GameObject previewObject;
    private SpriteRenderer previewRenderer;
    private bool isPreviewMode = false;
    private Vector2 lastValidPosition;
    private HashSet<Vector2> existingLampPositions = new HashSet<Vector2>();

    public Sprite lampSprite;
    
    void Start()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        playerInventory = player?.GetComponent<PlayerInventory>();
        messageDisplay = PlayerMessageDisplay.Instance;
        
        // Find all existing lamp placement points
        LampPlacementPoint[] existingPoints = FindObjectsOfType<LampPlacementPoint>();
        foreach (var point in existingPoints)
        {
            Vector2 snappedPos = GetSnappedPosition(point.transform.position);
            existingLampPositions.Add(snappedPos);
        }
    }
    
    void Update()
    {
        if (Keyboard.current == null || Mouse.current == null) return;
        
        // Toggle preview mode with Tab key
        if (Keyboard.current.tabKey.wasPressedThisFrame)
        {
            if (playerInventory != null && playerInventory.HasItem("Bulb"))
            {
                TogglePreviewMode();
            }
        }
        
        if (isPreviewMode)
        {
            UpdatePreview();
            
            // Place lamp with left click
            if (Mouse.current.leftButton.wasPressedThisFrame && IsValidPlacement(lastValidPosition))
            {
                PlaceLamp(lastValidPosition);
            }
            
            // Cancel with right click or Escape
            if (Mouse.current.rightButton.wasPressedThisFrame || Keyboard.current.escapeKey.wasPressedThisFrame)
            {
                TogglePreviewMode();
            }
        }
    }
    
    private void TogglePreviewMode()
    {
        isPreviewMode = !isPreviewMode;
        
        if (isPreviewMode)
        {
            // Create preview object
            if (lampPreviewPrefab != null)
            {
                previewObject = Instantiate(lampPreviewPrefab);
            }
            else if (lampPlacementPointPrefab != null)
            {
                previewObject = Instantiate(lampPlacementPointPrefab);
                // Disable scripts on preview
                LampPlacementPoint placementScript = previewObject.GetComponent<LampPlacementPoint>();
                if (placementScript != null) placementScript.enabled = false;
            }
            
            if (previewObject != null)
            {
                previewRenderer = previewObject.GetComponent<SpriteRenderer>();
                if (previewRenderer != null)
                {
                    previewRenderer.color = invalidPlacementColor;
                }
            }
            
            if (messageDisplay != null)
            {
                messageDisplay.ShowSprite(lampSprite);
            }
        }
        else
        {
            // Destroy preview
            if (previewObject != null)
            {
                Destroy(previewObject);
            }
            // Message will disappear automatically
        }
    }
    
    private void UpdatePreview()
    {
        if (previewObject == null) return;
        
        // Get mouse position in world space
        Vector2 mouseScreenPos = Mouse.current.position.ReadValue();
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(new Vector3(mouseScreenPos.x, mouseScreenPos.y, Camera.main.nearClipPlane));
        mousePos.z = 0;
        
        // Snap to grid
        Vector2 snappedPos = GetSnappedPosition(mousePos);
        
        // Check if position is valid
        bool isValid = IsValidPlacement(snappedPos);
        
        // Update preview position and color
        previewObject.transform.position = new Vector3(snappedPos.x, snappedPos.y, 0);
        
        if (previewRenderer != null)
        {
            previewRenderer.color = isValid ? validPlacementColor : invalidPlacementColor;
        }
        
        if (isValid)
        {
            lastValidPosition = snappedPos;
        }
    }
    
    private Vector2 GetSnappedPosition(Vector3 worldPos)
    {
        float snappedX = Mathf.Round(worldPos.x / placementInterval) * placementInterval;
        return new Vector2(snappedX, worldPos.y);
    }
    
    private bool IsValidPlacement(Vector2 position)
    {
        // Check distance from player
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            float distance = Vector2.Distance(position, player.transform.position);
            if (distance > maxPlacementDistance)
            {
                return false;
            }
        }
        
        // Check if there's already a lamp at this position
        if (existingLampPositions.Contains(position))
        {
            return false;
        }
        
        // Check if there's a ceiling above
        RaycastHit2D hit = Physics2D.Raycast(position, Vector2.up, ceilingCheckDistance, ceilingLayer);
        if (hit.collider == null)
        {
            return false;
        }
        
        return true;
    }
    
    private void PlaceLamp(Vector2 position)
    {
        if (playerInventory != null && playerInventory.RemoveItem("Bulb", 1))
        {
            // Create lamp placement point
            GameObject newLamp = Instantiate(lampPlacementPointPrefab, new Vector3(position.x, position.y, 0), Quaternion.identity);

            // Mark it as already having a lamp installed
            LampPlacementPoint lampPoint = newLamp.GetComponent<LampPlacementPoint>();
            if (lampPoint != null)
            {
                lampPoint.enabled = true;
                // The lamp will be installed through the normal interaction system
            }

            // Add to existing positions
            existingLampPositions.Add(position);

            // Exit preview mode
            TogglePreviewMode();
        }
    }
    
    void OnDrawGizmos()
    {
        if (!isPreviewMode) return;
        
        // Draw placement grid lines
        Gizmos.color = Color.yellow * 0.3f;
        float gridSize = 50f;
        
        for (float x = -gridSize; x <= gridSize; x += placementInterval)
        {
            Gizmos.DrawLine(new Vector3(x, -gridSize, 0), new Vector3(x, gridSize, 0));
        }
    }
}