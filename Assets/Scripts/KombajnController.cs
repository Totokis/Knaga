using UnityEngine;
using System.Collections;

public class KombajnController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 0.5f;
    
    [Header("Mining Settings")]
    public float miningInterval = 10f;
    public float miningRange = 3f;
    
    private float nextMiningTime;
    private SpriteRenderer spriteRenderer;
    
    void Start()
    {
        nextMiningTime = Time.time + miningInterval;
        spriteRenderer = GetComponent<SpriteRenderer>();
    }
    
    void Update()
    {
        // Powolny ruch w prawo
        transform.position += Vector3.right * moveSpeed * Time.deltaTime;
        
        // Sprawdź czy czas na kruszenie
        if (Time.time >= nextMiningTime)
        {
            MineWall();
            nextMiningTime = Time.time + miningInterval;
        }
        
        // Wizualna informacja o czasie do następnego kruszenia
        float timeToMine = nextMiningTime - Time.time;
        if (timeToMine < 2f && spriteRenderer != null)
        {
            // Miganie przed kruszeniem
            float flash = Mathf.PingPong(Time.time * 4f, 1f);
            spriteRenderer.color = Color.Lerp(new Color(0.8f, 0.5f, 0.2f, 1f), Color.red, flash);
        }
        else if (spriteRenderer != null)
        {
            spriteRenderer.color = new Color(0.8f, 0.5f, 0.2f, 1f);
        }
    }
    
    void MineWall()
    {
        // Znajdź najbliższy segment ściany
        GameObject[] wallSegments = GameObject.FindGameObjectsWithTag("WallSegment");
        GameObject closestSegment = null;
        float closestDistance = float.MaxValue;
        
        foreach (GameObject segment in wallSegments)
        {
            float distance = Vector2.Distance(transform.position, segment.transform.position);
            if (distance < closestDistance && segment.transform.position.x > transform.position.x)
            {
                closestDistance = distance;
                closestSegment = segment;
            }
        }
        
        if (closestSegment != null && closestDistance < miningRange)
        {
            // Zniszcz segment
            Destroy(closestSegment);
            Debug.Log($"Skruszono segment ściany! Odległość: {closestDistance:F2}");
        }
        else
        {
            Debug.Log($"Brak segmentu w zasięgu. Najbliższy segment w odległości: {closestDistance:F2}");
        }
    }
}