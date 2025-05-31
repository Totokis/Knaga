using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInventory : MonoBehaviour
{
    [Header("Inventory Settings")]
    public Item currentItem = null;
    public int maxStackSize = 99;
    
    private static PlayerInventory instance;
    public static PlayerInventory Instance => instance;
    
    void Awake()
    {
        currentItem = null;
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    void Start()
    {
        currentItem = null;

        UpdateInventoryDisplay();
    }
    
    void Update()
    {
        // Debug display with new Input System
        if (Keyboard.current != null && Keyboard.current.iKey.wasPressedThisFrame)
        {
            if (currentItem != null)
                Debug.Log($"Inventory: {currentItem.itemName} x{currentItem.amount}");
            else
                Debug.Log("Inventory: Empty");
        }
    }
    
    public bool AddItem(Item newItem)
    {
        if (currentItem == null)
        {
            currentItem = newItem.Clone();
            UpdateInventoryDisplay();
            return true;
        }
        else if (currentItem.itemName == newItem.itemName)
        {
            int spaceLeft = maxStackSize - currentItem.amount;
            if (spaceLeft > 0)
            {
                int amountToAdd = Mathf.Min(spaceLeft, newItem.amount);
                currentItem.amount += amountToAdd;
                UpdateInventoryDisplay();
                return amountToAdd == newItem.amount;
            }
        }
        
        return false;
    }
    
    public bool RemoveItem(int amount)
    {
        if (currentItem != null && currentItem.amount >= amount)
        {
            currentItem.amount -= amount;
            if (currentItem.amount <= 0)
            {
                currentItem = null;
            }
            UpdateInventoryDisplay();
            return true;
        }
        return false;
    }
    
    public void ClearInventory()
    {
        currentItem = null;
        UpdateInventoryDisplay();
    }
    
    void UpdateInventoryDisplay()
    {
        if (currentItem != null && currentItem.amount > 0)
        {
            Debug.Log($"Inventory updated: {currentItem.itemName} x{currentItem.amount}");
        }
        else
        {
            Debug.Log("Inventory is now empty");
        }
    }
    
    public bool HasItem(string itemName, int amount = 1)
    {
        return currentItem != null && 
               currentItem.itemName == itemName && 
               currentItem.amount >= amount;
    }
}