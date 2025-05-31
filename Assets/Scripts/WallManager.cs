using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class WallManager : MonoBehaviour
{
    [Header("Wall Prefab")]
    [SerializeField] public GameObject wallSegmentPrefab;
    
    [Header("Wall Generation Settings")]
    [SerializeField] public int numberOfSegments = 20;
    [SerializeField] public float segmentSpacing = 1f;
    
    [Header("Loot Settings")]
    [SerializeField] public GameObject itemDropPrefab;
    [SerializeField] public string dropItemName = "Ore";
    [SerializeField] public int minDropAmount = 1;
    [SerializeField] public int maxDropAmount = 3;
    [SerializeField] public Color itemColor = new Color(0.5f, 0.5f, 0.5f, 1f);
    
    private List<GameObject> wallSegments = new List<GameObject>();
    
    void Start()
    {
        if (wallSegmentPrefab == null)
        {
            Debug.LogError("Wall Segment Prefab is not assigned in WallManager!");
            return;
        }
        
        FindExistingWalls();
        GenerateWalls();
    }
    
    void FindExistingWalls()
    {
        foreach (Transform child in transform)
        {
            if (child.CompareTag("WallSegment"))
            {
                wallSegments.Add(child.gameObject);
                BoxCollider2D col = child.GetComponent<BoxCollider2D>();
                if (col != null && col.isTrigger)
                {
                    col.isTrigger = false;
                }
            }
        }
        
        wallSegments = wallSegments.OrderBy(w => w.transform.position.x).ToList();
        Debug.Log("Found " + wallSegments.Count + " existing wall segments");
    }
    
    void GenerateWalls()
    {
        if (wallSegments.Count == 0)
        {
            Debug.LogWarning("No existing wall segments found! Place at least one wall segment manually.");
            return;
        }
        
        GameObject rightmostWall = wallSegments[wallSegments.Count - 1];
        float startX = rightmostWall.transform.position.x;
        float wallY = rightmostWall.transform.position.y;
        
        int segmentsToGenerate = numberOfSegments - wallSegments.Count;
        
        for (int i = 0; i < segmentsToGenerate; i++)
        {
            float xPos = startX + ((i + 1) * segmentSpacing);
            CreateWallSegment(xPos, wallY, wallSegments.Count + i + 1);
        }
    }
    
    GameObject CreateWallSegment(float xPosition, float yPosition, int index)
    {
        if (wallSegmentPrefab == null) return null;
        
        GameObject newSegment = Instantiate(wallSegmentPrefab, transform);
        newSegment.name = "WallSegment" + index;
        newSegment.transform.position = new Vector3(xPosition, yPosition, 0);
        
        if (!newSegment.CompareTag("WallSegment"))
        {
            newSegment.tag = "WallSegment";
        }
        
        BoxCollider2D col = newSegment.GetComponent<BoxCollider2D>();
        if (col != null)
        {
            col.isTrigger = false;
        }
        
        wallSegments.Add(newSegment);
        return newSegment;
    }
    
    public void OnWallDestroyed(GameObject destroyedWall)
    {
        // Drop item at wall position
        DropItem(destroyedWall.transform.position);
        
        // Remove from list
        wallSegments.Remove(destroyedWall);
        
        // Generate new wall segment
        if (wallSegments.Count > 0)
        {
            GameObject rightmostWall = wallSegments.OrderBy(w => w.transform.position.x).Last();
            float maxX = rightmostWall.transform.position.x;
            float wallY = rightmostWall.transform.position.y;
            
            CreateWallSegment(maxX + segmentSpacing, wallY, wallSegments.Count + 100);
        }
    }
    
    void DropItem(Vector3 position)
    {
        GameObject droppedItem = null;
        
        if (itemDropPrefab != null)
        {
            // Use prefab
            droppedItem = Instantiate(itemDropPrefab);
        }
        else
        {
            // Create item from scratch
            droppedItem = new GameObject("DroppedItem");
            droppedItem.AddComponent<SpriteRenderer>();
            droppedItem.AddComponent<CircleCollider2D>();
        }
        
        // Position on ground
        droppedItem.transform.position = new Vector3(position.x, -5.5f, 0);
        
        // Add ItemPickup component
        ItemPickup pickup = droppedItem.GetComponent<ItemPickup>();
        if (pickup == null)
        {
            pickup = droppedItem.AddComponent<ItemPickup>();
        }
        
        // Setup item data
        int dropAmount = Random.Range(minDropAmount, maxDropAmount + 1);
        pickup.itemData = new Item(dropItemName, dropAmount);
        pickup.itemData.color = itemColor;
        
        // Setup sprite
        SpriteRenderer sr = droppedItem.GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            sr.sprite = Resources.Load<Sprite>("whitesquare");
            sr.color = itemColor;
            sr.sortingOrder = 1;
            droppedItem.transform.localScale = new Vector3(0.5f, 0.5f, 1f);
        }
        
        Debug.Log($"Dropped {dropAmount} {dropItemName} at position {position}");
    }
}