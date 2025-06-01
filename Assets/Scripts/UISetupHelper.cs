using UnityEngine;

public class UISetupHelper : MonoBehaviour
{
    public static void SetupWorldSpaceUI()
    {
        // Tworzenie InventoryDisplay
        GameObject inventoryObj = GameObject.Find("InventoryDisplay");
        if (inventoryObj == null)
        {
            inventoryObj = new GameObject("InventoryDisplay");
            inventoryObj.AddComponent<InventoryDisplay>();
            Debug.Log("Created InventoryDisplay object");
        }
        
        // Tworzenie PlayerMessageDisplay
        GameObject messageObj = GameObject.Find("PlayerMessageDisplay");
        if (messageObj == null)
        {
            messageObj = new GameObject("PlayerMessageDisplay");
            messageObj.AddComponent<PlayerMessageDisplay>();
            Debug.Log("Created PlayerMessageDisplay object");
        }
        
        // Upewnienie się, że mamy warstwę UI
        CreateUILayer();
        
        Debug.Log("World Space UI setup complete!");
    }
    
    private static void CreateUILayer()
    {
        // Lista wszystkich warstw
        for (int i = 0; i < 32; i++)
        {
            string layerName = LayerMask.LayerToName(i);
            if (layerName == "UI")
            {
                Debug.Log("UI layer already exists");
                return;
            }
        }
        
        // Jeśli nie ma warstwy UI, informujemy użytkownika
        Debug.LogWarning("Please create a 'UI' sorting layer in Edit > Project Settings > Tags and Layers > Sorting Layers");
    }
    
    // Metoda do testowania w runtime
    void Start()
    {
        // Automatyczne tworzenie UI jeśli nie istnieje
        if (GameObject.Find("InventoryDisplay") == null)
        {
            GameObject inventoryObj = new GameObject("InventoryDisplay");
            inventoryObj.AddComponent<InventoryDisplay>();
        }
        
        if (GameObject.Find("PlayerMessageDisplay") == null)
        {
            GameObject messageObj = new GameObject("PlayerMessageDisplay");
            messageObj.AddComponent<PlayerMessageDisplay>();
        }
    }
}