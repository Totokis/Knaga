using UnityEngine;
using UnityEngine.Rendering.Universal;

public class HeadlampController : MonoBehaviour
{
    [Header("References")]
    public Light2D headlamp;
    public Transform playerTransform;
    public SpriteRenderer playerSpriteRenderer;
    
    [Header("Direction Settings")]
    public float lightRotationSpeed = 15f;
    public float rightAngle = 270f;  // Kąt gdy gracz patrzy w prawo (flipX = false)
    public float leftAngle = 90f;    // Kąt gdy gracz patrzy w lewo (flipX = true)
    public float horizontalOffset = 1f; // Przesunięcie światła gdy patrzy w lewo
    
    [Header("Bounce Settings")]
    public float bounceAmplitude = 0.2f;  // Siła podskakiwania
    public float bounceFrequency = 10f;   // Szybkość podskakiwania
    public float minMovementSpeed = 0.01f; // Próg wykrywania ruchu
    
    private Vector3 originalLocalPosition;
    private float bounceTimer = 0f;
    private float currentTargetAngle;
    private Vector3 lastPosition;
    private bool lastFlipX = false;
    
    void Start()
    {
        // Znajdź komponenty jeśli nie są przypisane
        if (headlamp == null)
        {
            headlamp = GetComponent<Light2D>();
        }
        
        if (playerTransform == null && transform.parent != null)
        {
            playerTransform = transform.parent;
        }
        
        if (playerSpriteRenderer == null && playerTransform != null)
        {
            playerSpriteRenderer = playerTransform.GetComponent<SpriteRenderer>();
        }
        
        // Zapamiętaj oryginalną pozycję światła
        originalLocalPosition = transform.localPosition;
        lastPosition = playerTransform.position;
        
        // Ustaw początkowy kąt na podstawie flipX
        if (playerSpriteRenderer != null)
        {
            lastFlipX = playerSpriteRenderer.flipX;
            currentTargetAngle = playerSpriteRenderer.flipX ? leftAngle : rightAngle;
        }
    }
    
    void Update()
    {
        if (playerTransform == null || playerSpriteRenderer == null) return;
        
        // Sprawdź kierunek na podstawie flipX z SpriteRenderer
        bool currentFlipX = playerSpriteRenderer.flipX;
        
        // Ustaw docelowy kąt na podstawie flipX
        currentTargetAngle = currentFlipX ? leftAngle : rightAngle;
        
        // Płynnie obracaj światło do docelowego kąta
        Quaternion targetRotation = Quaternion.Euler(0, 0, currentTargetAngle);
        transform.localRotation = Quaternion.Lerp(transform.localRotation, targetRotation, Time.deltaTime * lightRotationSpeed);
        
        // Sprawdź czy gracz się porusza
        Vector3 currentPosition = playerTransform.position;
        float deltaX = Mathf.Abs(currentPosition.x - lastPosition.x);
        bool isMoving = deltaX > minMovementSpeed;
        
        // Oblicz bazową pozycję X z przesunięciem w zależności od kierunku
        // Gdy patrzy w lewo (flipX = true) -> przesuń o -1
        // Gdy patrzy w prawo (flipX = false) -> zostaw oryginalną pozycję
        float baseXOffset = currentFlipX ? -horizontalOffset : 0f;
        Vector3 basePosition = new Vector3(originalLocalPosition.x + baseXOffset, originalLocalPosition.y, originalLocalPosition.z);
        
        // Efekt podskakiwania gdy gracz się porusza
        if (isMoving)
        {
            bounceTimer += Time.deltaTime * bounceFrequency;
            
            // Używam Abs(Sin) dla bardziej wyraźnego efektu podskakiwania
            float bounceOffset = Mathf.Abs(Mathf.Sin(bounceTimer)) * bounceAmplitude;
            
            // Dodaj również lekkie ruchy na boki
            float sideOffset = Mathf.Sin(bounceTimer * 0.5f) * bounceAmplitude * 0.3f;
            
            // Kierunek ruchu na boki zależy od flipX
            float sideDirection = currentFlipX ? -1f : 1f;
            
            // Zastosuj wszystkie efekty
            transform.localPosition = basePosition + new Vector3(sideOffset * sideDirection, bounceOffset, 0);
        }
        else
        {
            // Gdy gracz stoi, powoli wróć do bazowej pozycji (z uwzględnieniem kierunku)
            bounceTimer = 0f;
            transform.localPosition = Vector3.Lerp(transform.localPosition, basePosition, Time.deltaTime * 8f);
        }
        
        // Zapisz pozycję dla następnej klatki
        lastPosition = currentPosition;
        lastFlipX = currentFlipX;
    }
    
    // Metoda pomocnicza do debugowania
    void OnDrawGizmosSelected()
    {
        if (headlamp != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawRay(transform.position, transform.right * 2f);
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, 0.2f);
            
            // Pokaż kierunek
            if (playerSpriteRenderer != null)
            {
                Gizmos.color = playerSpriteRenderer.flipX ? Color.blue : Color.green;
                Vector3 dir = playerSpriteRenderer.flipX ? Vector3.left : Vector3.right;
                Gizmos.DrawRay(transform.position, dir * 1f);
            }
        }
    }
}
