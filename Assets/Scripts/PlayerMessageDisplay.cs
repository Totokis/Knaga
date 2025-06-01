using UnityEngine;
using TMPro;
using System.Collections;

public class PlayerMessageDisplay : MonoBehaviour
{
    [Header("Message Settings")]
    public float messageHeight = 1.5f; // Wysokość nad graczem
    public float messageDuration = 3f; // Czas wyświetlania
    public float fadeSpeed = 2f;
    public float textSize = 18f;
    public Color defaultColor = Color.white;

    
    [Header("Item Sprites")]
    public Sprite woodSprite;
    public Sprite oilSprite;
    public Sprite metalSprite;
    
    private TextMeshPro messageText;
    private Transform playerTransform;
    private Coroutine currentMessageCoroutine;
    
    private static PlayerMessageDisplay instance;
    public static PlayerMessageDisplay Instance => instance;
    
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }
    
    void Start()
    {
        // Znajdź gracza
        GameObject player = GameObject.Find("Player");
        if (player != null)
        {
            playerTransform = player.transform;
        }
        
        SetupMessageDisplay();
    }
    
    void SetupMessageDisplay()
    {
        // Tworzenie obiektu dla tekstu
        GameObject textObj = new GameObject("PlayerMessage");
        textObj.transform.parent = transform;
        
        // Dodanie TextMeshPro
        messageText = textObj.AddComponent<TextMeshPro>();
        messageText.fontSize = textSize;
        messageText.color = defaultColor;
        messageText.alignment = TextAlignmentOptions.Center;
        messageText.text = "";
        
        // Ustawienie sortowania żeby było na wierzchu
        messageText.sortingOrder = 1001;
        MeshRenderer textRenderer = messageText.GetComponent<MeshRenderer>();
        if (textRenderer != null)
        {
            textRenderer.sortingLayerName = "UI";
            textRenderer.sortingOrder = 1001;
        }
        
        // Włączenie outline dla lepszej widoczności
        messageText.fontMaterial.EnableKeyword("OUTLINE_ON");
        messageText.outlineColor = Color.black;
        messageText.outlineWidth = 0.2f;
    }
    
    void Update()
    {
        if (playerTransform != null && messageText != null)
        {
            // Pozycjonowanie nad graczem
            Vector3 messagePos = playerTransform.position + Vector3.up * messageHeight;
            transform.position = messagePos;
        }
    }
    
    public void ShowMessage(string message, Color? color = null, float? duration = null)
    {
        if (messageText == null) return;
        
        // Zatrzymaj poprzednią wiadomość
        if (currentMessageCoroutine != null)
        {
            StopCoroutine(currentMessageCoroutine);
        }
        
        // Ustaw kolor
        Color msgColor = color ?? defaultColor;
        float msgDuration = duration ?? messageDuration;
        
        // Rozpocznij wyświetlanie
        currentMessageCoroutine = StartCoroutine(DisplayMessage(message, msgColor, msgDuration));
    }
    
    private IEnumerator DisplayMessage(string message, Color color, float duration)
    {
        // Pokaż wiadomość
        messageText.text = message;
        messageText.color = color;
        messageText.alpha = 1f;
        
        // Czekaj
        yield return new WaitForSeconds(duration);
        
        // Zanikanie
        float fadeTimer = 0f;
        while (fadeTimer < 1f)
        {
            fadeTimer += Time.deltaTime * fadeSpeed;
            messageText.alpha = Mathf.Lerp(1f, 0f, fadeTimer);
            yield return null;
        }
        
        // Wyczyść tekst
        messageText.text = "";
        messageText.alpha = 1f;
        currentMessageCoroutine = null;
    }
    
    // Metody pomocnicze dla różnych typów wiadomości
    public void ShowPickupPrompt(string itemName)
    {
        ShowMessage($"Press E to pick up {itemName}", Color.yellow, 2f);
    }
    
    public void ShowInventoryFull()
    {
        ShowMessage("Inventory full!", Color.red, 2f);
    }
    
    public void ShowItemPickedUp(string itemName, int amount)
    {
        ShowMessage($"Picked up {itemName} x{amount}", Color.green, 2f);
    }
    
    public void ShowInteraction(string message)
    {
        ShowMessage(message, Color.cyan, 2.5f);
    }
}