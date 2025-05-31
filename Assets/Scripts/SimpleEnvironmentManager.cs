using UnityEngine;
using System.Collections.Generic;

public class SimpleEnvironmentManager : MonoBehaviour
{
    [Header("Prefabs")]
    public GameObject floorSegmentPrefab;
    public GameObject backgroundSegmentPrefab;
    public GameObject ceilingSegmentPrefab;
    
    [Header("Generation Settings")]
    public int initialSegments = 30;
    public float startX = -10f;
    
    [Header("Segment Sizes")]
    public float floorSegmentWidth = 10f;
    public float backgroundSegmentWidth = 10f;
    public float ceilingSegmentWidth = 10f;
    
    [Header("Position Settings")]
    public float floorY = -3f;
    public float backgroundY = 0f;
    public float backgroundZ = 2f;
    public float ceilingY = 3f;
    
    [Header("Dynamic Generation")]
    public bool enableDynamicGeneration = true;
    public float generateAheadDistance = 50f;
    public float removeBackDistance = 100f;
    public int maxSegmentsPerFrame = 3;
    
    private List<GameObject> floorSegments = new List<GameObject>();
    private List<GameObject> backgroundSegments = new List<GameObject>();
    private List<GameObject> ceilingSegments = new List<GameObject>();
    
    private Transform player;
    private float lastFloorX;
    private float lastBackgroundX;
    private float lastCeilingX;
    
    void Start()
    {
        GameObject playerObj = GameObject.Find("Player");
        if (playerObj != null) player = playerObj.transform;
        
        lastFloorX = startX;
        lastBackgroundX = startX;
        lastCeilingX = startX;
        
        for (int i = 0; i < initialSegments; i++)
        {
            CreateFloorSegment();
            CreateBackgroundSegment();
            CreateCeilingSegment();
        }
    }
    
    void Update()
    {
        if (!enableDynamicGeneration || player == null) return;
        
        float playerX = player.position.x;
        float rightEdge = playerX + generateAheadDistance;
        float leftEdge = playerX - removeBackDistance;
        
        int generated = 0;
        while (lastFloorX < rightEdge && generated < maxSegmentsPerFrame)
        {
            CreateFloorSegment();
            generated++;
        }
        
        generated = 0;
        while (lastBackgroundX < rightEdge && generated < maxSegmentsPerFrame)
        {
            CreateBackgroundSegment();
            generated++;
        }
        
        generated = 0;
        while (lastCeilingX < rightEdge && generated < maxSegmentsPerFrame)
        {
            CreateCeilingSegment();
            generated++;
        }
        
        RemoveOldSegments(leftEdge);
    }
    
    void CreateFloorSegment()
    {
        if (floorSegmentPrefab == null) return;
        
        GameObject segment = Instantiate(floorSegmentPrefab, transform);
        segment.name = "Floor_" + floorSegments.Count;
        segment.transform.position = new Vector3(lastFloorX + floorSegmentWidth / 2f, floorY, 0);
        
        floorSegments.Add(segment);
        lastFloorX += floorSegmentWidth;
    }
    
    void CreateBackgroundSegment()
    {
        if (backgroundSegmentPrefab == null) return;
        
        GameObject segment = Instantiate(backgroundSegmentPrefab, transform);
        segment.name = "Background_" + backgroundSegments.Count;
        segment.transform.position = new Vector3(lastBackgroundX + backgroundSegmentWidth / 2f, backgroundY, backgroundZ);
        
        backgroundSegments.Add(segment);
        lastBackgroundX += backgroundSegmentWidth;
    }
    
    void CreateCeilingSegment()
    {
        if (ceilingSegmentPrefab == null) return;
        
        GameObject segment = Instantiate(ceilingSegmentPrefab, transform);
        segment.name = "Ceiling_" + ceilingSegments.Count;
        segment.transform.position = new Vector3(lastCeilingX + ceilingSegmentWidth / 2f, ceilingY, 0);
        
        ceilingSegments.Add(segment);
        lastCeilingX += ceilingSegmentWidth;
    }
    
    void RemoveOldSegments(float leftEdge)
    {
        for (int i = floorSegments.Count - 1; i >= 0; i--)
        {
            if (floorSegments[i] != null && 
                floorSegments[i].transform.position.x + floorSegmentWidth / 2f < leftEdge)
            {
                Destroy(floorSegments[i]);
                floorSegments.RemoveAt(i);
            }
        }
        
        for (int i = backgroundSegments.Count - 1; i >= 0; i--)
        {
            if (backgroundSegments[i] != null && 
                backgroundSegments[i].transform.position.x + backgroundSegmentWidth / 2f < leftEdge)
            {
                Destroy(backgroundSegments[i]);
                backgroundSegments.RemoveAt(i);
            }
        }
        
        for (int i = ceilingSegments.Count - 1; i >= 0; i--)
        {
            if (ceilingSegments[i] != null && 
                ceilingSegments[i].transform.position.x + ceilingSegmentWidth / 2f < leftEdge)
            {
                Destroy(ceilingSegments[i]);
                ceilingSegments.RemoveAt(i);
            }
        }
    }
    
    [ContextMenu("Clear All Segments")]
    public void ClearAllSegments()
    {
        foreach (var s in floorSegments) if (s) DestroyImmediate(s);
        foreach (var s in backgroundSegments) if (s) DestroyImmediate(s);
        foreach (var s in ceilingSegments) if (s) DestroyImmediate(s);
        
        floorSegments.Clear();
        backgroundSegments.Clear();
        ceilingSegments.Clear();
        
        lastFloorX = startX;
        lastBackgroundX = startX;
        lastCeilingX = startX;
    }
}