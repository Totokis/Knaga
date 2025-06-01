using UnityEngine;

public class CuttingOrganRotation : MonoBehaviour
{
    [Header("Rotation Settings")]
    public float minRotation = -7f;     // Minimalna rotacja w stopniach
    public float maxRotation = 7f;      // Maksymalna rotacja w stopniach
    public float rotationSpeed = 2f;    // Prędkość zmiany rotacji podczas kopania
    public float idleSpeed = 0.5f;      // Prędkość gdy nie kopie (powolne ruchy)
    public float smoothSpeed = 3f;      // Prędkość wygładzania przejść
    
    [Header("Noise Settings")]
    public bool addNoise = true;        // Dodatkowe drgania
    public float noiseStrength = 1f;    // Siła dodatkowych drgań
    public float noiseFrequency = 10f;  // Częstotliwość drgań
    
    private MiningTriggerNew miningTrigger;
    private float currentRotation = 0f;
    private float targetRotation = 0f;
    private float vibrationTime = 0f;
    private float currentSpeed;
    private bool wasMining = false;
    
    void Start()
    {
        // Znajdź MiningTriggerNew w tej samej hierarchii
        Transform current = transform;
        while (current != null && miningTrigger == null)
        {
            miningTrigger = current.GetComponent<MiningTriggerNew>();
            if (miningTrigger == null && current.parent != null)
            {
                miningTrigger = current.parent.GetComponentInChildren<MiningTriggerNew>();
            }
            current = current.parent;
        }
        
        if (miningTrigger != null)
        {
            Debug.Log($"CuttingOrganRotation znalazł MiningTrigger na {miningTrigger.gameObject.name}");
        }
        
        // Zachowaj obecną rotację jako punkt startowy
        currentRotation = transform.localEulerAngles.z;
        if (currentRotation > 180f) currentRotation -= 360f;
        targetRotation = currentRotation;
        currentSpeed = idleSpeed;
    }
    
    void Update()
    {
        // Sprawdź czy kopie
        bool isMining = false;
        if (miningTrigger != null)
        {
            isMining = miningTrigger.IsMining();
        }
        
        // Płynnie przejdź między prędkościami
        float targetSpeed = isMining ? rotationSpeed : idleSpeed;
        currentSpeed = Mathf.Lerp(currentSpeed, targetSpeed, Time.deltaTime * smoothSpeed);
        
        // Jeśli kopie lub właśnie przestał
        if (isMining || wasMining)
        {
            vibrationTime += Time.deltaTime * currentSpeed;
            
            // Główna oscylacja
            targetRotation = Mathf.Sin(vibrationTime) * (maxRotation - minRotation) * 0.5f;
            
            // Dodaj szum/drgania jeśli włączone
            if (addNoise && isMining)
            {
                float noise = Mathf.PerlinNoise(Time.time * noiseFrequency, 0f) - 0.5f;
                targetRotation += noise * noiseStrength;
            }
            
            // Ogranicz do zakresu
            targetRotation = Mathf.Clamp(targetRotation, minRotation, maxRotation);
        }
        
        // Płynnie przejdź do docelowej rotacji
        currentRotation = Mathf.Lerp(currentRotation, targetRotation, Time.deltaTime * currentSpeed * 2f);
        
        // Zastosuj rotację
        Vector3 currentEuler = transform.localEulerAngles;
        currentEuler.z = currentRotation;
        transform.localEulerAngles = currentEuler;
        
        wasMining = isMining;
    }
    
    // Pomocnicza metoda do debugowania
    void OnDrawGizmosSelected()
    {
        // Pokaż zakres rotacji
        Gizmos.color = Color.yellow;
        Vector3 center = transform.position;
        
        // Linia pokazująca minimalną rotację
        Gizmos.color = Color.red;
        Vector3 minDir = Quaternion.Euler(0, 0, minRotation) * transform.right;
        Gizmos.DrawRay(center, minDir * 2f);
        
        // Linia pokazująca maksymalną rotację
        Gizmos.color = Color.green;
        Vector3 maxDir = Quaternion.Euler(0, 0, maxRotation) * transform.right;
        Gizmos.DrawRay(center, maxDir * 2f);
        
        // Aktualna rotacja
        Gizmos.color = Color.white;
        Gizmos.DrawRay(center, transform.right * 2.5f);
    }
}
