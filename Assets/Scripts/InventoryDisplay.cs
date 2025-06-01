using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class InventoryDisplay : MonoBehaviour
{
    [Header("Display Settings")]
    public Vector2 screenOffset = new Vector2(50f, 50f);
    public float itemSpacing = 10f; // Odstęp między przedmiotami
    public float spriteSize = 0.5f; // Rozmiar sprite'ów
    public float textSize = 18f;
    public Color textColor = Color.white;
    public Color backgroundColor = new Color(0, 0, 0, 0.8f);

    [Header("Item Sprites")]
    public Sprite woodSprite;
    public Sprite oilSprite;
    public Sprite metalSprite;
    public Sprite coalSprite;

    private Camera mainCamera;
    private PlayerInventory inventory;
    private GameObject background;
    private List<InventoryItemUI> itemUIList = new List<InventoryItemUI>();

    // Pozycje dla pierwszych trzech elementów
    private Vector3[] itemPositions = new Vector3[]
    {
        new Vector3(-3.5f, -1f, 0f),  // Pierwszy element
        new Vector3(0f, -1f, 0f),     // Drugi element
        new Vector3(3.5f, -1f, 0f)    // Trzeci element
    };

    [System.Serializable]
    private class InventoryItemUI
    {
        public GameObject itemObject;
        public SpriteRenderer spriteRenderer;
        public TextMeshPro quantityText;
    }

    void Start()
    {
        mainCamera = Camera.main;
        inventory = PlayerInventory.Instance;

        SetupBackground();
        UpdateDisplay();
    }

    void SetupBackground()
    {
        // Tworzenie tła
        background = GameObject.CreatePrimitive(PrimitiveType.Quad);
        background.name = "InventoryBackground";
        background.transform.parent = transform;

        // Usunięcie collidera
        Destroy(background.GetComponent<Collider>());

        // Ustawienie materiału i koloru
        MeshRenderer bgRenderer = background.GetComponent<MeshRenderer>();
        bgRenderer.material = new Material(Shader.Find("Sprites/Default"));
        bgRenderer.material.color = backgroundColor;
        bgRenderer.sortingLayerName = "UI";
        bgRenderer.sortingOrder = 998;
    }

    void Update()
    {
        if (mainCamera == null) return;

        // Pozycjonowanie w lewym dolnym rogu
        Vector3 screenPos = new Vector3(screenOffset.x, screenOffset.y, 10f);
        Vector3 worldPos = mainCamera.ScreenToWorldPoint(screenPos);
        transform.position = new Vector3(worldPos.x, worldPos.y, 0f);

        // Aktualizacja zawartości co kilka klatek
        if (Time.frameCount % 30 == 0)
        {
            UpdateDisplay();
        }
    }

    void UpdateDisplay()
    {
        if (inventory == null) return;

        // Usuwamy stare UI elementy
        ClearItemUI();

        if (inventory.items.Count == 0)
        {
            // Jeśli brak przedmiotów, ukrywamy tło
            if (background != null)
                background.SetActive(false);
            return;
        }

        // Pokazujemy tło
        if (background != null)
            background.SetActive(true);

        // Wyświetlamy pierwsze 3 przedmioty (lub mniej jeśli jest mniej przedmiotów)
        int itemsToShow = Mathf.Min(inventory.items.Count, 3);

        for (int i = 0; i < itemsToShow; i++)
        {
            var item = inventory.items[i];
            CreateItemUI(item, i);
        }

        // Dopasowanie rozmiaru tła do poziomego układu
        if (background != null)
        {
            // Szerokość: od -4 do 4 (8 jednostek) + margines
            // Wysokość: stała wysokość dla jednego rzędu
            background.transform.localScale = new Vector3(
                9f * 0.01f,  // Szerokość tła
                3f * 0.01f,  // Wysokość tła
                1f
            );
            background.transform.localPosition = new Vector3(
                0f,     // Wyśrodkowane w poziomie
                -1f,    // Na poziomie elementów
                0.1f    // Lekko z tyłu
            );
        }
    }

    void CreateItemUI(Item item, int index)
    {
        InventoryItemUI itemUI = new InventoryItemUI();

        // Tworzenie głównego obiektu dla itemu
        itemUI.itemObject = new GameObject($"InventoryItem_{index}");
        itemUI.itemObject.transform.parent = transform;

        // Pozycjonowanie według określonych pozycji
        itemUI.itemObject.transform.localPosition = itemPositions[index];

        // Tworzenie sprite'a
        GameObject spriteObj = new GameObject("ItemSprite");
        spriteObj.transform.parent = itemUI.itemObject.transform;
        spriteObj.transform.localPosition = Vector3.zero; // Sprite na 0,0,0 względem rodzica

        itemUI.spriteRenderer = spriteObj.AddComponent<SpriteRenderer>();
        itemUI.spriteRenderer.sprite = item.icon;
        itemUI.spriteRenderer.sortingLayerName = "UI";
        itemUI.spriteRenderer.sortingOrder = 1000;

        // Skalowanie sprite'a
        spriteObj.transform.localScale = Vector3.one * spriteSize;

        // Tworzenie tekstu z ilością
        GameObject textObj = new GameObject("QuantityText");
        textObj.transform.parent = itemUI.itemObject.transform;
        textObj.transform.localPosition = new Vector3(1.5f, 0f, 0f); 

        itemUI.quantityText = textObj.AddComponent<TextMeshPro>();
        itemUI.quantityText.text = $"x{item.amount}";
        itemUI.quantityText.fontSize = textSize;
        itemUI.quantityText.color = textColor;
        itemUI.quantityText.alignment = TextAlignmentOptions.Center; // Wyśrodkowanie tekstu
        itemUI.quantityText.sortingOrder = 1000;

        // Ustawienie rozmiaru tekstu
        RectTransform textRect = itemUI.quantityText.GetComponent<RectTransform>();
        if (textRect != null)
        {
            textRect.sizeDelta = new Vector2(2f, 1f);
        }

        // Ustawienie sortowania dla tekstu
        MeshRenderer textRenderer = itemUI.quantityText.GetComponent<MeshRenderer>();
        if (textRenderer != null)
        {
            textRenderer.sortingLayerName = "UI";
            textRenderer.sortingOrder = 1000;
        }

        itemUIList.Add(itemUI);
    }

    void ClearItemUI()
    {
        foreach (var itemUI in itemUIList)
        {
            if (itemUI.itemObject != null)
                DestroyImmediate(itemUI.itemObject);
        }
        itemUIList.Clear();
    }

    void OnDestroy()
    {
        ClearItemUI();
    }
}