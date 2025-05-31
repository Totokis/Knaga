using UnityEngine;
using System.Collections;

public class MiningTrigger : MonoBehaviour
{
    [Header("Mining Settings")]
    public float miningTime = 7f;
    
    private GameObject currentWallSegment;
    private float miningTimer = 0f;
    private bool isMining = false;
    private Coroutine miningCoroutine;
    private Vector3 originalWallPosition;
    
    void Start()
    {
        // Upewnij się, że mamy trigger collider
        BoxCollider2D collider = GetComponent<BoxCollider2D>();
        if (collider == null)
        {
            collider = gameObject.AddComponent<BoxCollider2D>();
        }
        collider.isTrigger = true;
        collider.size = new Vector2(0.8f, 1.8f); // Trochę większy niż organ
    }
    
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("WallSegment"))
        {
            // Sprawdź czy to najbardziej lewy segment (tylko on może być niszczony)
            if (IsLeftmostWall(other.gameObject))
            {
                currentWallSegment = other.gameObject;
                originalWallPosition = currentWallSegment.transform.localPosition;
                StartMining();
                Debug.Log($"Rozpoczęto kruszenie ściany: {currentWallSegment.name}");
            }
        }
    }
    
    void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject == currentWallSegment)
        {
            Debug.Log("Przerwano kruszenie - organ wyszedł z triggera");
            StopMining();
        }
    }
    
    bool IsLeftmostWall(GameObject wall)
    {
        GameObject[] allWalls = GameObject.FindGameObjectsWithTag("WallSegment");
        float minX = float.MaxValue;
        GameObject leftmostWall = null;
        
        foreach (GameObject w in allWalls)
        {
            if (w.transform.position.x < minX)
            {
                minX = w.transform.position.x;
                leftmostWall = w;
            }
        }
        
        return wall == leftmostWall;
    }
    
    void StartMining()
    {
        if (!isMining && currentWallSegment != null)
        {
            isMining = true;
            miningTimer = 0f;
            if (miningCoroutine != null)
            {
                StopCoroutine(miningCoroutine);
            }
            miningCoroutine = StartCoroutine(MiningProcess());
        }
    }
    
    void StopMining()
    {
        isMining = false;
        miningTimer = 0f;
        if (currentWallSegment != null)
        {
            // Przywróć oryginalny kolor i pozycję
            SpriteRenderer sr = currentWallSegment.GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                sr.color = new Color(0.6f, 0.4f, 0.2f, 1f);
            }
            currentWallSegment.transform.localPosition = originalWallPosition;
        }
        currentWallSegment = null;
        if (miningCoroutine != null)
        {
            StopCoroutine(miningCoroutine);
            miningCoroutine = null;
        }
    }
    
    IEnumerator MiningProcess()
    {
        SpriteRenderer wallRenderer = currentWallSegment?.GetComponent<SpriteRenderer>();
        Color originalColor = new Color(0.6f, 0.4f, 0.2f, 1f);
        
        while (isMining && currentWallSegment != null && miningTimer < miningTime)
        {
            miningTimer += Time.deltaTime;
            float progress = miningTimer / miningTime;
            
            // Wizualna informacja o postępie kruszenia
            if (wallRenderer != null)
            {
                // Pulsowanie intensywności w zależności od postępu
                float pulseSpeed = Mathf.Lerp(2f, 10f, progress);
                float pulse = Mathf.Sin(Time.time * pulseSpeed) * 0.3f + 0.7f;
                
                // Zmiana koloru od brązowego przez pomarańczowy do czerwonego
                Color targetColor = Color.Lerp(new Color(1f, 0.5f, 0f), Color.red, progress);
                wallRenderer.color = Color.Lerp(originalColor, targetColor, pulse);
                
                // Drżenie ściany - coraz silniejsze
                float shakeIntensity = progress * 0.15f;
                currentWallSegment.transform.localPosition = originalWallPosition + new Vector3(
                    Random.Range(-shakeIntensity, shakeIntensity),
                    Random.Range(-shakeIntensity, shakeIntensity),
                    0
                );
            }
            
            // Debug info co sekundę
            if (Mathf.FloorToInt(miningTimer) != Mathf.FloorToInt(miningTimer - Time.deltaTime))
            {
                Debug.Log($"Kruszenie: {Mathf.Ceil(miningTimer)}/{miningTime}s");
            }
            
            yield return null;
        }
        
        // Jeśli minął czas, zniszcz ścianę
        if (miningTimer >= miningTime && currentWallSegment != null)
        {
            // Powiadom WallManager
            WallManager wallManager = FindObjectOfType<WallManager>();
            if (wallManager != null)
            {
                wallManager.OnWallDestroyed(currentWallSegment);
            }
            
            // Efekt zniszczenia
            Debug.Log($"SUKCES! Skruszono ścianę '{currentWallSegment.name}' po {miningTime} sekundach!");
            
            // Zniszcz ścianę
            Destroy(currentWallSegment);
            currentWallSegment = null;
        }
        
        isMining = false;
        miningCoroutine = null;
    }
    
    void OnDestroy()
    {
        if (miningCoroutine != null)
        {
            StopCoroutine(miningCoroutine);
        }
    }
}