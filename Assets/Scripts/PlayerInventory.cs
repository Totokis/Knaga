using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using System.Linq;

public class PlayerInventory : MonoBehaviour
{
    [Header("Inventory Settings")]
    public List<Item> items = new List<Item>();  // Zmiana z currentItem na listę
    public int maxStackSize = 99;
    public int maxInventorySize = 20;  // Maksymalna liczba różnych przedmiotów
    
    private static PlayerInventory instance;
    public static PlayerInventory Instance => instance;
    public Item GetCurrentItem()
    {
        if (items.Any())
        {
            Item takenItem = items.First();
            items.RemoveAt(0);
            UpdateInventoryDisplay();
            return takenItem;
        }
        else
            return null;
    }
    void Awake()
    {
        items.Clear();  // Puste inventory na start
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
        items.Clear();  // Upewniamy się że inventory jest puste
        UpdateInventoryDisplay();
    }
    
    void Update()
    {
        // Debug display with new Input System
        if (Keyboard.current != null && Keyboard.current.iKey.wasPressedThisFrame)
        {
            if (items.Count > 0)
            {
                Debug.Log($"Inventory ({items.Count} types):");
                foreach (var item in items)
                {
                    Debug.Log($"  - {item.itemName} x{item.amount}");
                }
            }
            else
            {
                Debug.Log("Inventory: Empty");
            }
        }
    }
    
    public bool AddItem(Item newItem)
    {
        // Szukamy czy już mamy taki przedmiot
        Item existingItem = items.FirstOrDefault(i => i.itemName == newItem.itemName);
        
        if (existingItem != null)
        {
            // Przedmiot już istnieje, próbujemy dodać do stacka
            int spaceLeft = maxStackSize - existingItem.amount;
            if (spaceLeft > 0)
            {
                int amountToAdd = Mathf.Min(spaceLeft, newItem.amount);
                existingItem.amount += amountToAdd;
                UpdateInventoryDisplay();
                return amountToAdd == newItem.amount;
            }
            return false;
        }
        else
        {
            // Nowy typ przedmiotu
            if (items.Count < maxInventorySize)
            {
                items.Add(newItem.Clone());
                UpdateInventoryDisplay();
                return true;
            }
            return false;
        }
    }
    
    public bool RemoveItem(string itemName, int amount)
    {
        Item item = items.FirstOrDefault(i => i.itemName == itemName);
        
        if (item != null && item.amount >= amount)
        {
            item.amount -= amount;
            if (item.amount <= 0)
            {
                items.Remove(item);
            }
            UpdateInventoryDisplay();
            return true;
        }
        return false;
    }
    
    // Przeciążona metoda dla kompatybilności wstecznej
    public bool RemoveItem(int amount)
    {
        // Usuwa z pierwszego przedmiotu na liście (jeśli istnieje)
        if (items.Count > 0)
        {
            return RemoveItem(items[0].itemName, amount);
        }
        return false;
    }
    
    public void ClearInventory()
    {
        items.Clear();
        UpdateInventoryDisplay();
    }
    
    void UpdateInventoryDisplay()
    {
        if (items.Count > 0)
        {
            Debug.Log($"Inventory updated: {items.Count} item type(s)");
            foreach (var item in items)
            {
                Debug.Log($"  - {item.itemName} x{item.amount}");
            }
        }
        else
        {
            Debug.Log("Inventory is now empty");
        }
    }
    
    public bool HasItem(string itemName, int amount = 1)
    {
        Item item = items.FirstOrDefault(i => i.itemName == itemName);
        return item != null && item.amount >= amount;
    }
    
    // Nowe metody pomocnicze
    public Item GetItem(string itemName)
    {
        return items.FirstOrDefault(i => i.itemName == itemName);
    }
    
    public int GetItemCount(string itemName)
    {
        Item item = items.FirstOrDefault(i => i.itemName == itemName);
        return item != null ? item.amount : 0;
    }
    
    public int GetTotalItemTypes()
    {
        return items.Count;
    }
    
    public int GetTotalItemCount()
    {
        return items.Sum(i => i.amount);
    }
}