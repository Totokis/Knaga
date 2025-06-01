using UnityEngine;
using System.Collections;

public class PlayerMessageDisplay : MonoBehaviour
{
    [Header("Sprite Settings")]
    public float spriteHeight = 1.5f; // Wysokość nad graczem
    public float spriteDuration = 3f; // Czas wyświetlania
    public float fadeSpeed = 2f;
    public float spriteSize = 1f;

    [Header("Sprites")]
    public Sprite tradeSprite;
    public Sprite pickUpSprite;
    public Material Maberial;

    private SpriteRenderer spriteRenderer;
    private Transform playerTransform;
    private Coroutine currentSpriteCoroutine;

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

        SetupSpriteDisplay();
    }

    void SetupSpriteDisplay()
    {
        // Tworzenie obiektu dla sprite'a
        GameObject spriteObj = new GameObject("PlayerSprite");
        spriteObj.transform.parent = transform;

        // Dodanie SpriteRenderer
        spriteRenderer = spriteObj.AddComponent<SpriteRenderer>();
        spriteRenderer.color = Color.white;
        spriteRenderer.material = Maberial;

        // Ustawienie sortowania żeby było na wierzchu
        spriteRenderer.sortingLayerName = "UI";
        spriteRenderer.sortingOrder = 1001;

        // Ustawienie rozmiaru
        spriteObj.transform.localScale = Vector3.one * spriteSize;

        // Początkowo ukryty
        spriteRenderer.color = new Color(1f, 1f, 1f, 0f);
    }

    void Update()
    {
        if (playerTransform != null && spriteRenderer != null)
        {
            // Pozycjonowanie nad graczem
            Vector3 spritePos = playerTransform.position + Vector3.up * spriteHeight;
            transform.position = spritePos;
        }
    }

    public void ShowSprite(Sprite sprite, float? duration = null)
    {
        if (spriteRenderer == null || sprite == null) return;

        // Zatrzymaj poprzedni sprite
        if (currentSpriteCoroutine != null)
        {
            StopCoroutine(currentSpriteCoroutine);
        }

        float spriteDur = duration ?? spriteDuration;

        // Rozpocznij wyświetlanie
        currentSpriteCoroutine = StartCoroutine(DisplaySprite(sprite, spriteDur));
    }

    private IEnumerator DisplaySprite(Sprite sprite, float duration)
    {
        // Pokaż sprite
        spriteRenderer.sprite = sprite;
        spriteRenderer.color = Color.white;

        // Czekaj
        yield return new WaitForSeconds(duration);

        // Zanikanie
        float fadeTimer = 0f;
        Color startColor = spriteRenderer.color;
        while (fadeTimer < 1f)
        {
            fadeTimer += Time.deltaTime * fadeSpeed;
            float alpha = Mathf.Lerp(1f, 0f, fadeTimer);
            spriteRenderer.color = new Color(startColor.r, startColor.g, startColor.b, alpha);
            yield return null;
        }

        // Ukryj sprite
        spriteRenderer.color = new Color(1f, 1f, 1f, 0f);
        spriteRenderer.sprite = null;
        currentSpriteCoroutine = null;
    }

    // Metody pomocnicze dla różnych typów sprite'ów
    public void ShowPickupSprite()
    {
        ShowSprite(pickUpSprite, 2f);
    }

    public void ShowTradeSprite()
    {
        ShowSprite(tradeSprite, 2.5f);
    }

    public void HideSprite()
    {
        if (currentSpriteCoroutine != null)
        {
            StopCoroutine(currentSpriteCoroutine);
            currentSpriteCoroutine = null;
        }

        if (spriteRenderer != null)
        {
            spriteRenderer.color = new Color(1f, 1f, 1f, 0f);
            spriteRenderer.sprite = null;
        }
    }
}