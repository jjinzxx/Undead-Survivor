using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public Player player;
    public PoolManager pool;

    [Header("Survival Time")]
    [SerializeField] private Font survivalTimeFont;
    [SerializeField] private int survivalTimeFontSize = 36;
    [SerializeField] private Vector2 survivalTimeOffset = new Vector2(0f, -32f);

    public float survivalTime;

    private Text survivalTimeText;

    private void Awake()
    {
        instance = this;
        CreateSurvivalTimeUI();
    }

    private void Update()
    {
        survivalTime += Time.deltaTime;
        UpdateSurvivalTimeText();
    }

    private void CreateSurvivalTimeUI()
    {
        GameObject canvasObject = new GameObject("Survival Time Canvas");
        canvasObject.transform.SetParent(transform, false);

        Canvas canvas = canvasObject.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 100;

        CanvasScaler canvasScaler = canvasObject.AddComponent<CanvasScaler>();
        canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        canvasScaler.referenceResolution = new Vector2(1920f, 1080f);
        canvasScaler.matchWidthOrHeight = 0.5f;

        canvasObject.AddComponent<GraphicRaycaster>();

        GameObject textObject = new GameObject("Survival Time Text");
        textObject.transform.SetParent(canvasObject.transform, false);

        survivalTimeText = textObject.AddComponent<Text>();
        survivalTimeText.font = survivalTimeFont != null
            ? survivalTimeFont
            : Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        survivalTimeText.fontSize = survivalTimeFontSize;
        survivalTimeText.alignment = TextAnchor.MiddleCenter;
        survivalTimeText.color = Color.white;
        survivalTimeText.raycastTarget = false;

        RectTransform textRect = survivalTimeText.rectTransform;
        textRect.anchorMin = new Vector2(0.5f, 1f);
        textRect.anchorMax = new Vector2(0.5f, 1f);
        textRect.pivot = new Vector2(0.5f, 1f);
        textRect.anchoredPosition = survivalTimeOffset;
        textRect.sizeDelta = new Vector2(240f, 60f);

        UpdateSurvivalTimeText();
    }

    private void UpdateSurvivalTimeText()
    {
        int minutes = Mathf.FloorToInt(survivalTime / 60f);
        int seconds = Mathf.FloorToInt(survivalTime % 60f);

        survivalTimeText.text = $"{minutes:00}:{seconds:00}";
    }
}
