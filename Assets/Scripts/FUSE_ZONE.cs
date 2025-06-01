using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class FUSE_ZONE : MonoBehaviour
{
    public List<ItemMonoBehaviour> itemsToFuse = new List<ItemMonoBehaviour>();
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<ItemMonoBehaviour>() != null)
        {
            other.GetComponent<ItemMonoBehaviour>().MarkReadyToFuse(true);
            itemsToFuse.Add(other.GetComponent<ItemMonoBehaviour>());
            CheckIfItemsCanFuse();
        }
    }

    private void CheckIfItemsCanFuse()
    {
       if (itemsToFuse.Count >= 2)
       {
           FuseItems();
       }
    }

    private void FuseItems()
    {
        foreach (var item in itemsToFuse)
        {
            LeanTween.cancel(item.gameObject);
            FindAnyObjectByType<PlayerInventory>().RemoveItem(item.item.itemName, 1);
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

        foreach (var item in itemsToFuse)
        {
            LeanTween.scale(item.gameObject, Vector3.zero, 0.2f).setOnComplete(() => Destroy(item.gameObject));
        }
        itemsToFuse.Clear();
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
