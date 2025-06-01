using UnityEngine;

public class FuelBar : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float totalDuration = 100f; // Total time to deplete fuel in seconds
    [SerializeField] private bool startDepleting = true; // Start depleting on start
    
    [Header("Visual")]
    [SerializeField] private SpriteRenderer fuelBarSprite; // The sprite renderer for the fuel bar
    [SerializeField] private Color fullColor = Color.green;
    [SerializeField] private Color emptyColor = Color.red;
    
    private float currentFuel = 1f; // 1 = full, 0 = empty
    private float initialScaleX;
    private float initialWidth;
    private Vector3 initialPosition;
    private bool isDepleting = false;
    
    void Start()
    {
        // Get sprite renderer if not assigned
        if (fuelBarSprite == null)
        {
            fuelBarSprite = GetComponent<SpriteRenderer>();
            if (fuelBarSprite == null)
            {
                Debug.LogError("[FuelBar] No SpriteRenderer found!");
                return;
            }
        }
        
        // Store initial values
        initialScaleX = transform.localScale.x;
        initialPosition = transform.position;
        
        // Calculate initial width (sprite width * scale)
        if (fuelBarSprite.sprite != null)
        {
            initialWidth = fuelBarSprite.sprite.bounds.size.x * initialScaleX;
        }
        else
        {
            initialWidth = initialScaleX; // fallback
        }
        
        // Start depleting if enabled
        if (startDepleting)
        {
            StartDepleting();
        }
        
        UpdateVisual();
    }
    
    void Update()
    {
        if (isDepleting && currentFuel > 0)
        {
            // Decrease fuel over time
            currentFuel -= Time.deltaTime / totalDuration;
            currentFuel = Mathf.Clamp01(currentFuel);
            
            UpdateVisual();
            
            // Check if fuel is empty
            if (currentFuel <= 0)
            {
                OnFuelEmpty();
            }
        }
    }
    
    void UpdateVisual()
    {
        if (fuelBarSprite == null) return;
        
        // Update scale (shrink from right to left)
        Vector3 newScale = transform.localScale;
        newScale.x = initialScaleX * currentFuel;
        fuelBarSprite.transform.localScale = newScale;
        
        // Move position to keep left side fixed
        float widthDifference = initialWidth * (1f - currentFuel);
        Vector3 newPosition = initialPosition;
        newPosition.x = initialPosition.x - (widthDifference * 0.5f);
        fuelBarSprite.transform.position = newPosition;
        
        // Update color based on fuel level
        fuelBarSprite.color = Color.Lerp(emptyColor, fullColor, currentFuel);
    }
    
    public void StartDepleting()
    {
        isDepleting = true;
        Debug.Log("[FuelBar] Started depleting fuel");
    }
    
    public void StopDepleting()
    {
        isDepleting = false;
        Debug.Log("[FuelBar] Stopped depleting fuel");
    }
    
    public void RefillFuel(float amount = 1f)
    {
        currentFuel = Mathf.Clamp01(currentFuel + amount);
        UpdateVisual();
        Debug.Log($"[FuelBar] Refilled fuel. Current level: {currentFuel * 100}%");
    }
    
    public void SetFuelLevel(float level)
    {
        currentFuel = Mathf.Clamp01(level);
        UpdateVisual();
    }
    
    public float GetFuelLevel()
    {
        return currentFuel;
    }
    
    public float GetFuelPercentage()
    {
        return currentFuel * 100f;
    }
    
    void OnFuelEmpty()
    {
        Debug.LogWarning("[FuelBar] Fuel is empty!");
        isDepleting = false;
        
        // You can add additional logic here
        // For example: stop machine, show warning, etc.
    }
    
    // Debug methods
    [ContextMenu("Refill Fuel")]
    void DebugRefillFuel()
    {
        RefillFuel(1f);
    }
    
    [ContextMenu("Empty Fuel")]
    void DebugEmptyFuel()
    {
        SetFuelLevel(0f);
    }
    
    [ContextMenu("Set Half Fuel")]
    void DebugSetHalfFuel()
    {
        SetFuelLevel(0.5f);
    }
}