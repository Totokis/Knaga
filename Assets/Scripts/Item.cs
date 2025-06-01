using UnityEngine;

[System.Serializable]
public class Item
{
    public string itemName;
    public ItemType itemType;
    public int amount;
    public Sprite icon;
    public Color color = Color.white;
    
    public Item() { }
    public Item(string name, int amt = 1, ItemType itemType = ItemType.Coal)
    {
        itemName = name;
        amount = amt;
        this.itemType = itemType;
    }
    
    public Item Clone()
    {
        Item newItem = new Item(itemName, amount);
        newItem.icon = icon;
        newItem.color = color;
        return newItem;
    }
}