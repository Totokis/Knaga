using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class WallVisualManager : MonoBehaviour
{
    [Header("Color Settings")]
    public Color darkColor = Color.black;
    public bool autoUpdate = true;
    
    private Dictionary<GameObject, Color> originalColors = new Dictionary<GameObject, Color>();
    private List<GameObject> walls = new List<GameObject>();
    private int lastWallCount = 0;
    
    void Start()
    {
        // Poczekaj chwile na generacje scian
        Invoke(nameof(Initialize), 0.5f);
    }
    
    void Initialize()
    {
        CollectWalls();
        ApplyColors();
    }
    
    void Update()
    {
        if (!autoUpdate) return;
        
        // Sprawdz czy liczba scian sie zmienila
        int currentCount = transform.childCount;
        if (currentCount != lastWallCount)
        {
            lastWallCount = currentCount;
            CollectWalls();
            ApplyColors();
        }
    }
    
    void CollectWalls()
    {
        walls.Clear();
        
        foreach (Transform child in transform)
        {
            if (child.CompareTag("WallSegment"))
            {
                GameObject wall = child.gameObject;
                walls.Add(wall);
                
                // Zapisz oryginalny kolor jesli nie mamy
                if (!originalColors.ContainsKey(wall))
                {
                    SpriteRenderer sr = wall.GetComponent<SpriteRenderer>();
                    if (sr != null)
                    {
                        originalColors[wall] = sr.color;
                    }
                }
            }
        }
        
        // Sortuj po pozycji X
        walls = walls.OrderBy(w => w.transform.position.x).ToList();
    }
    
    void ApplyColors()
    {
        for (int i = 0; i < walls.Count; i++)
        {
            GameObject wall = walls[i];
            if (wall == null) continue;
            
            SpriteRenderer sr = wall.GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                if (i == 0)
                {
                    // Pierwsza sciana - oryginalny kolor
                    if (originalColors.ContainsKey(wall))
                    {
                        sr.color = originalColors[wall];
                    }
                }
                else
                {
                    // Reszta - ciemny kolor
                    sr.color = darkColor;
                }
            }
        }
    }
    
    // Recznie wywolaj aktualizacje
    public void RefreshColors()
    {
        CollectWalls();
        ApplyColors();
    }
}
