using Unity.VisualScripting;
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

    [Header("Item Sprites")]
    public Sprite woodSprite;
    public Sprite oilSprite;
    public Sprite metalSprite;

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
        if (woodButton != null) woodButton.onClick.AddListener(() => OnExchangeClick("Wood", woodCost, woodSprite));
        if (oilButton != null) oilButton.onClick.AddListener(() => OnExchangeClick("Oil Barrel", oilCost, oilSprite));
        if (metalButton != null) metalButton.onClick.AddListener(() => OnExchangeClick("Metal", metalCost, metalSprite));
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

    void OnExchangeClick(string itemName, int cost, Sprite itemSprite)
    {
        if (inventory != null && inventory.HasItem("Ore", cost))
        {
            inventory.RemoveItem("Ore", cost);
            DropItem(itemName, itemSprite);

            if (messageDisplay != null)
                messageDisplay.ShowTradeSprite();

            UpdateOreDisplay();
            UpdateButtonStates();
        }
    }

    void DropItem(string name, Sprite itemSprite)
    {
        GameObject obj = new GameObject("DroppedItem");
        obj.transform.position = transform.position + Vector3.right * 2 + Vector3.down * 8f;

        SpriteRenderer sr = obj.AddComponent<SpriteRenderer>();

        if (itemSprite != null)
        {
            sr.sprite = itemSprite;
        }
        else
        {
            return; 
        }

        sr.sortingOrder = 1;
        obj.transform.localScale = Vector3.one * 0.6f;

        ItemPickup pickup = obj.AddComponent<ItemPickup>();
        pickup.itemData = new Item(name, 1);
        pickup.itemData.icon = itemSprite;
    }
}