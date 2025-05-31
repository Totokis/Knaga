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
    private PlayerMessageDisplay messageDisplay;
    private ExchangeMenuUI menuUI;
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

        messageDisplay = PlayerMessageDisplay.Instance;

        // Find or create menu UI
        menuUI = ExchangeMenuUI.Instance;
        if (menuUI == null)
        {
            GameObject menuObj = GameObject.Find("ExchangeMenuUI");
            if (menuObj == null)
            {
                menuObj = new GameObject("ExchangeMenuUI");
                menuUI = menuObj.AddComponent<ExchangeMenuUI>();
            }
            else
            {
                menuUI = menuObj.GetComponent<ExchangeMenuUI>();
            }
        }
    }

    void Update()
    {
        if (player == null || Keyboard.current == null) return;

        float dist = Vector2.Distance(transform.position, player.position);
        bool wasInRange = isInRange;
        isInRange = dist <= interactionRange;

        // Show prompt when entering range
        if (isInRange && !wasInRange && messageDisplay != null)
        {
            messageDisplay.ShowInteraction("Press E to open Exchange Station");
        }

        if (isInRange && Keyboard.current.eKey.wasPressedThisFrame)
        {
            menuOpen = !menuOpen;
            if (menuOpen)
            {
                OpenMenu();
            }
            else
            {
                CloseMenu();
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

    void OpenMenu()
    {
        if (menuUI != null && inventory != null)
        {
            int currentOre = inventory.GetItemCount("Ore");
            menuUI.ShowMenu(currentOre);
            Time.timeScale = 0f;
            Debug.Log("Exchange Menu Open - Press 1/2/3 to exchange, E to close");
        }
    }

    void CloseMenu()
    {
        if (menuUI != null)
        {
            menuUI.HideMenu();
        }
        menuOpen = false;
        Time.timeScale = 1f;
    }

    void Exchange(string item, int cost, Color color)
    {
        if (inventory != null && inventory.HasItem("Ore", cost))
        {
            inventory.RemoveItem("Ore", cost);
            DropItem(item, color);
            CloseMenu();

            if (messageDisplay != null)
                messageDisplay.ShowMessage($"Exchanged {cost} Ore for {item}!", Color.green, 2f);
            else
                Debug.Log($"Exchanged {cost} Ore for {item}!");

            // Update menu if still open
            if (menuOpen && menuUI != null)
            {
                int currentOre = inventory.GetItemCount("Ore");
                menuUI.UpdateMenuContent(currentOre);
            }
        }
        else
        {
            if (messageDisplay != null)
                messageDisplay.ShowMessage("Not enough ore!", Color.red, 2f);
            else
                Debug.Log("Not enough ore!");
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
            // Try to load from assets
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

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, interactionRange);
    }
}