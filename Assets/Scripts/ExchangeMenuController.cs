using UnityEngine;
using UnityEngine.UI;

public class ExchangeMenuController : MonoBehaviour
{
    [Header("References")]
    public GameObject menuPanel;
    public Button woodButton;
    public Button oilButton;
    public Button metalButton;
    public Button closeButton;
    public Text oreDisplay;
    
    [Header("Exchange Rates")]
    public int woodCost = 5;
    public int oilCost = 10;
    public int metalCost = 3;
    
    [Header("Item Colors")]
    public Color woodColor = new Color(0.6f, 0.4f, 0.2f);
    public Color oilColor = new Color(0.1f, 0.1f, 0.1f);
    public Color metalColor = new Color(0.7f, 0.7f, 0.8f);
    
    private PlayerInventory inventory;
    private PlayerMessageDisplay messageDisplay;
    private ExchangeStation exchangeStation;
    
    void Start()
    {
        GameObject player = GameObject.Find("Player");
        if (player != null)
        {
            inventory = player.GetComponent<PlayerInventory>();
        }
        
        messageDisplay = PlayerMessageDisplay.Instance;
        exchangeStation = GetComponent<ExchangeStation>();
        
        // Setup button listeners
        if (woodButton != null) woodButton.onClick.AddListener(() => OnExchangeClick("Wood", woodCost, woodColor));
        if (oilButton != null) oilButton.onClick.AddListener(() => OnExchangeClick("Oil Barrel", oilCost, oilColor));
        if (metalButton != null) metalButton.onClick.AddListener(() => OnExchangeClick("Metal", metalCost, metalColor));
        if (closeButton != null) closeButton.onClick.AddListener(CloseMenu);
        
        // Hide menu initially
        if (menuPanel != null) menuPanel.SetActive(false);
    }
    
    public void ShowMenu()
    {
        if (menuPanel != null)
        {
            menuPanel.SetActive(true);
            UpdateOreDisplay();
            UpdateButtonStates();
        }
    }
    
    public void CloseMenu()
    {
        if (menuPanel != null)
        {
            menuPanel.SetActive(false);
            // Inform ExchangeStation that menu is closed
            if (exchangeStation != null)
                exchangeStation.SetMenuOpen(false);
        }
    }
    
    void UpdateOreDisplay()
    {
        if (inventory != null && oreDisplay != null)
        {
            int oreCount = inventory.GetItemCount("Ore");
            oreDisplay.text = "Your Ore: " + oreCount;
        }
    }
    
    void UpdateButtonStates()
    {
        if (inventory == null) return;
        
        int oreCount = inventory.GetItemCount("Ore");
        
        // Update button interactability
        if (woodButton != null) woodButton.interactable = oreCount >= woodCost;
        if (oilButton != null) oilButton.interactable = oreCount >= oilCost;
        if (metalButton != null) metalButton.interactable = oreCount >= metalCost;
    }
    
    void OnExchangeClick(string itemName, int cost, Color itemColor)
    {
        if (inventory != null && inventory.HasItem("Ore", cost))
        {
            inventory.RemoveItem("Ore", cost);
            DropItem(itemName, itemColor);
            
            if (messageDisplay != null)
                messageDisplay.ShowMessage("Exchanged " + cost + " Ore for " + itemName + "!", Color.green, 2f);
            
            UpdateOreDisplay();
            UpdateButtonStates();
        }
    }
    
    void DropItem(string name, Color color)
    {
        GameObject obj = new GameObject("DroppedItem");
        obj.transform.position = transform.position + Vector3.right * 2 + Vector3.down * 2.5f;
        
        SpriteRenderer sr = obj.AddComponent<SpriteRenderer>();
        sr.color = color;
        sr.sortingOrder = 1;
        obj.transform.localScale = Vector3.one * 0.6f;
        
        // Load sprite
        Sprite whiteSquare = Resources.Load<Sprite>("whitesquare");
        if (whiteSquare == null)
        {
            UnityEngine.Object[] sprites = UnityEditor.AssetDatabase.LoadAllAssetsAtPath("Assets/Sprites/whitesquare.png");
            if (sprites.Length > 0)
            {
                foreach (var s in sprites)
                {
                    if (s is Sprite)
                    {
                        whiteSquare = s as Sprite;
                        break;
                    }
                }
            }
        }
        
        if (whiteSquare != null)
            sr.sprite = whiteSquare;
        
        ItemPickup pickup = obj.AddComponent<ItemPickup>();
        pickup.itemData = new Item(name, 1);
        pickup.itemData.color = color;
    }
}