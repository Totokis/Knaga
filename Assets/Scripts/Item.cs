using UnityEngine;

[System.Serializable]
public class Item
{
    public string itemName;
    public int amount;
    public Sprite icon;
    public Color color = Color.white;
    
    public Item(string name, int amt = 1)
    {
        itemName = name;
        amount = amt;
    }
    
    public Item Clone()
    {
        Item newItem = new Item(itemName, amount);
        newItem.icon = icon;
        newItem.color = color;
        return newItem;
    }
}