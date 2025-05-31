using UnityEngine;

public class SimpleMachineVibration : MonoBehaviour
{
    [Header("Vibration Settings")]
    public float idleSpeed = 10f;
    public float idleAmount = 0.01f;
    public float miningSpeed = 25f;
    public float miningAmount = 0.05f;
    
    [Header("Special Effects")]
    public bool addRandomShake = true;
    public float shakeChance = 0.02f;
    public float shakeStrength = 2f;
    
    private Vector3 startPos;
    private MiningTriggerNew miningTrigger;
    private float currentSpeed;
    private float currentAmount;
    
    void Start()
    {
        startPos = transform.localPosition;
        
        // Find mining trigger in parent hierarchy
        Transform current = transform;
        while (current != null && miningTrigger == null)
        {
            miningTrigger = current.GetComponentInChildren<MiningTriggerNew>();
            if (miningTrigger == null)
            {
                current = current.parent;
            }
        }
        
        if (miningTrigger != null)
        {
            Debug.Log($"SimpleMachineVibration on {gameObject.name} found MiningTrigger!");
        }
        
        currentSpeed = idleSpeed;
        currentAmount = idleAmount;
    }
    
    void Update()
    {
        // Check if mining
        bool isMining = false;
        if (miningTrigger != null)
        {
            isMining = miningTrigger.IsMining();
        }
        
        // Smoothly transition between idle and mining
        float targetSpeed = isMining ? miningSpeed : idleSpeed;
        float targetAmount = isMining ? miningAmount : idleAmount;
        
        currentSpeed = Mathf.Lerp(currentSpeed, targetSpeed, Time.deltaTime * 3f);
        currentAmount = Mathf.Lerp(currentAmount, targetAmount, Time.deltaTime * 3f);
        
        // Calculate vibration
        float x = Mathf.Sin(Time.time * currentSpeed) * currentAmount;
        float y = Mathf.Cos(Time.time * currentSpeed * 0.7f) * currentAmount * 0.5f;
        
        // Add random shake
        if (addRandomShake && Random.Range(0f, 1f) < shakeChance)
        {
            x += Random.Range(-1f, 1f) * currentAmount * shakeStrength;
            y += Random.Range(-1f, 1f) * currentAmount * shakeStrength;
        }
        
        // Apply vibration - ONLY modify local position relative to start position
        transform.localPosition = startPos + new Vector3(x, y, 0);
    }
}