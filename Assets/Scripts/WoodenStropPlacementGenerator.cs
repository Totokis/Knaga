using UnityEngine;
using System.Collections.Generic;

public class WoodenStropPlacementGenerator : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private GameObject stropPlacementPrefab;
    [SerializeField] private float placementInterval = 8f;
    [SerializeField] private int numberOfStrops = 15;
    [SerializeField] private float startX = 0f;
    [SerializeField] private float placementY = 2f;
    
    [Header("Debug")]
    [SerializeField] private bool regenerateStrops = false;
    
    private List<GameObject> generatedStrops = new List<GameObject>();
    
    void Start()
    {
        ClearGenerated();
        GenerateStropPlacements();
    }
    
    void OnValidate()
    {
        if (regenerateStrops && Application.isPlaying)
        {
            regenerateStrops = false;
            ClearGenerated();
            GenerateStropPlacements();
        }
    }
    
    void GenerateStropPlacements()
    {
        if (stropPlacementPrefab == null) return;
        
        for (int i = 0; i < numberOfStrops; i++)
        {
            Vector3 pos = new Vector3(startX + i * placementInterval, placementY, 0);
            GameObject strop = Instantiate(stropPlacementPrefab, pos, Quaternion.identity, transform);
            strop.name = $"StropSpot_{i}";
            
            WoodenStropPlacementPoint point = strop.GetComponent<WoodenStropPlacementPoint>();
            if (point != null)
                point.hasStropInstalled = false;
                
            generatedStrops.Add(strop);
        }
    }
    
    void ClearGenerated()
    {
        foreach (var strop in generatedStrops)
        {
            if (strop != null)
                Destroy(strop);
        }
        generatedStrops.Clear();
    }
    
    void OnDrawGizmos()
    {
        Gizmos.color = new Color(0.6f, 0.4f, 0.2f, 0.5f);
        
        for (int i = 0; i < numberOfStrops; i++)
        {
            float xPos = startX + i * placementInterval;
            Vector3 position = new Vector3(xPos, placementY, 0);
            
            Gizmos.DrawCube(position, new Vector3(1f, 0.2f, 1f));
            
            if (i < numberOfStrops - 1)
            {
                Vector3 nextPos = new Vector3(xPos + placementInterval, placementY, 0);
                Gizmos.DrawLine(position, nextPos);
            }
        }
    }
}