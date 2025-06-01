using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class WallColorManager : MonoBehaviour
{
    [Header("Color Settings")]
    public Color darkWallColor = Color.black;
    public bool enableDynamicColoring = true;
    
    private WallVisualManager wallManager;
    private Dictionary<GameObject, Color> originalColors = new Dictionary<GameObject, Color>();
    
    void Start()
    {
        wallManager = GetComponent<WallVisualManager>();
        if (wallManager == null)
        {
            Debug.LogError("WallColorManager requires WallManager component!");
            enabled = false;
            return;
        }
        
        // Poczekaj chwile az WallManager wygeneruje sciany
        Invoke("InitializeColors", 0.1f);
    }
    
    void InitializeColors()
    {
        if (!enableDynamicColoring) return;
        
        // Znajdz wszystkie sciany
        List<GameObject> walls = new List<GameObject>();
        foreach (Transform child in transform)
        {
            if (child.CompareTag("WallSegment"))
            {
                walls.Add(child.gameObject);
                
                // Zapisz oryginalny kolor
                SpriteRenderer sr = child.GetComponent<SpriteRenderer>();
                if (sr != null)
                {
                    originalColors[child.gameObject] = sr.color;
                }
            }
        }
        
        UpdateWallColors(walls);
    }
    
    public void UpdateWallColors(List<GameObject> walls = null)
    {
        if (!enableDynamicColoring) return;
        
        // Jesli nie podano listy, znajdz sciany
        if (walls == null)
        {
            walls = new List<GameObject>();
            foreach (Transform child in transform)
            {
                if (child.CompareTag("WallSegment"))
                {
                    walls.Add(child.gameObject);
                }
            }
        }
        
        // Sortuj wedlug pozycji X
        walls = walls.Where(w => w != null).OrderBy(w => w.transform.position.x).ToList();
        
        // Ustaw kolory
        for (int i = 0; i < walls.Count; i++)
        {
            GameObject wall = walls[i];
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
                    // Pozostale - czarne
                    sr.color = darkWallColor;
                }
            }
        }
    }
    
    public void OnWallDestroyed(GameObject destroyedWall)
    {
        // Usun z dictionary
        if (originalColors.ContainsKey(destroyedWall))
        {
            originalColors.Remove(destroyedWall);
        }
        
        // Zaktualizuj kolory po zniszczeniu
        UpdateWallColors();
    }
}
