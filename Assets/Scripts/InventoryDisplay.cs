using UnityEngine;
using TMPro;
using System.Linq;

public class InventoryDisplay : MonoBehaviour
{
    [Header("Display Settings")]
    public Vector2 screenOffset = new Vector2(50f, 50f); // Offset od lewego dolnego rogu
    public float textSize = 24f;
    public Color textColor = Color.white;
    public Color backgroundColor = new Color(0, 0, 0, 0.8f);
    
    private TextMeshPro inventoryText;
    private GameObject background;
    private Camera mainCamera;
    private PlayerInventory inventory;
    
    void Start()
    {
        mainCamera = Camera.main;
        inventory = PlayerInventory.Instance;
        
        SetupDisplay();
        UpdateDisplay();
    }
    
    void SetupDisplay()
    {
        // Tworzenie obiektu dla tekstu
        GameObject textObj = new GameObject("InventoryText");
        textObj.transform.parent = transform;
        
        // Dodanie TextMeshPro
        inventoryText = textObj.AddComponent<TextMeshPro>();
        inventoryText.fontSize = textSize;
        inventoryText.color = textColor;
        inventoryText.alignment = TextAlignmentOptions.BottomLeft;
        inventoryText.text = "Inventory:\nEmpty";
        
        // Ustawienie sortowania żeby było na wierzchu
        inventoryText.sortingOrder = 1000;
        MeshRenderer textRenderer = inventoryText.GetComponent<MeshRenderer>();
        if (textRenderer != null)
        {
            textRenderer.sortingLayerName = "UI";
            textRenderer.sortingOrder = 1000;
        }
        
        // Tworzenie tła
        background = GameObject.CreatePrimitive(PrimitiveType.Quad);
        background.name = "InventoryBackground";
        background.transform.parent = transform;
        
        // Usunięcie collidera
        Destroy(background.GetComponent<Collider>());
        
        // Ustawienie materiału i koloru
        MeshRenderer bgRenderer = background.GetComponent<MeshRenderer>();
        bgRenderer.material = new Material(Shader.Find("Sprites/Default"));
        bgRenderer.material.color = backgroundColor;
        bgRenderer.sortingLayerName = "UI";
        bgRenderer.sortingOrder = 999;
        
        // Skalowanie tła
        background.transform.localScale = new Vector3(4f, 2f, 1f);
    }
    
    void Update()
    {
        if (mainCamera == null || inventoryText == null) return;
        
        // Pozycjonowanie w lewym dolnym rogu
        Vector3 screenPos = new Vector3(screenOffset.x, screenOffset.y, 10f);
        Vector3 worldPos = mainCamera.ScreenToWorldPoint(screenPos);
        transform.position = new Vector3(worldPos.x, worldPos.y, 0f);
        
        // Aktualizacja zawartości co kilka klatek
        if (Time.frameCount % 30 == 0)
        {
            UpdateDisplay();
        }
    }
    
    void UpdateDisplay()
    {
        if (inventory == null || inventoryText == null) return;
        
        string displayText = "INVENTORY:\n";
        
        if (inventory.items.Count == 0)
        {
            displayText += "Empty";
        }
        else
        {
            // Wyświetlamy pierwsze 5 przedmiotów
            int itemsToShow = Mathf.Min(inventory.items.Count, 5);
            for (int i = 0; i < itemsToShow; i++)
            {
                var item = inventory.items[i];
                displayText += $"{item.itemName} x{item.amount}\n";
            }
            
            if (inventory.items.Count > 5)
            {
                displayText += $"... and {inventory.items.Count - 5} more";
            }
        }
        
        inventoryText.text = displayText;
        
        // Dopasowanie rozmiaru tła
        if (background != null)
        {
            float textWidth = inventoryText.preferredWidth * 0.1f;
            float textHeight = inventoryText.preferredHeight * 0.1f;
            background.transform.localScale = new Vector3(
                textWidth + 0.5f,
                textHeight + 0.5f,
                1f
            );
            background.transform.localPosition = new Vector3(
                textWidth * 0.5f - 0.25f,
                textHeight * 0.5f - 0.25f,
                0.1f
            );
        }
    }
}