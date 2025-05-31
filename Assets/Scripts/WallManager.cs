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
        
        wallSegments.Add(newSegment);
        return newSegment;
    }
    
    public void OnWallDestroyed(GameObject destroyedWall)
    {
        wallSegments.Remove(destroyedWall);
        
        if (wallSegments.Count > 0)
        {
            GameObject rightmostWall = wallSegments.OrderBy(w => w.transform.position.x).Last();
            float maxX = rightmostWall.transform.position.x;
            float wallY = rightmostWall.transform.position.y;
            
            CreateWallSegment(maxX + segmentSpacing, wallY, wallSegments.Count + 100);
        }
    }
}