using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Analytics;

public class FUSE_ZONE : MonoBehaviour
{
    public List<ItemMonoBehaviour> itemsToFuse = new List<ItemMonoBehaviour>();
    public GameObject FuseTable;
    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<ItemMonoBehaviour>() != null)
        {
            other.GetComponent<ItemMonoBehaviour>().MarkReadyToFuse(true);
            itemsToFuse.Add(other.GetComponent<ItemMonoBehaviour>());
            CheckIfItemsCanFuse();
        }
    }
    private Boolean _fuzing = false;
    private void CheckIfItemsCanFuse()
    {
       if (itemsToFuse.Count >= 2 && !_fuzing && CanBeFuzed(itemsToFuse[0].item, itemsToFuse[1].item))
       {
            _fuzing = true;
           FuseItems();
       }
    }
    private Boolean CanBeFuzed(Item item1, Item item2)
    {
        return CraftItem(item1, item2) != null;
    }
    private void FuseItems()
    {
        foreach (var item in itemsToFuse)
        {
            LeanTween.cancel(item.gameObject);
            FindAnyObjectByType<PlayerInventory>().RemoveItem(item.item.itemName, 1);
            item.FuseTime();
        }
        StartCoroutine(WhirlAndDisappear());
        
        
        
    }
    private IEnumerator WhirlAndDisappear()
    {
        float duration = 1.5f;
        float elapsed = 0f;
        Vector3 center = Vector3.zero;
        foreach (var item in itemsToFuse)
            center += item.transform.position;
        center /= itemsToFuse.Count;

        List<float> startAngles = new List<float>();
        List<float> startDistances = new List<float>();
        for (int i = 0; i < itemsToFuse.Count; i++)
        {
            Vector3 dir = itemsToFuse[i].transform.position - center;
            startAngles.Add(Mathf.Atan2(dir.y, dir.x));
            startDistances.Add(dir.magnitude);
        }

        while (elapsed < duration)
        {
            float t = elapsed / duration;
            float curRadius = Mathf.Lerp(startDistances[0], 0f, t); // zakładamy podobny dystans
            float angleStep = Mathf.PI * 2f / itemsToFuse.Count;
            for (int i = 0; i < itemsToFuse.Count; i++)
            {
                float angle = startAngles[i] + t * 8f; // szybkość obrotu
                Vector3 pos = center + new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0) * Mathf.Lerp(startDistances[i], 0f, t);
                itemsToFuse[i].transform.position = pos;
            }
            elapsed += Time.deltaTime;
            yield return null;
        }

        ItemType? itemType = CraftItem(itemsToFuse[0].item, itemsToFuse[1].item);

        foreach (var item in itemsToFuse)
        {
            LeanTween.scale(item.gameObject, Vector3.zero, 0.2f).setOnComplete(() => Destroy(item.gameObject));
            //Destroy(item.gameObject);
        }
        itemsToFuse.Clear();

        _fuzing = false;
        // Create new item at "center"

        // 


        Item newItem = new Item()
        {
            itemName = itemType.ToString(),
            itemType = itemType.Value,
            amount = 1,
            icon = FindAnyObjectByType<ItemSpriteManager>().GetSpriteByItemType(itemType.Value)
        };

        FuseTable.GetComponent<FusionMenuController>().AddCreatedItem(newItem, center);
    }

    public ItemType? CraftItem(Item item1, Item item2)
    {
        if (item1.itemType == ItemType.Wood && item2.itemType == ItemType.Wood)
            return ItemType.WoodenStrop;
        if (item1.itemType == ItemType.Coal && item2.itemType == ItemType.Coal) // for tests
            return ItemType.Metal;
        else if (item1.itemType == ItemType.Metal && item2.itemType == ItemType.Metal)
            return ItemType.MetalStrop;
        else if ((item1.itemType == ItemType.Wood && item2.itemType == ItemType.Coal) || (item1.itemType == ItemType.Coal && item2.itemType == ItemType.Wood))
            return ItemType.Torch;
        else if ((item1.itemType == ItemType.Metal && item2.itemType == ItemType.Coal) || (item1.itemType == ItemType.Coal && item2.itemType == ItemType.Metal))
            return ItemType.Bulb;
        else if ((item1.itemType == ItemType.Metal && item2.itemType == ItemType.Wood) || (item1.itemType == ItemType.Wood && item2.itemType == ItemType.Metal))
            return ItemType.Tracks;

        Debug.Log("No fusion recipe");
        return null;
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<ItemMonoBehaviour>() != null)
        {
            other.GetComponent<ItemMonoBehaviour>().MarkReadyToFuse(false);
            itemsToFuse.Remove(other.GetComponent<ItemMonoBehaviour>());
        }
    }
}
