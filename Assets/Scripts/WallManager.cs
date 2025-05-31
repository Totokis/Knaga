using UnityEngine;
using System.Collections.Generic;

public class WallManager : MonoBehaviour
{
    [Header("Wall Generation Settings")]
    public GameObject wallSegmentPrefab;
    public int numberOfSegments = 20;
    public float segmentSpacing = 1f;
    public float startX = 25f;
    
    [Header("Wall Properties")]
    public Color wallColor = new Color(0.6f, 0.4f, 0.2f, 1f);
    
    private List<GameObject> wallSegments = new List<GameObject>();
    private Sprite wallSprite;
    
    void Start()
    {
        // Znajdź pierwszy segment ściany i użyj go jako wzoru
        GameObject firstSegment = GameObject.Find("WallSegment1");
        if (firstSegment != null)
        {
            wallSegmentPrefab = firstSegment;
            startX = firstSegment.transform.position.x;
            
            // Pobierz sprite z pierwszego segmentu
            SpriteRenderer sr = firstSegment.GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                wallSprite = sr.sprite;
                wallColor = sr.color;
            }
        }
        
        GenerateWalls();
    }
    
    void GenerateWalls()
    {
        // Dodaj pierwszy segment do listy
        GameObject firstSegment = GameObject.Find("WallSegment1");
        if (firstSegment != null)
        {
            wallSegments.Add(firstSegment);
        }
        
        // Generuj pozostałe segmenty
        for (int i = 1; i < numberOfSegments; i++)
        {
            float xPos = startX + (i * segmentSpacing);
            CreateWallSegment(xPos, i + 1);
        }
    }
    
    void CreateWallSegment(float xPosition, int index)
    {
        // Klonuj pierwszy segment jeśli mamy prefab
        GameObject newSegment;
        
        if (wallSegmentPrefab != null)
        {
            newSegment = Instantiate(wallSegmentPrefab);
            newSegment.name = $"WallSegment{index}";
            newSegment.transform.position = new Vector3(xPosition, 0, 0);
        }
        else
        {
            // Tworzymy nowy segment od zera
            newSegment = new GameObject($"WallSegment{index}");
            newSegment.transform.position = new Vector3(xPosition, 0, 0);
            newSegment.transform.localScale = new Vector3(1, 4, 1);
            newSegment.tag = "WallSegment";
            
            // Dodaj SpriteRenderer
            SpriteRenderer sr = newSegment.AddComponent<SpriteRenderer>();
            sr.color = wallColor;
            if (wallSprite != null)
            {
                sr.sprite = wallSprite;
            }
            
            // Dodaj BoxCollider2D
            BoxCollider2D collider = newSegment.AddComponent<BoxCollider2D>();
            collider.size = new Vector2(1, 4);
        }
        
        // Ustaw parent
        newSegment.transform.SetParent(transform);
        
        wallSegments.Add(newSegment);
    }
    
    public void OnWallDestroyed(GameObject destroyedWall)
    {
        // Usuń z listy
        wallSegments.Remove(destroyedWall);
        
        // Generuj nowy segment na końcu
        if (wallSegments.Count > 0)
        {
            float lastX = wallSegments[wallSegments.Count - 1].transform.position.x;
            CreateWallSegment(lastX + segmentSpacing, wallSegments.Count + numberOfSegments);
        }
    }
}