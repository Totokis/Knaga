using JetBrains.Annotations;
using System;
using System.Drawing;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class CraftingTable : MonoBehaviour
{
    [Header("Settings")]

    public float interactionRange = 3f;

    public Item CurrentItem;
    private Transform player;
    private PlayerMessageDisplay messageDisplay;
    private bool isInRange = false;
    [SerializeField] private GameObject fusionMenu;

    [Header("Item Sprites")]
    public Sprite metalSprite;
    public Sprite fushionSprite;

    void Start()
    {
        GameObject p = GameObject.Find("Player");
        if (p != null)
        {
            player = p.transform;
        }

        messageDisplay = PlayerMessageDisplay.Instance;
        // menuController = GetComponent<ExchangeMenuController>();
    }

    void Update()
    {
        if (player == null || Keyboard.current == null) return;

        float dist = Math.Abs(transform.position.x - player.position.x);
        bool wasInRange = isInRange;
        isInRange = dist <= interactionRange;

        // Show prompt when entering range
        if (isInRange && !wasInRange && messageDisplay != null && !IsOpened())
        {
            messageDisplay.ShowSprite(fushionSprite);
        }

        if (isInRange && Keyboard.current.eKey.wasPressedThisFrame && !IsOpened())
        {
            OpenFusionMenu();
        }

        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            CloseFusionMenu();
        }
    }

    private void CloseFusionMenu()
    {
        fusionMenu.GetComponent<FusionMenuController>().CloseTable();
        fusionMenu.SetActive(false);
    }
    public Boolean IsOpened()
    {
        return fusionMenu.activeInHierarchy;
    }
    private void OpenFusionMenu()
    {
        fusionMenu.SetActive(true);
        fusionMenu.GetComponent<FusionMenuController>().SetCraftingTable(player.GetComponent<PlayerInventory>());
    }

    void OnDrawGizmosSelected()
    {
        // Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, interactionRange);
    }

    public void CraftItem(Item item1, Item item2)
    {
        Item newItem = null;
        if (item1.itemType == ItemType.Wood && item2.itemType == ItemType.Wood)
        {
            newItem = new Item()
            {
                itemName = "Wooden Strop",
                itemType = ItemType.WoodenStrop,
                amount = 1
            };
        }
        if (item1.itemType == ItemType.Coal && item2.itemType == ItemType.Coal) // for tests
        {
            newItem = new Item()
            {
                itemName = "Metal",
                itemType = ItemType.Metal,
                icon = metalSprite,
                amount = 1
            };
        }
        else if (item1.itemType == ItemType.Metal && item2.itemType == ItemType.Metal)
        {
            newItem = new Item()
            {
                itemName = "Metal Strop",
                itemType = ItemType.MetalStrop,
                amount = 1
            };
        }
        else if((item1.itemType == ItemType.Wood && item2.itemType == ItemType.Coal) || (item1.itemType == ItemType.Coal && item2.itemType == ItemType.Wood))
        {
            newItem = new Item()
            {
                itemName = "Torch",
                itemType = ItemType.Torch,
                amount = 1
            };
        }
        else if ((item1.itemType == ItemType.Metal && item2.itemType == ItemType.Coal) || (item1.itemType == ItemType.Coal && item2.itemType == ItemType.Metal))
        {
            newItem = new Item()
            {
                itemName = "Bulb",
                itemType = ItemType.Bulb,
                amount = 1
            };
        }
        else if ((item1.itemType == ItemType.Metal && item2.itemType == ItemType.Wood) || (item1.itemType == ItemType.Wood && item2.itemType == ItemType.Metal))
        {
            newItem = new Item()
            {
                itemName = "Tracks",
                itemType = ItemType.Tracks,
                amount = 1
            };
        }

        if(newItem != null)
        {
            DropItem(newItem);
        }
        else
        {
            DropItem(item1);
            DropItem(item2);
        }

        CurrentItem = null;
    }

    void DropItem(Item item)
    {
        GameObject obj = new GameObject("ItemFromCrafting");
        obj.transform.position = transform.position + Vector3.right * 2 + Vector3.down * (-3f); // drobny random na boki?

        SpriteRenderer sr = obj.AddComponent<SpriteRenderer>();
        // sr.color = color;
        sr.sortingOrder = 1;
        obj.transform.localScale = Vector3.one * 0.6f;

        Sprite sprite = FindAnyObjectByType<ItemSpriteManager>().GetSpriteByItemType(item.itemType);

        obj.GetComponent<SpriteRenderer>().sprite = sprite;

        ItemPickup pickup = obj.AddComponent<ItemPickup>();
        pickup.itemData = item;
        pickup.itemData.icon = item.icon;
        // pickup.itemData.color = color;
    }
}