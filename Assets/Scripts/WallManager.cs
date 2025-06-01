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
    [SerializeField] public Sprite itemDropSprite;
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
        if (itemDropSprite == null)
        {
            Debug.LogWarning("Item drop sprite is not assigned!");
            return;
        }

        // Tworzenie nowego obiektu z podstawowymi komponentami
        GameObject droppedItem = new GameObject("DroppedItem");

        // Dodawanie SpriteRenderer i ustawianie sprite
        SpriteRenderer spriteRenderer = droppedItem.AddComponent<SpriteRenderer>();
        spriteRenderer.sprite = itemDropSprite;
        spriteRenderer.color = itemColor;

        // Dodawanie kolizji
        CircleCollider2D collider = droppedItem.AddComponent<CircleCollider2D>();
        collider.isTrigger = true;

        // Pozycjonowanie na ziemi
        droppedItem.transform.position = new Vector3(position.x, -5.5f, 0);

        // Dodawanie komponentu ItemPickup
        ItemPickup pickup = droppedItem.AddComponent<ItemPickup>();

        // Ustawianie danych przedmiotu
        int dropAmount = Random.Range(minDropAmount, maxDropAmount + 1);
        pickup.itemData = new Item(dropItemName, dropAmount);
        pickup.itemData.icon = itemDropSprite;

        // Opcjonalnie: dodaj tag dla ³atwiejszego zarz¹dzania
        droppedItem.tag = "DroppedItem";
    }
}