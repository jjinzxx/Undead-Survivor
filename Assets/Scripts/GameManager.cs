using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    
    [Header("Game Objects")]
    public Player player;
    public PoolManager pool;
    
    [Header("Play Time")]
    public float gameTime;      // 흐르는 게임 시간 - 난이도 계산 용도/서바이벌 타임 계산 용도
    public float maxGameTime;   // 최대 게임 시간 - 난이도 증가 기준
    
    [Header("Game Player Data")]
    public int level;
    public int kill;
    public int exp;
    public int[] nextExp = { 3, 5, 10, 20, 30, 40, 50, 60, 70, 80 }; // 임시 레벨업 테이블
    public int health;
    public int maxHealth = 100;

    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        health = maxHealth; // 게임 시작 시 체력을 최대 체력으로 초기화
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
