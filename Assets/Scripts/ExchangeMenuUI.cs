using UnityEngine;
using TMPro;

public class ExchangeMenuUI : MonoBehaviour
{
    [Header("Menu Settings")]
    public float menuWidth = 8f;
    public float menuHeight = 6f;
    public Color backgroundColor = new Color(0.1f, 0.1f, 0.1f, 0.95f);
    public Color borderColor = new Color(0.8f, 0.6f, 0.2f, 1f);
    public Color textColor = Color.white;
    public Color highlightColor = new Color(1f, 0.8f, 0.2f, 1f);
    public float textSize = 20f;

    private GameObject menuContainer;
    private GameObject background;
    private GameObject border;
    private TextMeshPro titleText;
    private TextMeshPro optionsText;
    private TextMeshPro inventoryText;
    private TextMeshPro instructionsText;

    private Camera mainCamera;
    private bool isVisible = false;

    private static ExchangeMenuUI instance;
    public static ExchangeMenuUI Instance => instance;

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
        mainCamera = Camera.main;
        CreateMenu();
        HideMenu();
    }

    void CreateMenu()
    {
        // Container for menu
        menuContainer = new GameObject("ExchangeMenuContainer");
        menuContainer.transform.parent = transform;

        // Background quad
        background = GameObject.CreatePrimitive(PrimitiveType.Quad);
        background.name = "MenuBackground";
        background.transform.parent = menuContainer.transform;
        Destroy(background.GetComponent<Collider>());

        MeshRenderer bgRenderer = background.GetComponent<MeshRenderer>();
        bgRenderer.material = new Material(Shader.Find("Sprites/Default"));
        bgRenderer.material.color = backgroundColor;
        bgRenderer.sortingLayerName = "UI";
        bgRenderer.sortingOrder = 1002;

        background.transform.localScale = new Vector3(menuWidth, menuHeight, 1f);

        // Border quad
        border = GameObject.CreatePrimitive(PrimitiveType.Quad);
        border.name = "MenuBorder";
        border.transform.parent = menuContainer.transform;
        Destroy(border.GetComponent<Collider>());

        MeshRenderer borderRenderer = border.GetComponent<MeshRenderer>();
        borderRenderer.material = new Material(Shader.Find("Sprites/Default"));
        borderRenderer.material.color = borderColor;
        borderRenderer.sortingLayerName = "UI";
        borderRenderer.sortingOrder = 1001;

        border.transform.localScale = new Vector3(menuWidth + 0.2f, menuHeight + 0.2f, 1f);
        border.transform.localPosition = new Vector3(0, 0, 0.01f);

        // Title text
        GameObject titleObj = new GameObject("MenuTitle");
        titleObj.transform.parent = menuContainer.transform;
        titleText = titleObj.AddComponent<TextMeshPro>();
        titleText.text = "EXCHANGE STATION";
        titleText.fontSize = textSize * 1.5f;
        titleText.color = highlightColor;
        titleText.alignment = TextAlignmentOptions.Center;
        titleText.sortingOrder = 1003;

        MeshRenderer titleRenderer = titleText.GetComponent<MeshRenderer>();
        titleRenderer.sortingLayerName = "UI";
        titleRenderer.sortingOrder = 1003;

        titleObj.transform.localPosition = new Vector3(0, menuHeight * 0.35f, -0.01f);

        // Options text
        GameObject optionsObj = new GameObject("MenuOptions");
        optionsObj.transform.parent = menuContainer.transform;
        optionsText = optionsObj.AddComponent<TextMeshPro>();
        optionsText.fontSize = textSize;
        optionsText.color = textColor;
        optionsText.alignment = TextAlignmentOptions.Center;
        optionsText.sortingOrder = 1003;

        MeshRenderer optionsRenderer = optionsText.GetComponent<MeshRenderer>();
        optionsRenderer.sortingLayerName = "UI";
        optionsRenderer.sortingOrder = 1003;

        optionsObj.transform.localPosition = new Vector3(0, 0, -0.01f);

        // Inventory display
        GameObject invObj = new GameObject("MenuInventory");
        invObj.transform.parent = menuContainer.transform;
        inventoryText = invObj.AddComponent<TextMeshPro>();
        inventoryText.fontSize = textSize * 0.8f;
        inventoryText.color = highlightColor;
        inventoryText.alignment = TextAlignmentOptions.Center;
        inventoryText.sortingOrder = 1003;

        MeshRenderer invRenderer = inventoryText.GetComponent<MeshRenderer>();
        invRenderer.sortingLayerName = "UI";
        invRenderer.sortingOrder = 1003;

        invObj.transform.localPosition = new Vector3(0, menuHeight * 0.2f, -0.01f);

        // Instructions
        GameObject instrObj = new GameObject("MenuInstructions");
        instrObj.transform.parent = menuContainer.transform;
        instructionsText = instrObj.AddComponent<TextMeshPro>();
        instructionsText.text = "Press number key to exchange | Press E to close";
        instructionsText.fontSize = textSize * 0.7f;
        instructionsText.color = new Color(0.7f, 0.7f, 0.7f, 1f);
        instructionsText.alignment = TextAlignmentOptions.Center;
        instructionsText.sortingOrder = 1003;

        MeshRenderer instrRenderer = instructionsText.GetComponent<MeshRenderer>();
        instrRenderer.sortingLayerName = "UI";
        instrRenderer.sortingOrder = 1003;

        instrObj.transform.localPosition = new Vector3(0, -menuHeight * 0.4f, -0.01f);
    }

    public void ShowMenu(int currentOre)
    {
        if (menuContainer == null) return;

        isVisible = true;
        menuContainer.SetActive(true);

        // Position at center of screen
        if (mainCamera != null)
        {
            Vector3 centerScreen = new Vector3(Screen.width / 2f, Screen.height / 2f, 10f);
            Vector3 worldPos = mainCamera.ScreenToWorldPoint(centerScreen);
            transform.position = new Vector3(worldPos.x, worldPos.y, 0f);
        }

        // Update text content
        UpdateMenuContent(currentOre);
    }

    public void HideMenu()
    {
        if (menuContainer == null) return;

        isVisible = false;
        menuContainer.SetActive(false);
    }

    public void UpdateMenuContent(int currentOre)
    {
        if (optionsText == null || inventoryText == null) return;

        // Build options text with colors
        string options = "<color=#8B4513>[1] Wood</color> - 5 Ore\n";
        options += "<color=#1C1C1C>[2] Oil Barrel</color> - 10 Ore\n";
        options += "<color=#C0C0C0>[3] Metal</color> - 3 Ore\n";
        options += "\n<size=80%>Available exchanges:</size>\n";

        // Check what player can afford
        if (currentOre >= 5)
            options += "<color=#00FF00>✓ Wood</color> ";
        if (currentOre >= 10)
            options += "<color=#00FF00>✓ Oil</color> ";
        if (currentOre >= 3)
            options += "<color=#00FF00>✓ Metal</color> ";

        if (currentOre < 3)
            options += "<color=#FF0000>Not enough ore!</color>";

        optionsText.text = options;

        // Update inventory display
        inventoryText.text = $"Your Ore: <color=#FFD700>{currentOre}</color>";
    }

    public bool IsVisible()
    {
        return isVisible;
    }
}