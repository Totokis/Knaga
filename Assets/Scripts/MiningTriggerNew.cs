using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MiningTriggerNew : MonoBehaviour
{
    [Header("Mining Settings")]
    [SerializeField] public float miningTime = 7f;
    
    private GameObject currentWallSegment;
    private float miningTimer = 0f;
    private bool isMining = false;
    private Coroutine miningCoroutine;
    private KombajnController kombajnController;
    private List<GameObject> wallsInTrigger = new List<GameObject>();
    
    // Public getter for mining state
    public bool IsMining() { return isMining; }
    
    void Start()
    {
        kombajnController = GetComponentInParent<KombajnController>();
        if (kombajnController == null)
        {
            Debug.LogError("KombajnController not found in parent!");
        }
        
        BoxCollider2D collider = GetComponent<BoxCollider2D>();
        if (collider != null) 
        {
            collider.isTrigger = true;
            Debug.Log("MiningTrigger initialized with collider size: " + collider.size);
        }
    }
    
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("WallSegment"))
        {
            wallsInTrigger.Add(other.gameObject);
            Debug.Log("Wall entered trigger: " + other.name);
            TryStartMining();
        }
    }
    
    void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("WallSegment") && !isMining)
        {
            if (!wallsInTrigger.Contains(other.gameObject))
            {
                wallsInTrigger.Add(other.gameObject);
            }
            TryStartMining();
        }
    }
    
    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("WallSegment"))
        {
            wallsInTrigger.Remove(other.gameObject);
            Debug.Log("Wall exited trigger: " + other.name);
        }
    }
    
    void TryStartMining()
    {
        if (isMining) return;
        
        wallsInTrigger.RemoveAll(w => w == null);
        GameObject leftmost = GetLeftmostWall();
        
        if (leftmost != null)
        {
            currentWallSegment = leftmost;
            Debug.Log("Starting to mine wall: " + leftmost.name);
            StartMining();
        }
    }
    
    GameObject GetLeftmostWall()
    {
        GameObject[] allWalls = GameObject.FindGameObjectsWithTag("WallSegment");
        GameObject leftmost = null;
        float minX = float.MaxValue;
        
        // Find leftmost wall in trigger
        foreach (GameObject wall in wallsInTrigger)
        {
            if (wall != null && wall.transform.position.x < minX)
            {
                minX = wall.transform.position.x;
                leftmost = wall;
            }
        }
        
        if (leftmost == null) return null;
        
        // Check if it's the global leftmost
        foreach (GameObject wall in allWalls)
        {
            if (wall != null && wall != leftmost && wall.transform.position.x < leftmost.transform.position.x)
            {
                Debug.Log("Wall " + leftmost.name + " is not the leftmost globally");
                return null;
            }
        }
        
        return leftmost;
    }
    
    void StartMining()
    {
        isMining = true;
        miningTimer = 0f;
        if (kombajnController != null) 
        {
            kombajnController.StopMoving();
        }
        if (miningCoroutine != null) StopCoroutine(miningCoroutine);
        miningCoroutine = StartCoroutine(MineWall());
    }
    
    IEnumerator MineWall()
    {
        // Wait for kombajn to stop
        if (kombajnController != null)
        {
            while (!kombajnController.IsStopped())
            {
                yield return null;
            }
            Debug.Log("Kombajn stopped, starting mining process");
        }
        
        Vector3 originalPos = currentWallSegment.transform.localPosition;
        SpriteRenderer sr = currentWallSegment.GetComponent<SpriteRenderer>();
        
        while (miningTimer < miningTime && currentWallSegment != null)
        {
            miningTimer += Time.deltaTime;
            float progress = miningTimer / miningTime;
            
            if (sr != null)
            {
                float pulse = Mathf.Sin(Time.time * 5f) * 0.3f + 0.7f;
                sr.color = Color.Lerp(new Color(0.6f, 0.4f, 0.2f), Color.red, progress * pulse);
            }
            
            float shake = progress * 0.1f;
            currentWallSegment.transform.localPosition = originalPos + 
                new Vector3(Random.Range(-shake, shake), Random.Range(-shake, shake), 0);
            
            // Log progress every second
            if (Mathf.FloorToInt(miningTimer) != Mathf.FloorToInt(miningTimer - Time.deltaTime))
            {
                Debug.Log("Mining progress: " + Mathf.Ceil(miningTimer) + "/" + miningTime + "s");
            }
            
            yield return null;
        }
        
        if (currentWallSegment != null)
        {
            Debug.Log("Mining complete! Destroying wall: " + currentWallSegment.name);
            wallsInTrigger.Remove(currentWallSegment);
            
            WallManager wm = FindObjectOfType<WallManager>();
            if (wm != null) wm.OnWallDestroyed(currentWallSegment);
            
            Destroy(currentWallSegment);
        }
        
        if (kombajnController != null) 
        {
            kombajnController.StartMoving();
        }
        
        // Short delay before checking for next wall
        yield return new WaitForSeconds(0.5f);
        
        isMining = false;
        currentWallSegment = null;
        
        // Check for next wall
        TryStartMining();
    }
}