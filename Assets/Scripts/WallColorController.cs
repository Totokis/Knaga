using UnityEngine;
using System.Linq;

public class WallColorController : MonoBehaviour
{
    public Color darkColor = Color.black;
    
    void Start()
    {
        InvokeRepeating(nameof(UpdateWallColors), 0.5f, 1f);
    }
    
    void UpdateWallColors()
    {
        GameObject[] walls = GameObject.FindGameObjectsWithTag("WallSegment");
        if (walls.Length == 0) return;
        
        // Sort by X position
        walls = walls.OrderBy(w => w.transform.position.x).ToArray();
        
        // Apply colors
        for (int i = 0; i < walls.Length; i++)
        {
            SpriteRenderer sr = walls[i].GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                if (i == 0)
                {
                    // First wall keeps original color
                    // Do nothing - leave as is
                }
                else
                {
                    // Other walls become dark
                    sr.color = darkColor;
                }
            }
        }
    }
}
