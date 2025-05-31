using UnityEngine;
using System.Collections.Generic;

public class EnvironmentManager : MonoBehaviour
{
    [Header("Prefabs")]
    [SerializeField] public GameObject floorSegmentPrefab;
    [SerializeField] public GameObject backgroundSegmentPrefab;
    [SerializeField] public GameObject ceilingSegmentPrefab;
    
    [Header("Generation Settings")]
    [SerializeField] public int segmentsToGenerate = 30;
    [SerializeField] public float segmentWidth = 10f;
    [SerializeField] public float startX = -10f;
    
    [Header("Position Settings")]
    [SerializeField] public float floorY = -3f;
    [SerializeField] public float backgroundY = 0f;
    [SerializeField] public float backgroundZ = 2f;
    [SerializeField] public float ceilingY = 3f;
    
    [Header("Dynamic Generation")]
    [SerializeField] public bool enableDynamicGeneration = true;
    [SerializeField] public float generateAheadDistance = 20f;
    [SerializeField] public float removeBackDistance = 30f;
    
    private List<GameObject> floorSegments = new List<GameObject>();
    private List<GameObject> backgroundSegments = new List<GameObject>();
    private List<GameObject> ceilingSegments = new List<GameObject>();
    
    private Transform player;
    private float lastGeneratedX;
    
    void Start()
    {
        // Find player
        GameObject playerObj = GameObject.Find("Player");
        if (playerObj != null) player = playerObj.transform;
        
        lastGeneratedX = startX;
        GenerateInitialSegments();
    }
    
    void Update()
    {
        if (enableDynamicGeneration && player != null)
        {
            float playerX = player.position.x;
            float rightEdge = playerX + generateAheadDistance;
            
            // Generate new segments ahead
            while (lastGeneratedX < rightEdge)
            {
                GenerateSegmentSet(lastGeneratedX);
                lastGeneratedX += segmentWidth;
            }
            
            // Remove old segments behind
            float leftEdge = playerX - removeBackDistance;
            RemoveOldSegments(leftEdge);
        }
    }
    
    void GenerateInitialSegments()
    {
        for (int i = 0; i < segmentsToGenerate; i++)
        {
            float xPos = startX + (i * segmentWidth);
            GenerateSegmentSet(xPos);
        }
        lastGeneratedX = startX + (segmentsToGenerate * segmentWidth);
    }
    
    void GenerateSegmentSet(float xPosition)
    {
        // Generate floor
        if (floorSegmentPrefab != null)
        {
            GameObject floor = Instantiate(floorSegmentPrefab, transform);
            floor.name = "FloorSegment_" + floorSegments.Count;
            floor.transform.position = new Vector3(xPosition, floorY, 0);
            AdjustSegmentScale(floor, segmentWidth);
            floorSegments.Add(floor);
        }
        
        // Generate background
        if (backgroundSegmentPrefab != null)
        {
            GameObject bg = Instantiate(backgroundSegmentPrefab, transform);
            bg.name = "BackgroundSegment_" + backgroundSegments.Count;
            bg.transform.position = new Vector3(xPosition, backgroundY, backgroundZ);
            AdjustSegmentScale(bg, segmentWidth);
            backgroundSegments.Add(bg);
        }
        
        // Generate ceiling
        if (ceilingSegmentPrefab != null)
        {
            GameObject ceiling = Instantiate(ceilingSegmentPrefab, transform);
            ceiling.name = "CeilingSegment_" + ceilingSegments.Count;
            ceiling.transform.position = new Vector3(xPosition, ceilingY, 0);
            AdjustSegmentScale(ceiling, segmentWidth);
            ceilingSegments.Add(ceiling);
        }
    }
    
    void AdjustSegmentScale(GameObject segment, float targetWidth)
    {
        SpriteRenderer sr = segment.GetComponent<SpriteRenderer>();
        if (sr != null && sr.sprite != null)
        {
            float spriteWidth = sr.sprite.bounds.size.x;
            float scaleX = targetWidth / spriteWidth;
            segment.transform.localScale = new Vector3(scaleX, segment.transform.localScale.y, 1);
        }
    }
    
    void RemoveOldSegments(float leftEdge)
    {
        RemoveSegmentsBehind(floorSegments, leftEdge);
        RemoveSegmentsBehind(backgroundSegments, leftEdge);
        RemoveSegmentsBehind(ceilingSegments, leftEdge);
    }
    
    void RemoveSegmentsBehind(List<GameObject> segments, float leftEdge)
    {
        for (int i = segments.Count - 1; i >= 0; i--)
        {
            if (segments[i] != null && segments[i].transform.position.x < leftEdge)
            {
                Destroy(segments[i]);
                segments.RemoveAt(i);
            }
        }
    }
    
    [ContextMenu("Regenerate Environment")]
    public void RegenerateEnvironment()
    {
        // Clear existing
        foreach (var s in floorSegments) if (s != null) DestroyImmediate(s);
        foreach (var s in backgroundSegments) if (s != null) DestroyImmediate(s);
        foreach (var s in ceilingSegments) if (s != null) DestroyImmediate(s);
        
        floorSegments.Clear();
        backgroundSegments.Clear();
        ceilingSegments.Clear();
        
        lastGeneratedX = startX;
        GenerateInitialSegments();
    }
}