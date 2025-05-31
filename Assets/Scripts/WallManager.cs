using UnityEngine;
using System.Collections.Generic;

public class WallManager : MonoBehaviour
{
    [Header("Wall Prefab")]
    [SerializeField] public GameObject wallSegmentPrefab;
    
    [Header("Wall Generation Settings")]
    [SerializeField] public int numberOfSegments = 20;
    [SerializeField] public float segmentSpacing = 1f;
    [SerializeField] public float startX = 25f;
    [SerializeField] public float wallY = 0f;
    
    private List<GameObject> wallSegments = new List<GameObject>();
    
    void Start()
    {
        if (wallSegmentPrefab == null)
        {
            Debug.LogError("Wall Segment Prefab is not assigned in WallManager!");
            return;
        }
        
        GenerateWalls();
    }
    
    void GenerateWalls()
    {
        // Clear any existing wall segments first
        foreach (Transform child in transform)
        {
            if (child.CompareTag("WallSegment"))
            {
                wallSegments.Add(child.gameObject);
            }
        }
        
        // Generate wall segments
        int startIndex = wallSegments.Count;
        for (int i = startIndex; i < numberOfSegments; i++)
        {
            float xPos = startX + (i * segmentSpacing);
            CreateWallSegment(xPos, i + 1);
        }
        
        Debug.Log($"Generated {numberOfSegments - startIndex} wall segments. Total: {wallSegments.Count}");
    }
    
    GameObject CreateWallSegment(float xPosition, int index)
    {
        if (wallSegmentPrefab == null)
        {
            Debug.LogError("Cannot create wall segment - prefab is null!");
            return null;
        }
        
        // Instantiate the prefab
        GameObject newSegment = Instantiate(wallSegmentPrefab, transform);
        newSegment.name = $"WallSegment{index}";
        newSegment.transform.position = new Vector3(xPosition, wallY, 0);
        
        // Ensure it has the correct tag
        if (!newSegment.CompareTag("WallSegment"))
        {
            newSegment.tag = "WallSegment";
        }
        
        // Ensure it has required components
        if (newSegment.GetComponent<BoxCollider2D>() == null)
        {
            Debug.LogWarning($"Wall segment {newSegment.name} is missing BoxCollider2D!");
        }
        
        if (newSegment.GetComponent<SpriteRenderer>() == null)
        {
            Debug.LogWarning($"Wall segment {newSegment.name} is missing SpriteRenderer!");
        }
        
        wallSegments.Add(newSegment);
        return newSegment;
    }
    
    public void OnWallDestroyed(GameObject destroyedWall)
    {
        // Remove from list
        wallSegments.Remove(destroyedWall);
        
        // Generate new segment at the end
        if (wallSegments.Count > 0 && wallSegments.Count < numberOfSegments)
        {
            // Find the rightmost segment
            float maxX = -float.MaxValue;
            foreach (GameObject segment in wallSegments)
            {
                if (segment != null && segment.transform.position.x > maxX)
                {
                    maxX = segment.transform.position.x;
                }
            }
            
            // Create new segment
            GameObject newSegment = CreateWallSegment(maxX + segmentSpacing, wallSegments.Count + 100);
            if (newSegment != null)
            {
                Debug.Log($"Created new wall segment at x={maxX + segmentSpacing}");
            }
        }
    }
    
    // Method to manually regenerate walls (can be called from Inspector button)
    [ContextMenu("Regenerate Walls")]
    public void RegenerateWalls()
    {
        // Clear existing generated walls (but keep manually placed ones)
        for (int i = wallSegments.Count - 1; i >= 0; i--)
        {
            if (wallSegments[i] != null && wallSegments[i].name.Contains("WallSegment"))
            {
                DestroyImmediate(wallSegments[i]);
            }
        }
        wallSegments.Clear();
        
        GenerateWalls();
    }
}