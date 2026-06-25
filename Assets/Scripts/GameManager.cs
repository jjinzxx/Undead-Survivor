using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    
    [Header("Game Objects")]
    public Player player;
    public PoolManager pool;

    [Header("Survival Time")]
    [SerializeField] private Font survivalTimeFont;
    [SerializeField] private int survivalTimeFontSize = 36;
    [SerializeField] private Vector2 survivalTimeOffset = new Vector2(0f, -32f);

    [Header("Play Time")]
    public float gameTime;      // 흐르는 게임 시간 - 난이도 계산 용도/서바이벌 타임 계산 용도
    public float maxGameTime;   // 최대 게임 시간 - 난이도 증가 기준
    private Text survivalTimeText; // 시간 표시용 변수
    
    [Header("Game Player Data")]
    public int level;
    public int kill;
    public int exp;
    public int[] nextExp = { 3, 5, 10, 20, 150, 210, 280, 360, 450, 600 }; // 임시 레벨업 테이블

    private void Awake()
    {
        instance = this;
        CreateSurvivalTimeUI();
    }

    private void Update()
    {
        // 매 프레임마다 실제 흐른 시간을 누적
        gameTime += Time.deltaTime;
        
        // 최대 시간을 넘지 않도록 고정 (게임 종료 등 처리에 활용)
        if (gameTime > maxGameTime)
        {
            gameTime = maxGameTime;
        }
        
        UpdateSurvivalTimeText();
    }

    // ui 갱신 함수
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
        int minutes = Mathf.FloorToInt(gameTime / 60f);
        int seconds = Mathf.FloorToInt(gameTime % 60f);

        survivalTimeText.text = $"{minutes:00}:{seconds:00}";
    }
    
    // 경험치 획득 및 레벨업 로직
    public void GetExp()
    {
        exp++;
        if (exp == nextExp[level])
        {
            level++;
            exp = 0;
        }
    }
}
