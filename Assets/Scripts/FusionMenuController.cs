using NUnit.Framework.Interfaces;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class FusionMenuController: MonoBehaviour
{
    [SerializeField] private List<Item> _items = new List<Item>();
    [SerializeField] private float radius = 2f;
    [SerializeField] private GameObject spawnPoint;
    
    private List<ItemMonoBehaviour> _itemMonos = new List<ItemMonoBehaviour>();


    public void AddCreatedItem(Item item, Vector3 position)
    {
            var newItem = item.Clone();
            newItem.amount = 1;
            _items.Add(newItem);

            GameObject go = new GameObject(item.itemName)
            {
                transform =
                {
                    position = position,
                    localScale = 0.4f * Vector3.one,
                }
            };
            var itemMb = go.AddComponent<ItemMonoBehaviour>();
            itemMb.SetItem(_items.Last());
            itemMb.GetComponent<SphereCollider>().radius = 6.16f;
        _itemMonos.Add(itemMb);

            go.GetComponent<SpriteRenderer>().sprite = FindAnyObjectByType<ItemSpriteManager>().GetSpriteByItemType(item.itemType);

        PlayerInventory inventory = PlayerInventory.Instance;
        if (inventory != null)
        {
            inventory.AddItem(item);
        }
    }
    public void SetCraftingTable(PlayerInventory inventory)
    {
        _items = new List<Item>();
        foreach (var item in inventory.items)
        {
            for (int i = 0; i < item.amount; i++)
            {
                var newItem = item.Clone();
                newItem.amount = 1;
                _items.Add(newItem);
            }
        }
        
        for (int i = 0; i < _items.Count; i++)
        {
            float angle = i * Mathf.PI * 2f / _items.Count;
            Vector3 pos = spawnPoint.transform.position + new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0) * radius;
            GameObject go = new GameObject(_items[i].itemName)
            {
                transform =
                {
                    position = pos,
                    localScale = 0.4f * Vector3.one,
                }
            };
            var itemMb = go.AddComponent<ItemMonoBehaviour>();
            itemMb.SetItem(_items[i]);
            _itemMonos.Add(itemMb);
          
        }

    }
    public void CloseTable()
    {
        foreach (var itemMono in _itemMonos)
        {
            if (itemMono != null)
            {
                Destroy(itemMono.gameObject);
            }
        }
        _itemMonos.Clear();
        _items.Clear();
        
    }
}