using UnityEngine;

public class KombajnController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] public float moveSpeed = 0.3f;
    [SerializeField] public float stopAcceleration = 2f;
    [SerializeField] public float startAcceleration = 1f;
    
    private float currentSpeed;
    private bool canMove = true;
    private SpriteRenderer spriteRenderer;
    
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        currentSpeed = moveSpeed;
    }
    
    void Update()
    {
        // Płynne zatrzymywanie i ruszanie
        if (canMove)
        {
            currentSpeed = Mathf.MoveTowards(currentSpeed, moveSpeed, startAcceleration * Time.deltaTime);
        }
        else
        {
            currentSpeed = Mathf.MoveTowards(currentSpeed, 0f, stopAcceleration * Time.deltaTime);
        }
        
        // Ruch w prawo z aktualną prędkością
        if (currentSpeed > 0)
        {
            transform.position += Vector3.right * currentSpeed * Time.deltaTime;
        }
    }
    
    public void StopMoving()
    {
        canMove = false;
        Debug.Log("Kombajn: Zatrzymuję się do kruszenia");
    }
    
    public void StartMoving()
    {
        canMove = true;
        Debug.Log("Kombajn: Ruszam dalej");
    }
    
    public bool IsMoving()
    {
        return currentSpeed > 0.01f;
    }
    
    public bool IsStopped()
    {
        return currentSpeed < 0.01f;
    }
}