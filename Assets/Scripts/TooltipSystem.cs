using UnityEngine;
using UnityEngine.UI;

public class TooltipSystem : MonoBehaviour
{
    private static TooltipSystem instance;
    public static TooltipSystem Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<TooltipSystem>();
                if (instance == null)
                {
                    GameObject go = new GameObject("TooltipSystem");
                    instance = go.AddComponent<TooltipSystem>();
                }
            }
            return instance;
        }
    }
    
    private GameObject tooltipObject;
    private Text tooltipText;
    private Canvas canvas;
    private float displayTime = 0f;
    private Transform followTarget;
    private Vector3 offset = Vector3.up * 1.5f;
    
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            CreateTooltipUI();
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }
    
    void CreateTooltipUI()
    {
        // Find or create canvas
        canvas = FindObjectOfType<Canvas>();
        if (canvas == null)
        {
            GameObject canvasObj = new GameObject("TooltipCanvas");
            canvas = canvasObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasObj.AddComponent<CanvasScaler>();
            canvasObj.AddComponent<GraphicRaycaster>();
        }
        
        // Create tooltip object
        tooltipObject = new GameObject("Tooltip");
        tooltipObject.transform.SetParent(canvas.transform, false);
        
        // Background
        Image bg = tooltipObject.AddComponent<Image>();
        bg.color = new Color(0, 0, 0, 0.8f);
        
        // Text
        GameObject textObj = new GameObject("TooltipText");
        textObj.transform.SetParent(tooltipObject.transform, false);
        tooltipText = textObj.AddComponent<Text>();
        tooltipText.text = "";
        tooltipText.color = Color.white;
        tooltipText.fontSize = 16;
        tooltipText.alignment = TextAnchor.MiddleCenter;
        
        // Font
        Font arialFont = Resources.GetBuiltinResource<Font>("Arial.ttf");
        if (arialFont != null)
            tooltipText.font = arialFont;
        
        // Setup RectTransform
        RectTransform rect = tooltipObject.GetComponent<RectTransform>();
        rect.sizeDelta = new Vector2(200, 40);
        
        RectTransform textRect = textObj.GetComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.sizeDelta = Vector2.zero;
        textRect.anchoredPosition = Vector2.zero;
        
        tooltipObject.SetActive(false);
    }
    
    public void ShowTooltip(string text, float duration = 0f)
    {
        if (tooltipObject == null) CreateTooltipUI();
        
        tooltipText.text = text;
        tooltipObject.SetActive(true);
        displayTime = duration;
        followTarget = null;
        
        // Position at center of screen
        RectTransform rect = tooltipObject.GetComponent<RectTransform>();
        rect.anchoredPosition = Vector2.zero;
    }
    
    public void ShowTooltipAtWorldPosition(string text, Vector3 worldPosition, float duration = 0f)
    {
        if (tooltipObject == null) CreateTooltipUI();
        
        tooltipText.text = text;
        tooltipObject.SetActive(true);
        displayTime = duration;
        followTarget = null;
        
        UpdateTooltipPosition(worldPosition);
    }
    
    public void ShowTooltipFollowTarget(string text, Transform target, Vector3 offset, float duration = 0f)
    {
        if (tooltipObject == null) CreateTooltipUI();
        
        tooltipText.text = text;
        tooltipObject.SetActive(true);
        displayTime = duration;
        followTarget = target;
        this.offset = offset;
    }
    
    public void HideTooltip()
    {
        if (tooltipObject != null)
            tooltipObject.SetActive(false);
        followTarget = null;
    }
    
    void Update()
    {
        if (tooltipObject != null && tooltipObject.activeSelf)
        {
            // Handle timed tooltips
            if (displayTime > 0)
            {
                displayTime -= Time.deltaTime;
                if (displayTime <= 0)
                {
                    HideTooltip();
                    return;
                }
            }
            
            // Follow target if set
            if (followTarget != null)
            {
                UpdateTooltipPosition(followTarget.position + offset);
            }
        }
    }
    
    void UpdateTooltipPosition(Vector3 worldPosition)
    {
        if (Camera.main != null)
        {
            Vector3 screenPos = Camera.main.WorldToScreenPoint(worldPosition);
            tooltipObject.transform.position = screenPos;
        }
    }
}