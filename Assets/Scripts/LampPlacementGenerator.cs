using UnityEngine;
using System.Collections.Generic;

public class LampPlacementGenerator : MonoBehaviour
{
    [Header("Generation Settings")]
    [SerializeField] private GameObject lampPlacementPrefab; // Prefab with LampPlacementPoint
    [SerializeField] private float placementInterval = 10f; // Distance between lamps
    [SerializeField] private int numberOfLamps = 10; // How many lamp spots to generate
    [SerializeField] private float startX = 0f; // Starting X position
    [SerializeField] private float placementY = 3f; // Y position for lamps (ceiling height)
    [SerializeField] private bool generateOnStart = true;
    [SerializeField] private bool followPlayerStartPosition = true; // Start generating from player position
    
    [Header("Debug")]
    [SerializeField] private bool regenerateLamps = false;
    [SerializeField] private bool clearAllLampPlacementPoints = false; // Clears ALL lamp placement points in scene
    
    private List<GameObject> generatedLamps = new List<GameObject>();
    
    void Start()
    {
        if (generateOnStart)
        {
            ClearGeneratedLamps(); // Clear any existing lamps first
            GenerateLampPlacements();
        }
    }
    
    void OnValidate()
    {
        if (regenerateLamps && Application.isPlaying)
        {
            regenerateLamps = false;
            RegenerateLamps();
        }
        
        if (clearAllLampPlacementPoints && Application.isPlaying)
        {
            clearAllLampPlacementPoints = false;
            ClearAllLampPointsInScene();
        }
    }
    
    public void RegenerateLamps()
    {
        Debug.LogWarning("[LampPlacementGenerator] Regenerating lamps with new settings");
        ClearGeneratedLamps();
        GenerateLampPlacements();
    }
    
    public void GenerateLampPlacements()
    {
        Debug.LogWarning($"[LampPlacementGenerator] Starting lamp placement generation. Count: {numberOfLamps}, Interval: {placementInterval}, Y: {placementY}");
        
        if (lampPlacementPrefab == null)
        {
            Debug.LogError("[LampPlacementGenerator] Lamp placement prefab is not assigned!");
            return;
        }
        
        // Check if prefab has required components
        LampPlacementPoint placementPoint = lampPlacementPrefab.GetComponent<LampPlacementPoint>();
        if (placementPoint == null)
        {
            Debug.LogError("[LampPlacementGenerator] Prefab doesn't have LampPlacementPoint component!");
            return;
        }
        
        // Determine starting position
        float actualStartX = startX;
        if (followPlayerStartPosition)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                actualStartX = player.transform.position.x - (placementInterval * 2); // Start a bit before player
                Debug.LogWarning($"[LampPlacementGenerator] Using player position as reference. Starting at X: {actualStartX}");
            }
        }
        
        // Generate lamp placement points
        for (int i = 0; i < numberOfLamps; i++)
        {
            float xPos = actualStartX + (i * placementInterval);
            Vector3 position = new Vector3(xPos, placementY, 0);
            
            GameObject lampSpot = Instantiate(lampPlacementPrefab, position, Quaternion.identity, transform);
            lampSpot.name = $"LampPlacementSpot_{i}";
            
            // Ensure it's set as empty placement
            LampPlacementPoint lampPoint = lampSpot.GetComponent<LampPlacementPoint>();
            if (lampPoint != null)
            {
                lampPoint.hasLampInstalled = false;
                Debug.LogWarning($"[LampPlacementGenerator] Created lamp spot at position ({xPos}, {placementY})");
            }
            
            generatedLamps.Add(lampSpot);
        }
        
        Debug.LogWarning($"[LampPlacementGenerator] Generated {numberOfLamps} lamp placement spots at Y={placementY}");
    }
    
    public void ClearGeneratedLamps()
    {
        Debug.LogWarning("[LampPlacementGenerator] Clearing all generated lamp spots");
        
        foreach (GameObject lamp in generatedLamps)
        {
            if (lamp != null)
            {
                if (Application.isPlaying)
                {
                    Destroy(lamp);
                }
                else
                {
                    DestroyImmediate(lamp);
                }
            }
        }
        
        generatedLamps.Clear();
    }
    
    public void ClearAllLampPointsInScene()
    {
        Debug.LogWarning("[LampPlacementGenerator] Clearing ALL lamp placement points in scene");
        
        // Find all lamp placement points in scene
        LampPlacementPoint[] allLampPoints = FindObjectsOfType<LampPlacementPoint>();
        int count = 0;
        
        foreach (var lampPoint in allLampPoints)
        {
            // Only destroy if it's not an installed lamp
            if (!lampPoint.hasLampInstalled)
            {
                if (Application.isPlaying)
                {
                    Destroy(lampPoint.gameObject);
                }
                else
                {
                    DestroyImmediate(lampPoint.gameObject);
                }
                count++;
            }
        }
        
        Debug.LogWarning($"[LampPlacementGenerator] Removed {count} empty lamp placement points");
        generatedLamps.Clear();
    }
    
    void OnDrawGizmos()
    {
        // Draw preview of where lamps will be placed
        Gizmos.color = Color.yellow;
        
        // Determine starting position for preview
        float previewStartX = startX;
        if (followPlayerStartPosition && Application.isPlaying)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                previewStartX = player.transform.position.x - (placementInterval * 2);
            }
        }
        
        for (int i = 0; i < numberOfLamps; i++)
        {
            float xPos = previewStartX + (i * placementInterval);
            Vector3 position = new Vector3(xPos, placementY, 0);
            
            Gizmos.DrawWireSphere(position, 0.5f);
            
            if (i < numberOfLamps - 1)
            {
                Vector3 nextPos = new Vector3(xPos + placementInterval, placementY, 0);
                Gizmos.DrawLine(position, nextPos);
            }
        }
        
        // Draw text info at generator position
        #if UNITY_EDITOR
        UnityEditor.Handles.Label(transform.position, $"Lamp Generator\nInterval: {placementInterval}\nY: {placementY}");
        #endif
    }
}