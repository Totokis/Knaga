using UnityEngine;
using UnityEngine.InputSystem;

public class ExchangeStation : MonoBehaviour
{
    [Header("Settings")]
    public float interactionRange = 3f;
    public int orePerWood = 5;
    public int orePerOil = 10;
    public int orePerMetal = 3;
    
    [Header("Colors")]
    public Color woodColor = new Color(0.6f, 0.4f, 0.2f);
    public Color oilColor = new Color(0.1f, 0.1f, 0.1f);
    public Color metalColor = new Color(0.7f, 0.7f, 0.8f);
    
    private Transform player;
    private PlayerInventory inventory;
    private bool isInRange = false;
    private bool menuOpen = false;
    
    void Start()
    {
        GameObject p = GameObject.Find("Player");
        if (p != null)
        {
            player = p.transform;
            inventory = p.GetComponent<PlayerInventory>();
        }
    }
    
    void Update()
    {
        if (player == null || Keyboard.current == null) return;
        
        float dist = Vector2.Distance(transform.position, player.position);
        isInRange = dist <= interactionRange;
        
        if (isInRange && Keyboard.current.eKey.wasPressedThisFrame)
        {
            menuOpen = !menuOpen;
            if (menuOpen)
            {
                Debug.Log("Exchange Menu Open - Press 1/2/3 to exchange, E to close");
                Time.timeScale = 0f;
            }
            else
            {
                Time.timeScale = 1f;
            }
        }
        
        if (menuOpen)
        {
            if (Keyboard.current.digit1Key.wasPressedThisFrame)
                Exchange("Wood", orePerWood, woodColor);
            else if (Keyboard.current.digit2Key.wasPressedThisFrame)
                Exchange("Oil Barrel", orePerOil, oilColor);
            else if (Keyboard.current.digit3Key.wasPressedThisFrame)
                Exchange("Metal", orePerMetal, metalColor);
        }
    }
    
    void Exchange(string item, int cost, Color color)
    {
        if (inventory != null && inventory.HasItem("Ore", cost))
        {
            inventory.RemoveItem(cost);
            DropItem(item, color);
            menuOpen = false;
            Time.timeScale = 1f;
            Debug.Log($"Exchanged {cost} Ore for {item}!");
        }
        else
        {
            Debug.Log("Not enough ore!");
        }
    }
    
    void DropItem(string name, Color color)
    {
        GameObject obj = new GameObject(name);
        obj.transform.position = transform.position + Vector3.right * 2 + Vector3.down * 2.5f;
        
        SpriteRenderer sr = obj.AddComponent<SpriteRenderer>();
        sr.color = color;
        sr.sortingOrder = 1;
        obj.transform.localScale = Vector3.one * 0.6f;
        
        ItemPickup pickup = obj.AddComponent<ItemPickup>();
        pickup.itemData = new Item(name, 1);
        pickup.itemData.color = color;
    }
    
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, interactionRange);
    }
}