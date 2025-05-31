using UnityEngine;

public class DrillRotation : MonoBehaviour
{
    [Header("Rotation Settings")]
    public float miningSpeed = 720f; // degrees per second when mining - fast!
    public float idleSpeed = 45f; // degrees per second when idle - slow rotation
    public float acceleration = 8f; // how fast to change speed
    
    [Header("Visual Effects")]
    public float vibrationIntensity = 0.03f; // how much to shake when mining
    public float vibrationFrequency = 80f; // how fast to vibrate
    public bool enableColorPulse = true; // whether to pulse color when mining
    
    [Header("Spark Effects")]
    public GameObject sparkEffectPrefab; // optional spark effect
    public Transform sparkSpawnPoint; // where to spawn sparks
    public float sparkInterval = 0.1f; // how often to spawn sparks
    
    private float currentSpeed = 0f;
    private MiningTriggerNew miningTrigger;
    private bool wasMiningSoundPlaying = false;
    private Vector3 originalLocalPosition;
    private float lastSparkTime = 0f;
    private Transform drillSparks;
    
    void Start()
    {
        // Find MiningTriggerNew in parent (OrganTnacy)
        Transform parent = transform.parent;
        if (parent != null)
        {
            miningTrigger = parent.GetComponent<MiningTriggerNew>();
            if (miningTrigger == null)
            {
                Debug.LogError("MiningTriggerNew not found on parent!");
            }
        }
        
        // Find DrillSparks child object
        drillSparks = transform.Find("DrillSparks");
        if (drillSparks != null)
        {
            drillSparks.gameObject.SetActive(false); // Hide initially
        }
        
        currentSpeed = idleSpeed;
        originalLocalPosition = transform.localPosition;
    }
    
    void Update()
    {
        if (miningTrigger == null) return;
        
        // Check if mining
        bool isMining = miningTrigger.IsMining();
        
        // Set target speed based on mining state
        float targetSpeed = isMining ? miningSpeed : idleSpeed;
        
        // Smoothly transition to target speed
        currentSpeed = Mathf.Lerp(currentSpeed, targetSpeed, acceleration * Time.deltaTime);
        
        // Rotate the drill around Z axis (2D rotation)
        transform.Rotate(0, 0, currentSpeed * Time.deltaTime);
        
        // Visual effects when mining
        if (isMining)
        {
            // Add vibration when mining
            float vibrationX = Mathf.Sin(Time.time * vibrationFrequency) * vibrationIntensity;
            float vibrationY = Mathf.Cos(Time.time * vibrationFrequency * 0.7f) * vibrationIntensity * 0.5f;
            
            transform.localPosition = originalLocalPosition + new Vector3(vibrationX, vibrationY, 0);
            
            // Optional: Add color pulsing effect if sprite renderer exists
            if (enableColorPulse)
            {
                SpriteRenderer sr = GetComponent<SpriteRenderer>();
                if (sr != null)
                {
                    // Slight red/orange tint when mining hard
                    float pulse = Mathf.Sin(Time.time * 3f) * 0.3f + 0.7f;
                    sr.color = new Color(1f, pulse * 0.9f, pulse * 0.7f);
                }
            }
            
            // Show spark effects
            if (drillSparks != null)
            {
                drillSparks.gameObject.SetActive(true);
                
                // Animate spark position slightly
                float sparkOffset = Mathf.Sin(Time.time * 20f) * 0.05f;
                drillSparks.localPosition = new Vector3(0.5f + sparkOffset, sparkOffset * 0.5f, -0.1f);
                
                // Optional: Rotate sparks for dynamic effect
                drillSparks.Rotate(0, 0, 500f * Time.deltaTime);
            }
            
            // Spawn spark particles (if prefab is set)
            if (sparkEffectPrefab != null && Time.time - lastSparkTime > sparkInterval)
            {
                Vector3 spawnPos = sparkSpawnPoint != null ? sparkSpawnPoint.position : transform.position;
                GameObject spark = Instantiate(sparkEffectPrefab, spawnPos, Quaternion.identity);
                Destroy(spark, 1f); // Clean up after 1 second
                lastSparkTime = Time.time;
            }
            
            // Log mining state change
            if (!wasMiningSoundPlaying)
            {
                Debug.Log("Drill spinning at full speed - MINING! Sparks flying!");
                wasMiningSoundPlaying = true;
            }
        }
        else
        {
            // Smoothly return to original position when not mining
            transform.localPosition = Vector3.Lerp(
                transform.localPosition, 
                originalLocalPosition, 
                Time.deltaTime * 5f
            );
            
            // Reset color
            if (enableColorPulse)
            {
                SpriteRenderer sr = GetComponent<SpriteRenderer>();
                if (sr != null)
                {
                    sr.color = Color.Lerp(sr.color, Color.white, Time.deltaTime * 3f);
                }
            }
            
            // Hide spark effects
            if (drillSparks != null)
            {
                drillSparks.gameObject.SetActive(false);
            }
            
            if (wasMiningSoundPlaying)
            {
                Debug.Log("Drill slowing down - idle rotation");
                wasMiningSoundPlaying = false;
            }
        }
    }
}