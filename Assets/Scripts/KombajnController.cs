using UnityEngine;

public class KombajnController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 0.5f;
    
    private SpriteRenderer spriteRenderer;
    
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }
    
    void Update()
    {
        // Powolny ruch w prawo
        transform.position += Vector3.right * moveSpeed * Time.deltaTime;
    }
}